using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using FirebirdSql.Data.FirebirdClient;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DataAnalytics
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string connstr = @"User=SYSDBA;Password=masterkey;Database=localhost:E:\Dropbox\databases\ANALYTICS.FDB";
        FbConnection conn = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            conn = new FbConnection(connstr);
            conn.Open();

            //populate dropdown box

        }

        private void Gobutton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
