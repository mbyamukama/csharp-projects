using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Windows.Documents;
using GenericUtilities;

namespace JMHotel
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

        public Reports()
        {
            InitializeComponent();
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            columnWidths = new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 150 };
             try
            {
                DateTime fromDate = fromdtPicker.SelectedDate.Value;
                DateTime toDate = todtPicker.SelectedDate.Value;
                FixedPage page1 = new FixedPage();

                if (reportTypeCbx.Text == "VISITS")
                {
                    dt = JMFBDataHelper.GetVisits(fromDate, toDate);

                    int countWk = 0, countNR = 0, countOn = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        if (Convert.ToDateTime(row["VTIMESTAMP"]).DayOfWeek == DayOfWeek.Friday
                            || Convert.ToDateTime(row["VTIMESTAMP"]).DayOfWeek == DayOfWeek.Saturday
                            || Convert.ToDateTime(row["VTIMESTAMP"]).DayOfWeek == DayOfWeek.Sunday)
                        {
                            ++countWk;
                        }
                        if (row["GUESTTYPE"].ToString() == "WALK IN")
                        {
                            ++countNR;
                        }
                        if (row["STATUS"].ToString() == "ONGOING")
                        {
                            ++countOn;
                        }
                    }

                    title = "VISITS FROM " + fromDate.ToShortDateString() + " to " + toDate.ToShortDateString();
                    descrCols = new string[] { "ENTRIES", "WEEKEND", "NON-RESIDENTIAL", "ON-GOING", "", "" };
                    values = new string[] { dt.Rows.Count.ToString(), countWk.ToString(), countNR.ToString(), countOn.ToString(), "", "" };

                    columnWidths = new int[] { 50, 80, 50, 120, 80, 90, 80, 80, 100, 200, 200, 150 };
                }

                if (reportTypeCbx.Text == "BILLS")
                {
                    dt = JMFBDataHelper.GetBills(fromDate, toDate);

                    title = "BILL REPORT FOR " + fromDate.ToShortDateString() + " to " + toDate.ToShortDateString();
                    descrCols = new string[] { "ENTRIES", "TOTAL VALUE", "", "", "", "" };
                    values = new string[] { dt.Rows.Count.ToString(), dt.Columns["AMOUNT"].AsEnumerable<int>().Sum().ToString("N0"), "", "", "", "" };
                    columnWidths = new int[] { 100, 100, 200, 100, 100, 100, 100, 100, 100, 100, 100 };
                }

                if (reportTypeCbx.Text == "RESERVATIONS")
                {

                }

                if (reportTypeCbx.Text == "DELIVERIES")
                {
                    dt = JMFBDataHelper.GetDeliveries(fromDate, toDate);
                    title = "DELIVERIES REPORT FOR " + fromDate.ToShortDateString() + " to " + toDate.ToShortDateString();
                    descrCols = new string[] { "ENTRIES", "COST", "", "", "", "" };
                    values = new string[] { dt.Rows.Count.ToString(), dt.Columns["PRICE"].AsEnumerable<int>().Sum().ToString("N0"), "", "", "", "" };
                }
                Report report = new Report(title, descrCols, values);

                PageContent[] pagesOnly = Paginator.Paginate(dt, 10, columnWidths, null);
                page1.Children.Add(report);
                docViewer1.Document = pagesOnly.AppendFirstPage(page1);
            }
             catch (Exception ex)
             {            
                 MessageBox.Show("An error occurred while generating report.\nDETAILS:" + ex.Message,"ERROR",MessageBoxButton.OK,MessageBoxImage.Error);
             }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            fromdtPicker.SelectedDate = DateTime.Now.AddDays(-30);
            todtPicker.SelectedDate = DateTime.Now;
        }

    }
}
