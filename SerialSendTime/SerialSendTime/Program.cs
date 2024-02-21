using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.IO.Ports;

namespace SerialSendTime
{
    class Program
    {
        static SerialPort sp = null;
        public static DateTime GetNistTime()
        {
            var myHttpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.microsoft.com");
            var response = myHttpWebRequest.GetResponse();
            string todaysDates = response.Headers["date"];
            return DateTime.ParseExact(todaysDates,
                                       "ddd, dd MMM yyyy HH:mm:ss GMT",
                                       CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
        }

        static void Main(string[] args)
        {
            DateTime now = GetNistTime();
            Console.WriteLine("The date and time is: " + now.ToString());
            Console.Write("Enter offset in +ve or -ve hours:\t");
            int offset = Int32.Parse(Console.ReadLine());

            Console.WriteLine("Enter COM Port:\t");
            try
            {
                sp = new SerialPort(Console.ReadLine(), 38400);
                sp.DataReceived += Sp_DataReceived;
                sp.Open();
            }
            catch { }
            now = GetNistTime();
            Console.WriteLine("The date to write  is:\t" + now.ToString() + " plus " + offset + " hours");
            sp.WriteLine(now.Second + "," + now.Minute + "," + now.Hour + offset + "," +
                (((int)now.DayOfWeek) + 1) + "," + now.Day + "," + now.Month + "," + now.Year.ToString().Substring(2) + ",");
            System.Threading.Thread.Sleep(-1);
        }

        private static void Sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine(sp.ReadLine());
        }
    }
}
