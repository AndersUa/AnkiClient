using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnkiClient
{
    public interface IAnkiTransport : IDisposable
    {
        string Request(string method, Dictionary<string, string> headers, JObject data);
        string Download(string userToken, string clientInfo);
        string Finish(Dictionary<string, string> headers);
    }
}
