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
    public partial class ViewUsers: Window
    {
        DataTable dt = new DataTable();
        public ViewUsers()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataGridResults.IsReadOnly = true;
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


        private void exitMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void menuChange_Click(object sender, RoutedEventArgs e)
        {
            string username="";
            if (viewChkBox.IsChecked==false)
            {
               username  = (dataGridResults.SelectedItem as DataRowView).Row["USERNAME"].ToString();
            }
            else
            {
                username = (dataGridResults.SelectedItem as DataRowView).Row["VUREFNO"].ToString();
            }

            ChangePass changePass = new ChangePass();
            changePass.Title = username;
            string newPassWord = "";
            if (changePass.ShowDialog() == true)
            {
                newPassWord = changePass.NewPassword;
             
               if( VUARMSFBDataHelper.UpdateUser(username, Hasher.CreateHash(newPassWord)))
                {
                    MessageBox.Show("The password for user " + username+ " was updated.", 
                        "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            
        }

      
        private void menuPrint_Click(object sender, RoutedEventArgs e)
        {
          
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            dt = VUARMSFBDataHelper.GetStudents();
            dataGridResults.ItemsSource = dt.DefaultView;
        }

        private void viewChkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            dt = VUARMSFBDataHelper.GetStaff();
            dataGridResults.ItemsSource = dt.DefaultView;
        }
    }
}