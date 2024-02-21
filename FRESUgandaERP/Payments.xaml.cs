using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FRESUgandaERP
{
    /// <summary>
    /// Interaction logic for Paymenys.xaml
    /// </summary>
    public partial class Payments : Window
    {
        public Payments()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnOther_Click(object sender, RoutedEventArgs e)
        {
            if (FRESUGERP.AppUtilities.UtilityExtensions.currentSession.CurrentCLR > 1)
            {
                new OtherIncome().Show();
            }
            else
            {
                MessageBox.Show("You do not have sufficient permissions to access this information", "ERROR");
            }
        }

        private void btnServicePay_Click(object sender, RoutedEventArgs e)
        {
            new ViewPayments().Show();
        }

        private void btnExpenses_Click(object sender, RoutedEventArgs e)
        {
            if (FRESUGERP.AppUtilities.UtilityExtensions.currentSession.CurrentCLR > 1)
            {
                new Expenses().Show();
            }
            else
            {
                MessageBox.Show("You do not have sufficient permissions to access this information", "ERROR");
            }
        }
    }
}
