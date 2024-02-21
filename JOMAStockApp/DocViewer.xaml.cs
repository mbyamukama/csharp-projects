using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for BillViewer.xaml
    /// </summary>
    public partial class DocViewer : Window
    {
        private FixedDocument doc = null;
        public DocViewer(FixedDocument document)
        {
            InitializeComponent();
            doc = document;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            documentViewer1.Document = doc;
        }
    }
}
