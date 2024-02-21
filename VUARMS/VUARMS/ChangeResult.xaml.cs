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
    public partial class ChangeResult : Window
    {

       public int NewResult = 0;

        public ChangeResult(string details, int oldResult)
        {
            InitializeComponent();
            txtOriginal.Text = oldResult.ToString();
            NewResult = oldResult;
            txtDetails.Text = details;

        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {

            if (txtNewresult.Text != "")
            {
                try
                {
                    NewResult = Convert.ToInt32(txtNewresult.Text);
                    this.DialogResult = true;
                }
                catch (FormatException ex)
                {
                    MessageBox.Show("Only numeric values are permitted. \nDETAILS: "+ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please insert a value", "VALUE NEEDED", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

    }
}
