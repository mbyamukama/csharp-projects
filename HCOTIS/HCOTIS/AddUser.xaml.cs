using System;
using System.Windows;

namespace HCOTIS
{
    /// <summary>
    /// Interaction logic for Users.xaml
    /// </summary>
    public partial class AddUser : Window
    {
        public AddUser()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUserName.Text.Trim();
            string pass1 = txtPass.Password;
            string pass2 = txtConfirmPass.Password;
            int clr = Int32.Parse(cbxClearance.Text);  
            if (pass1.Equals(pass2))
            {
                if (FBDataHelper.AddUser(username, pass1, clr))
                {
                    MessageBox.Show("User Added.","SUCCESS");
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("The passwords do not match", "PASSWORD MISMATCH");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
         
        }
    }
}
