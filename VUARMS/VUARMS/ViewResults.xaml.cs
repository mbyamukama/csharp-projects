using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;     
using System.Windows.Controls; 
using GenericUtilities;            
using System.Data;

namespace VUARMS
{
    public partial class ViewResults : Window
    {
        SDataTable dt = null;

        public ViewResults()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dt = VUARMSFBDataHelper.GetAllResults();
            dt.ColumnChanged += Dt_ColumnChanged;

            dataGridResults.ItemsSource = dt.DefaultView;

            if (AppUtilities.CurrentUser.CLRLevel < 3)
            {
                mainMenu.Visibility = menuDelete.Visibility =  btnUpdate.Visibility = Visibility.Hidden;
                dataGridResults.IsReadOnly = true;
            }
        }

        private void Dt_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (this.IsLoaded)
            {
                VUARMSFBDataHelper.AddLog(AppUtilities.CurrentUser.UserName, AppUtilities.sessionID, "ACTION: Value Change for: " + e.Row["ENTRYID"] +
                                 " COLUMN: " + e.Column.ColumnName +
                                 " ORIGINAL: " + e.Row[e.Column, DataRowVersion.Original] +
                                 " FINAL: " + e.Row[e.Column, DataRowVersion.Proposed]);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            int affectedRows = dt.UpdateSource();
            if (affectedRows > 0)
            {
                MessageBox.Show("The update was successful.\n" + affectedRows + " rows were affected","SUCCESS",MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void txtItemName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string sQuery = txtSearch.Text.Trim();
            try
            {
                EnumerableRowCollection<DataRow> qresult = from rtndRows in dt.AsEnumerable()
                                                           where rtndRows[0].ToString().ToUpper().Contains(sQuery.ToUpper()) ||
                                                           rtndRows[1].ToString().ToUpper().Contains(sQuery.ToUpper())
                                                           || rtndRows[2].ToString().ToUpper().Contains(sQuery.ToUpper())
                                                           select rtndRows;
                if (qresult.Count() > 0)
                {
                    dataGridResults.ItemsSource = qresult.AsDataView<DataRow>();
                    lblCount.Content = qresult.Count() + " items returned";
                }
                else
                {
                    MessageBox.Show("The search has not returned any results", "ERROR");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unknown error occurred. Searching in this information may not be enabled yet.\n DETAILS:" + ex.Message, "ERROR");
            }
        }

        private void menuViewNames_Click(object sender, RoutedEventArgs e)
        {

        }
        private void menuDelete_Click(object sender, RoutedEventArgs e)
        {
            List<DataRow> rows = new List<DataRow>();

            if (MessageBox.Show("Are you sure you want to delete the selected items?", "DELETE?",
                MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                foreach (var item in dataGridResults.SelectedItems)
                {
                    DataRow row = (item as DataRowView).Row;
                    rows.Add(row);
                }
                foreach (DataRow row in rows)
                {
                    try
                    {
                        dt.Rows[dt.Rows.IndexOf(row)].Delete();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred and the delete operation failed.\n" +
                            "DETAILS:" + ex.Message, "DELETE ERROR");
                    }

                }
                int affRows = dt.UpdateSource();
                MessageBox.Show(affRows + " rows were deleted.", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);

            }
        }

        private void dataGridResults_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(DateTime))
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "{0:dd/MM/yyyy}";
            }
        }

        private void exitMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
                if (dt.GetChanges() != null)
                {
                    MessageBoxResult result= MessageBox.Show("You have pending changes.\nPress YES to commit , NO to discard and CANCEL to cancel",
                    "PENDING CHANGES?", MessageBoxButton.YesNoCancel);
                    if(result==MessageBoxResult.Yes)
                    {
                        btnUpdate_Click(null, null);
                    }
                    else if (result == MessageBoxResult.Cancel)
                    {
                        e.Cancel = true;
                    }
                }
        }

        private void menuImportResults_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                string fileName = "";
                if (ofd.ShowDialog() == true)
                {
                    fileName = ofd.FileName;
                }
                DataTable results = Utilities.ReadCSVFile(fileName);


                if (MessageBox.Show("You'll now be asked to select the module for which the results are to be imported", "CONFIRM",
                    MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
                {
                    Selector selector = new Selector();
                    string coursecode = "";
                    if (selector.ShowDialog() == true)
                    {
                        coursecode = selector.SelectedItems[0];
                    }
                    Import importWindow = new Import(results, coursecode, 0);
                    importWindow.Show();
                }
            }
            catch (Exception ex)
            {
                VUARMSFBDataHelper.ShowErrorWindow(ex);
            } 
        }

        private void menuCreateGrids_Click(object sender, RoutedEventArgs e)
        {
            new CreateGrid().Show();
        }
        private void menuDeansList_Click(object sender, RoutedEventArgs e)
        {
            new DataViewer("DEAN").Show();
        }

        private void dataGridResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
          /*  DataRow row = null;
            if ((dataGridResults.SelectedItem as DataRowView) != null)
            {
                row = (dataGridResults.SelectedItem as DataRowView).Row;
                string name = VUARMSFBDataHelper.GetStudentDetails(row["VUREFNO"].ToString()).Rows[0]["FULLNAME"].ToString();
                string course = VUARMSFBDataHelper.GetCourseName(row["COURSECODE"].ToString());

                infoLabel.Content = "         " + name + " -- " + course;
            }*/

        }

        private void menuStats_Click(object sender, RoutedEventArgs e)
        {
            //1. get distinct courses
            var myrows = from myRows in dt.AsEnumerable()
                         where myRows["BATCH"].ToString() == "8.2016"
                         select myRows;

            DataTable temp = myrows.CopyToDataTable();
            List<string> list = temp.AsEnumerable().Select(r => r.Field<string>("COURSECODE")).Distinct().ToList();

            DataTable results = new DataTable();
            results.Columns.Add("CODE");
            results.Columns.Add("NAME");
            results.Columns.Add("AVERAGE");

            foreach(string code in list)
            {
                double average = (from myRows in temp.AsEnumerable()
                               where myRows["COURSECODE"].ToString() == code
                               select myRows).Average(r => r.Field<int>("TOTAL"));

                results.Rows.Add(code, VUARMSFBDataHelper.GetCourseName(code), average);
                Utilities.WriteCSVFile(results, @"D:\results.csv");
                                 
            }

        }
    }
}