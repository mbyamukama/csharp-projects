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
    /// Interaction logic for PrintReceipt.xaml
    /// </summary>
    public partial class PrintReceipt : Window
    {
        public PrintReceipt()
        {
            InitializeComponent();
        }

        private void btnYES_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void btnNO_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
