using System;                  
using System.Linq;       
using System.Windows;     
using System.Data;
using System.Windows.Input;

namespace StockApp
{
    /// <summary>
    /// Interaction logic for VisaWindow.xaml
    /// </summary>
    public partial class RedeemWindow : Window
    {
        DataRow customerData = null;
        int points = 0, pointsValue = 0, redeemRate = 5, amtdue = 0;
        public Int32 AuthorizedAmount = 0, PointsUsed = 0;
        public RedeemWindow(DataRow customerData, int redeemRate, int amtdue)
        {
            InitializeComponent();
            this.customerData = customerData;
            this.redeemRate = redeemRate;
            this.amtdue = amtdue;
        }

        private void txtRedeemAmt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtRedeemAmt.Text != "")
                {
                    if ((AuthorizedAmount = Convert.ToInt32(txtRedeemAmt.Text)) <= pointsValue)
                    {
                        if (AuthorizedAmount <= amtdue)
                        {
                            PointsUsed = AuthorizedAmount / redeemRate;
                            this.DialogResult = true;
                        }
                        else
                            MessageBox.Show("The amount to redeem is greater than the amount due.\nThe maximum value you can enter is " + amtdue.ToString("N0"),
                            "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                        MessageBox.Show("The amount to redeem is greater than the avalaible points value",
                            "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("There was no data entered.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            lblCustName.Content = " NAME:\t" + customerData["CNAME"];
            points = Convert.ToInt32(customerData["POINTS"]);
            pointsValue = points * redeemRate;
            lblPointsValue.Content = "POINTS:\t" + points + "   \t VALUE:\t" + pointsValue.ToString("N0") ;
        }
    }
}
