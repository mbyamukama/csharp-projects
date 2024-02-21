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
    /// Interaction logic for StatementViewer.xaml
    /// </summary>
    public partial class StatementViewer : Window
    {
        public StatementViewer(FixedDocument fdoc)
        {
            InitializeComponent();
            this.dViewer.Document = fdoc;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
