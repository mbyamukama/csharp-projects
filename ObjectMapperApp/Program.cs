using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectMapperApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var connString = "Host=localhost;Username=postgres;Password=mysecretpassword;Database=postgres";

            var conn = new NpgsqlConnection(connString);
            conn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand(@"SELECT * FROM ""Messages""", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();
            List<Message> msgList = new List<Message>();
            TypedObjectMapper<Message> tobjMapper = new TypedObjectMapper<Message>(reader);
            tobjMapper.Map(out msgList);
            Console.WriteLine(msgList.Count);

            
        }
    }
}