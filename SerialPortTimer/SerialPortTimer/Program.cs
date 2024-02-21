using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace SerialPortTimer
{
    class Program
    {
        static System.Timers.Timer tm;
        static SerialPort p;
        static double[] times = new double[10];
        static int j = 0;
        static double elapsed = 0;

        static void Main(string[] args)
        {
            Console.Write("Enter Serial Port:\t");
            string comport = Console.ReadLine();
            p = new SerialPort(comport, 115200);
            p.DataReceived += P_DataReceived;
            p.Open();
            System.Threading.Thread.Sleep(-1);
        }

        private static void P_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            times[j] = DateTime.Now.Second + DateTime.Now.Millisecond * 0.001;
            ++j;
            if (j == 10)
            {
                Console.WriteLine("capture complete");
                for(int i=1; i<10; i++)
                {
                    elapsed = times[i] - times[i - 1]; if (elapsed < 0) elapsed += 60;
                    Console.WriteLine(elapsed);
                }
                j = 0;
            }
        }
    }
}
