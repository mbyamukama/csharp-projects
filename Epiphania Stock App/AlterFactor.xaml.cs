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
    /// Interaction logic for AlterFactor.xaml
    /// </summary>
    public partial class AlterFactor : Window
    {
        private  double costpx = 0, salepx = 0;
        DataRow row = null;
        public Double Factor = 0;
        public AlterFactor(DataRow row)
        {
            InitializeComponent();
            this.row = row;
        }

        private void txtSalePrice_KeyDown(object sender, KeyEventArgs e)
        {         
            if (e.Key == Key.Enter)
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblCostPrice.Content = costpx = Convert.ToDouble(row["COSTPPU"]);
            lblFactor.Content = Factor = Math.Round(Convert.ToDouble(row["FACTOR"]), 6);
        }

        private void txtSalePrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtSalePrice != null && txtSalePrice.Text.Trim() != "")
            {
                 try
                {
                    salepx = Convert.ToDouble(txtSalePrice.Text.Trim());
                    lblFactor.Content = Factor = Math.Round(salepx / costpx, 5);
                }
                  catch (FormatException ex)
                  {
                      MessageBox.Show("Only numeric characters are permitted.\nDETAILS: "+ ex.Message, "ERROR");
                  }
                  catch (Exception ex)
                  {
                      MessageBox.Show("An unknown error occurred.\nDETAILS: " + ex.Message, "ERROR");
                  }
            }
        }
    }
}
