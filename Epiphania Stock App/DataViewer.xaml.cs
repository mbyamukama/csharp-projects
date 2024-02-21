using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using GenericUtilities;

namespace StockApp
{
    /// <summary>
    /// Interaction logic for InventoryWindow.xaml
    /// </summary>
    public partial class DataViewer : Window
    {
        SDataTable dt = null;
        EnumerableRowCollection<DataRow> qresult = null;
        string param = "";
        bool labelSet = false;

        public DataViewer(string parameter)
        {
            InitializeComponent();
            param = parameter;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            menuViewDetails.IsEnabled = menuAlterFactor.IsEnabled = menuPay.IsEnabled = false;
            if (param == "SALES")
            {
                fromdtPicker.IsEnabled = todtPicker.IsEnabled = btnGo.IsEnabled = true;
                dt = FBDataHelper.GetSales(DateTime.Now.AddDays(-30), DateTime.Now);
                menuViewDetails.IsEnabled = true;
            }
            if (param == "STOCK")
            {
                fromdtPicker.IsEnabled = todtPicker.IsEnabled = btnGo.IsEnabled = false;
                menuAlterFactor.IsEnabled = true;
                dt = FBDataHelper.GetStock(FBDataHelper.StockType.Basic, DateTime.Now, DateTime.Now);
            }
            if (param == "SUPPLIERS")
            {
                fromdtPicker.IsEnabled = todtPicker.IsEnabled = btnGo.IsEnabled = false;
                menuDelete.IsEnabled = menuPay.IsEnabled = true;
                dt = FBDataHelper.GetSuppliers();
                dataGridResults.FontSize = 20;
            }
            if (param == "USERS")
            {
                fromdtPicker.IsEnabled = todtPicker.IsEnabled = btnGo.IsEnabled = menuViewStatement.IsEnabled = false;
                dt = FBDataHelper.GetEmployees();
                dataGridResults.FontSize = 14;
            }
            if (param == "EXPENSES")
            {
                dt = FBDataHelper.GetExpenses(DateTime.Now.AddDays(-30), DateTime.Now);
                dataGridResults.FontSize = 14;
            }
            if (param == "LOG")
            {
                dt = FBDataHelper.GetLog(DateTime.Now.AddDays(-30), DateTime.Now);
                dataGridResults.FontSize = 14;
            }
            if (param == "PAYMENTS")
            {
                dt = FBDataHelper.GetPayments(true, "", false);
                dataGridResults.FontSize = 14;
            }
            if (param == "PURCHASES")
            {
                dt = FBDataHelper.GetPurchases(DateTime.Now.AddDays(-30), DateTime.Now);
                dataGridResults.FontSize = 14;
            }
            if (param == "ALERTS")
            {
                fromdtPicker.IsEnabled = todtPicker.IsEnabled = btnGo.IsEnabled = btnUpdate.IsEnabled = btnAddNew.IsEnabled = false;
                dt = FBDataHelper.GetAlerts();
                dt.Columns.Add("ALERT TYPE");
                foreach (DataRow row in dt.Rows)
                {
                    if (Convert.ToInt32(row["QUANTITY"]) < 5)
                    {
                        row["ALERT TYPE"] = "Low Stock";
                    }
                    else
                    {
                        row["ALERT TYPE"] = "Expiry in " + Convert.ToDateTime(row["EXPIRYDATE"]).Subtract(DateTime.Now).Days + " days";
                    }
                }

                dataGridResults.FontSize = 14;
            }
            if (param.Contains("SALEID"))
            {
                fromdtPicker.IsEnabled = todtPicker.IsEnabled = btnGo.IsEnabled = btnUpdate.IsEnabled = btnAddNew.IsEnabled = false;
                string saleid = param.Split(new char[] { ':' })[1];
                dt = FBDataHelper.GetDetailedSales(saleid);
            }
            if (param.Contains("SUPID"))
            {
                string supid = param.Split(new char[] { ':' })[1];
                dt = FBDataHelper.GetStatement(supid);

                int owed = 0;
                foreach (DataRow row in dt.Rows)
                {
                    if (row["TRANSACTION"].ToString() == "PURCHASE (CREDIT)")
                    {
                        owed += Convert.ToInt32(row["AMOUNT"]);
                    }
                    if (row["TRANSACTION"].ToString() == "PAYMENT")
                    {
                        owed += -1 * Convert.ToInt32(row["AMOUNT"]);
                    }
                    row["AMOUNT"] = Convert.ToInt32(row["AMOUNT"]).ToString("N0");
                }

                lblCount.Content = "TOTAL OWED: " + owed.ToString("N0");
                labelSet = true;

            }
            dataGridResults.ItemsSource = dt.DefaultView;
            if (!labelSet)
            {
                lblCount.Content = dt.Rows.Count + " items";
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (AppUtilities.session.User.CLRlevel > 1)
            {
                int affrows = dt.UpdateSource();
                MessageBox.Show("Update Successful.\n" + affrows + " items were changed.", "SUCCESS");
                FBDataHelper.AddLog(DateTime.Now, "USER " + AppUtilities.session.User.UserName + " edited " + affrows + " items in " + param + " at " + DateTime.Now);
            }
            else
            {
                MessageBox.Show("You do not have enough permissions to alter this detail.", "ERROR");
            }
        }

        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
            if (param == "SUPPLIERS")
            {
                if (new AddSupplier().ShowDialog() == true) this.Close();
            }
            if (param == "SALES")
            {
                if (new SaleWindow().ShowDialog() == true) this.Close();
            }

            if (param == "USERS")
            {
                new AddEmployee().Show();
            }
            if (param == "EXPENSES")
            {
                new Expenses().Show();
            }
        }

        private void txtDrugName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string dName = txtSearch.Text.Trim();
            int index = 0;
            if (param == "EXPENSES") index = 1;
            if (param == "SALES") index = 0;
            if (param == "PURCHASES") index = 1;
            try
            {
                qresult = from rtndRows in dt.AsEnumerable()
                          where rtndRows[index].ToString().ToUpper().Contains(dName.ToUpper())
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
                MessageBox.Show("An unknown error occurred. Searching in this information may not be enabled yet.\n DETAILS:" + ex.Message, "ERROR");
            }
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (param == "SALES")
                {
                    dt = FBDataHelper.GetSales(fromdtPicker.SelectedDate.Value, todtPicker.SelectedDate.Value);
                    dataGridResults.FontSize = 13;
                    lblCount.Content = dt.Rows.Count + " items, " + " Total=" + dt.Columns["AMOUNTDUE"].GetColumnTotal().ToString("N0");
                }
                if (param == "EXPENSES")
                {
                    dt = FBDataHelper.GetExpenses(fromdtPicker.SelectedDate.Value, todtPicker.SelectedDate.Value);
                    dataGridResults.FontSize = 14;
                    lblCount.Content = dt.Rows.Count + " items, " + " Total=" + dt.Columns["AMTPAID"].GetColumnTotal().ToString("N0");
                }
                if (param == "PURCHASES")
                {
                    dt = FBDataHelper.GetPurchases(fromdtPicker.SelectedDate.Value, todtPicker.SelectedDate.Value);
                    dataGridResults.FontSize = 14;
                    lblCount.Content = dt.Rows.Count + " items";
                }
                if (param == "LOG")
                {
                    dt = FBDataHelper.GetLog(fromdtPicker.SelectedDate.Value, todtPicker.SelectedDate.Value);
                    dataGridResults.ItemsSource = dt.DefaultView;
                }
                dataGridResults.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred. Verify that you have selected a range of dates for this search.\n" +
                    "DETAILS:" + ex.Message);
            }
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            DataRow selectedRow = (dataGridResults.SelectedItem as DataRowView).Row;
            if (AppUtilities.session.User.CLRlevel > 1)
            {
                if (MessageBox.Show("Are you sure you want to delete this item?\n" +
                    selectedRow[0],
                    "DELETE?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    selectedRow.Delete();
                    dt.UpdateSource();
                    MessageBox.Show("Item deleted.", "ERROR");
                }
            }
            else
            {
                MessageBox.Show("You can not delete this item.", "ERROR");
            }
        }

        private void dataGridResults_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(DateTime))
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd-MMM-yy";
            }
            if ((e.PropertyType == typeof(Int32) || e.Column.Header.ToString() == "AMOUNT") && e.Column.DisplayIndex != 1)
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "N0";
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string selectedItem = (dataGridResults.SelectedItem as DataRowView).Row["SALEID"].ToString();
            new DataViewer("SALEID:" + selectedItem).Show();
        }

        private void AlterFactor_Click(object sender, RoutedEventArgs e)
        {
            DataRow row = (dataGridResults.SelectedItem as DataRowView).Row;
            AlterFactor af = new AlterFactor(row);
            if (af.ShowDialog() == true)
            {
                dt.Rows[dt.Rows.IndexOf(row)]["FACTOR"] = af.Factor;
            }
            dataGridResults.ItemsSource = dt.DefaultView;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!(param == "ALERTS" || param == "LOG" || param.Contains("SUPID")))
            {
                try
                {
                    if (dt.GetChanges() != null)
                    {
                        if (MessageBox.Show("You have pending changes.\nPress YES to commit and NO to discard.", "CONFIRM",
                            MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.OK)
                        {
                            btnUpdate_Click(null, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    /* MessageBox.Show("An error occured.\nThe window may not have loaded properly.\n" +
                         "Please close the window and try again.", "ERROR");*/
                }
            }

        }
        private void menuPay_Click(object sender, RoutedEventArgs e)
        {
            string selectedItem = (dataGridResults.SelectedItem as DataRowView).Row["NAME"].ToString();
            new MakePayment(selectedItem).Show();
        }

        private void menuViewStatement_Click(object sender, RoutedEventArgs e)
        {
            string selectedItem = (dataGridResults.SelectedItem as DataRowView).Row["SUPPLIER"].ToString();
            new DataViewer("SUPID:" + selectedItem).Show();
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = "CSV File|*.csv";
            if (sfd.ShowDialog(this) == true)
            {
                try
                {
                    if (qresult != null)
                    {
                        Utilities.WriteCSVFile(qresult.CopyToDataTable(), sfd.FileName + ".csv");
                    }
                    else
                    {
                        if (dt.Rows.Count > 0)
                        {
                            Utilities.WriteCSVFile(dt, sfd.FileName + ".csv");
                        }
                        else
                            MessageBox.Show("There are no rows to export.", "ERROR");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occured.\n DETAILS: " + ex.Message, "ERROR");
                }
            }
        }

        private void dataGridResults_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (e.Key == System.Windows.Input.Key.F6 && AppUtilities.session.User.CLRlevel > 2)
                {
                    if (dataGridResults.SelectedItems.Count == 1)
                    {
                        string user = (dataGridResults.SelectedItem as DataRowView).Row["USERNAME"].ToString();
                        if (MessageBox.Show("Are you sure you want to change the password of the selected user?\n" + "USER:  " + user, "CONFIRM",
                            MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            ChangePass wincPass = new ChangePass();
                            if (wincPass.ShowDialog() == true)
                            {
                                if (FBDataHelper.ResetPassword(user, Hasher.CreateHash(wincPass.NewPassword)))
                                {
                                    MessageBox.Show("The password was altered successfully.", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                else
                                {
                                    MessageBox.Show("The password change action failed. Please try again.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("You must select 1 user to proceed.", "PASSWORD RESET ERROR", MessageBoxButton.OK, MessageBoxImage.Stop);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An exception occurred. The password change action failed. Please try again.\n"+ex.Message, 
                    "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
