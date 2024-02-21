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

namespace StockApp
{
    /// <summary>
    /// Interaction logic for AddSupplier.xaml
    /// </summary>
    public partial class AddSupplier : Window
    {
        public AddSupplier()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (txtSupName.Text.Trim() != string.Empty)
            {
                if (FBDataHelper.AddSupplier(txtSupName.Text.Trim().ToUpper()))
                    MessageBox.Show("Supplier successfully added.", "SUCCESS");

                this.DialogResult = true;
                this.Close();
            }
            else MessageBox.Show("The supplier name has not been entered.", "ERROR");
           
        }
    }
}
