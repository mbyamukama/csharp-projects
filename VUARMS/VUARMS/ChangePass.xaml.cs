using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

namespace VUARMS
{
    /// <summary>
    /// Interaction logic for AddEmployee.xaml
    /// </summary>
    public partial class ChangePass : Window
    {

       public String NewPassword = "";

        public ChangePass()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (txtPass1.Password.Equals(txtPass2.Password))
            {
                NewPassword = txtPass2.Password;
                this.DialogResult = true;
            }
            else
                MessageBox.Show("The passwords do not match.", "ERROR");
            
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

    }
}
