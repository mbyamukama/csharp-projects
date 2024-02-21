using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GenericUtilities;
using System.Data;

namespace VUARMS
{
    /// <summary>
    /// Interaction logic for Import.xaml
    /// </summary>
    public partial class Import : Window
    {
        DataTable dt = null;
        int importOption = 0, rowcount = 0;
        string coursecode = "";
        public Import(DataTable results, string coursecode, int option)
        {
            InitializeComponent();
            dt = results;
            this.coursecode = coursecode;
            this.importOption = option;
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            string error = "", batchId = DateTime.Now.Month.ToString() + "." + DateTime.Now.Year.ToString(), vuRefNo = "";
            System.IO.StreamWriter sw = new System.IO.StreamWriter("log.txt");
            int rowAddSucess = 0, affRows = 0, cw = 0, exam = 0, total = 0;

            if (importOption == 0) //module based import
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row["VUREFNO"].ToString() != "")            //blanks in csv file
                    {
                        error = "";
                        try
                        {
                            vuRefNo = row["VUREFNO"].ToString().Trim().Replace(" ", "");
                            cw = row["CW"].ToString().Trim(' ') == "" ? 0 : Convert.ToInt32(row["CW"]);
                            exam = row["EXAM"].ToString().Trim(' ') == "" ? 0 : Convert.ToInt32(row["EXAM"]);
                            total = row["TOTAL"].ToString().Trim(' ') == "" ? 0 : Convert.ToInt32(row["TOTAL"]);
                            rowAddSucess = VUARMSFBDataHelper.InsertResult(vuRefNo, coursecode, cw, exam, total, batchId);//writes to log!!
                            affRows += rowAddSucess;
                            if (rowAddSucess == 0)  //an exception occurred
                            {
                                continue;
                            }
                        }
                        catch (Exception ex)
                        {
                            error += vuRefNo + ";" + batchId + ";" + ex.Message;
                            AppUtilities.errors.Add(error);
                        }
                    }
                }
            }
            if (importOption == 1)  //grid based import
            {
                //start import process
                //Look for non-matching records
                foreach (DataRow row in dt.Rows)
                {
                    error = "";
                    try
                    {
                        vuRefNo = row["VUREFNO"].ToString().Trim().Replace(" ", "");
                        foreach (DataColumn col in dt.Columns)
                        {
                            if (!(col.ColumnName == "VUREFNO" || col.ColumnName == "NAME"))
                            {
                                //get course code --code to follow
                                 coursecode = col.ColumnName.ToString().Replace(" ", "");
                                total = 0;
                                if (!(row[col] == DBNull.Value || row[col].ToString() == ""))
                                {
                                    total = (int) Math.Round(Convert.ToDouble(row[col]), 0);
                                    affRows += VUARMSFBDataHelper.InsertResult(vuRefNo, coursecode, 0, 0, total, batchId);
                                }
                                else continue;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        error += vuRefNo + ";" + batchId + ";" + ex.Message;
                        AppUtilities.errors.Add(error);
                    }
                }
            }
            foreach (string item in AppUtilities.errors)
            {
                sw.WriteLine(error);
            }
            sw.Close();
            MessageBox.Show("Imported " + affRows + "of " + rowcount, "RESULT", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dt.Columns.Contains("VUREGNO"))
                {
                    dt.Columns["VUREGNO"].ColumnName = "VUREFNO";
                }     
                dataGridResults.ItemsSource = dt.DefaultView;

                foreach(DataRow row in dt.Rows)
                {
                    row["VUREFNO"] = row["VUREFNO"].ToString().Trim().Replace(" ", "");
                    if (row["VUREFNO"].ToString()[2] == '-')
                    {
                        row["VUREFNO"] = row["VUREFNO"].ToString().Remove(2, 1);
                    }

                    row["CW"] = row["CW"].ToString().Trim()=="" | row["CW"].ToString().Contains("-")? 0:  Math.Round(Convert.ToDouble(row["CW"]));
                    row["EXAM"] = row["EXAM"].ToString().Trim() == "" | row["EXAM"].ToString().Contains("-") ? 0  : Math.Round(Convert.ToDouble(row["EXAM"]));
                    row["TOTAL"] = Math.Round(Convert.ToDouble(row["TOTAL"]));
                }
                rowcount = (from myRows in dt.AsEnumerable()
                            where myRows["VUREFNO"].ToString().Trim() != ""
                            select myRows).Count();
                lblCount.Content = "Valid Records: " + rowcount;
            }
            catch (Exception ex)
            {
                VUARMSFBDataHelper.ShowErrorWindow(ex);
            }      
        }

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            List<string> notFound = new List<string>();
            string list = "";
            if (importOption == 1)
            {
                List<string> allCodes = VUARMSFBDataHelper.GetCourseCodes();
                List<string> currentCodes = new List<string>();
                foreach (DataColumn col in dt.Columns)
                {
                    if (!(col.ColumnName == "VUREFNO" || col.ColumnName == "NAME"))
                    {
                        //get course code --code to follow
                        string coursecode = col.ColumnName.ToString().Replace(" ", "");
                        currentCodes.Add(coursecode);
                    }
                }
                notFound = currentCodes.Except(allCodes).ToList();
                list = "";
                foreach (string item in notFound)
                {
                    list += item + "\n";
                }
                MessageBox.Show(list, "NOT FOUND CODES");
            }

            List<string> allRefNos = VUARMSFBDataHelper.GetAllStudents().Columns["VUREFNO"].AsEnumerable<string>();
            List<string> currentStudents = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                currentStudents.Add(row["VUREFNO"].ToString().Replace(" ", ""));
            }

            notFound = currentStudents.Except(allRefNos).ToList();
            list = "";
            foreach (string item in notFound)
            {
                list += item + "\n";
            }
            if (MessageBox.Show(list, "VUREFNOs NOT FOUND. PROCEED TO ADD TO DATABASE?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                int affRows = 0;
                foreach (string refNo in notFound)
                {
                    var fullname = (from myRows in dt.AsEnumerable()
                                    where myRows["VUREFNO"].ToString() == refNo
                                    select myRows).ElementAt(0)["FULLNAME"].ToString();
                    affRows += VUARMSFBDataHelper.AddVUREFNO(refNo, fullname);
                }
                MessageBox.Show(affRows + " affected");
            }


        }
    }
}

