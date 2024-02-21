using System;
using System.Collections.Generic;
using System.Linq;      
using System.Windows;  
using System.Data;
using System.Collections.ObjectModel;
using GenericUtilities;

namespace VUARMS
{
    /// <summary>
    /// Interaction logic for CreateGrid.xaml
    /// </summary>
    public partial class CreateGrid : Window
    {
        string fileName = "";
        public CreateGrid()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbxFaculty.ItemsSource = new ObservableCollection<string>((VUARMSFBDataHelper.GetFaculties()));
            cbxCohort.ItemsSource = new ObservableCollection<string>((VUARMSFBDataHelper.GetCohorts()));
            cbxBatch.ItemsSource = new ObservableCollection<string>((VUARMSFBDataHelper.GetBatches()));
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            fileName = cbxFaculty.Text + cbxCohort.Text;
            DataTable rawGrid = VUARMSFBDataHelper.GetRawGrid(cbxFaculty.Text, cbxCohort.Text, cbxBatch.Text);
            //get distinct coursecodes
            IEnumerable <string> distinctCodes = rawGrid.Columns["COURSECODE"].AsEnumerable<string>().Distinct();
            IEnumerable<string> distinctRefNos = rawGrid.Columns["VUREFNO"].AsEnumerable<string>().Distinct();

            DataTable grid = new DataTable();
            grid.Columns.Add("VUREFNO");
            grid.Columns.Add("NAME");
            grid.Columns.Add("PROGRAMME");
            foreach (string item in distinctCodes)
            {
                grid.Columns.Add(item);
            }

            grid.Columns.Add("GPA");
            grid.Columns.Add("CGPA");
            grid.Columns.Add("COMMENTS");

            //add results
            foreach (string item in distinctRefNos)
            {
                DataRow row = grid.NewRow();
                row["VUREFNO"]=item;
                grid.Rows.Add(row);
            }

            foreach (DataRow row in grid.Rows)
            {
                string vurefno = row["VUREFNO"].ToString();
                DataRow details = VUARMSFBDataHelper.GetStudentDetails(vurefno).Rows[0];
                row["NAME"] = details["FULLNAME"];
                row["PROGRAMME"] = details["PROGRAMME"];

                foreach (string item in distinctCodes)
                {
                    IEnumerable<DataRow> rows = from myRows in rawGrid.AsEnumerable()
                                                where (myRows["VUREFNO"].ToString() == vurefno & myRows["COURSECODE"].ToString() == item)
                                                select myRows;
                    if (rows.Count() > 0)
                    {
                        row[item] = rows.ElementAt(0)["TOTAL"];
                    }
                    else
                        row[item] = null;

                }

                DataTable allResults = VUARMSFBDataHelper.GetResults(vurefno, true, "ALL");
                row["GPA"] = AppUtilities.CalculateGPA(VUARMSFBDataHelper.GetResults(vurefno, false, cbxBatch.Text));
                row["CGPA"] = AppUtilities.GetCGPA(allResults);

                //check if retakes exist
                int count = (from myrows in allResults.AsEnumerable()
                             where myrows.Field<int>("TOTAL") < 50
                             select myrows).Count();
                if (count > 0)
                {
                    row["COMMENTS"] = "PP: ";
                    foreach (DataRow entry in allResults.Rows)
                    {
                        if (entry.Field<int>("TOTAL") < 50)
                        {
                            row["COMMENTS"] = row["COMMENTS"] + entry.Field<string>("COURSECODE") + "; ";
                        }
                    }
                    row["COMMENTS"] = row["COMMENTS"].ToString().Remove(row["COMMENTS"].ToString().Length - 2);

                    if (Convert.ToDouble(row["CGPA"]) < 2)
                    {
                        row["COMMENTS"] = row["COMMENTS"].ToString() + " : LOW CGPA";

                        //add previous CGPAs

                        DataTable temp = AppUtilities.RemoveRows(allResults, "8.2016");
                        float pcgpa = AppUtilities.GetCGPA(temp);
                        row["COMMENTS"] = row["COMMENTS"].ToString() + "  PRV: " + pcgpa + " ; ";

                        temp = AppUtilities.RemoveRows(temp, "1.2016");
                        pcgpa = AppUtilities.GetCGPA(temp);
                        row["COMMENTS"] = row["COMMENTS"].ToString() + pcgpa;
                    }

                }



            }

            foreach (DataColumn col in grid.Columns)
            {
                if (distinctCodes.Contains(col.ColumnName))
                {
                    col.ColumnName = col.ColumnName+" - "+ VUARMSFBDataHelper.GetCourseName(col.ColumnName);
                }
            }

            //now get student details and course details

            gridDataGrid.ItemsSource = grid.DefaultView;

        }

        private void menuExport_Click(object sender, RoutedEventArgs e)
        {
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = fileName; // Default file name
            dlg.DefaultExt = ".csv"; // Default file extension
            //dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension 

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results 
            if (result == true)
            {
                // Save document 
                Utilities.WriteCSVFile(((DataView)gridDataGrid.ItemsSource).ToTable(),dlg.FileName);
            }
        }
    }
}
