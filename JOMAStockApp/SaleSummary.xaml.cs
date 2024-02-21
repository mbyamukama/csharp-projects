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
    /// Interaction logic for SaleSummary.xaml
    /// </summary>
    public partial class SaleSummary : UserControl
    {
        public SaleSummary(string info, string start, string end, string total, string cashcard)
        {
            InitializeComponent();

            lblInfo.Content =  info;
            lblStart.Content = start;
            lblEnd.Content = end;
            lblAmount.Content = total;
            lblTotal.Content = cashcard;
        }
    }
}
