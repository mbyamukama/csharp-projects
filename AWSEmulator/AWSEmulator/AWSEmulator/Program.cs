
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.IO;

namespace AWSEmulator
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime dt = DateTime.Now;

            Console.WriteLine("Enter IP Address:\t");      //129.177.63.214 - bergen
            //196.43.133.125 - MUK
            string ipadd = Console.ReadLine();
            Console.WriteLine("Enter Port:\t");
            int port = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Enter Delay between transmissions (in milli seconds):\t");
            int ms = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Enter TXT:\t");
            string txt = Console.ReadLine();
            Console.WriteLine("Enter Number of reports to send:\t");
            int reps = Int32.Parse(Console.ReadLine());
            TcpClient client = new TcpClient(ipadd, port);
            try
            {
                Stream s = client.GetStream();
                Console.WriteLine("connected!");
                StreamWriter sw = new StreamWriter(s);       
                sw.AutoFlush = true;
                int count = 0;
                int seq = 0;
                while(count <= reps)
                {
                    dt = DateTime.Now;
                    string rtc_t = dt.Year + "-" + dt.Month.ToString("00") + "-" + dt.Day.ToString("00") + " " + dt.Hour.ToString("00") + ":" + dt.Minute.ToString("00") + ":" + dt.Second.ToString("00");
                    string v_bat = (new Random().Next(270, 420) * 0.01).ToString();
                    string t = (new Random().Next(200, 300) * 0.1).ToString();
                    string rssi = (new Random().Next(27, 100) * -1).ToString();
                    sw.WriteLine("RTC_T=" + rtc_t + " TXT=emulator V_IN="+ v_bat + " T=" + t + " E64="+txt+" SEQ=" + (seq++) + " TTL=15 RSSI=" + rssi + " LQI=4");
                    Console.WriteLine("RTC_T=" + rtc_t + " TXT=emulator V_IN=" + v_bat + " T=" + t + " E64=" + txt + " SEQ=" + (seq++) + " TTL=15 RSSI=" + rssi + " LQI=4");
                    System.Threading.Thread.Sleep(ms);
                    count++;
                }
                sw.WriteLine("DISCONNECT");
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }
    }
}