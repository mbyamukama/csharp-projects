using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Data;
using FRESUGERP.AppUtilities;

namespace FRESUgandaERP
{
    /// <summary>
    /// Interaction logic for Reports.xaml
    /// </summary>
    public partial class Reports : Window
    {
        DataTable final = null;
        string energyCentre = "ALL";
        string serviceLevel = "ALL";

        public Reports()
        {
            InitializeComponent();
        }

        private void cbxReportType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxReportType.GetSelectedItemContent() == "PAYMENTS")
            {
                cbxPaymentType.IsEnabled = true;
            }
            else
            {
                cbxPaymentType.IsEnabled = false;
            }

            if (cbxReportType.GetSelectedItemContent() == "DEBTORS")
            {
                fromdatePicker.IsEnabled = todatePicker.IsEnabled = false;
            }
            if (cbxReportType.GetSelectedItemContent() == "CLIENTS")
            {
                cbxEnergyCentre.IsEnabled = cbxServiceLevel.IsEnabled = false;
            }
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            final = new DataTable();

            DateTime fromDate = new DateTime(); 
            DateTime toDate = new DateTime();

            FixedDocument fDoc = new FixedDocument();
            Size a4PaperSize = new Size(797, 1123);

            energyCentre = UtilityExtensions.MapEnergyCentre(energyCentre);
            if (serviceLevel == "4+") serviceLevel = "5";
     
            try
            {
                if (fromdatePicker.IsEnabled == true && todatePicker.IsEnabled == true)
                {
                    fromDate = fromdatePicker.SelectedDate.Value;
                    toDate = todatePicker.SelectedDate.Value;
                }
                string title = "";
                string[] descrCols = null, values = null;

                if (cbxReportType.GetSelectedItemContent() == "COLLECTIONS")
                {
                    final = FRESUGDBHelper.GetBillExpectations(fromDate, toDate, energyCentre, serviceLevel);
                    title = "BILL REPORT: " + fromDate.ToShortDateString() + " to " + toDate.ToShortDateString() +
                        "\t\tENERGY CENTRE:  " + cbxEnergyCentre.GetSelectedItemContent() + "\tSERVICE:"
                        + cbxServiceLevel.GetSelectedItemContent();

                    int exp = final.Columns["TOTAL"].GetSum(), bal = final.Columns["BALANCE"].GetSum();

                    descrCols = new string[] { "EXPECTED COLLECTIONS", "AMOUNT UNPAID", "AMOUNT PAID", "% UNPAID" };
                    values = new string[] { exp.ToString("N0"), bal.ToString("N0"), (exp - bal).ToString(), Math.Round(bal * 100.0 / exp, 1).ToString() + "%" };
                }
                if (cbxReportType.GetSelectedItemContent() == "DEBTORS")
                {
                    final = FRESUGDBHelper.GetDebtors(energyCentre,serviceLevel);
                    title = "DEBTORS FOR ENERGY CENTRE:  " + cbxEnergyCentre.GetSelectedItemContent() + "\tSERVICE:"
                       + cbxServiceLevel.GetSelectedItemContent();

                     descrCols = new string[]{ "NUM. OF DEBTORS", "TOTAL SERV", "TOTAL CONN", "TOTAL PAID" };
                     values = new string[] { final.Rows.Count.ToString(), final.Columns["TOTALSERV"].GetSum().ToString("N0"), 
                         final.Columns["CONNFEEBAL"].GetSum().ToString("N0"), final.Columns["TOTALPAID"].GetSum().ToString("N0") };
                }
                if (cbxReportType.GetSelectedItemContent() == "CLIENTS")
                {
                    final = FRESUGDBHelper.GetNewClients(fromDate, toDate, energyCentre, serviceLevel);
                    title = "NEW CLIENTS  FOR PERIOD " + fromDate.ToShortDateString() + " to " + toDate.ToShortDateString() +
                        "\t\tENERGY CENTRE:  " + cbxEnergyCentre.GetSelectedItemContent() + "\tSERVICE:"
                        + cbxServiceLevel.GetSelectedItemContent();

                     descrCols = new string[]{ "NEW CLIENTS", "", "", "" };
                     values = new string[]{ final.Rows.Count.ToString(), "", "", "" };
                }
                if (cbxReportType.GetSelectedItemContent() == "PAYMENTS")
                {
                    string payType = cbxPaymentType.GetSelectedItemContent();
                    final = FRESUGDBHelper.GetPayments(fromDate, toDate, payType);
                    title = "PAYMENTS FOR PERIOD " + fromDate.ToShortDateString() + " to " + toDate.ToShortDateString();

                    descrCols = new string[] { "TOTAL", "", "", "" };
                    values = new string[] { final.Columns["AMTPAID"].GetSum().ToString("N0"), "", "", "" };
                }

                if (cbxReportType.GetSelectedItemContent() == "FINANCE")
                {

                    int spayments = FRESUGDBHelper.GetTotalPayments(fromDate, toDate);
                    int connpayments = FRESUGDBHelper.GetTotalConnFeePayments(fromDate, toDate);
                    DataTable indirectIncomes = FRESUGDBHelper.GetTotalIndirectIncomes(fromDate, toDate);
                    DataTable expenses = FRESUGDBHelper.GetTotalExpenses(fromDate, toDate);

                    final.Columns.Add("ITEM");
                    final.Columns.Add("AMOUNT");
                    final.Rows.Add("SERVICE PAYMENTS", spayments);
                    final.Rows.Add("CONN PAYMENTS", connpayments);

                    final.Rows.Add();
                    if (indirectIncomes.Rows.Count > 0)
                    {
                        final.Rows.Add("INDIRECT INCOMES");
                        foreach (DataRow row in indirectIncomes.Rows)
                        {
                            final.Rows.Add(row.ItemArray);
                        }
                    }

                    final.Rows.Add(); //space
                    if (expenses.Rows.Count > 0)
                    {
                        final.Rows.Add("EXPENSES");
                        foreach (DataRow row in expenses.Rows)
                        {
                            final.Rows.Add(row.ItemArray);
                        }
                    }

                    title = "INCOME REPORT FOR PERIOD " + fromDate.ToShortDateString() + " to " +
                        toDate.ToShortDateString();
                    descrCols = new string[]{ "SERVICE PAYMENTS", "CONN PAYMENTS", "INDIRECT INCOMES", "EXPENSES" };
                    values = new string[] { spayments.ToString("N0"), connpayments.ToString("N0"), indirectIncomes.Columns["SUM"].GetSum().ToString("N0"), expenses.Columns["TSUM"].GetSum().ToString("N0") };
                   
                }

                if (cbxReportType.GetSelectedItemContent() == "MAINTENANCE")
                {
                    final = FRESUGDBHelper.GetMaintenanceSchedule(fromDate, toDate, energyCentre, serviceLevel);
                    title = "MAINTENANCE STATUS FOR PERIOD " + fromDate.ToShortDateString() + " to " +
                        toDate.ToShortDateString();
                    descrCols = new string[] { "COUNT", "", "", "" };
                    values = new string[] { final.Rows.Count.ToString(), "", "", "" };
                    a4PaperSize = new Size( 1123, 797); //landscape
                }

                BillReport report = new BillReport(title, descrCols, values);
                fDoc = FRESUGERP.AppUtilities.Paginator.Paginate(a4PaperSize, final, 13, report);
                documentViewer1.Document = fDoc;
            }
            catch (System.InvalidOperationException ex)
            {
                MessageBox.Show("An error occurred.\n"+
                "Please verify that you have selected a range of dates for this report.\n TECHNICAL DETAILS: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unknown error occurred.\n TECHNICAL DETAILS: " + ex.Message);
            }
        }        

        private void cbxEnergyCentre_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            energyCentre = cbxEnergyCentre.GetSelectedItemContent();
        }

        private void cbxServiceLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            serviceLevel = cbxServiceLevel.GetSelectedItemContent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           cbxPaymentType.IsEnabled  = false;
        }
        
    }

}
