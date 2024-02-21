using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GenericUtilities;
using System.Data;

namespace UIAApplication
{
    class Program
    {
        static string StripCompanyName(string companyName)
        {
            if (companyName.Contains("LIMITED"))
            {
                int index = companyName.IndexOf("LIMITED");
                companyName = companyName.Remove(index, 7);
            }
            if (companyName.Contains("LTD"))
            {
                int index = companyName.IndexOf("LTD");
                companyName = companyName.Remove(index, 3);
            }
            return companyName;
        }
        static void Main(string[] args)
        {
            DataTable ura = Utilities.ReadCSVFile("ura.csv");
            DataTable uia = Utilities.ReadCSVFile("uia.csv");
            DataTable notFound = uia.Clone();
            bool found = false;

            DataTable results = new DataTable();
            results.Columns.Add("UIA_NAME");
            results.Columns.Add("URA_NAME");
            results.Columns.Add("LD_DIST");
            results.Columns.Add("BUSINESS");
            results.Columns.Add("LOCATION");

            string uraname="", uianame="";
            int ld_dist = 0, rowcount = uia.Rows.Count, rowsdone = 0;
            string business = "", location = "";

            DateTime start = DateTime.Now;

            foreach(DataRow uiarow in uia.Rows)
            {
                uianame = uiarow["COMPANY"].ToString().ToUpper();
                Console.WriteLine("Finding match for: " + uianame);
                found = false;

                for (int i=0; i< ura.Rows.Count; i++)
                {
                    uraname = ura.Rows[i]["COMPANY"].ToString().ToUpper();
                    ld_dist = LevenshteinDistance.Compute(StripCompanyName(uraname), StripCompanyName(uianame));
                    if (StripCompanyName(uianame).Contains(uraname) || StripCompanyName(uraname).Contains(uianame) || ld_dist<=2)
                    {
                        try
                        {
                            business = ura.Rows[i]["BUSINESS"].ToString();
                            location = ura.Rows[i]["LOCATION"].ToString();     
                            results.Rows.Add(uianame, uraname, ld_dist, business,location);
                            Console.WriteLine("Found match for " + uianame + " as , " + uraname+ " at ura file index "+ i + " LD=" + ld_dist +"\n\n");
                            found = true;
                            break;
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                if (!found)
                {
                    notFound.Rows.Add(uiarow.ItemArray);
                    Console.WriteLine("No match found. Added to not found list. DB Count =" + notFound.Rows.Count);
                }
                Console.WriteLine("Processed Row " + (rowsdone = uia.Rows.IndexOf(uiarow)) + " of " + rowcount);
                DateTime now = DateTime.Now;
                int elapsed = now.Subtract(start).Minutes*60 + now.Subtract(start).Seconds;
                double secsperrow = elapsed * 1.0 / rowsdone;
                Console.WriteLine("Speed = " + Math.Round(secsperrow, 2) + " seconds/row.");
                Console.Write("Rows Left: " + (rowcount - rowsdone) + "\t");
                int ETA = (int)((rowcount - rowsdone) * secsperrow);
                Console.Write(" ETA=" + ETA + "s (" + ETA / 60 + " mins)\n");

            }
            Utilities.WriteCSVFile(results, "uia_ura_results.csv");
            Utilities.WriteCSVFile(notFound, "notfound_remaining.csv");
        }
    }
}
