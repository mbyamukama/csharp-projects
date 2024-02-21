using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace AWSStatistics
{
    class Program
    {
        static void Main(string[] args)
        {
           //Console.WriteLine(DateTime.Parse("2/21/2018 6:54:54 PM", CultureInfo.CreateSpecificCulture("en-US")).ToString());
            System.IO.StreamReader sr = new System.IO.StreamReader("gen3log.log");
            string line = "";
            System.IO.StreamWriter sw = new System.IO.StreamWriter("processed.csv", true);
            while ((line = sr.ReadLine()) != null)
            {
                if (line.Contains("DISCONNECT"))
                {
                    string procLine = line.Split('\t')[0];
                    int len = procLine.Length;
                    procLine = procLine.Substring(0, len - 1);
                    DateTime date = DateTime.Parse(procLine, CultureInfo.CreateSpecificCulture("en-US"));
                    sw.WriteLine("COMPLETE," + Math.Round(date.Hour + (date.Minute * 1.0 / 60), 1));
                    sw.Flush();
                }                           
                if (line.Contains("manner"))
                {
                    string procLine = line.Split('\t')[0];
                    int len = procLine.Length;
                    procLine = procLine.Substring(0, len - 1);
                    DateTime date = DateTime.Parse(procLine, CultureInfo.CreateSpecificCulture("en-US"));
                    sw.WriteLine("INCOMPLETE," + Math.Round(date.Hour + (date.Minute * 1.0 / 60), 1));
                    sw.Flush();
                }
            } 
        }
    }
}
