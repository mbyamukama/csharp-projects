using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using StockApp.AppExtensions;
using System.Runtime.Serialization.Formatters.Binary;
using StockApp.AppExtensions;

namespace StockApp
{
    /// <summary>
    /// Interaction logic for InventoryWindow.xaml
    /// </summary>
    public partial class DataViewer: Window
    {
        SDataTable dt = null;
        string param = "", teller = "";
        int total = 0, cardamt = 0;
        string[] alignments = new string[] { "L", "L", "L", "L", "L", "L", "L", "L", "L", "L", "L" , "L"};

        public DataViewer(string parameter)
        {
            InitializeComponent();
            param = parameter;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            label1.Visibility = label2.Visibility = fromdtPicker.Visibility=todtPicker.Visibility = 
            menuAddQty.Visibility = lblFilterCat.Visibility = label1.Visibility = label2.Visibility = menuViewDetails.Visibility =
               cbxFilterCategory.Visibility = fromdtPicker.Visibility = todtPicker.Visibility = btnGo.Visibility =
              menuEdit.Visibility = menuPrintBarcodes.Visibility = menuShowSaleHistory.Visibility= menuShowHistory.Visibility = menuReprintReceipt.Visibility =
              lblTeller.Visibility = cbxTeller.Visibility = todayCheckBox.Visibility= menuViewSaleSummary.Visibility= Visibility.Hidden;          

            if (param == "SALES")
            {
                menuDelete.Visibility = menuShowSaleHistory.Visibility = Visibility.Hidden;
                label1.Visibility = label2.Visibility = menuViewDetails.Visibility =
                    fromdtPicker.Visibility = todtPicker.Visibility = btnGo.Visibility =
                menuReprintReceipt.Visibility = lblTeller.Visibility = cbxTeller.Visibility = todayCheckBox.Visibility =
                cbxTeller.Visibility =  menuViewSaleSummary.Visibility = Visibility.Visible;

                fromdtPicker.SelectedDate = DateTime.Now;
                todtPicker.SelectedDate = DateTime.Now;

                dt = FBDataHelper.GetSales(DateTime.Now.AddDays(-7), DateTime.Now, GlobalSystemData.SaleViewType.All, "");
            }
            if (param=="TOTALSALES")
            {
                DataTable dt = FBDataHelper.GetDailyTotals(DateTime.Now.AddDays(-500), DateTime.Now);
                dataGridResults.ItemsSource = dt.DefaultView; 
            }

            if (param == "CHECKSALES")
            {
                label1.Visibility = label2.Visibility  = fromdtPicker.Visibility = todtPicker.Visibility = btnGo.Visibility =
                todayCheckBox.Visibility   = Visibility.Visible;
                dt = FBDataHelper.CrossCheckSales(DateTime.Now.AddDays(-30), DateTime.Now);
            }

            if (param == "STOCK")
            {
              menuAddQty.Visibility= lblFilterCat.Visibility =  cbxFilterCategory.Visibility = 
              menuPrintBarcodes.Visibility =menuShowSaleHistory.Visibility= menuShowHistory.Visibility = System.Windows.Visibility.Visible;
               
                dt = FBDataHelper.GetStock(FBDataHelper.StockType.Basic, DateTime.Now, DateTime.Now);
                List<string> categories = (from r in dt.AsEnumerable()
                                           select r.Field<string>("CATEGORY")).Distinct<string>().ToList();
                cbxFilterCategory.ItemsSource = new ObservableCollection<string>(categories);

                dt.ColumnChanged += dt_ColumnChanged;
            }
            if (param == "SUPPLIERS")
            {             
                dt = FBDataHelper.GetSuppliers();
                dataGridResults.FontSize = 20;
            }
            if (param == "USERS")
            {
                dt = FBDataHelper.GetEmployees();
                dataGridResults.FontSize = 14;
                menuEdit.Visibility = System.Windows.Visibility.Visible;
                dataGridResults.IsReadOnly = true;
            }
            if (param == "LOG")
            {
                btnGo.Visibility = System.Windows.Visibility.Visible;
                label1.Visibility = label2.Visibility = fromdtPicker.Visibility = todtPicker.Visibility = Visibility.Visible;
                btnUpdate.Visibility = System.Windows.Visibility.Hidden;
                dataGridResults.IsReadOnly = true;
                dt = FBDataHelper.GetLog(DateTime.Now.AddDays(-30), DateTime.Now);
                dataGridResults.FontSize = 14;
            }
            if (param.Contains("SALEID"))
            {
                string saleid = param.Split(new char[] { ':' })[1];
                dt = FBDataHelper.GetDetailedSales(saleid);
            }
            if (param == "ROYALTY")
            {
                menuPrintBarcodes.Visibility = System.Windows.Visibility.Visible;
                dt = FBDataHelper.GetRoyalty();
            }
            if (param == "SETTINGS")
            {
                dt = FBDataHelper.GetSettings();
            }

            dataGridResults.ItemsSource = dt.DefaultView;
            lblCount.Content = dt.Rows.Count + " items";

      }

        void dt_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            try
            {
                if (param == "STOCK")
                {
                    if (GlobalSystemData.Session.Log.Length < 30000)
                    {
                        GlobalSystemData.Session.Log +=
                            "\nVIEW : " + param + ". ACTION: Value Change for item " + e.Row["ITEMNAME"] +
                            " COLUMN: " + e.Column.ColumnName +
                            " ORIGINAL: " + e.Row[e.Column, DataRowVersion.Original] +
                            " FINAL: " + e.Row[e.Column, DataRowVersion.Proposed] +
                            " DATE: " + DateTime.Now + "\n";
                    }
                    else
                    {
                        GlobalSystemData.Session.Log += "\n Log FULL at " + DateTime.Now;
                    }
                }

                if (e.Column.ColumnName == "SELLPPU")
                {
                    e.Row["FACTOR"] = Math.Round(Convert.ToDouble(e.Row[e.Column]) / (Convert.ToDouble(e.Row["COSTPPU"])), 5);
                }
            }
            catch (Exception ex)
            {
                //do nothing
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!(param == "ALERTS" || param == "LOG"))
            {
                try
                {
                    int affrows = dt.UpdateSource();
                    MessageBox.Show("Update Successful.\n" + affrows + " items were changed.", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred during the update.\n The update function may not be supported for this view.\n DETAILS: " +
                        ex.Message, "ERROR");
                }
            }
        }


        private void txtItemName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string dName = txtSearch.Text.Trim();
            try
            {
                EnumerableRowCollection<DataRow> qresult = from rtndRows in dt.AsEnumerable()
                                                           where rtndRows[0].ToString().ToUpper().Contains(dName.ToUpper()) ||
                                                           rtndRows[1].ToString().ToUpper().Contains(dName.ToUpper())
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
                MessageBox.Show("An unknown error occurred. Searching in this information may not be enabled yet.\n DETAILS:"+ex.Message, "ERROR");
            }
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (param == "CHECKSALES")
                {
                    dt = FBDataHelper.CrossCheckSales(fromdtPicker.SelectedDate.Value, todtPicker.SelectedDate.Value);
                    dataGridResults.FontSize = 10;
                    lblCount.Content = dt.Rows.Count + " items, " + 
                        "Detail Total=" +  DataUtilities.AsEnumerable<int>( dt.Columns["TOTALINDETAILED"]).Sum().ToString("N0")
                      + ", Summary Total=" + DataUtilities.AsEnumerable<int>(dt.Columns["TOTALINSUMMARY"]).Sum().ToString("N0")
                    +", Difference = " + DataUtilities.AsEnumerable<int>(dt.Columns["DIFF"]).Sum().ToString("N0");
                }
                if (param == "SALES")
                {
                    dt = FBDataHelper.GetSales(fromdtPicker.SelectedDate.Value, todtPicker.SelectedDate.Value, GlobalSystemData.SaleViewType.All, "");
                    dataGridResults.FontSize = 13;
                    lblCount.Content = dt.Rows.Count + " items, " + " Total=" + dt.AsEnumerable().Sum(row=>row.Field<int>("AMOUNTDUE")).ToString("N0");

                    List<string> tellers = new List<string>();
                    foreach (DataRow row in dt.Rows)
                    {
                        tellers.Add(row["TELLER"].ToString());
                    }

                    cbxTeller.ItemsSource =
                  new System.Collections.ObjectModel.ObservableCollection<string>(tellers.AsEnumerable<string>().Distinct<string>());

                    cbxTeller.SelectionChanged += cbxTeller_SelectionChanged;
                }
                if (param == "LOG")
                {
                    dt = FBDataHelper.GetLog(fromdtPicker.SelectedDate.Value, todtPicker.SelectedDate.Value);
                    btnUpdate.Visibility = System.Windows.Visibility.Hidden;
                    dataGridResults.IsReadOnly = true;
                }
                dataGridResults.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred. Verify that you have selected a range of dates for this search.\n" +
                    "DETAILS:" + ex.Message,"ERROR",MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void menuDelete_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalSystemData.Session.CurrentUser.IsAdmin())
            {
                try
                {
                    DataRow selectedRow = (dataGridResults.SelectedItem as DataRowView).Row;
                        if (MessageBox.Show("Are you sure you want to delete this item?\n" +
                            selectedRow[0],
                            "DELETE?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            GlobalSystemData.Session.Log += "\nDELETED SALES ITEM: " + selectedRow[0];
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
            else
            {
                MessageBox.Show("This operation cannot be completed because the user does not have the required privileges.", "ERROR");
                GlobalSystemData.Session.Log += "\nAttempted a DELETE operation on window " + param;
            }
        }

        private void dataGridResults_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(DateTime))
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "{0:dd/MM/yyyy}";
            }
            if (e.PropertyType == typeof(Int32)&&e.Column.DisplayIndex!=1)
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "N0";
            }
        }

        private void AddNewMenu_Click(object sender, RoutedEventArgs e)
        {
            if (param == "USERS")
            {
                new AddEmployee(false, null).Show();
            }
            else if (param == "STOCK")
            {
                new AddMultipleStock().Show();
            }
            else
            {
                MessageBox.Show("This function is unavailable for the current view.", "ERROR");
            }
        }

        private void exitMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void printBarcodes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRow selectedRow = (dataGridResults.SelectedItem as DataRowView).Row;
                string text = selectedRow[0].ToString();
                string descr = "DEFAULT";
                int copies=1, leftMargin=10, spacing=85;
                string paperType = "45x55mm";
                bool viewPreview = false, compress = false;

                BarcodePrinterDialog bpd = new BarcodePrinterDialog();
                if(bpd.ShowDialog()==true)
                {
                    copies = bpd.Copies;
                    paperType = bpd.PaperType;
                    viewPreview = bpd.ViewPreview;
                    compress = bpd.CompressImages;
                    leftMargin = bpd.LeftMargin;
                    spacing = bpd.Spacing;
                }

                FixedDocument doc = null;
                if (param == "ROYALTY")
                {
                    descr = selectedRow["CNAME"].ToString();
                    doc = DocumentHelper.GetBarCodes(text, descr, paperType, copies, false, 5, 0);
                }
                else
                {
                    descr = "";// selectedRow["ITEMNAME"].ToString();
                    doc = DocumentHelper.GetBarCodes(text, descr.Length > 25 ? descr.Substring(0, 25) : descr, paperType, copies, compress, leftMargin, spacing);
                }

                if (viewPreview)
                {
                    new DocViewer(doc).Show();
                }
                else
                {
                    PrintDialog pd = new PrintDialog();
                    string printQueueName = pd.PrintQueue.FullName;
                    pd.PrintDocument(doc.DocumentPaginator, "Barcode");
                }
            }
            catch (System.Printing.PrintSystemException ex)
            {
                MessageBox.Show("A printing error occurred.\n" + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message);
            }

        }

        private void cbxFilterCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                IEnumerable<DataRow> qresult = from rtndRows in dt.AsEnumerable()
                                                           where rtndRows.Field<string>("CATEGORY").Equals(cbxFilterCategory.SelectedValue as string)
                                                           select rtndRows;
                if (qresult.Count() > 0)
                {
                    dataGridResults.ItemsSource = qresult.CopyToDataTable().DefaultView;
                    lblCount.Content = qresult.Count() + " items";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message+ "\nDETAILS: " + ex.InnerException.Message);
            }
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!(param == "ALERTS" || param == "LOG"))
            {
                if (dt.GetChanges() != null)
                {
                    if (MessageBox.Show("You have pending changes.\nPress YES to commit and NO to discard.?",
                        "CONFIRM", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        btnUpdate_Click(null, null);
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
            }
        }

        private void menuShowHistory_Click(object sender, RoutedEventArgs e)
        {
            DataRow selectedRow = (dataGridResults.SelectedItem as DataRowView).Row;
            string itemName = selectedRow[1].ToString();

            DataTable dt = FBDataHelper.GetPurchaseHistory(itemName);
			StockApp.AppExtensions.Windows.VisualizeDataTable(dt, "Purchase History for " + itemName, 300, 400);

        }

        private void menuEdit_Click(object sender, RoutedEventArgs e)
        {
            DataRow selectedRow = (dataGridResults.SelectedItem as DataRowView).Row;

                new AddEmployee(true, new string[] { selectedRow[0].ToString(), selectedRow[1].ToString(), selectedRow[2].ToString(), 
            selectedRow[3].ToString()}).Show();
        }

        private void menuViewDetails_Click(object sender, RoutedEventArgs e)
        {
            DataRow selectedRow = (dataGridResults.SelectedItem as DataRowView).Row;
            string saleId = selectedRow[0].ToString();
            new ViewSaleDetails(saleId).ShowDialog();
        }

        private void dataGridResults_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
             if(e.Key == Key.F3)
             {
                 if (param == "ROYALTY")
                 {
                     if (MessageBox.Show("Would you like to generate a new royalty customer entry?",
                    "NEW CUSTOMER?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                     {
                         dt.Rows.Add(new object[] { new RNGRandom().RandomNumericString(10) });
                     }
                     
                 }
                 if (param == "STOCK")
                 {
                     (dataGridResults.ItemsSource as DataView).Table.
                         Rows.Add(new object[] { new RNGRandom().RandomNumericString(12) });
                 }
             }
        }

        private void menuAddQty_Click(object sender, RoutedEventArgs e)
        {
            DataRow row = (dataGridResults.SelectedItem as DataRowView).Row;
            AlterQuantity aqty = new AlterQuantity(row);
            if (aqty.ShowDialog() == true)
            {
                row["QUANTITY"] = aqty.CurrQty;
            }

        }

        private void menuReprintReceipt_Click(object sender, RoutedEventArgs e)
        {
            //dt already contains detailed sales, param contains saleid    
            DataRow sale = (dataGridResults.SelectedItem as DataRowView).Row;//sale
            string saleId = sale["SALEID"].ToString();

            MemoryStream ms = FBDataHelper.GetReceipt(saleId);

			BinaryFormatter bf = new BinaryFormatter();
            ReceiptDataModel rDataModel = (ReceiptDataModel)bf.Deserialize(ms);

			FixedDocument fdoc = DocumentHelper.GenerateReceipt(rDataModel);
            PrintDialog pd = new PrintDialog();
            string printQueueName = pd.PrintQueue.FullName;
            pd.PrintDocument(fdoc.DocumentPaginator, "Receipt_Reprint_" + saleId); 

        }
        private void todayCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            fromdtPicker.SelectedDate = DateTime.Today;
            todtPicker.SelectedDate = DateTime.Today;
        }

        private void cbxTeller_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                teller = cbxTeller.SelectedValue.ToString();
                if (dt.Rows.Count > 0)
                {
                    IEnumerable<DataRow> matchingRows = from myRows in dt.AsEnumerable()
                                                        where myRows["TELLER"].ToString() == teller
                                                        select myRows;

                    dataGridResults.ItemsSource = matchingRows.CopyToDataTable().DefaultView;
                    lblCount.Content = matchingRows.Count() + " items, " + " Total=" + (total = matchingRows.Sum(rows => rows.Field<int>("AMOUNTDUE"))).ToString("N0");

                   cardamt = (from myRows in matchingRows.AsEnumerable()
                                where myRows["SALETYPE"].ToString() == "VISA"
                                select myRows).Sum(rows => rows.Field<int>("AMOUNTDUE"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\nDETAILS:" + ex.Message);
            }
        }

        private void menuPrintSelection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int[] columnWidths = new int[dataGridResults.Columns.Count];
                
                foreach (DataGridColumn col in dataGridResults.Columns)
                {
                    columnWidths[col.DisplayIndex] = (int)col.ActualWidth;
                }

                IEnumerable<DataRowView> rowViews = dataGridResults.SelectedItems.Cast<DataRowView>();
                DataTable table = rowViews.ElementAt(0).Row.Table.Clone();
                foreach (DataRowView view in rowViews)
                {
                    table.Rows.Add(view.Row.ItemArray);
                    
                }
                PageContent[] pages =
                     Paginator.Paginate(table, 11, columnWidths, alignments);
                FixedDocument doc = new FixedDocument();
                foreach (PageContent p in pages)
                {
                    doc.Pages.Add(p);
                }
                new DocViewer(doc).Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\n DETAILS: " + ex.Message, "ERROR");
            }
        }

        private void menuPrintAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int[] columnWidths = new int[dataGridResults.Columns.Count];

                foreach (DataGridColumn col in dataGridResults.Columns)
                {
                    columnWidths[col.DisplayIndex] = (int)col.ActualWidth;
                    if(dataGridResults.Columns.Count>8)
                    {
                        columnWidths[col.DisplayIndex] = (int)(col.ActualWidth * 0.9);
                    }
                }

                PageContent[] pages =
                    Paginator.Paginate(((System.Data.DataView)(dataGridResults.ItemsSource)).ToTable(), 11, columnWidths,
                    alignments);

                FixedDocument doc = new FixedDocument();
                foreach (PageContent p in pages)
                {
                    doc.Pages.Add(p);
                }
                new DocViewer(doc).Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\n DETAILS: " + ex.Message, "ERROR");
            }
        }

        private void menuPrintLow_Click(object sender, RoutedEventArgs e)
        {
             try
                {
                    int[] columnWidths = new int[dataGridResults.Columns.Count];

                    foreach (DataGridColumn col in dataGridResults.Columns)
                    {
                        columnWidths[col.DisplayIndex] = (int)col.ActualWidth;
                    }

                    DataTable table = ((System.Data.DataView)(dataGridResults.ItemsSource)).ToTable();
                    DataTable lowStock = (from myRows in table.AsEnumerable()
                                          where myRows.Field<int>("QUANTITY") < 2
                                          select myRows).CopyToDataTable();
                    PageContent[] pages =
                        Paginator.Paginate(lowStock, 11, columnWidths, alignments);

                    FixedDocument doc = new FixedDocument();
                    foreach (PageContent p in pages)
                    {
                        doc.Pages.Add(p);
                    }
                    new DocViewer(doc).Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred.\n DETAILS: " + ex.Message, "ERROR");
                }
            
        }

        private void menuShowSaleHistory_Click(object sender, RoutedEventArgs e)
        {
            DataRow selectedRow = (dataGridResults.SelectedItem as DataRowView).Row;
            string itemName = selectedRow[1].ToString();
            SaleHistory sh = new SaleHistory(itemName);
            sh.Show();
        }

        private void menuViewSaleSummary_Click(object sender, RoutedEventArgs e)
        {
            if (teller != "")
            {
                SaleSummary summary = new SaleSummary("SUMMARY FOR: " + teller, "FROM: " + fromdtPicker.SelectedDate.Value, "TO  : " + todtPicker.SelectedDate.Value, "TOTAL: " + total.ToString("N0"),
                    "CASH : CARD : " + (total - cardamt) + " : " + cardamt);
                FixedDocument doc = new FixedDocument();
                FixedPage page = new FixedPage();
                page.Children.Add(summary);
                PageContent pc = new PageContent();                                                 
                pc.Child = page;
                doc.Pages.Add(pc);

                new DocViewer(doc).Show();
            }
            else
                MessageBox.Show("A teller has currently not been selected", "ERROR");
        }
    }
}
