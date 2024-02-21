using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LICDataExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamReader sr = new StreamReader("temp.txt");
            string line = "";
            List<string> temps = new List<string>();
            while ((line = sr.ReadLine()) != null)
            {
                temps.Add(line);
            }
            int lcount = temps.Count;

            Console.WriteLine("Enter csv file name:\t");
            string name = Console.ReadLine();
            Random rand = new Random();

            sr = new StreamReader(name);
            line = "";
            StreamWriter sw = new StreamWriter("new_" + name);

            int count = 0;
            DateTime initial = new DateTime();
            while ((line = sr.ReadLine()) != null)
            {
               line= line.Replace("VA1=", "");
               line= line.Replace("VA2=", "");
               line= line.Replace("T=", "");

                string[] columns = line.Split(',');

                if (count == 0)
                {
                    initial = DateTime.Parse(columns[0]);
                }

                if (Double.Parse(columns[1]) < 0) columns[1] = temps[rand.Next(0, lcount)];
                int minsElapsed = (int)DateTime.Parse(columns[0]).Subtract(initial).TotalMinutes;

                sw.WriteLine(minsElapsed + "," + columns[1] + "," + columns[2] +","+ columns[3]);
                sw.Flush();
                count++;
            }

            Console.WriteLine(count + " record processed.");
            Console.ReadLine();
        }
    }
}
