using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnkiClient
{
    public interface IAnkiTransport : IDisposable
    {
        Stream Request(string method, Dictionary<string, string> headers, string data);
    }
}
