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
    public partial class ViewStudents : Window
    {
        SDataTable dt = null;
       
        public ViewStudents()
        {
            InitializeComponent();
        }

        void dt_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (this.IsLoaded)
            {
                VUARMSFBDataHelper.AddLog(AppUtilities.CurrentUser.UserName, AppUtilities.sessionID, "ACTION: Value Change for" + e.Row["VUREFNO"] +
                                 " COLUMN: " + e.Column.ColumnName +
                                 " ORIGINAL: " + e.Row[e.Column, DataRowVersion.Original] +
                                 " FINAL: " + e.Row[e.Column, DataRowVersion.Proposed]);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
                dt = VUARMSFBDataHelper.GetAllStudents();
                dt.ColumnChanged += dt_ColumnChanged;

            dataGridResults.ItemsSource = dt.DefaultView;

            if (AppUtilities.CurrentUser.CLRLevel < 4)
            {
               menuChangePass.Visibility = Visibility.Hidden;
            }
            if (AppUtilities.CurrentUser.CLRLevel < 3)
            {
                menuDelete.Visibility  = Visibility.Hidden;
            }
            if (AppUtilities.CurrentUser.CLRLevel < 2)
            {
                menuViewResults.Visibility  =  Visibility.Hidden;
            }
        }


        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            int affectedRows = dt.UpdateSource();
            if (affectedRows > 0)
            {
                MessageBox.Show("The update was successful.\n" + affectedRows + " rows were affected", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void txtItemName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string sQuery = txtSearch.Text.Trim();
            try
            {
                EnumerableRowCollection<DataRow> qresult = from rtndRows in dt.AsEnumerable()
                                                           where rtndRows["VUREFNO"].ToString().ToUpper().Contains(sQuery.ToUpper()) ||
                                                           rtndRows["FULLNAME"].ToString().ToUpper().Contains(sQuery.ToUpper())
                                                           || rtndRows["PROGRAMME"].ToString().ToUpper().Contains(sQuery.ToUpper())
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

        private void menuViewResults_Click(object sender, RoutedEventArgs e)
        {
            string vurefno = (dataGridResults.SelectedItem as DataRowView).Row["VUREFNO"].ToString();
            new StudentResults(vurefno).Show();
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

        private void menuChangePass_Click(object sender, RoutedEventArgs e)
        {
            ChangePass winChangePass = new ChangePass();
            String newPassHash = "";
            if (winChangePass.ShowDialog() == true)
            {
                newPassHash = Hasher.CreateHash(winChangePass.NewPassword);
               if(VUARMSFBDataHelper.UpdateUser(AppUtilities.CurrentUser.UserName, newPassHash))
               {
                   MessageBox.Show("Your password was updated successfully", "SUCCESS");
               }
            }
            
        }
    }
}