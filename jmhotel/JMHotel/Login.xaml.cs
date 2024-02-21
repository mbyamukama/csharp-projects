using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using GenericUtilities;
using System.Reflection;

namespace JMHotel
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

        }

        private void DoLogin()
        {
            string username = txtUserName.Text.Trim();
            object [] cred = JMFBDataHelper.GetCredentials(username.ToUpper());

            if (cred[0] != null)
            {
                bool authenticated = Hasher.ValidatePassword(txtPassword.Password, cred[0].ToString());
                if (authenticated)
                {
                    Session session = new Session(username, Convert.ToInt32(cred[1]), DateTime.Now);
                    AppUtilities.Session = session;
                    new MainWindow().Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Your username and password do not match.\n Please try again.", "ERROR");
                }
            }
            else
            {
                MessageBox.Show("An error occurred.\n The user may not be present in the database.", "ERROR");
            }
        }
        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    DoLogin();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occured.\nMESSAGE: " + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            DoLogin();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (JMFBDataHelper.OpenConnection())
            {
                this.Title = "Connection State: Open";
            }
            else
            {
                MessageBox.Show("An error occurred while trying to open the connection.\nThe servername may not be valid.\nPlease input correct server name or IP address", "ERROR");
            }

        }
    }
}
