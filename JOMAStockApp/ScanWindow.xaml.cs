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

namespace StockApp
{
    /// <summary>
    /// Interaction logic for ScanWindow.xaml
    /// </summary>
    public partial class ScanWindow : Window
    {
        public ScanWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (txtCustomerID.Text != "")
            {
                GlobalSystemData.Session.CurrentCustomer = txtCustomerID.Text.Trim();
                this.DialogResult = true;
                this.Close();
            }
            else
                MessageBox.Show("The field contains no customer information.", "ERROR");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void txtCustomerID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtCustomerID.Text != "")
                {
                    GlobalSystemData.Session.CurrentCustomer = txtCustomerID.Text.Trim();
                    this.DialogResult = true;
                    this.Close();
                }
                else
                    MessageBox.Show("The field contains no customer information.", "ERROR");
            }
        }
    }
}
