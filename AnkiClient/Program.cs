using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
            var res = transport.Request("hostKey", new Dictionary<string, string>() {["c"] = "1" }, new JObject() {["u"] = "alexandrov.ua@gmail.com",["p"] = "016777216" });
            Console.WriteLine(res);
            var json = JObject.Parse(res);
            res = transport.Request("meta", new Dictionary<string, string>() {["c"] = "1",["s"] = "59e1a913",["k"] = json["key"].ToString() }, JObject.Parse("{\"cv\": \"ankidesktop, 2.0.33, win: 8\", \"v\": 8}"));
            Console.WriteLine(res);
            Console.WriteLine();
            var path = transport.Download(json["key"].ToString(), JObject.Parse("{\"cv\": \"ankidesktop, 2.0.33, win: 8\", \"v\": 8}").ToString(Newtonsoft.Json.Formatting.None));
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
