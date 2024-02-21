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
    /// Interaction logic for ChequeDetails.xaml
    /// </summary>
    public partial class ChequeDetails : Window
    {
        public String Bank, ChequeNo = "";
        public ChequeDetails()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (txtBank.Text.Trim() == "" || txtChequeNo.Text.Trim() == "")
            {
                MessageBox.Show("All Fields must be filled before submission.", "ERROR");
            }
            else
            {
                Bank = txtBank.Text.Trim();
                ChequeNo = txtChequeNo.Text.Trim();
                this.DialogResult = true;
            }
        }
    }
}
