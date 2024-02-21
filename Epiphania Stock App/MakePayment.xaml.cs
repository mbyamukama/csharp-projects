using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data;

namespace StockApp
{
    /// <summary>
    /// Interaction logic for Payments.xaml
    /// </summary>
    public partial class MakePayment : Window
    {
        string supplier = "";
        Int64 amount = 0;
        public MakePayment(string supplier)
        {
            InitializeComponent();
            this.supplier = supplier;
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (amount >= 0)
            {
                bool result = FBDataHelper.MakePayment(supplier, amount, txtREF.Text.Trim());
                if (result) MessageBox.Show("Payment Success", "SUCCESS");
                this.Close();
            }
            else
                MessageBox.Show("Error. Negative payments are not permitted.", "ERROR");

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
             lblPayee.Content = "Make payment to " + supplier;
        }

        private void txtAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtAmount.Text.Trim() != "")
            {
                try
                {
                    amount = Convert.ToInt32(txtAmount.Text.Trim());
                }
                catch (FormatException ex)
                {
                    MessageBox.Show("Only numeric characters are permitted.\n DETAILS: "+ ex.Message, "ENTRY ERROR");
                }
                catch (OverflowException ex)
                {
                    MessageBox.Show("Value is too large.\n DETAILS: "+ ex.Message, "ENTRY ERROR");
                }
            }
        }
    }
}
