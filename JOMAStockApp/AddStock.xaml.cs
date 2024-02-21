using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Data;
using StockApp.AppExtensions;

namespace StockApp
{
    /// <summary>
    /// Interaction logic for AddStock.xaml
    /// </summary>
    public partial class AddStock : Window
    {
        DataRow returnedRow = null;
        int maxId = 0;
        string barcode = "";

        double costppu = 0;
        int costprice = 0;
        int qty = 0;
        float factor = 1.5F;
        float originalFactor = 1.5F;
        DataTable dtStock = null;
        string category = "";

        public AddStock()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dtStock = FBDataHelper.GetStock(FBDataHelper.StockType.Basic, DateTime.Now, DateTime.Now);
            maxId = FBDataHelper.GetMaxEntryID("DETSTOCK", "ENTRYID") + 1;

            cbxSupplier.ItemsSource =
          new ObservableCollection<string>
          (DataUtilities.AsEnumerable<string>(FBDataHelper.GetSuppliers().Columns["NAME"]));
        }

        private void txtCostPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtCostPrice.Text.Trim() != "" && txtQty.Text.Trim() != "")
            {
                try
                {
                    costprice = Int32.Parse(txtCostPrice.Text.Trim());
                    qty = Int32.Parse(txtQty.Text.Trim());
                    if (qty == 0) lblUnitCostPx.Content = "Zero Quantity";
                    if (costprice != 0)
                    {
                        costppu = Math.Round(costprice * 1.0 / qty, 4);
                        lblUnitCostPx.Content = costppu;
                    }
                }
                catch (FormatException ex)
                {
                    MessageBox.Show("ERROR.\n The costprice and quatity fields only take numeric characters.\n" +
                                     "TECHNICAL DETAILS: " + ex.Message, "ERROR");
                }
            }
        }

        private void txtQty_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtCostPrice.Text.Trim() != "" && txtQty.Text.Trim() != "")
            {
                try
                {
                    costprice = Int32.Parse(txtCostPrice.Text.Trim());
                    qty = Int32.Parse(txtQty.Text.Trim());
                    if (costprice == 0) lblUnitCostPx.Content = "Zero Price";
                    if (qty != 0)
                    {
                        costppu = Math.Round(costprice * 1.0 / qty, 4);
                        lblUnitCostPx.Content = costppu;
                    }
                }
                catch (FormatException ex)
                {
                    MessageBox.Show("ERROR.\n The costprice and quatity fields only take numeric characters.\n"+
                                     "TECHNICAL DETAILS: "+ ex.Message, "ERROR");
                }
            }
        }

        private void txtSalePrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtSalePrice.Text.Trim() != "" && txtCostPrice.Text.Trim()!="")
            {
                try
                {
                    int saleppu = Int32.Parse(txtSalePrice.Text.Trim());
                    if (saleppu < costppu) factor = originalFactor;
                    else
                    {
                        factor = (float)Math.Round(saleppu * 1.0F / costppu, 5);
                    }

                }
                catch (FormatException ex)
                {
                    MessageBox.Show("ERROR.\n The costprice and quatity fields only take numeric characters.\n"+
                                     "TECHNICAL DETAILS: "+ ex.Message, "ERROR");
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            category = cbxCategory.Text;
            if (category != "" && txtItemName.Text != "")
            {
                DataRow row = GlobalSystemData.addStockdataTable.NewRow();
                row["ENTRYID"] = maxId;
                row["BARCODE"] = barcode;
                row["ITEMNAME"] = txtItemName.Text.Trim();
                row["SUPPLIER"] = cbxSupplier.Text.Trim();
                row["INVOICENUM"] = txtInvoiceNum.Text.Trim();
                row["DATEOFENTRY"] = DateTime.Now;
                row["QUANTITY"] = qty;
                row["COSTPPU"] = costppu;
                row["COSTTOTAL"] = costprice;
                row["FACTOR"] = factor;
                row["CATEGORY"] = category;
                GlobalSystemData.addStockdataTable.Rows.Add(row);
                maxId += 1;
            }
            else
                MessageBox.Show("Some fields are missing", "ERROR");
        }

        private void txtDrugName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    DataRow row = (from myrows in dtStock.AsEnumerable()
                                   where myrows.Field<string>("BARCODE") == txtItemName.Text.Trim()
                                   select myrows).ElementAt(0);
                    txtItemName.Text = row["ITEMNAME"].ToString();
                    barcode = row["BARCODE"].ToString();
                    category = row["CATEGORY"].ToString();
                    if (category != "")
                    {
                        cbxCategory.Text = category;
                    }
                    else
                    {
                        cbxCategory.ItemsSource = new ObservableCollection<string>(FBDataHelper.GetCategories());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred.\nThe barcode entered was not found.\n DETAILS: " + ex.Message, "ERROR");
                }
            }
            if (e.Key == Key.F1)
            {
                ItemSearch window = new ItemSearch();
                window.ShowDialog();
                returnedRow = window.SelectedRow;
                if (returnedRow != null)
                {
                    txtItemName.Text = returnedRow["ITEMNAME"].ToString();
                    barcode = returnedRow["BARCODE"].ToString();
                    category = returnedRow["CATEGORY"].ToString();
                    if (category != "")
                    {
                        cbxCategory.Text = category;
                    }
                    else
                    {
                        cbxCategory.ItemsSource = new ObservableCollection<string>(FBDataHelper.GetCategories());
                    }
                }
            }
        }
    }
}
