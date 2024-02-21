using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Windows.Documents;
using CEDAT.MathLab;
using FirebirdSql.Data.FirebirdClient;
using System.IO;
using FRESUGERP.AppUtilities;

namespace FRESUgandaERP
{
    /// <summary>
    /// Interaction logic for Bills.xaml
    /// </summary>
    public partial class Bills : Window
    {
        DataTable bills = null, ageingTable = null;
        DataTable fees = null;
        DateTime selectedDate = DateTime.Now;

        public Bills()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //get current month and year: load ALL dueClients of this month into DB
            fees = FRESUGDBHelper.GetFees();
        }
        

        private void btnPrintAll_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<DataRowView> items = dtGridBillsDue.SelectedItems.Cast<DataRowView>();
            List<DataRow> billList = new List<DataRow>();
            foreach (DataRowView item in items)
            {
                billList.Add(item.Row);
            }

            Window window = new Window();
            window.Title = "Selected Bills List";
            DocumentViewer dv = new DocumentViewer();
            dv.Height = 450; dv.Width = 900;
            dv.Document = DocumentHelper.GenerateBills(billList);
            window.Content = dv;
            window.Show();


        }

        private void dtGridBillsDue_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(DateTime))
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd-MMM-yy";
            }
            if (e.PropertyType == typeof(Int32) || e.PropertyType == typeof(Int64))
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "N0";

            }
        }

        private void menuGenerateBills_Click(object sender, RoutedEventArgs e)
        {
            MonthWindow mWindow = new MonthWindow();
            string selectedMonth = "";
            if (mWindow.ShowDialog() == true)
            {
                selectedMonth = mWindow.SelectedMonth;
            }

            if (selectedMonth != "")
            {
                DateTime selectedMonthStart = DateTime.Parse(selectedMonth + " 01, " + DateTime.Now.Year);
                bool thisMonthAlreadyProcessed = FRESUGDBHelper.CheckThisMonthAlreadyProcessed(selectedMonthStart);
                if (thisMonthAlreadyProcessed)
                {
                    MessageBox.Show("The bills for " + selectedMonth + ", " + DateTime.Now.Year + " have already been processed.", "ERROR");
                }
                else
                {
                    //check if its future month
                    MessageBoxResult result = MessageBoxResult.Yes;
                    if (selectedMonthStart.Month != DateTime.Now.Month)
                    {
                        result = MessageBox.Show("The current month is : " + DateTime.Now.ToString("MMMM") +
                              "\nYou are about to generate bills of : " + selectedMonthStart.ToString("MMMM") +
                              "\nAre you sure you would like to continue?", "CONTINUE", MessageBoxButton.YesNo);
                    }
                    if (result == MessageBoxResult.Yes)
                    {
                        DataTable dt = FRESUGDBHelper.GetClients("CLIENTID, FULLNAME, SLEVEL, CONNDATE ");  //get all clients
                        foreach (DataRow row in dt.Rows)
                        {
                            //2. Current date is now 01/Month/Year
                            //Test to see whose date is current date
                            DateTime currClientDuedate = selectedMonthStart;
                            for (int i = 0; i <= 30; i++)
                            {
                                currClientDuedate = selectedMonthStart.Date.AddDays(i);
                                int daysElapsed = currClientDuedate.Subtract(Convert.ToDateTime(row["CONNDATE"])).Days;
                                if (daysElapsed % 30 == 0 && daysElapsed > 30) //don't bill new clients
                                {
                                    break;  //due date for this client was found
                                }
                            }

                            if (currClientDuedate.Month == selectedMonthStart.Month)  //dont step into next month!!
                            {
                                int amtDue = 0;
                                string billNo = "";
                                string clientId = row["CLIENTID"].ToString();
                                FRESUGDBHelper.GetClientBills(clientId, out billNo);
                                string billduedate = currClientDuedate.AsFBDateTime();

                                //start of period is 30 days less
                                DateTime startOfPeriod = currClientDuedate.Date.AddDays(-29);
                                string billPeriod = startOfPeriod.ToShortDateString() + " to " + currClientDuedate.Date.ToShortDateString();

                                amtDue = Convert.ToInt32((from myRow in fees.AsEnumerable()
                                                          where Convert.ToInt32(myRow["SLEVEL"]) == Convert.ToInt32(row["SLEVEL"])
                                                          select myRow).ElementAt(0)["SERVICEFEE"]);

                                FRESUGDBHelper.AddBill(billNo, billduedate, billPeriod, clientId, amtDue);
                            }
                        }
                    }
                    else
                        MessageBox.Show("The operation has been aborted.", "ABORTED");
                }

            }
        }

        private void menuExport_Click(object sender, RoutedEventArgs e)
        {
            //get underlying items source
            DataTable source = (dtGridBillsDue.ItemsSource as DataView).Table;

            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = "CSV File|*.csv";
            if (sfd.ShowDialog(this) == true)
            {
                CEDAT.MathLab.Utilities.WriteCSVFile(source, sfd.FileName);
            }
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void menuViewAgeing_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ageingTable = bills.Copy();
                ageingTable.Columns.Add("1-30 days");
                ageingTable.Columns.Add("31-60 days");
                ageingTable.Columns.Add("61-90 days");
                ageingTable.Columns.Add("over 90 days");

                foreach (DataRow row in ageingTable.Rows)
                {
                    int[] ages = FRESUGERP.AppUtilities.UtilityExtensions.Decompose(Convert.ToInt32(row["TOTAL"]), Convert.ToInt32(row["SERVICEFEE"]));

                    row["1-30 days"] = ages[0];
                    row["31-60 days"] = ages[1];
                    row["61-90 days"] = ages[2];
                    row["over 90 days"] = ages[3];
                }
                dtGridBillsDue.ItemsSource = ageingTable.DefaultView;
                dtGridBillsDue.FontSize = 10;
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show("Loading ageing information failed.\nPlease confirm that some bills are in view currently.\nDETAILS: "
                    + ex.Message, "ERROR");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unknown error occurred.\nDETAILS: " + ex.Message, "ERROR");
            }
                             
        }

        private void menuBillsDue_Click(object sender, RoutedEventArgs e)
        {
            DateSelector dateWindow = new DateSelector();
            if (dateWindow.ShowDialog() == true)
            {
                bills = FRESUGDBHelper.GetBills(dateWindow.startDate, dateWindow.endDate);
                dtGridBillsDue.ItemsSource = bills.DefaultView;
                lblCount.Content = dtGridBillsDue.Items.Count + " items returned";
                
            }
        }


        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text.Trim().ToUpper();
           EnumerableRowCollection <DataRow> results = from rows in (dtGridBillsDue.ItemsSource as DataView).Table.AsEnumerable()
                      where rows.Field<string>("CLIENTID").ToUpper().StartsWith(searchText)
                      || rows.Field<string>("FULLNAME").ToUpper().StartsWith(searchText)
                      select rows;
            int rowCount = results.Count();
            lblCount.Content = rowCount + " items found";

            if (rowCount > 0)
            {
                dtGridBillsDue.ItemsSource = results.AsDataView<DataRow>();
            }
            else
            {
                MessageBox.Show("No Matching Results", "ERROR");
            }
        }

        private void menuDeleteBills_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuRawData_Click(object sender, RoutedEventArgs e)
        {
            if (FRESUGERP.AppUtilities.UtilityExtensions.currentSession.CurrentCLR > 2)
            {
                new RawDataView().ShowDialog();
            }
            else
            {
                MessageBox.Show("This functionality is only for Admin", "ERROR");
            }
        }
    }
}
