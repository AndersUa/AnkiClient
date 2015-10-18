using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnkiClient
{
    class Program
    {
        static void Main(string[] args)
        {

            DefaultTransport transport = new DefaultTransport(new Uri("https://ankiweb.net/sync/"));
            IAnkiApi api = new AnkiApi(transport);
            string token = null;
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (string.IsNullOrEmpty(config.AppSettings.Settings["UserToken"].Value))
            {
                Console.WriteLine("Login");
                token = api.Login("alexandrov.ua@gmail.com", "016777216");
                config.AppSettings.Settings["UserToken"].Value = token;
                config.Save();

            }
            else
            {
                token = config.AppSettings.Settings["UserToken"].Value;
            }

            Console.WriteLine(token);
            var meta = api.Meta(token);
            Console.WriteLine(meta);
            var path = api.Download(token);
            string a = string.Format(@"Data Source={0};Version=3;New=False;Compress=True;", path);
            var sql_con = new SQLiteConnection(a);
            sql_con.Open();
            var command = sql_con.CreateCommand();
            command.CommandText = "SELECT flds FROM notes";
            var t = command.ExecuteReader();
            while (t.Read())
            {
                Console.WriteLine(string.Format("{0,20} {1,20}", t.GetString(0).Split('')));
            }
            Console.ReadKey();

        }
    }
}
