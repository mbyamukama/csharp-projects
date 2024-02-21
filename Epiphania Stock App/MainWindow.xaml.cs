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
    /// Interaction logic for NzaraWindow2.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnInventory_Click(object sender, RoutedEventArgs e)
        {
            new DataViewer("STOCK").ShowDialog();
        }

        private void btnAddStock_Click(object sender, RoutedEventArgs e)
        {
            new AddMultipleStock().ShowDialog();
        }

        private void btnMakeSale_Click(object sender, RoutedEventArgs e)
        {
            new SaleWindow().Show();
        }

        private void btnFinance_Click(object sender, RoutedEventArgs e)
        {
            new FinanceWindow().ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FBDataHelper.OpenConnection();
            if (AppUtilities.session.User.CLRlevel < 2)
            {
                btnFinance.IsEnabled = btnInventory.IsEnabled = btnTools.IsEnabled = false;
            }
        } 

        private void btnTools_Click(object sender, RoutedEventArgs e)
        {
            new ToolsWindow().ShowDialog();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F4)
            {
                if (AppUtilities.session.User.CLRlevel > 1)
                {
                    new Search().Show();
                }
            }
            if (e.Key == Key.F3)
            {
                if (AppUtilities.session.User.CLRlevel > 1)
                {
                    new SettingsWindow().Show();
                }
            }
            if (e.Key == Key.F5)
            {
                if (AppUtilities.session.User.CLRlevel > 2)
                {
                    new QueryWindow().Show();
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           /* if((sender as UIElement).GetType() == typeof(Button))
            {

            }*/
            if (MessageBox.Show("The application is going to shutdown now. Proceed?",
                "WARNING", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                AppUtilities.session.EndTime = DateTime.Now;
                FBDataHelper.AddLog(DateTime.Now, "USER " + AppUtilities.session.User.UserName + " logged on at " + AppUtilities.session.StartTime +
                    " and logged out at " + AppUtilities.session.EndTime);
                FBDataHelper.CloseConnection();
                Application.Current.Shutdown();
            }
            else
                e.Cancel = true;
        }
    }
}
