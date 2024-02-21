using System;
using System.Windows;
using System.Windows.Controls;
using GenericUtilities;

namespace StockApp
{
    /// <summary>
    /// Interaction logic for AddEmployee.xaml
    /// </summary>
    public partial class AddEmployee : Window
    {
        public AddEmployee()
        {
            InitializeComponent();
            cbxClearence.Items.Add(1);
            cbxClearence.Items.Add(2);
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (!(txtFName.Text == "" || txtLName.Text == "" || txtPass1.Password == "" ||
                txtPass2.Password == "" || txtPhone.Text == "" || txtUserName.Text == ""))
            {
                if (txtPass1.Password.Trim().Equals(txtPass2.Password.Trim()))
                {
                    string password = txtPass1.Password;
                   bool success = FBDataHelper.AddEmployee(txtFName.Text.Trim().ToUpper(), txtLName.Text.Trim().ToUpper(), txtPhone.Text.Trim(),
                        txtUserName.Text.Trim(), Hasher.CreateHash(password), Int32.Parse(cbxClearence.Text));
                   if (success)
                   {
                       MessageBox.Show("Employee added successfully.", "SUCCESS");
                   }
                   else
                   {
                       MessageBox.Show("Employee not added. An error occurred.", "ERROR");
                   }
                }
                else
                {
                    MessageBox.Show("The two passwords do not match.");
                }
            }
            else
            {
                MessageBox.Show("Some fields are empty.","ERROR");
            }
        }

        private void txtLName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(txtLName.Text!="")
            txtUserName.Text = txtFName.Text.ToLower()[0] + txtLName.Text.Trim().ToLower();
        }

        private void txtFName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtFName.Text != "")
            txtUserName.Text = txtFName.Text.ToLower()[0] + txtLName.Text.Trim().ToLower();
        }
    }
}
