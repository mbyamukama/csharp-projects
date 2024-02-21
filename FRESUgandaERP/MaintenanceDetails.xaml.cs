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
    /// Interaction logic for MaintenanceDetails.xaml
    /// </summary>
    public partial class MaintenanceDetails : Window
    {
        public DateTime MtnDate;
        public string MtnLog = "";
        public MaintenanceDetails()
        {
            InitializeComponent();
        }

        private void txtMtnDetails_TextChanged(object sender, TextChangedEventArgs e)
        {
           // if(txtMtnDetails.Text.Contains(''))
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (txtMtnDetails.Text.Trim() == "" || mtnDatePicker.SelectedDate.Value==null)
            {
                MessageBox.Show("All Fields must be filled before submission.", "ERROR");
            }
            else
            {
                MtnLog = txtMtnDetails.Text.Trim();
                MtnDate = mtnDatePicker.SelectedDate.Value;
                this.DialogResult = true;
            }
        }
    }
}
