using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Windows.Documents;
using System.Windows.Media;
using GenericUtilities;
using System.Collections.Generic;

namespace VUARMS
{
    /// <summary>
    /// Interaction logic for Results.xaml
    /// </summary>
    public partial class StudentResults : Window
    {
        string vurefno = "", fullname="";
        float cgpa = 0.00F;
        DataTable results = null, trDetails = null;

        public StudentResults(string vurefno)
        {
            InitializeComponent();
            this.vurefno = vurefno;
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                results = VUARMSFBDataHelper.GetResults(vurefno, false, "ALL");
                trDetails = VUARMSFBDataHelper.GetTranscriptDetails(vurefno);
                fullname = trDetails.Rows[0]["FULLNAME"].ToString();
                lblName.Content = vurefno + " - " + fullname;

                results.Columns["RT"].ColumnName = "RETAKE";

                int sumCu = AppUtilities.GetTotalCredits(results);
                cgpa = AppUtilities.GetCGPA(results);

                DataTable toView = AppUtilities.AppendGrades(results);
                resultsDataGrid.ItemsSource = toView.DefaultView;
                lblCGPA.Content = "CGPA: " + Math.Round(cgpa, 2) + "     TOTAL CU:" + sumCu;
                resultsDataGrid.IsReadOnly = true;

                if (AppUtilities.CurrentUser.CLRLevel < 3)
                {
                    resultsDataGrid.ContextMenu.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\nDETAILS: " + ex.Message, "ERROR");
            }
        }


        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (chkFilter.IsChecked.Value)
            {
                results = VUARMSFBDataHelper.GetResults(vurefno, true, "ALL");
            }
            else
            {
                results = VUARMSFBDataHelper.GetResults(vurefno, false, "ALL");
            }
            results.Columns["RT"].ColumnName = "RETAKE";

            int sumCu = AppUtilities.GetTotalCredits(results);
            cgpa = AppUtilities.GetCGPA(results);

            DataTable toView = AppUtilities.AppendGrades(results);
            resultsDataGrid.ItemsSource = toView.DefaultView;
            lblCGPA.Content = "CGPA: " + Math.Round(cgpa, 2) + "     TOTAL CU:" + sumCu;
            resultsDataGrid.IsReadOnly = true;

            if (AppUtilities.CurrentUser.CLRLevel < 3)
            {
                resultsDataGrid.ContextMenu.Visibility = Visibility.Hidden;
            }

        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FixedPage page = new FixedPage();
                int vPointer = 30, leftMargin = 75, rowheight = 22;

                Transcript tHeader = new Transcript();
                string date = trDetails.Rows[0]["DOB"].ToString();
                date = date.Length > 0 ? date.Substring(0, 10) : "";

                tHeader.lblName.Content += fullname;
                tHeader.lblRefNo.Content += trDetails.Rows[0]["VUREFNO"].ToString();
                tHeader.lblProgram.Content += trDetails.Rows[0]["PFNAME"].ToString();
                tHeader.lblFaculty.Content += trDetails.Rows[0]["DEPT"].ToString();
                tHeader.lblDOB.Content += date;
                tHeader.lblNationality.Content += trDetails.Rows[0]["NATIONALITY"].ToString();

                tHeader.Margin = new Thickness(leftMargin, vPointer, 0, 0);

                DataTable toPrintTable = results.Clone();     //just for printing purposes

                toPrintTable.Columns["TOTAL"].DataType = typeof(string);

                foreach (DataRow row in results.Rows)
                {
                    toPrintTable.Rows.Add(row.ItemArray);
                }

                
                //print view
                toPrintTable.Columns["COURSECODE"].ColumnName = "CODE";
                toPrintTable.Columns["COURSENAME"].ColumnName = "COURSE NAME";

                List <string> retakeEntryIDs = AppUtilities.ProcessRetakenModules(toPrintTable);

                foreach(DataRow row in toPrintTable.Rows)
                {
                    if (retakeEntryIDs.Contains(row["ENTRYID"].ToString()))
                    {
                        //this row has a retaken module
                        row["TOTAL"] = row["TOTAL"] + " (R)";
                    }
                }

                toPrintTable.Columns.Remove("ENTRYID");
                toPrintTable.Columns.Remove("BATCH");
                toPrintTable.Columns.Remove("RETAKE");

                page.Children.Add(tHeader);
                Grid grid = Utilities.LabelTable(toPrintTable, new Thickness(1), 10, "Arial", rowheight, new int[] { 70, 350, 50, 70, 60, 80 },
                    new string[] { "L", "L", "C", "C", "L", "C", "C", });
                vPointer += (int)tHeader.Height;
                grid.Margin = new Thickness(leftMargin, vPointer, 0, 0);

                page.Children.Add(grid);

                vPointer += results.Rows.Count * rowheight + 30;
                Label labelCGPA = new Label()
                {
                    Content = "CGPA: " + Math.Round(cgpa, 2) + "\t\t\t\t\t\t\t\t\tACCUMULATED LOAD (CU): " + results.Columns["CU"].AsEnumerable<int>().Sum(),
                    Margin = new Thickness(leftMargin, vPointer, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    FontFamily = new FontFamily("Times New Roman"),
                };

                page.Children.Add(labelCGPA);

                vPointer += 20;
                Label labelSign = new Label()
                {
                    Content = "\t\t\tACADEMIC REGISTRAR ",
                    FontFamily = new FontFamily("Arial"),
                    FontSize = 11,
                    Margin = new Thickness(350, vPointer, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                page.Children.Add(labelSign);

                vPointer += 20;
                MessageBox.Show(vPointer.ToString());
                Label labelDisclaimer = new Label()
                {
                    Content = "*This is a partial transcript. Some details may change before graduation.",
                    FontFamily = new FontFamily("Arial"),
                    FontSize = 10,
                    Margin = new Thickness(50, vPointer, 0, 0),
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center
                };
                page.Children.Add(labelDisclaimer);

                PageContent pc = new PageContent();
                pc.Child = page;
                FixedDocument doc = new FixedDocument();
                doc.Pages.Add(pc);

                PrintDialog pd = new PrintDialog();
                if (pd.ShowDialog() == true)
                {
                    pd.PrintDocument(doc.DocumentPaginator, vurefno);
                }
            }
           catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\n\nDETAILS: " + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void resultsDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            // Get the DataRow corresponding to the DataGridRow that is loading.
            DataRowView item = e.Row.Item as DataRowView;
            if (item != null)
            {
                DataRow row = item.Row;
                if(Convert.ToInt32(row["TOTAL"])<50)
                {
                    e.Row.Foreground = new SolidColorBrush(Colors.Red);
                    e.Row.FontWeight = FontWeights.Bold;
                }
               
            }	
        }

        private void menuChange_Click(object sender, RoutedEventArgs e)
        {
            DataRow row = (resultsDataGrid.SelectedItem as DataRowView).Row;
            string coursecode = row["COURSECODE"].ToString();
            string name = row["COURSENAME"].ToString();
            int total = Convert.ToInt32(row["TOTAL"]);

            ConfirmPass win = new ConfirmPass();
            string password = "";
            if (win.ShowDialog() == true)
            {
                password = win.Password;
            }

           bool authenticated = Hasher.ValidatePassword(password, AppUtilities.CurrentUser.HPass);
           if (authenticated)
           {
               string details = "Change Result for " + vurefno + "-" + fullname + "-" + "-" + coursecode + "-" + name;
               ChangeResult changeRes = new ChangeResult(details, total);
               changeRes.ShowDialog();
               int newresult = total;
               if (changeRes.DialogResult == true)
               {
                   newresult = changeRes.NewResult;
                  int affrows= VUARMSFBDataHelper.ChangeResult(vurefno, coursecode, newresult);
                  if (affrows > 0)
                  {
                      VUARMSFBDataHelper.AddLog(AppUtilities.CurrentUser.UserName, AppUtilities.sessionID, "Student Result Change: " + vurefno + ": " + " OLD: " + total + ": NEW: " + newresult);
                      MessageBox.Show("The result was updated.", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                  }
               }
           }
            else
           {
               VUARMSFBDataHelper.AddLog(AppUtilities.CurrentUser.UserName.ToUpper(), AppUtilities.sessionID, "Failed Authentication to Change Result.");
               MessageBox.Show("user authentication failed.\n Please try again.", "ERROR");
           }
        }


        private void menuAdd_Click(object sender, RoutedEventArgs e)
        {
            new AddResult(vurefno, fullname).Show();
        }

        private void menuDelete_Click(object sender, RoutedEventArgs e)
        {
            DataRow row = (resultsDataGrid.SelectedItem as DataRowView).Row;
            string coursecode = row["COURSECODE"].ToString();
            string name = row["COURSENAME"].ToString();
            int total = Convert.ToInt32(row["TOTAL"]);

            ConfirmPass win = new ConfirmPass();
            string password = "";
            if (win.ShowDialog() == true)
            {
                password = win.Password;
            }

            bool authenticated = Hasher.ValidatePassword(password, AppUtilities.CurrentUser.HPass);
            if (authenticated)
            {
                string details = "Delete Result for " + vurefno + "-" + fullname + "-" + "-" + coursecode + "-" + name;
                if (MessageBox.Show("The following operation has been requested. Do you wish to proceed?\n\n" + details, "CONFIRM",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    int affrows = VUARMSFBDataHelper.DeleteResult(vurefno, coursecode, total);
                    if (affrows > 0)
                    {
                        VUARMSFBDataHelper.AddLog(AppUtilities.CurrentUser.UserName, AppUtilities.sessionID, "Student Result Delete: " + vurefno + ": " + " CODE: " + coursecode);
                        MessageBox.Show("The result was deleted.", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            else
            {
                VUARMSFBDataHelper.AddLog(AppUtilities.CurrentUser.UserName.ToUpper(), AppUtilities.sessionID, "Failed Authentication to Change Result.");
                MessageBox.Show("user authentication failed.\n Please try again.", "ERROR");
            }
        }
    }
}

