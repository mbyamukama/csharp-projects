using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;

namespace RainDataExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            System.IO.StreamReader reader = new System.IO.StreamReader("rain.dat");
            System.IO.StreamWriter writer = new System.IO.StreamWriter("rain.csv");

            string line = "";
            int count = 0;

            while ((line = reader.ReadLine()) != null)
            {
                try
                {
                    if (line.Contains("P0_LST60"))
                    {
                        ++count; 
                        string date = DateTime.Parse(line.Substring(0, 19)).ToString("yyyy-MM-dd");
                        int index = line.IndexOf("P0_LST60");
                        string rain = line.Substring(index, 11).Split('=')[1];
                        writer.WriteLine(date + "," + rain);
                    }
                }
                catch
                {
                    continue;
                }
            }
            Console.WriteLine(count);
            writer.Close(); //flush the data to physical disk
        }
    }
}
