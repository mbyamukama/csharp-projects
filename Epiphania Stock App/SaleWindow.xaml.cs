using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Data;

namespace StockApp
{
    /// <summary>
    /// Interaction logic for SaleWindow.xaml
    /// </summary>
    public partial class SaleWindow : Window
    {
        DataTable dtStock = null;
        DataTable dtItemsBought = null;

        string drugName = "";
        int qty = 0;
        int availQty = 0;
        float factor = 0.0F;
        int costprice = 0, totAmtDue = 0, amtPaid = 0, balance = 0, discount = 0, origAmtDue = 0;
        char insurance = 'N';


        public SaleWindow()
        {
            InitializeComponent();
            dtStock = FBDataHelper.GetStock(FBDataHelper.StockType.Basic,DateTime.Now,DateTime.Now);
 
            dtItemsBought = new DataTable();
            dtItemsBought.Columns.Add("DRUGNAME");
            dtItemsBought.Columns.Add("QUANTITY");
            dtItemsBought.Columns.Add("COSTPPU");
            dtItemsBought.Columns.Add("FACTOR");
            dtItemsBought.Columns.Add("AMOUNTDUE");

            dtGridItemsBought.ItemsSource = dtItemsBought.AsDataView();
            dtItemsBought.RowDeleted += dtItemsBought_RowDeleted;
            dtItemsBought.ColumnChanged += dtItemsBought_ColumnChanged;

        }

        private void dtItemsBought_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (dtItemsBought.Rows.Count > 0)
            {
                e.Row.AcceptChanges();
                if (e.Column.ColumnName == "QUANTITY")
                {
                    qty = Convert.ToInt32(e.Row["QUANTITY"]);
                    e.Row["AMOUNTDUE"] = chkInsurance.IsChecked.Value ?
                            AppUtilities.RoundUpToNearestNote(Math.Round(qty * costprice * (factor + 0.2), 0)) :
                            AppUtilities.RoundUpToNearestNote(Math.Round(qty * costprice * factor, 0));

                    //get total in rows
                    totAmtDue = 0;
                    foreach (DataRow r in dtItemsBought.Rows)
                    {
                        totAmtDue += (int)Convert.ToDouble(r["AMOUNTDUE"]);
                    }
                    lblAmountDue.Content = (totAmtDue = AppUtilities.RoundUpToNearestNote(totAmtDue)).ToString("N0");
                }
            }
        }

        void dtItemsBought_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            totAmtDue = 0;
            foreach (DataRow r in dtItemsBought.Rows)
            {
                totAmtDue += (int)Convert.ToDouble(r["AMOUNTDUE"]);
            }
            lblAmountDue.Content = (totAmtDue = AppUtilities.RoundUpToNearestNote(totAmtDue)).ToString("N0");
        }
		

        private void txtQty_TextChanged(object sender, TextChangedEventArgs e)
        {
            string qtyText = txtQty.Text.Trim();
            if (qtyText != "")
            {
                try
                {
                    qty = Int32.Parse(qtyText);
                    if(qty > availQty) 
                    {
                        MessageBox.Show("The available quantity is only " + availQty, "ERROR");
                        txtQty.Clear();
                        qty = 0;
                    }                  
                }
                catch (FormatException ex)
                {
                    MessageBox.Show("The quatity field only takes numeric characters.\n" +
                                     "DETAILS: " + ex.Message, "ERROR");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An unknown error occurred.\n" +
                                     "DETAILS: " + ex.Message, "ERROR");
                }
            }
        }

        private void txtAmtPaid_TextChanged(object sender, TextChangedEventArgs e)
        {
            string amtPaidText = txtAmtPaid.Text.Trim();
            if (amtPaidText != "")
            {
                try
                {
                    amtPaid = Int32.Parse(amtPaidText);
                    balance = amtPaid - totAmtDue;

                    if (balance < 0)
                    {
                        lblBalance.Foreground = System.Windows.Media.Brushes.Red;
                    }
                    else lblBalance.Foreground = System.Windows.Media.Brushes.Black;

                    lblBalance.Content = balance.ToString("N0");
                }
                catch (FormatException ex)
                {
                    MessageBox.Show("The quantity field only takes numeric characters.\n" +
                                     "TECHNICAL DETAILS: " + ex.Message, "ERROR");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An unknown error occurred.\n" +
                                     "TECHNICAL DETAILS: " + ex.Message, "ERROR");
                }
            }
        }

        private void txtAmtPaid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && txtAmtPaid.Text != "")
            {
                   if (balance >= 0)
                    {
                        string saleid = "";
                        bool success = FBDataHelper.AddSale(drugName, totAmtDue, amtPaid, balance, AppUtilities.session.User.UserName,insurance, out saleid);

                        foreach (DataRow row in dtItemsBought.Rows)
                        {
                            success &=
                             FBDataHelper.AddDetailedSale(saleid, row["DRUGNAME"].ToString(),
                             Convert.ToInt32(row["QUANTITY"]), Convert.ToInt32(row["AMOUNTDUE"]));
                            if (!success)
                            {
                                FBDataHelper.DeleteSale(saleid); //DB will handle the rest!
                                break;
                            }
                        }
                        if (success)
                        {
                            if (AppUtilities.session.IsPrintingEnabled)
                            {
                                try
                                {
                                    FixedDocument fdoc = AppUtilities.GenerateReceipt(dtItemsBought, amtPaid, totAmtDue, balance, saleid, discount);
                                    PrintDialog pd = new PrintDialog();
                                    pd.PrintDocument(fdoc.DocumentPaginator, "Receipt");
                                    MessageBox.Show("The transaction completed successfully.", "SALE SUCCESS");
                                }
                                catch (System.Printing.PrintSystemException ex)
                                {
                                    MessageBox.Show("A printing error occurred.\n" + ex.Message);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("A  error occurred.\n" + ex.Message);
                                }
                            }
                        }
                       else
                        {
                            MessageBox.Show("The transaction failed.", "SALE ERROR");
                        }
                        
                        dtItemsBought.Clear();
                        txtAmtPaid.Clear(); 
                        lblAmountDue.Content = lblBalance.Content = 0;

                    }
                    else
                    {
                        MessageBox.Show("Amount due is greater than amount paid", "ERROR");
                    }

                }       
        }

        private void chkInsurance_Checked(object sender, RoutedEventArgs e)
        {
            totAmtDue = 0;
            insurance = 'Y';
            if (dtItemsBought.Rows.Count > 0)
            {
                double f, cp;
                foreach (DataRow row in dtItemsBought.Rows)
                {
                    f = Convert.ToDouble(row["FACTOR"]);
                    cp = Convert.ToInt32(row["COSTPPU"]);
                    qty = Convert.ToInt32(row["QUANTITY"]);

                    row["AMOUNTDUE"] = Math.Round(qty * cp * (f + 0.2), 0);
                    totAmtDue += (int)Convert.ToDouble(row["AMOUNTDUE"]);
                }
            }
            lblAmountDue.Content = (totAmtDue = AppUtilities.RoundUpToNearestNote(totAmtDue)).ToString("N0");
        }

        private void chkInsurance_Unchecked(object sender, RoutedEventArgs e)
        {
            totAmtDue = 0;
            insurance = 'N';
            if (dtItemsBought.Rows.Count > 0)
            {
                double f, cp;
                foreach (DataRow row in dtItemsBought.Rows)
                {
                    f = Convert.ToDouble(row["FACTOR"]);
                    cp = Convert.ToInt32(row["COSTPPU"]);
                    qty = Convert.ToInt32(row["QUANTITY"]);

                    row["AMOUNTDUE"] = Math.Round(qty * cp * f, 0);
                    totAmtDue += (int)Convert.ToDouble(row["AMOUNTDUE"]);
                }
            }
            lblAmountDue.Content = (totAmtDue = AppUtilities.RoundUpToNearestNote(totAmtDue)).ToString("N0");
        }

        private void txtQty_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtQty.Text != "" && qty!=0)
                {
                    DataRow row = dtItemsBought.NewRow();
                    row["DRUGNAME"] = drugName;
                    row["QUANTITY"] = qty;
                    row["COSTPPU"] = costprice;
                    row["FACTOR"] = factor;
                    row["AMOUNTDUE"] = chkInsurance.IsChecked.Value ?
                        AppUtilities.RoundUpToNearestNote(Math.Round(qty * costprice * (factor + 0.2), 0)) :
                        AppUtilities.RoundUpToNearestNote(Math.Round(qty * costprice * factor, 0));
                   

                    dtItemsBought.Rows.Add(row);
                    txtQty.Clear();
                    txtDrugName.Text = "";

                    //get total in rows
                    totAmtDue = 0;
                    foreach (DataRow r in dtItemsBought.Rows)
                    {
                        totAmtDue += (int)Convert.ToDouble(r["AMOUNTDUE"]);
                    }
                }
                else
                {
                    MessageBox.Show("The quantity is invalid.", "ERROR");
                }
                origAmtDue = totAmtDue;
                lblAmountDue.Content = (totAmtDue = AppUtilities.RoundUpToNearestNote(totAmtDue)).ToString("N0");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dtGridItemsBought.FontSize = 12;
            dtGridItemsBought.Columns[2].Visibility = Visibility.Hidden;
            dtGridItemsBought.Columns[3].Visibility = Visibility.Hidden;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            DrugSearch window = new DrugSearch();
            window.ShowDialog();
            DataRow row  = window.selected;
            if (row != null)
            {
                txtDrugName.Text = drugName = row["DRUGNAME"].ToString();
                factor = (float)Convert.ToDouble(row["FACTOR"]);
                costprice = Convert.ToInt32(row["COSTPPU"]);
                availQty = Convert.ToInt32(row["QUANTITY"]);
            }
            txtQty.Focus();
        }
        private void viewSalesMenu_Click(object sender, RoutedEventArgs e)
        {
            new DataViewer("SALES").Show();
        }

        private void exitMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtDiscount_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtDiscount.Text.Trim() == "") discount = 0;
                else
                    discount = Int32.Parse(txtDiscount.Text.Trim());

                if (totAmtDue > 0 && discount <= totAmtDue)
                {
                    totAmtDue = origAmtDue - discount;
                    lblAmountDue.Content = totAmtDue.ToString("N0");

                    if (amtPaid > 0)
                    {
                        balance = amtPaid - totAmtDue;
                        if (balance < 0)
                        {
                            lblBalance.Foreground = System.Windows.Media.Brushes.Red;
                        }
                        else lblBalance.Foreground = System.Windows.Media.Brushes.Black;
                    }

                    lblBalance.Content = balance.ToString("N0");
                }
            }
            catch (FormatException ex)
            {
                MessageBox.Show("The quatity field only takes numeric characters.\n" + ex.Message, "ALERT", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unknown error occurred.\n" + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
