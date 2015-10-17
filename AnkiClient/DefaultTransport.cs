using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Data.SQLite;
using System.Security.Cryptography.X509Certificates;

namespace AnkiClient
{
    public class DefaultTransport : IAnkiTransport
    {
        private readonly Uri baseUri;

        const string boundary = "Anki-sync-boundary";

        private readonly X509Certificate cert = X509Certificate.CreateFromCertFile("ankiweb.certs");

        public DefaultTransport(Uri uri)
        {
            this.baseUri = uri;


        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public string Download(string userToken, string clientInfo)
        {
            var req = CreateRequest(new Uri(this.baseUri, "download"));
            var stream = req.GetRequestStream();
            var sw = new StreamWriter(stream);
            Dictionary<string, string> headers = new Dictionary<string, string>() { ["k"] = userToken, ["c"] = "1", ["v"] = clientInfo };
            foreach (var h in headers)
            {
                sw.WriteLine($"--{DefaultTransport.boundary}");
                sw.WriteLine($"Content-Disposition: form-data; name=\"{h.Key}\"");
                sw.WriteLine();
                sw.WriteLine(h.Value);
            }
            sw.WriteLine($"--{DefaultTransport.boundary}");
            sw.WriteLine("Content-Disposition: form-data; name=\"data\"; filename=\"data\"");
            sw.WriteLine("Content-Type: application/octet-stream");
            sw.WriteLine();
            sw.Flush();
            //StreamWriter csw = new StreamWriter(new GZipStream(stream, CompressionMode.Compress));

           /* BinaryWriter bw = new BinaryWriter(stream);
            bw.Write(Zip(data.ToString(Newtonsoft.Json.Formatting.None)));
            bw.Flush();
            //csw.Flush();
            sw.WriteLine();
            sw.WriteLine($"--{DefaultTransport.boundary}--");
            //sw.Flush();*/
            sw.Close();
            //csw.Close();


            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream rs = resp.GetResponseStream();

            var temPath = $"{AppDomain.CurrentDomain.BaseDirectory}\\tmp_{DateTime.Now.ToString("H_mm_ss_ffff")}.tmp";
            FileStream tmpfile = new FileStream(temPath, FileMode.OpenOrCreate);
            rs.CopyTo(tmpfile);
            tmpfile.Flush();
            tmpfile.Close();

            return temPath;

        }

        public string Finish(Dictionary<string, string> headers)
        {
            throw new NotImplementedException();
        }

        public string Request(string method, Dictionary<string, string> headers, JObject data)
        {
            var req = CreateRequest(new Uri(this.baseUri, method));
            var stream = req.GetRequestStream();
            var sw = new StreamWriter(stream);
            foreach (var h in headers)
            {
                sw.WriteLine($"--{DefaultTransport.boundary}");
                sw.WriteLine($"Content-Disposition: form-data; name=\"{h.Key}\"");
                sw.WriteLine();
                sw.WriteLine(h.Value);
            }
            sw.WriteLine($"--{DefaultTransport.boundary}");
            sw.WriteLine("Content-Disposition: form-data; name=\"data\"; filename=\"data\"");
            sw.WriteLine("Content-Type: application/octet-stream");
            sw.WriteLine();
            sw.Flush();
            //StreamWriter csw = new StreamWriter(new GZipStream(stream, CompressionMode.Compress));

            BinaryWriter bw = new BinaryWriter(stream);
            bw.Write(Zip(data.ToString(Newtonsoft.Json.Formatting.None)));
            bw.Flush();
            //csw.Flush();
            sw.WriteLine();
            sw.WriteLine($"--{DefaultTransport.boundary}--");
            //sw.Flush();
            sw.Close();
            //csw.Close();


            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream rs = resp.GetResponseStream();
            StreamReader sr = new StreamReader(rs);
            return sr.ReadToEnd();
        }

        public static byte[] Zip(string value)
        {
            using (MemoryStream ms = new MemoryStream())
            using (GZipStream s = new GZipStream(ms, System.IO.Compression.CompressionMode.Compress))
            {
                StreamWriter sw = new StreamWriter(s);
                sw.Write(value);
                sw.Close();
                return ms.ToArray();
            }
        }

        private HttpWebRequest CreateRequest(Uri uri)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);
            webRequest.Method = "POST";
            webRequest.ContentType = $"multipart/form-data; boundary={DefaultTransport.boundary}";
            webRequest.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            webRequest.ClientCertificates.Add(this.cert);
            return webRequest;
        }
    }
}
