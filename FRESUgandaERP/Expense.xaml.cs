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
    /// Interaction logic for OtherIncome.xaml
    /// </summary>
    public partial class Expenses: Window
    {
        public Expenses()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string item = cbxItem.Text.Trim();
                int amount = Int32.Parse(txtAmount.Text);
                DateTime date = dtPicker.SelectedDate.Value;
                if (item != "" && date != null)
                {
                    if (FRESUGDBHelper.AddExpense(item, amount, date))
                    {
                        MessageBox.Show("Item Added.", "SUCCESS");
                        txtAmount.Text = txtAmount.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Item Not Added.", "ERROR");
                    }
                }
                else
                {
                    MessageBox.Show("Some fields are empty.", "ERROR");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\n Ensure that all fields are filled in."+ ex.Message, "ERROR");
            }

            
            
        }
    }
}
