using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Documents;
using GenericUtilities;
using System.Collections.ObjectModel;
using System.Data;
using FirebirdSql.Data.FirebirdClient;

namespace VUARMS
{
    /// <summary>
    /// Interaction logic for InventoryWindow.xaml
    /// </summary>
    public partial class CourseInfo: Window
    {
        SDataTable dt = null;
        bool allowChange = false;

        public CourseInfo()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dt = VUARMSFBDataHelper.GetCourses();
            dt.ColumnChanged += Dt_ColumnChanged;
            dataGridResults.ItemsSource = dt.DefaultView;
                  
            if (AppUtilities.CurrentUser.CLRLevel < 4)
            {
                dataGridResults.Columns[6].Visibility = Visibility.Hidden;
            }
        }

        private void Dt_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            /*  if (AppUtilities.CurrentUser.Faculty != e.Row["DEPT"].ToString() & AppUtilities.CurrentUser.Faculty != "ALL")
              {
                  MessageBox.Show("Course Information for " + e.Row["DEPT"] + " can only be changed by the Dean of " + e.Row["DEPT"],
                      "ERROR", MessageBoxButton.OK, MessageBoxImage.Information);
              }
              else
              {
                  allowChange = true;    */
            if (this.IsLoaded)
            {
                VUARMSFBDataHelper.AddLog(AppUtilities.CurrentUser.UserName, AppUtilities.sessionID, "ACTION: Value Change for " + e.Row["COURSECODE"] +
                                 " COLUMN: " + e.Column.ColumnName +
                                 " ORIGINAL: " + e.Row[e.Column, DataRowVersion.Original] +
                                 " FINAL: " + e.Row[e.Column, DataRowVersion.Proposed]);
            }
        }
        

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
           // if (allowChange)
            {
                int affectedRows = dt.UpdateSource();
                if (affectedRows > 0)
                {
                    MessageBox.Show("The update was successful.\n" + affectedRows + " rows were affected", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                }
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

        private void menuDelete_Click(object sender, RoutedEventArgs e)
        {
            if (AppUtilities.CurrentUser.CLRLevel > 3)
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
            else
            {
                MessageBox.Show("Deleting a course leads to the deletion of results under the course.\n"+
                    "Currently, only ADMIN has the credentials to perform this action because the  backup system is incomplete.", "AUTH DENIED", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void menuViewResults_Click(object sender, RoutedEventArgs e)
        {
            string code = (dataGridResults.SelectedItem as DataRowView).Row["COURSECODE"].ToString();
            int affRes = VUARMSFBDataHelper.GetResultCount(code);
            MessageBox.Show("There are "+affRes+ " results under this module.", "RESULT COUNT", MessageBoxButton.OK, MessageBoxImage.Information);

        }
    }
}