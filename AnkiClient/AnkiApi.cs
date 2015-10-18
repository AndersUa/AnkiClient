using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnkiClient
{
    public class AnkiApi : IAnkiApi
    {
        private IAnkiTransport transport;

        public AnkiApi(IAnkiTransport transport)
        {
            if (transport == null)
            {
                throw new ArgumentNullException(nameof(transport));
            }
            this.transport = transport;
        }

        public string Download(string token)
        {
            using (var stream = this.transport.Request("download", new Dictionary<string, string>() {["k"] = token,["c"] = "1" }, null))
            {
                var temPath = $"{AppDomain.CurrentDomain.BaseDirectory}\\tmp_{DateTime.Now.ToString("H_mm_ss_ffff")}.tmp";
                FileStream tmpfile = new FileStream(temPath, FileMode.OpenOrCreate);
                stream.CopyTo(tmpfile);
                tmpfile.Flush();
                tmpfile.Close();
                return temPath;
            }
        }

        public string Login(string userName, string password)
        {
            JObject data = new JObject() {["u"] = userName,["p"] = password };
            using (var stream = this.transport.Request("hostKey", new Dictionary<string, string>() {["c"] = "1" }, data.ToString(Newtonsoft.Json.Formatting.None)))
            {
                StreamReader reader = new StreamReader(stream);
                JObject resp = JObject.Parse(reader.ReadToEnd());
                return resp["key"].ToString();
            }
        }

        public string Meta(string token)
        {
            // v = 8 it's magic!
            using (var stream = this.transport.Request("meta", new Dictionary<string, string>() {["c"] = "1",["k"] = token }, JObject.Parse("{\"v\": 8}").ToString(Newtonsoft.Json.Formatting.None)))
            {
                StreamReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }
    }
}
