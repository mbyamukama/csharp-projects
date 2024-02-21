using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AWSReportCounter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter File Path\t:");
            string filepath = Console.ReadLine();
            StreamReader sr = new StreamReader(filepath);

            Console.Write("Enter Start Date\t:");
            DateTime start = DateTime.Parse(Console.ReadLine());

            Console.Write("Enter End   Date\t:");
            DateTime end = DateTime.Parse(Console.ReadLine());
            //populate arrays
            List <string> datesinFile = new List<string>();
            DateTime t = start;
            do
            {
                datesinFile.Add(t.ToString("yyy-MM-dd"));
                t = t.AddDays(1);
            } while (t <= end);

            List<int> count = new List<int>();
            for(int i=0; i < datesinFile.Count; i++)
            {
                count.Add(0);
            }

            string line = "";
            int index = 0;
            while ((line = sr.ReadLine()) != null)
            {
                if (line != "" && line.Contains("sink"))
                {
                    try
                    {
                        string dateFound = line.Substring(0, 10);
                        if ((index = datesinFile.IndexOf(dateFound)) > -1)
                        {
                            count[index]++;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            StreamWriter sw = new StreamWriter("results.dat");
            for(int i=0; i<datesinFile.Count; i++)
            {
                sw.WriteLine(datesinFile[i] + " , " + count[i]);
                Console.WriteLine(datesinFile[i] + " , " + count[i]);
            }
            sw.Flush();
            Console.WriteLine("Process finished.");

        }
    }
}
