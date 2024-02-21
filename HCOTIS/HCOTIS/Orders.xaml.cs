using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections.ObjectModel;
using System.Data;

namespace HCOTIS
{
   
    public partial class Orders: Window
    {
        SDataTable dt = null;
       
        public Orders()
        {
            InitializeComponent();
        }

        private void menuMakePayment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRow selectedRow = (dataGridResults.SelectedItem as DataRowView).Row;
                string selectedOrderId = selectedRow["ORDERID"].ToString();

                int balance = Convert.ToInt32(selectedRow["BALANCE"]);
                string otherDetails = "CLIENT: " + selectedRow["CLIENTNAME"] + "  PHONE: " + selectedRow["PHONENO"] + "  BAL: " + balance;


                if (balance > 0)
                {
                    AddPayment payWindow = new AddPayment(selectedOrderId, balance, otherDetails);
                    payWindow.Show();
                }
                else
                    MessageBox.Show("This order has already been fully paid.", "PAYMENT", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\n DETAILS: " + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void menuViewPayments_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string orderid = (dataGridResults.SelectedItem as DataRowView).Row["ORDERID"].ToString();
                new Payments(orderid).Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\n DETAILS: " + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void menuDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRow selectedRow = (dataGridResults.SelectedItem as DataRowView).Row;
                if (MessageBox.Show("Are you sure you want to delete this item?\n" +
                    selectedRow[0],
                    "DELETE?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    selectedRow.Delete();
                    dt.UpdateSource();
                    MessageBox.Show("Item deleted.", "SUCCESS");

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred and the delete operation failed.\n" +
                    "DETAILS:" + ex.Message, "DELETE ERROR");
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int affrows = dt.UpdateSource();
                MessageBox.Show("Update Successful.\n" + affrows + " items were changed.", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred during the update.\nDETAILS: " + ex.Message, "ERROR");
            }
        }

        private void menuAddNew_Click(object sender, RoutedEventArgs e)
        {
            new NewOrder().Show();                  
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            dt = FBDataHelper.GetOrders(fromdtPicker.SelectedDate.Value, todtPicker.SelectedDate.Value, false);
            dataGridResults.ItemsSource = dt.DefaultView;
            lblCount.Content = "Count= "+ dt.Rows.Count;
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string dName = txtSearch.Text.Trim();
            try
            {
                EnumerableRowCollection<DataRow> qresult = from rtndRows in dt.AsEnumerable()
                                                           where 
                                                           rtndRows[0].ToString().ToUpper().Contains(dName.ToUpper()) ||
                                                           rtndRows[1].ToString().ToUpper().Contains(dName.ToUpper()) ||
                                                           rtndRows[2].ToString().ToUpper().Contains(dName.ToUpper()) ||
                                                           rtndRows[3].ToString().ToUpper().Contains(dName.ToUpper()) ||
                                                           rtndRows[7].ToString().ToUpper().Contains(dName.ToUpper()) ||
                                                           rtndRows[8].ToString().ToUpper().Contains(dName.ToUpper()) ||
                                                           rtndRows[10].ToString().ToUpper().Contains(dName.ToUpper()) ||
                                                           rtndRows[11].ToString().ToUpper().Contains(dName.ToUpper()) ||
                                                           rtndRows[12].ToString().ToUpper().Contains(dName.ToUpper()) ||
                                                           rtndRows[13].ToString().ToUpper().Contains(dName.ToUpper()) ||
                                                           rtndRows[15].ToString().ToUpper().Contains(dName.ToUpper()) 
                                                           select rtndRows;
                if (qresult.Count() > 0)
                {
                    dataGridResults.ItemsSource = qresult.AsDataView<DataRow>();
                    lblCount.Content = qresult.Count() + " items";
                }
                else
                {
                    MessageBox.Show("The search has not returned any results", "ERROR");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unknown error occurred. \n DETAILS:" + ex.Message, "ERROR");
            }
        }

        private void exitMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dataGridResults_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(DateTime))
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "{0:dd/MM/yyyy}";
            }
            if (e.PropertyType == typeof(Int32) && e.Column.DisplayIndex != 1)
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "N0";
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                todtPicker.SelectedDate = DateTime.Now;
                fromdtPicker.SelectedDate = DateTime.Now.AddDays(-90);

                dt = FBDataHelper.GetOrders(DateTime.Now.AddDays(-365), DateTime.Now, true);
                dataGridResults.ItemsSource = dt.DefaultView;
                lblCount.Content = "Count= " + dt.Rows.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\n DETAILS: " + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void menuViewCake_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string orderid = (dataGridResults.SelectedItem as DataRowView).Row["ORDERID"].ToString();
                new ViewCake(orderid).Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\n DETAILS: " + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
