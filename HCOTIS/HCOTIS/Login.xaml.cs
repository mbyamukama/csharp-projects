
using System.Windows;
using GenericUtilities;
using System;

namespace HCOTIS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }


        private void txtPassword_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (e.Key == System.Windows.Input.Key.Enter)
                {
                    string hpass = ""; int clr = 1;
                    bool wasFound = FBDataHelper.LookUp(txtUserName.Text.Trim().ToUpper(), out hpass, out clr);
                    if (wasFound)
                    {
                        bool authenticated = Hasher.ValidatePassword(txtPassword.Password, hpass);
                        if (authenticated)
                        {
                            Session.clrLevel = clr;
                            Session.currentUser = txtUserName.Text.Trim();

                            new MainWindow().Show();
                            this.Close();
                        }

                        else
                        {
                            MessageBox.Show("Your username and password do not match.\n Please try again.", "ERROR");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\n DETAILS: " + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {
                bool isOpen = FBDataHelper.OpenConnection();
                if (isOpen)
                {
                    this.Title = "Connection Status: Open";
                }
                else
                    this.Title = "Connection Status: Closed";
            }

            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\n DETAILS: " + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
