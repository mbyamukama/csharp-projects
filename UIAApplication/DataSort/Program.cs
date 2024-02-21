using GenericUtilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DataSort
{
    class Program
    {
        private static int count;

        static void Main(string[] args)
        {
            DataTable ura = Utilities.ReadCSVFile("uia_ura_norm.csv");
            DataTable results = new DataTable();
            results.Columns.Add("BUSINESS");
            results.Columns.Add("COUNT");
            List<string> list = new List<string>();
            foreach (DataRow row in ura.Rows)
            {
                /* if (row["BUSINESS"].ToString().Trim() != "")
                 {
                     list.Add(row["BUSINESS"].ToString());
                 }  */
                if (row["LOCATION"].ToString().Contains("JINJA") || row["LOCATION2"].ToString().Contains("JINJA"))
                    count++;
            }
            Console.WriteLine(count);
           /* list = list.Distinct().ToList();

            foreach (String item in list)
            {
               int count = 0;
                Console.WriteLine("Processing " + item);
               foreach  (DataRow row in ura.Rows)
                {
                    if(row["BUSINESS"].ToString()==item)
                    {
                        count++;
                    }
                }
                results.Rows.Add(item, count);
            }
            Utilities.WriteCSVFile(results, "results.csv");             */

        }
    }
}
