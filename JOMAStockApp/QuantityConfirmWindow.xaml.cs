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
    public partial class QuantityConfirmWindow : Window
    {
        int maxAllowedValue = 0;

        public int EnteredValue { get; private set; }

        public QuantityConfirmWindow(int maxAllowedValue)
        {
            InitializeComponent();
            this.maxAllowedValue = maxAllowedValue;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (txtQty.Text != "")
            {
                try
                {
                    if ((EnteredValue=Int32.Parse(txtQty.Text)) <= maxAllowedValue)
                    {
                        this.DialogResult = true;
                        this.Close();
                    }
                    else MessageBox.Show("The entered value is greater than the quantity value in the purchase.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch(FormatException ex)
                {
                    MessageBox.Show("The value entered was not recognized as a valid number.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
                if (txtQty.Text != "")
                {
                    try
                    {
                        if ((EnteredValue = Int32.Parse(txtQty.Text)) <= maxAllowedValue)
                        {
                            this.DialogResult = true;
                            this.Close();
                        }
                        else MessageBox.Show("The entered value is greater than the quantity value in the purchase.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (FormatException ex)
                    {
                        MessageBox.Show("The value entered was not recognized as a valid number.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                    MessageBox.Show("The field contains no customer information.", "ERROR");
            }
        }
    }
}
