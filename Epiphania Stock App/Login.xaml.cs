using System;
using System.Windows;
using System.Windows.Input;
using GenericUtilities;

namespace StockApp
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
 
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FBDataHelper.OpenConnection();
        }
        private void DoLogin()
        {
            string[] cred = FBDataHelper.GetCredentials(txtUserName.Text.Trim());
            if (cred[0] != null)
            {
                bool authenticated = Hasher.ValidatePassword(txtPassword.Password, cred[0]);
                if (authenticated)
                {
                    User user = new User(txtUserName.Text.Trim().ToUpper(), Int32.Parse(cred[1]));
                    Session session = new Session(user, DateTime.Now);
                    AppUtilities.session = session;
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
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            DoLogin();
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DoLogin();
            }
        }
    }
}
