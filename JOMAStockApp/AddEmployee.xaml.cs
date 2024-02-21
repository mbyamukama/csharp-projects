using StockApp.AppExtensions;
using System;
using System.Windows;
using System.Windows.Controls;

namespace StockApp
{
    /// <summary>
    /// Interaction logic for AddEmployee.xaml
    /// </summary>
    public partial class AddEmployee : Window
    {
        bool update = false;
        string[] details = null;

        public AddEmployee(bool upDate, string [] details)
        {
            InitializeComponent();

            this.update = upDate;
            this.details = details;
            if (update)
            {
                txtFName.Text = details[0]; txtFName.IsEnabled = false;
                txtLName.Text = details[1]; txtLName.IsEnabled = false;
                txtPhone.Text = details[2];
                txtUserName.Text = details[3]; txtUserName.IsEnabled = false;
                if (GlobalSystemData.Session.CurrentUser.UserName == details[3])  //same user
                {
                    cbxClearence.IsEnabled = false;
                }
                
            }
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            bool success = false;

            if (!(txtFName.Text == "" || txtLName.Text == "" || txtPass1.Password == "" ||
                txtPass2.Password == "" || txtPhone.Text == "" || txtUserName.Text == ""))
            {
                if (txtPass1.Password.Trim().Equals(txtPass2.Password.Trim()))
                {
                    string password = txtPass1.Password;
                    if (update)
                    {
                        success = FBDataHelper.UpdateEmployee(txtUserName.Text.Trim().ToUpper(),
                            txtPhone.Text.Trim(), Hasher.CreateHash(password), Int32.Parse(cbxClearence.Text));
                        if (success)
                        {
                            MessageBox.Show("Employee details updated successfully.", "SUCCESS");
                        }
                        else
                        {
                            MessageBox.Show("Employee details NOT updated.", "ERROR");
                        }
                    }
                    else
                    {
                        success = FBDataHelper.AddEmployee(txtFName.Text.Trim().ToUpper(), txtLName.Text.Trim().ToUpper(), txtPhone.Text.Trim(),
                       txtUserName.Text.Trim().ToUpper(), Hasher.CreateHash(password), Int32.Parse(cbxClearence.Text));
                        if (success)
                        {
                            MessageBox.Show("Employee added successfully.", "SUCCESS");
                        }
                        else
                        {
                            MessageBox.Show("Employee not added.", "ERROR");
                        }
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

        private void txtLName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!update)
            {
                if (txtLName.Text != "")
                    txtUserName.Text = txtFName.Text.ToLower()[0] + txtLName.Text.Trim().ToLower();
            }
        }

        private void txtFName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!update)
            {
                if (txtFName.Text != "")
                    txtUserName.Text = txtFName.Text.ToLower()[0] + txtLName.Text.Trim().ToLower();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbxClearence.SelectedIndex = 0;
        }
    }
}
