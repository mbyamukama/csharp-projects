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

namespace JMHotel
{
    /// <summary>
    /// Interaction logic for AddEmployee.xaml
    /// </summary>
    public partial class AddUser : Window
    {
        public AddUser()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            bool success = false;
            if (!(txtPass1.Password == "" || txtPass2.Password == "" || txtUserName.Text == ""))
            {
                if (txtPass1.Password.Trim().Equals(txtPass2.Password.Trim()))
               {
                    string password = txtPass1.Password;                
                    success = JMFBDataHelper.AddUser(txtUserName.Text.Trim().ToUpper(), Hasher.CreateHash(password), Int32.Parse(cbxClearence.Text));
                        if (success)
                        {
                            MessageBox.Show("Employee added successfully.", "SUCCESS");
                        }
                        else
                        {
                            MessageBox.Show("Employee not added.", "ERROR");
                        }
                }
                else
                {
                    MessageBox.Show("The two passwords do not match. Please re-enter.", "PASSWORD MISMATCH");
                }
            }
            else
            {
                MessageBox.Show("Some fields are empty. Please enter information for all fields.", "MISSING INFORMATION");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbxClearence.SelectedIndex = 0;
            txtUserName.Focus();
        }
    }
}
