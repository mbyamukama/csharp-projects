using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StockApp
{
    /// <summary>
    /// Interaction logic for VisaWindow.xaml
    /// </summary>
    public partial class VisaWindow : Window
    {
        public String AuthCode = "";
        int amountdue = 0;
        public VisaWindow(int amount)
        {
            InitializeComponent();
            amountdue = amount;
        }

        private void txtAuthCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtAuthCode.Text != "")
                {
                    AuthCode = txtAuthCode.Text;
                    this.DialogResult = true;
                }
                else
                {
                    MessageBox.Show("There was no data entered.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblAmountDue.Content = amountdue.ToString("N0");
        }
    }
}
