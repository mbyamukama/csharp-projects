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
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {                                                                                                     
            Application.Current.Shutdown();
        }

        private void btnInventory_Click(object sender, RoutedEventArgs e)
        {
            new DataViewer("STOCK").Show();
        }

        private void btnAddStock_Click(object sender, RoutedEventArgs e)
        {
            new AddMultipleStock().Show();
        }

        private void btnMakeSale_Click(object sender, RoutedEventArgs e)
        {
            new SaleWindow().Show();
        }

		private void btnAdmin_Click(object sender, RoutedEventArgs e)
		{
			new AdminWindow() { Owner = this }.Show();
		}

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string server = "";
            FBDataHelper.OpenConnection(out server);
            FirebirdSql.Data.Services.FbServerProperties sp = new FirebirdSql.Data.Services.FbServerProperties(FBDataHelper.connstr2);

            this.Title = "Logged on as: " + GlobalSystemData.Session.CurrentUser.UserName + "@" + server; // + "\t Version: " + sp.GetServerVersion();

            if (!GlobalSystemData.Session.CurrentUser.IsAdmin())
            {
                btnAdmin.IsEnabled = false;
            }
            SDataTable settings = FBDataHelper.GetSettings();
            GlobalSystemData.Settings = settings;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            GlobalSystemData.Session.Log += "\n Session End: " + DateTime.Now;
            FBDataHelper.AddLog(DateTime.Now, GlobalSystemData.Session.Log);
            FBDataHelper.CloseConnection();
        }

        private void btnAlerts_Click(object sender, RoutedEventArgs e)
        {
            new ViewAlerts().Show();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F4)
            {
                new Search().Show();
            }
            if (e.Key == Key.F5)
            {
                if (GlobalSystemData.Session.CurrentUser.IsAdmin())
                {
                    if (MessageBox.Show("You are about to bring up the Query Window.\n Statements executed in this window " +
                         "will affect the Database directly at a lower level, outside the management of the application.\n" +
                         "Would you like to proceed?", "CONFIRM", MessageBoxButton.YesNo,MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        new QueryWindow().Show();
                    }
                    else
                    {
                        MessageBox.Show("The operation has been aborted.", "ABORTED");
                    }
                }
            }
        }

    }
}
