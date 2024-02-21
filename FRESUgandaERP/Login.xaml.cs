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
using FRESUGERP.AppUtilities;

namespace FRESUgandaERP
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        Session session = new Session();

        public Login()
        {
            InitializeComponent();
            
        }

        private void DoLogin()
        {
            try
            {
                bool result = false;
                object[] cred = FRESUGDBHelper.GetCredentials(txtUserName.Text.Trim(), out result);              
                if (result)
                {
                    int clr = (int)cred[1];
                    string hash = (string)cred[0];
                    bool valid = CEDAT.MathLab.Hasher.ValidatePassword(txtPassword.Password, hash);
                    if (valid)
                    {
                        session.USER = txtUserName.Text.Trim();
                        session.CurrentCLR = clr;
                        session.STATION = Properties.Settings.Default.Station;
                        UtilityExtensions.currentSession = session;
                        new Home(clr).Show();
                        FRESUGDBHelper.AddLog(this.Title, UtilityExtensions.currentSession.USER, "log on was successful at " + DateTime.Now, UtilityExtensions.currentSession.STATION);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("The login details provided are invalid.", "ERROR");
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("An unknown error occurred.\n DETAILS: " + ex.Message, "ERROR");
            }
        }
        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                DoLogin();              
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FRESUGDBHelper.OpenConnection();
            
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
           DoLogin();
        }
    }
}
