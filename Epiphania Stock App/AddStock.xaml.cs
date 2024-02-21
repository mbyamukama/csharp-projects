using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Data;
using GenericUtilities;

namespace StockApp
{
    /// <summary>
    /// Interaction logic for AddStock.xaml
    /// </summary>
    public partial class AddStock : Window
    {
        DataRow returnedRow = null;
        int maxId = 0;

        double costppu = 0;
        int costprice = 0;
        int qty = 0;
        float factor = 1.5F;
        float originalFactor = 1.5F;
        string group = "";
        bool exists;

        public AddStock()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {           
            maxId = FBDataHelper.GetMaxEntryID("DETSTOCK", "ENTRYID") + 1;
            cbxSupplier.ItemsSource =
          new ObservableCollection<string>(FBDataHelper.GetSuppliers().Columns["NAME"].AsEnumerable<string>());
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

                    lblFactor.Content = factor;
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
            DataRow row = AppUtilities.addStockdataTable.NewRow();
            row["ENTRYID"] = maxId;
            row["DRUGNAME"] = txtDrugName.Text.Trim();
            row["BATCHNO"] = txtBatchNo.Text.Trim();
            row["SUPPLIER"] = cbxSupplier.Text.Trim();
            row["INVOICENUM"] = txtInvoiceNum.Text.Trim();
            row["PURCHASE"] = cbxPurchaseType.Text.Trim();
            row["DATEOFENTRY"] = DateTime.Now;
            row["QUANTITY"] = qty;
            row["EXPIRYDATE"] = dtPicker.SelectedDate.Value.ToShortDateString();
            row["COSTPPU"] = costppu;
            row["COSTTOTAL"] = costprice;
            row["FACTOR"] = factor;
            row["DGROUP"] = group;
            row["EXISTS"] = exists;
            AppUtilities.addStockdataTable.Rows.Add(row);
            maxId += 1;
        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            DrugSearch window = new DrugSearch();
            window.ShowDialog();
            exists = window.drugExists;
            returnedRow = window.selected;
            if (returnedRow != null)
            {
                txtDrugName.Text = returnedRow["DRUGNAME"].ToString();
                lblGroup.Content = group = returnedRow["DGROUP"].ToString();
                lblFactor.Content = returnedRow["FACTOR"];
            }

        }
    }
}
