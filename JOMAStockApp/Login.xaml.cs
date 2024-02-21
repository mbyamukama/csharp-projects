using System;
using System.Windows;
using System.Windows.Input;
using StockApp.AppExtensions;
using System.Data;
using System.Diagnostics;

namespace StockApp
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public User user = null;

        public Login()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

        }

        private void DoLogin()
        {
            try
            {
                string[] cred = FBDataHelper.GetCredentials(txtUserName.Text.Trim().ToUpper());
                if (cred[0] != null)
                {
                    bool authenticated = Hasher.ValidatePassword(txtPassword.Password, cred[0]);
                    if (authenticated)
                    {
                        user = new User(txtUserName.Text.Trim().ToUpper(), Int32.Parse(cred[1]), cred[0]);
                        GlobalSystemData.Session = new Session(user, DateTime.Now);
                        if (user.IsManager())
                        {
                            new MainWindow().Show();
                        }
                        else
                        {
                            new SaleWindow().Show();
                        }
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Your username and password do not match.\n Please try again.", "ERROR");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobalSystemData.Session.Log += "\n Error Code " + ((FirebirdSql.Data.FirebirdClient.FbException)ex).ErrorCode + "occured at Login for " + txtUserName.Text;
            }

        }
        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DoLogin();
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            DoLogin();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string server = "";
            if (FBDataHelper.OpenConnection(out server))
            {
                this.Title = "Connection State: " + server + ": Open";
            }
            else
            {
                MessageBox.Show("An error occurred while trying to open the connection.\nThe servername may not be valid.\nPlease input correct server name or IP address", "ERROR");
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                Process p = new Process();
                p.StartInfo.FileName = "diagnostic.exe";
                p.Start();
            }
        }
    }
}
