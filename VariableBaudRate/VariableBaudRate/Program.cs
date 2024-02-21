using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace VariableBaudRate
{
    class Program
    {
        static SerialPort p = null;
        static byte[] buff = null;

        static StreamWriter sw = null;
        static void Main(string[] args)
        {
            Console.WriteLine("Enter COM port:\t");
            string comport = Console.ReadLine();
            sw = new StreamWriter("data.log",true);
            sw.AutoFlush = true;
            int baudRate = 115200;
            p = new SerialPort("COM"+comport, baudRate);
            p.DataReceived += P_DataReceived;
            p.Open();
            buff = new byte[400];

             while (true)
             {
                string command = "rim\r\n";
                foreach (char c in command)
                {
                    p.Write(new byte[] { (byte)c }, 0, 1);
                    System.Threading.Thread.Sleep(1);
                }
                p.Write("\n");
                System.Threading.Thread.Sleep(2000);
             }
        }

        private static void P_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = p.ReadLine();
            Console.Write(data);
            sw.Write(data);
        }
    }
}
