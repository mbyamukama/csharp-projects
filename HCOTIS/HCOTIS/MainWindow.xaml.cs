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

namespace HCOTIS
{
    /// <summary>
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnReports_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnOrders_Click(object sender, RoutedEventArgs e)
        {
            new Orders().Show();
        }

        private void btnPayments_Click(object sender, RoutedEventArgs e)
        {
            new Payments("").Show();
        }

        private void btnUsers_Click(object sender, RoutedEventArgs e)
        {
            new ViewUsers().Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {


        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnCalendar_Click(object sender, RoutedEventArgs e)
        {
            new Calendar().Show();
        }
    }
}