using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using FRESUGERP.AppUtilities;

namespace FRESUgandaERP
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Window
    {
        int clr = 0;
        public Home(int clr)
        {
            InitializeComponent();
            this.clr = clr;   //uncomment this when login is active          
        }

        private void btnClients_Click(object sender, RoutedEventArgs e)
        {
                new ViewClient().ShowDialog();            
        }
        private void btnBills_Click(object sender, RoutedEventArgs e)
        {
            if (clr > 1)
            {
                new Bills().ShowDialog();
            }
            else
            {
                MessageBox.Show("You do not have sufficient permissions to access this information", "ERROR");
            }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FRESUGDBHelper.OpenConnection();
           
        }

        private void btnPayments_Click(object sender, RoutedEventArgs e)
        {
            new Payments().Show();
        }

        private void btnUsers_Click(object sender, RoutedEventArgs e)
        {
            if (clr > 2)
            {
                new ViewUsers().ShowDialog();
            }
            else
            {
                MessageBox.Show("You do not have sufficient permissions to access this information", "ERROR");
            }
            
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F4)
            {
                if (clr > 2)
                {
                    new Settings().Show();
                }
                else
                {
                    MessageBox.Show("The F4 button was pressed. \n "+
                        "The current user does not have the required credentials to access this information", "ERROR");
                }
            }
            if (e.Key == Key.F5)
            {
                if (clr > 2)
                {
                    new LogWindow().Show();
                }
                else
                {
                    MessageBox.Show("The F5 button was pressed. \n " +
                        "The current user does not have the required credentials to access this information", "ERROR");
                }
            }
        }

        private void btnReports_Click(object sender, RoutedEventArgs e)
        {
            if (clr > 2)
            {
                new Reports().Show();
            }
            else
            {
                MessageBox.Show("The current user does not have the required credentials to access this information", "ERROR");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            FRESUGDBHelper.AddLog("Home Window", UtilityExtensions.currentSession.USER, "application closed at " + DateTime.Now, UtilityExtensions.currentSession.STATION);
        }

        private void btnMaintenance_Click(object sender, RoutedEventArgs e)
        {
            new Maintenance().Show();
        }

    }
}
