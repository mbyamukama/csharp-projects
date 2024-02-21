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
using System.Data;
using FirebirdSql.Data.FirebirdClient;

namespace StockApp
{
    /// <summary>
    /// Interaction logic for ToolsWindow.xaml
    /// </summary>
    public partial class ToolsWindow : Window
    {
        
        public ToolsWindow()
        {
            InitializeComponent();
        }

        private void btnLog_Click(object sender, RoutedEventArgs e)
        {
            new DataViewer("LOG").Show();             
        }

        private void btnReports_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnStat_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            new SettingsWindow().Show();
        }

        private void btnUsers_Click(object sender, RoutedEventArgs e)
        {
            if (AppUtilities.session.User.CLRlevel> 1)
            {
                new DataViewer("USERS").ShowDialog();
            }
            else MessageBox.Show("This information is inaccessible to your clearance Level", "ERROR");
        }

        private void btnAlerts_Click(object sender, RoutedEventArgs e)
        {
            new DataViewer("ALERTS").Show();
        }

    }
}
