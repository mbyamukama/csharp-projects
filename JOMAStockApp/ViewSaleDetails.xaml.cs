using System;
using System.Windows;
using System.Data;

namespace StockApp
{
    /// <summary>
    /// Interaction logic for InventoryWindow.xaml
    /// </summary>
    public partial class ViewSaleDetails : Window
    {
        DataTable dt = null;
        string saleid = "";

        private void MenuReturnItem_Click(object sender, RoutedEventArgs e)
        {
            DataRow selectedRow = (dataGridResults.SelectedItem as DataRowView).Row;
            int maxQty = Convert.ToInt32(selectedRow["QUANTITY"].ToString());
            string saleid = selectedRow["SALEID"].ToString();
            string itemName = selectedRow["ITEMNAME"].ToString();
            int amtDue = Convert.ToInt32(selectedRow["AMOUNTDUE"].ToString());

            QuantityConfirmWindow qcw = new QuantityConfirmWindow(maxQty);
            if(qcw.ShowDialog()==true)
            {
                int returnQty = qcw.EnteredValue;
                string msg = "";
                bool success = FBDataHelper.ReturnItem(itemName, returnQty, (int)((returnQty * 1.0 / maxQty) * amtDue), returnQty+" of "+ maxQty+" items returned by Customer from SALE ID: "+saleid, out msg);
                if(success)
                {
                    MessageBox.Show("The return item transaction succeeded. The message returned by the system is:\n" + msg, "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                    MessageBox.Show("The return item transaction failed. The message returned by the system is:\n" + msg, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        public ViewSaleDetails(string saleid)
        {
            InitializeComponent();
            this.saleid = saleid;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dt = FBDataHelper.GetDetailedSales(saleid);
            dataGridResults.ItemsSource = dt.DefaultView;
            lblCount.Content = dt.Rows.Count + " items";
            labelDetails.Content = "Sale Details for Sale ID: " + saleid;
        }
    }
}
