using System;
using System.Linq;
using System.Windows;
using System.Data;
using System.Windows.Documents;
using GenericUtilities;

namespace StockApp
{
    /// <summary>
    /// Interaction logic for Reports.xaml
    /// </summary>
    public partial class Reports : Window
    {
        DataTable dt = null;
        public Reports()
        {
            InitializeComponent();
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            try
            {           
                string title = "";
                string[] descrCols = null, values = null;
                DateTime fromDate = fromdtPicker.SelectedDate.Value;
                DateTime toDate = todtPicker.SelectedDate.Value;
                if (reportTypeCbx.Text == "FINANCIAL")
                {
                    dt = FBDataHelper.GetPaymentDetails();

                    Int64[] stockVal = FBDataHelper.GetStockValue();
                    Int64 sales = FBDataHelper.GetTotalSales(fromDate, toDate);
                    Int64 credit = dt.Columns["ACCUMCREDIT"].GetColumnTotal();
                    Int64 expenses = FBDataHelper.GetTotalExpenses(fromDate, toDate);
                    Int64 profit = FBDataHelper.GetTotalProfit(fromDate, toDate);


                    title = "FINANCIAL REPORT " + fromDate.ToShortDateString() + " to " + toDate.ToShortDateString();
                    descrCols = new string[] { "STOCK VALUE(COST PRICE)", "STOCK VALUE(SALE PRICE)", "TOTAL CREDIT", "SALES", "EXPENSES", "PROFIT" };
                    values = new string[] { stockVal[0].ToString("N0"), stockVal[1].ToString("N0"), credit.ToString("N0"), sales.ToString("N0"), expenses.ToString("N0"), profit.ToString("N0") };
                }
                if (reportTypeCbx.Text == "EXPENSES")
                {
                    dt = FBDataHelper.GetExpenses(fromDate, toDate);
                    title = "EXPENSES FOR " + fromDate.ToShortDateString() + " to " + toDate.ToShortDateString();
                    descrCols = new string[] { "TOTAL EXPENSES", "", "", "", "", "" };
                    values = new string[] { dt.Columns["AMTPAID"].AsEnumerable<int>().Sum().ToString("N0"), "", "", "", "", "" };
                }

                if (reportTypeCbx.Text == "SALES (DETAILED)")
                {
                    dt = FBDataHelper.GetDetailedSales(fromDate, toDate);

                    title = "DETAILED SALES FOR " + fromDate.ToShortDateString() + " to " + toDate.ToShortDateString();
                    descrCols = new string[] { "ENTRIES", "TOTAL VALUE", "", "", "", "" };
                    values = new string[] { dt.Rows.Count.ToString(), dt.Columns["AMOUNTDUE"].AsEnumerable<int>().Sum().ToString("N0"), "", "", "", "" };

                }
                if (reportTypeCbx.Text == "SALES (SUMMARY)")
                {
                    dt = FBDataHelper.GetSales(fromDate, toDate);

                    title = "SALES SUMMARY FOR " + fromDate.ToShortDateString() + " to " + toDate.ToShortDateString();
                    descrCols = new string[] { "ENTRIES", "TOTAL VALUE", "", "", "", "" };
                    values = new string[] { dt.Rows.Count.ToString(), dt.Columns["AMOUNTDUE"].AsEnumerable<int>().Sum().ToString("N0"), "", "", "", "" };
                }
                if (reportTypeCbx.Text == "SALES (INSURANCE)")
                {
                    dt = FBDataHelper.GetInsuranceSales(fromDate, toDate);

                    title = "INSURANCE SALES FOR " + fromDate.ToShortDateString() + " to " + toDate.ToShortDateString();
                    descrCols = new string[] { "ENTRIES", "TOTAL VALUE", "", "", "", "" };
                    values = new string[] { dt.Rows.Count.ToString(), dt.Columns["AMOUNTDUE"].AsEnumerable<int>().Sum().ToString("N0"), "", "", "", "" };
                }
                if (reportTypeCbx.Text == "PURCHASES")
                {
                    dt = FBDataHelper.GetStock(FBDataHelper.StockType.Detailed, fromDate, toDate);

                    int cash = (from myRows in dt.AsEnumerable()
                                where myRows["PURCHASE"].ToString() == "CASH"
                                select myRows).GetColumnTotal("COSTPRICE");

                    int credit = (from myRows in dt.AsEnumerable()
                                  where myRows["PURCHASE"].ToString() == "CREDIT"
                                  select myRows).GetColumnTotal("COSTPRICE");

                    title = "PURCHASES FOR " + fromDate.ToShortDateString() + " to " + toDate.ToShortDateString();
                    descrCols = new string[] { "ENTRIES", "CASH", "CREDIT PUR", "", "", "" };
                    values = new string[] { dt.Rows.Count.ToString(), cash.ToString("N0"), credit.ToString("N0"), "", "", "" };
                }

                Report report = new Report(title, descrCols, values);

                PageContent[] pagesOnly = Paginator.Paginate(dt, 12);

                FixedPage page1 = new FixedPage();
                page1.Children.Add(report);
                docViewer1.Document = pagesOnly.AppendFirstPage(page1);
            }
            catch (Exception ex)
            {
                if (ex.Message.ToUpper().Contains("NULLABLE"))
                {
                    MessageBox.Show("Please select a range of dates for this function.", "ERROR");
                }
                else
                {
                    MessageBox.Show("An error occurred.\nDETAILS:" + ex.Message);
                }
                FBDataHelper.AddLog(DateTime.Now, ex.Message + "occurred on " + this.Name);
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = "CSV File|*.csv";
            if (sfd.ShowDialog(this) == true)
            {
                try
                {
                    if (dt.Rows.Count > 0)
                    {
                        Utilities.WriteCSVFile(dt, sfd.FileName + ".csv");
                    }
                    else
                        MessageBox.Show("There are no rows to export.", "ERROR");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occured.\n DETAILS: " + ex.Message, "ERROR");
                }
            }
        }
    }
}
