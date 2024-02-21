using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Data;      

namespace HCOTIS
{
    /// <summary>
    /// Interaction logic for AddPayment.xaml
    /// </summary>
    public partial class AddPayment : Window
    {
        string orderId = "", payType = "", otherDet = "";
        int amtPaid =0, bal=0, newbal = 0;

        public AddPayment(string orderId, int balance, string otherDetails)
        {
            InitializeComponent();
            this.orderId = orderId;
            this.otherDet = otherDetails;
            this.bal = balance;
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
          FBDataHelper.AddPayment(orderId, amtPaid, newbal, payType);
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblOtherDet.Content = otherDet;
            lblAmountDue.Content = bal;
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
                else
                {
                    amtPaid = 0;
                }
                newbal = bal - amtPaid;
            }
            catch (FormatException ex)
            {
                MessageBox.Show("This field only takes numeric characters.\n TECH DETAILS: " + ex.Message, "ERROR");
            }
        }


        private void cbxPayType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            payType = cbxPayType.Text;
        }


    }
}
