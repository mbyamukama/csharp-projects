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

namespace FRESUgandaERP
{
    /// <summary>
    /// Interaction logic for Users.xaml
    /// </summary>
    public partial class Users : Window
    {
        public Users()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUserName.Text.Trim();
            string pass1 = txtPass.Password;
            string pass2 = txtConfirmPass.Password;
            int clr = Int32.Parse(cbxClearance.Text);
            string station = cbxStation.Text;
            if (pass1.Equals(pass2))
            {
                if (FRESUGDBHelper.AddUser(username, pass1, clr,station))
                {
                    MessageBox.Show("User Added");
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
            cbxStation.SelectedIndex = 0;
        }
    }
}
