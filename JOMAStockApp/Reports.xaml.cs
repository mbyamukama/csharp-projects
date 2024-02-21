using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Windows.Documents;
using StockApp.AppExtensions;

namespace StockApp
{
    /// <summary>
    /// Interaction logic for Reports.xaml
    /// </summary>
    public partial class Reports : Window
    {
        DataTable dt = null;
        string title = "";
        string[] descrCols = null, values = null;
        int[] columnWidths = { };
        bool chkbxTINCustomersIsChecked = false;

        public Reports()
        {
            InitializeComponent();
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime fromDate = fromdtPicker.SelectedDate.Value;
                DateTime toDate = todtPicker.SelectedDate.Value;
                FixedPage page1 = new FixedPage();
                string customerIDTIN = txtTIN.Text.Trim();

                lblTeller.Visibility = cbxTeller.Visibility = Visibility.Hidden;
                descrCols = new string[] { "", "", "", "", "", ""};
                values = new string[] { "", "", "", "", "", "" };

                if (reportTypeCbx.Text == "FINANCIAL")
                {
                    dt = new DataTable();
                    Int64 stockVal = FBDataHelper.GetStockValue();
                    Int64 sales = FBDataHelper.GetTotalSales(fromDate, toDate);
                    Int64 profit = FBDataHelper.GetTotalProfit(fromDate, toDate);


                    title = "FINANCIAL REPORT " + fromDate.ToShortDateString() + " to " + toDate.ToShortDateString();
                    descrCols = new string[] { "STOCK VALUE(COST PRICE)", "TOTAL SALES", "TOTAL PROFIT", "", "", "" };
                    values = new string[] { stockVal.ToString("N0"), sales.ToString("N0"), profit.ToString("N0"), "", "", "" };
                }
                if (reportTypeCbx.Text == "EXPENSES")
                {
                    dt = FBDataHelper.GetExpenses(fromDate, toDate);

                    title = "EXPENSES REPORT FOR " + fromDate.ToShortDateString() + " to " + toDate.ToShortDateString();
                    descrCols = new string[] { "TOTAL EXPENSES", "", "", "", "", "" };
                    values = new string[] { dt.Columns["ITEMVALUE"].AsEnumerable<int>().Sum().ToString("N0"), "", "", "", "", "" };
                    columnWidths = new int[] { 300, 60, 60, 60, 250 };
                }

                if (reportTypeCbx.Text == "SALES (DETAILED)")
                {
                    dt = FBDataHelper.GetDetailedSales(fromDate, toDate);

                    title = "DETAILED SALES FOR " + fromDate.ToShortDateString() + " to " + toDate.ToShortDateString();
                    descrCols = new string[] { "ENTRIES", "TOTAL VALUE", "PROFIT", "", "", "" };
                    values = new string[] { dt.Rows.Count.ToString(),
                        DataUtilities.AsEnumerable<int>( dt.Columns["AMOUNTDUE"]).Sum().ToString("N0"),
                        DataUtilities.AsEnumerable<int>( dt.Columns["PROFIT"]).Sum().ToString("N0"), "", "", "" };
                    columnWidths = new int[] { 60, 300, 70, 90, 70, 100, 80 };

                }
                if (reportTypeCbx.Text == "SALES (SUMMARY)")
                {
                    string custTin = txtTIN.Text.Trim();
                    GlobalSystemData.SaleViewType viewType = 0;

                    if (chkbxTINCustomersIsChecked) viewType = GlobalSystemData.SaleViewType.TinCustomersOnly;
                    if (!chkbxTINCustomersIsChecked && custTin == "") viewType = GlobalSystemData.SaleViewType.AllShowingVAT;
                    if (!chkbxTINCustomersIsChecked && custTin != "") viewType = GlobalSystemData.SaleViewType.SingleCustomerWithTIN;

                    dt = FBDataHelper.GetSales(fromDate, toDate, viewType, custTin);
                    
                    title = "SALES SUMMARY FOR " + fromDate.ToShortDateString() + " to " + toDate.ToShortDateString();
                    descrCols = new string[] { "ENTRIES", "TOTAL VALUE", "VAT", "", "", "" };
                    values = new string[] { dt.Rows.Count.ToString(), DataUtilities.AsEnumerable<int>(dt.Columns["TOTAL"]).Sum().ToString("N0"), dt.Columns["VATAMT"].AsEnumerable<int>().Sum().ToString("N0"), "", "", "" };
                    columnWidths = new int[] { 70, 70, 70, 70, 70, 70, 80, 70, 100, 100 };

                    if (dt.Rows.Count > 0)
                    {
                        lblTeller.Visibility = cbxTeller.Visibility = Visibility.Visible;

                        List<string> tellers = new List<string>();
                        foreach (DataRow row in dt.Rows)
                        {
                            tellers.Add(row["TELLER"].ToString());
                        }

                        cbxTeller.ItemsSource =
                      new System.Collections.ObjectModel.ObservableCollection<string>(tellers.AsEnumerable<string>().Distinct<string>());
                        cbxTeller.SelectionChanged+=cbxTeller_SelectionChanged;
                    }

                }

                if (reportTypeCbx.Text == "PURCHASES")
                {
                    dt = FBDataHelper.GetStock(FBDataHelper.StockType.Detailed, fromDate, toDate);

                    int total = dt.Columns["TOTAMT"].AsEnumerable<int>().Sum();

                    title = "PURCHASES FOR " + fromDate.ToShortDateString() + " to " + toDate.ToShortDateString();
                    descrCols = new string[] { "ENTRIES", "TOTAL", "", "", "", "" };
                    values = new string[] { dt.Rows.Count.ToString(), total.ToString("N0"), "", "", "", "" };
                    columnWidths = new int[] { 60, 300, 200, 70, 70, 100, 80, 100 };
                }
                if (reportTypeCbx.Text == "LOYALTY")
                {
                    if (customerIDTIN != "")
                    {
                        DataRow dr = FBDataHelper.GetCustomerData(customerIDTIN);
                        descrCols = new string[] { "NAME", "EMAIL", "JOIN DATE", "CURRENT POINTS", "TIN", "" };
                        values = new string[] { dr["CNAME"].ToString(), dr["EMAIL"].ToString(),  dr["JOINDATE"].ToString(), dr["POINTS"].ToString(), dr["TIN"].ToString(), "" };
                        customerIDTIN = dr["CUSTOMERID"].ToString(); //we need customer ID for next method
                    }
                  
                    dt = FBDataHelper.GetDetailedLoyalty(customerIDTIN, fromDate, toDate);  

                    title = "LOYALTY REPORT FOR CUSTOMER ID: " + customerIDTIN+" for period "+ fromDate.ToShortDateString() + " to " + toDate.ToShortDateString();
                    columnWidths = new int[] { 60, 100, 70, 70, 100, 100, 100, 100};
                }

                ReportDocument report = new ReportDocument(title, descrCols, values);

                if (MessageBox.Show("You are about to render " + dt.Rows.Count + " records.\n" +
                    "This will fit on roughly " + Math.Ceiling((double) dt.Rows.Count / 45) + " pages.\n The total rendering time is approx. " + Math.Ceiling((double)dt.Rows.Count/108) + " seconds.\n" +
                    "Would you like to proceed?",
                    "REPORT INFO", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    PageContent[] pagesOnly =
                        Paginator.Paginate(dt, 9, columnWidths, new string[] { "L", "L", "L", "L", "L", "L", "L", "L", "L" , "L"});
                    page1.Children.Add(report);
                    docViewer1.Document = pagesOnly.AppendFirstPage(page1);
                }
                else
                {

                }
            }
            catch (Exception ex)
            {            
                MessageBox.Show("An error occurred while generating report.\nDETAILS:" + ex.Message);
            }    
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            fromdtPicker.SelectedDate = DateTime.Today;
            todtPicker.SelectedDate = DateTime.Today;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblTeller.Visibility = cbxTeller.Visibility = Visibility.Hidden;
            reportTypeCbx.SelectionChanged+=reportTypeCbx_SelectionChanged;

            fromdtPicker.SelectedDate = DateTime.Now;
            todtPicker.SelectedDate = DateTime.Now;
        }

        private void reportTypeCbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void cbxTeller_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string teller = cbxTeller.SelectedValue.ToString();

                IEnumerable<DataRow> matchingRows = from myRows in dt.AsEnumerable()  //use current source?
                                                    where myRows["TELLER"].ToString() == teller
                                                    select myRows;

                DataTable temp = matchingRows.CopyToDataTable();
                title = "SALES SUMMARY FOR " + teller + " ON SELECTED DATES";
                descrCols = new string[] { "ENTRIES", "TOTAL VALUE", "", "", "", "" };
                values = new string[] { temp.Rows.Count.ToString(), temp.Columns["AMOUNTPAID"].AsEnumerable<int>().Sum().ToString("N0"), "", "", "", "" };
                columnWidths = new int[] { 60, 70, 70, 70, 70, 100, 80, 100, 100 };

                ReportDocument report = new ReportDocument(title, descrCols, values);
                PageContent[] pagesOnly = Paginator.Paginate(temp, 11, columnWidths, new string[] { "L", "L", "L", "L", "L", "L", "L", "L", "L" });
                FixedPage page1 = new FixedPage();
                page1.Children.Add(report);
                docViewer1.Document = pagesOnly.AppendFirstPage(page1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\nDETAILS:" + ex.Message);
            }
        }

        private void chkbxTINCustomers_Checked(object sender, RoutedEventArgs e)
        {
            txtTIN.IsEnabled = false;
            chkbxTINCustomersIsChecked = true;
        }

        private void ChkbxTINCustomers_Unchecked(object sender, RoutedEventArgs e)
        {
            txtTIN.IsEnabled = true;
            chkbxTINCustomersIsChecked = false;
        }

        private void fromdtPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (todayChkBox.IsChecked==true)
            {
                todtPicker.SelectedDate = (e.Source as DatePicker).SelectedDate;
            }
        }
    }
}
