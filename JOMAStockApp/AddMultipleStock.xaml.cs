using System;
using System.Windows;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using StockApp.AppExtensions;


namespace StockApp
{
    /// <summary>
    /// Interaction logic for InventoryWindow.xaml
    /// </summary>
    public partial class AddMultipleStock : Window
    {
        DataTable errors = null;
        int count = 0;
        public AddMultipleStock()
        {
            InitializeComponent();
            GlobalSystemData.InitStockDataTable();
            errors = GlobalSystemData.addStockdataTable.Clone();

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dtGridAddSock.ItemsSource = GlobalSystemData.addStockdataTable.DefaultView;
        }

        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
            new AddStock().ShowDialog();
        }                                                                             

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            //verify

            DataTable copy = GlobalSystemData.addStockdataTable.Copy();
            IEnumerable<string> distinctInvoices = copy.Columns["INVOICENUM"].AsEnumerable<string>().Distinct();
            string message = "Please Confirm Invoice Prices\n";

            foreach (string invoice in distinctInvoices)
            {
                int sum = (from myrows in copy.AsEnumerable()
                           where myrows.Field<string>("INVOICENUM").Equals(invoice)
                           select myrows.Field<int>("COSTTOTAL")).ToList().Sum();
                message += "INVOICE: " + invoice + "\t" + sum + "\n";
            }

            MessageBoxResult result = MessageBox.Show(message, "VERIFICATION", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                try
                {
                    pgBarAddedItems.Minimum = 0;
                    pgBarAddedItems.Maximum = count = GlobalSystemData.addStockdataTable.Rows.Count;
                    int added = 0;
                    lblCount.Content = "Row Count: " + count;

                    foreach (DataRow row in GlobalSystemData.addStockdataTable.Rows)
                    {
                        bool res = false;

                        res = FBDataHelper.AddStock(row["ENTRYID"].ToString(),row["BARCODE"].ToString(), row["ITEMNAME"].ToString(), row["SUPPLIER"].ToString(),
                            row["INVOICENUM"].ToString(), Convert.ToInt32(row["QUANTITY"]), (float)Convert.ToDouble(row["COSTPPU"]),
                        (float)Convert.ToDouble(row["FACTOR"]), row["CATEGORY"].ToString(),
                        Convert.ToDateTime(row["DATEOFENTRY"].ToString()));

                        if (res)
                        {
                            ++added;
                            pgBarAddedItems.Value = added;
                            txtPercent.Text = added + " of " + count + " (" + Math.Round(added * 100.0 / count, 1).ToString() + "% )";
                            MessageBox.Show("EntryID " + row["ENTRYID"] + " added.", "SUCCESS");
                        }
                        else
                        {
                            MessageBox.Show("EntryID " + row["ENTRYID"] + " failed. An error log will be provided.", "FAILED");
                            errors.ImportRow(row);
                        }
                    }

                    if (errors.Rows.Count > 0)
                    {
                        MessageBox.Show("Some items were not added.", "ERROR");
						StockApp.AppExtensions.Windows.VisualizeDataTable(errors, "ERROR ITEMS", 300, 200);
                    }
                    else
                    {
                        MessageBox.Show("All new stock added.", "SUCCESS");
                        GlobalSystemData.addStockdataTable.Clear();
                        lblCount.Content = 0;
                        pgBarAddedItems.Value = 0;
                        txtPercent.Text = "";
                    }


                }

                catch (Exception ex)
                {
                    MessageBox.Show("An unknown error occurred.\nTECHNICAL DETAILS:\t" + ex.Message);
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GlobalSystemData.addStockdataTable.Dispose();
        }
    }
}
