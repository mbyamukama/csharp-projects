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

namespace VUARMS
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
            CurrentUser currentUser = VUARMSFBDataHelper.LookUp(txtUserName.Text.Trim().ToUpper());
            string session = "";
            if (currentUser.WasFound)
            {
                bool authenticated = Hasher.ValidatePassword(txtPassword.Password, currentUser.HPass);
                if (authenticated)
                {
                    AppUtilities.CurrentUser = currentUser;
                    AppUtilities.sessionID = session = AppUtilities.CurrentUser.UserName.Replace("-", "") + "-" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString();
                    VUARMSFBDataHelper.AddLog(AppUtilities.CurrentUser.UserName, session, "Logged on");
                    new MainWindow().Show();
                    this.Close();

                    //save properties
                    Properties.Settings.Default.LastUser = currentUser.UserName;
                    Properties.Settings.Default.Save();
                }

                else
                {
                    VUARMSFBDataHelper.AddLog(txtUserName.Text.Trim().ToUpper(), "NA", "Failed log on.");
                    MessageBox.Show("Your username and password do not match.\n Please try again.", "ERROR");
                }
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
            VUARMSFBDataHelper.OpenConnection();
            txtUserName.Text = Properties.Settings.Default.LastUser;
            txtPassword.Focus();

        }
    }
}
