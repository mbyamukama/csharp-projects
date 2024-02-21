using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Data;
using GenericUtilities;

namespace StockApp
{
    /// <summary>
    /// Interaction logic for InventoryWindow.xaml
    /// </summary>
    public partial class AddMultipleStock: Window
    {
        DataTable errors = null;
        public AddMultipleStock()
        {
            InitializeComponent();
            AppUtilities.InitStockDataTable();
            errors = AppUtilities.addStockdataTable.Clone();
           
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dtGridAddSock.ItemsSource = AppUtilities.addStockdataTable.DefaultView;
            lblCount.Content = dtGridAddSock.Items.Count;
        }

        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
            new AddStock().ShowDialog();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            //verify
            DataTable copy = AppUtilities.addStockdataTable.Copy();
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
                foreach (DataRow row in AppUtilities.addStockdataTable.Rows)
                {
                    bool res = false;
                    try
                    {
                        res = FBDataHelper.AddStock(row["ENTRYID"].ToString(), row["DRUGNAME"].ToString(),
                           row["BATCHNO"].ToString(), row["SUPPLIER"].ToString(), row["INVOICENUM"].ToString(),
                           Convert.ToInt32(row["QUANTITY"]), (float)Convert.ToDouble(row["COSTPPU"]),
                        (float)Convert.ToDouble(row["FACTOR"]), Convert.ToDateTime(row["EXPIRYDATE"].ToString()),
                        Convert.ToDateTime(row["DATEOFENTRY"].ToString()), row["PURCHASE"].ToString(), Convert.ToBoolean(row["EXISTS"]));

                        if (res)
                        {
                            MessageBox.Show("EntryID " + row["ENTRYID"] + " added.", "SUCCESS");
                        }
                        else
                        {
                            MessageBox.Show("EntryID " + row["ENTRYID"] + " failed. Please retry.", "FAILED");
                            errors.ImportRow(row);
                        }
                    }
                    catch (InvalidCastException ex)
                    {
                        MessageBox.Show("An error occurred.\n It appears some data is not in the expected format.\n" +
                            "TECHNICAL DETAILS:\t" + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An unknown error occurred.\nTECHNICAL DETAILS:\t" + ex.Message);
                    }
                }
                if (errors.Rows.Count > 0)
                {
                    MessageBox.Show("Some items were not added.", "ERROR");
                    Utilities.VisualizeDataTable(errors, "ERROR ITEMS",400,400);
                }
                else
                {
                    MessageBox.Show("All new stock added.", "SUCCESS");
                }
                AppUtilities.addStockdataTable.Clear();
                errors.Clear();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            AppUtilities.addStockdataTable.Dispose();
        }
    }
}
