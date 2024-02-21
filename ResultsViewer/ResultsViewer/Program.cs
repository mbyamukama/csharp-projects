using System;
using System.IO;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Linq;

namespace ResultsViewer
{
    class Program
    {
        public static void Serialize(DataTable dt, Stream stream)
        {
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(stream, dt);
        }

        public static DataTable Deserialize(Stream stream)
        {
            BinaryFormatter serializer = new BinaryFormatter();
            return (DataTable)serializer.Deserialize(stream);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("\t\tWELCOME TO THE ELE4112 CAT RESULTS AUTHENTICATION INTERFACE");
            Console.WriteLine("\t\tPLEASE TYPE ALL RESPONSES IN UPPERCASE CHARACTERS");
            DataTable dt = Deserialize(new FileStream("results.bin", FileMode.Open));

            while (true)
            {

                Console.Write("Enter your Reg. Number:\t");
                string regNo = Console.ReadLine();

                foreach (DataRow row in dt.Rows)
                {
                    if (row["REGNO"].ToString() == regNo)
                    {
                        Console.WriteLine("Welcome " + row["FULLNAME"].ToString());
                        Console.Write("Enter your group name:\t");
                        string grpName = Console.ReadLine();
                        if (grpName == row["GROUPNAME"].ToString().Trim())
                        {
                            Console.WriteLine("Group verified.");

                            IEnumerable<DataRow> matchingRows = from myRows in dt.AsEnumerable()
                                                                where myRows.Field<string>("GROUPNAME") == grpName
                                                                select myRows;
                            if (matchingRows.Count() == 0)
                            {
                                Console.WriteLine("This group does not have any members.");
                            }
                            else
                            {
                                Console.WriteLine("Enter at least 3 successive characters of any name of any one of your group mates.");
                                Console.WriteLine("For example: BYA or IMUS are valid for MAXIMUS BYAMUKAMA");
                                Console.Write("Enter the characters:\t");
                                string name = Console.ReadLine();

                                foreach (DataRow r in matchingRows)
                                {
                                    if (name.Length >= 3 && r["FULLNAME"].ToString().Contains(name) && r["REGNO"].ToString() != regNo)
                                    {
                                        Console.WriteLine("Matching Result: " + r.Field<string>("FULLNAME"));
                                        Console.WriteLine("The Simple Authentication Process was successfull.");
                                        Console.WriteLine("Your results are below.");
                                        Console.WriteLine("\n********************************************************************\n");

                                        foreach (DataColumn col in dt.Columns)
                                        {
                                            Console.WriteLine(col.ColumnName + " : " + row[col].ToString());
                                        }
                                        Console.WriteLine("Your combined Continuous Assessment Result is: " +
                                            (Convert.ToDouble(row["CW1"]) + Convert.ToDouble(row["CW2"])) * 0.5);
                                        break;
                                    }
                                }

                            }


                        }
                        else
                        {
                            Console.WriteLine("This is the wrong group name.");
                        }
                    }
                }
                Console.WriteLine("\n\n\n");
            }
        }
    }
}
