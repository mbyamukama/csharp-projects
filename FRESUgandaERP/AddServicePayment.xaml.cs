using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using CEDAT.MathLab;
using System.Windows.Documents;
using System.Data;
using FRESUGERP.AppUtilities;

namespace FRESUgandaERP
{
    /// <summary>
    /// Interaction logic for AddPayment.xaml
    /// </summary>
    public partial class AddServicePayment : Window
    {
        DataTable dt = null;
        string clientId = "";
        int amtPaid = 0;
        string recNo = "";
        string toPayFeeType="SERVICE FEES", chequeNo = "", bank = "", payType = "";
        int newbal = 0;

        public AddServicePayment(string clientId)
        {
            InitializeComponent();
           
            this.clientId = clientId;
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
                bool res = false;

                if (toPayFeeType == "SERVICE FEES")
                {
                    res = FRESUGDBHelper.MakeServicePayment(amtPaid, txtPaidby.Text.Trim(),
                       DateTime.Now.AsFBDateTime(), clientId, payType, txtPaidby.Text.Trim(), out recNo, out newbal);
                }
                if (toPayFeeType == "CONNECTION FEES")
                {
                    res = FRESUGDBHelper.UpdateConnFee(amtPaid, txtPaidby.Text.Trim(),
                  DateTime.Now.AsFBDateTime(), clientId, payType, txtPaidby.Text.Trim(), out recNo, out newbal);
                }

                if (res)
                {
                    FRESUGDBHelper.AddLog(this.Title, UtilityExtensions.currentSession.USER, "payment of " + amtPaid + " added for client," + clientId + "," + DateTime.Now, UtilityExtensions.currentSession.STATION);
                    MessageBox.Show("Payment was successful", "SUCCESS");

                    if (MessageBox.Show("Do you want to print this receipt?",
                        "CONFIRM", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        IDocumentPaginatorSource idocSource;
                        PrintDialog pd = new PrintDialog();
                        if (pd.ShowDialog() == true)
                        {
                            idocSource = DocumentHelper.GenerateReceipt(recNo, clientId, FRESUGDBHelper.GetClientName(clientId), amtPaid,
                            DateTime.Now.ToShortDateString(), toPayFeeType, newbal, txtPaidby.Text.Trim(), payType);
                            pd.PrintDocument(idocSource.DocumentPaginator, "FRESUG PAYMENT RECEIPT");
                        }
                    }
                }

                txtClientID.Text = "";
                txtAmountPaid.Text = "0";
                txtPaidby.Text = "SELF";
            }
 

        private void cbxfeeType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            toPayFeeType = cbxfeeType.GetSelectedItemContent();
        }
        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dt = new DataTable();
        }

        private void txtAmountPaid_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string sAmtpaid = txtAmountPaid.Text.Trim();
                if (sAmtpaid != "")
                {
                    amtPaid = Int32.Parse(sAmtpaid);
                }
            }
            catch (FormatException ex)
            {
                MessageBox.Show("This field only takes numeric characters.\n TECH DETAILS: "+ ex.Message, "ERROR");
            }
        }

        private void dataGridResults_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
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

        private void cbxPayType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            payType = cbxPayType.GetSelectedItemContent();
            if (payType == "CHEQUE")
            {
                ChequeDetails window = new ChequeDetails();
                if (window.ShowDialog() == true)
                {
                    bank = window.Bank;
                    chequeNo = window.ChequeNo;
                }
                payType += " Bank: " + bank + " Cheque No: " + chequeNo;
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            clientId = txtClientID.Text.Trim();
            if (toPayFeeType == "SERVICE FEES")
            {
                string nextBillno = ""; //useless
                dt = FRESUGDBHelper.GetClientBills(clientId, out  nextBillno);
                if (dt.Rows.Count > 0)
                {
                    lblAmountDue.Content = Int32.Parse(dt.Rows[dt.Rows.Count - 1]["TOTAL"]+"").ToString("N0");
                }
            }
            if (toPayFeeType == "CONNECTION FEES")
            {
                dt = FRESUGDBHelper.GetConnFeeDetails(clientId);
                if (dt.Rows.Count > 0)
                {
                    lblAmountDue.Content = Int32.Parse(dt.Rows[dt.Rows.Count - 1]["BALANCE"]+"").ToString("N0");
                }
            }
             if(dt.Rows.Count==0)
             {
                 MessageBox.Show("The search returned no results.", "CLIENTID ERROR");
             }
            dataGridResults.ItemsSource = dt.DefaultView;
        }

    }
}
