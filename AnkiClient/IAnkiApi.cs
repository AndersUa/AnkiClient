using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnkiClient
{
    public interface IAnkiApi
    {
        string Login(string userName, string password);
        string Meta(string token);
        string Download(string token);
    }
}
