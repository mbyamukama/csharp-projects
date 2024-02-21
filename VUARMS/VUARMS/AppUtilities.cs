using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using GenericUtilities;

namespace VUARMS
{
    class AppUtilities
    {
        public static CurrentUser CurrentUser = null;
        public static List<string> errors = new List<string>();
        public static string sessionID = "";
        public List<string> Log = new List<string>();

        public static int GetTotalCredits(DataTable results)
        {
            int cu = 0;
            foreach(DataRow row in results.Rows)
            {
                cu += Convert.ToInt32(row["CU"]);
            }
            return cu;
        }
        public static float CalculateGPA(DataTable results)
        {
            int EW = 0;
            float EWn = 0;
            float gradepoint = 0;
            foreach (DataRow row in results.Rows)
            {
                int total = row.Field<int>("TOTAL");
                gradepoint = total >= 80 ? 5.0f :
                    (total >= 75 & total < 80) ? 4.5f :
                    (total >= 70 & total < 75) ? 4.0f :
                    (total >= 65 & total < 70) ? 3.5f :
                    (total >= 60 & total < 65) ? 3.0f :
                    (total >= 55 & total < 60) ? 2.5f :
                    (total >= 50 & total < 55) ? 2.0f :
                    (total < 50) ? 0.0f : 0.0f;
                
                EW += row.Field<int>("CU");
                EWn += row.Field<int>("CU") * gradepoint;
            }
            return (float)Math.Round(EWn / EW, 2);
        }

        public static DataTable AppendGrades(DataTable results)
        {
            DataTable toView = results.Copy();

            results.Columns.Add("GRADE");
            results.Columns.Add("GRADE POINT");
            foreach (DataRow row in results.Rows)
            {
                int total = row.Field<int>("TOTAL");
                if (total >= 80)
                {
                    row["GRADE"] = "A";
                    row["GRADE POINT"] = 5.0f;
                }
                if (total >= 75 & total < 80)
                {
                    row["GRADE"] = "B+";
                    row["GRADE POINT"] = 4.5f;
                }
                if (total >= 70 & total < 75)
                {
                    row["GRADE"] = "B";
                    row["GRADE POINT"] = 4.0f;
                }
                if (total >= 65 & total < 70)
                {
                    row["GRADE"] = "B-";
                    row["GRADE POINT"] = 3.5f;
                }
                if (total >= 60 & total < 65)
                {
                    row["GRADE"] = "C+";
                    row["GRADE POINT"] = 3.0f;
                }
                if (total >= 55 & total < 60)
                {
                    row["GRADE"] = "C";
                    row["GRADE POINT"] = 2.5f;
                }
                if (total >= 50 & total < 55)
                {
                    row["GRADE"] = "C-";
                    row["GRADE POINT"] = 2.0f;
                }
                if (total < 50)
                {
                    row["GRADE"] = "F";
                    row["GRADE POINT"] = 0.0f;
                }
            }
            return toView;
        }

        public static float GetCGPA(DataTable results)
        {
            int EW = 0;
            float EWn = 0;
            float gradepoint = 0;
            foreach (DataRow row in results.Rows)
            {
                int total = row.Field<int>("TOTAL");
                if (total >= 80)
                {
                    gradepoint = 5.0f;
                }
                if (total >= 75 & total < 80)
                {
                   gradepoint = 4.5f;
                }
                if (total >= 70 & total < 75)
                {
                     gradepoint = 4.0f;
                }
                if (total >= 65 & total < 70)
                {
                     gradepoint = 3.5f;
                }
                if (total >= 60 & total < 65)
                {
                     gradepoint = 3.0f;
                }
                if (total >= 55 & total < 60)
                {
                    gradepoint = 2.5f;
                }
                if (total >= 50 & total < 55)
                {    
                     gradepoint = 2.0f;
                }
                if (total < 50)
                {
                    gradepoint = 0.0f;
                }
                EW += row.Field<int>("CU");
                EWn += row.Field<int>("CU") * gradepoint;
            }
            return (float)Math.Round(EWn / EW, 2);
        }
        public static string As6Digits(int x)
        {
            if (x.ToString().Length == 1) return "00000" + x;
            if (x.ToString().Length == 2) return "0000" + x;
            if (x.ToString().Length == 3) return "000" + x;
            if (x.ToString().Length == 4) return "00" + x;
            if (x.ToString().Length == 5) return "0" + x;
            else return x.ToString();
        }


        internal static List<string> ProcessRetakenModules(DataTable table)
        {
            List<string> codes = (from x in table.Columns["CODE"].AsEnumerable<string>()
                                           group x by x into grouped
                                           where grouped.Count() > 1
                                           select grouped.Key).ToList();

            List<string> retakeEntryIDs = new List<string>();

            //1. List codes contains  codes of  redone modules  
            foreach ( string code in codes)
            {
                //1. Retrieve all rows with this code, largest total first
                List<DataRow> matchingRows = (from myRows in table.AsEnumerable()
                                              where myRows["CODE"].ToString() == code
                                              orderby Convert.ToInt32(myRows["TOTAL"]) descending
                                              select myRows).ToList();
                //2. use highest
                retakeEntryIDs.Add(matchingRows[0]["ENTRYID"].ToString());
                //3. Delete other rows
                foreach(DataRow row in matchingRows)
                {
                    if (matchingRows.IndexOf(row) > 0)
                    {
                        row.Delete();
                    }
                }

            }
            return retakeEntryIDs;
  
        }
        internal static int GetRetakes(DataTable results)
        {
            return 0;
        }

        public static DataTable RemoveRows(DataTable dt, string batch)
        {
            for(int i=0; i<dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["BATCH"].ToString() == batch)
                {
                    dt.Rows.RemoveAt(i);
                }
            }
            return dt;
        }
        
    }

}
