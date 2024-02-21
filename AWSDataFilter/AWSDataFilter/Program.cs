using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSDataFilter
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamReader reader = new StreamReader("gen3data.dat");
            string line = "";
            int origCount = 0, finalCount = 0;
            while ((line=reader.ReadLine())!=null)
            {
                ++origCount;
                if(line.Contains("TXT") && line.Contains("RTC_T"))
                {
                    if (line.Length < 200 && line.Length > 100)
                    {
                        ++finalCount;
                    }
                }
            }
            Console.WriteLine("Valid Lines = " + finalCount * 100.0 / origCount);
        }
    }
}
