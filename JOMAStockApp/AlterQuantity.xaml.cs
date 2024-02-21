using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StockApp
{
    /// <summary>
    /// Interaction logic for AlterQuantity.xaml
    /// </summary>
    public partial class AlterQuantity : Window
    {
        DataRow row = null;
        public Double Factor = 0;
        int origQty = 0;
        public Int32 CurrQty = 0, amtToAdd = 0;

        public AlterQuantity(DataRow row)
        {
            InitializeComponent();
            this.row = row;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblCurrQty.Content = origQty = Convert.ToInt32(row["QUANTITY"]);
        }

        private void txtAmountAdd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void txtAmountAdd_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtAmountAdd != null)
            {
                if (txtAmountAdd.Text.Trim() == "") amtToAdd = 0;
                if (txtAmountAdd.Text.Trim() != "")
                {
                    try
                    {
                        amtToAdd = Convert.ToInt32(txtAmountAdd.Text.Trim());

                    }
                    catch (FormatException ex)
                    {
                        MessageBox.Show("Only numeric characters are permitted.\nDETAILS: " + ex.Message, "ERROR");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An unknown error occurred.\nDETAILS: " + ex.Message, "ERROR");
                    }
                }
            }
            lblNewQty.Content = CurrQty = origQty + amtToAdd;
        }
    }
}
