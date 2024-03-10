using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;
using System.Threading;

namespace Serial2File
{
    class Program
    {
        static SerialPort port = null;
        static StreamWriter sw = null;


        static void Main(string[] args)
        {
            try
            {
                sw = new StreamWriter("data.txt", true);
                sw.AutoFlush = true;
                string[] avports = SerialPort.GetPortNames();
                Console.WriteLine("Found these ports");
                foreach (string item in avports)
                {
                    Console.WriteLine(item);
                }
                Console.Write("Enter Port:\t");
                string testPort = Console.ReadLine();
                Console.Write("Enter Baud:\t");
                int baud = Int32.Parse(Console.ReadLine());

                port = new SerialPort(testPort, baud);
                port.DataReceived += port_DataReceived;
                port.Open();
                Thread.Sleep(-1);
            }
            catch
            {
                sw.Flush();
                sw.Close();
            }
        }

        static void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = port.ReadLine();      
            Console.WriteLine(data);
            sw.Write(DateTime.Now + ": " + data);
        }
    }
}
