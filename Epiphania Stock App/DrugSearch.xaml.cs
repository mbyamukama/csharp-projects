using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Data;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StockApp
{
    /// <summary>
    /// Interaction logic for DrugSearch.xaml
    /// </summary>
    public partial class DrugSearch : Window
    {
        SDataTable dtStock = null;
        public string dName = null;
        public DataRow selected = null;
        public bool drugExists = false;

        public DrugSearch()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dtStock = FBDataHelper.GetStock(FBDataHelper.StockType.Basic, DateTime.Now,DateTime.Now);
          txtSearch.Focus();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            dName = txtSearch.Text.Trim();
            IEnumerable<DataRow> qresult = from rtndRows in dtStock.AsEnumerable()
                                           where rtndRows.Field<string>("DRUGNAME").ToUpper().Contains(dName.ToUpper())
                                           select rtndRows;
            if (qresult.Count() > 0)
            {
                dataGridResults.ItemsSource = qresult.CopyToDataTable().DefaultView;
                drugExists = true;
            }
            else
            {
                drugExists = false;
                MessageBox.Show("The search has not returned any results.\n Verify that you have spelt the drug name correctly.\n"+
                    "If this is a new drug, please add it from the inventory window and click the Update button.", "ERROR");
            }
        }

        private void dataGridResults_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            selected = (dataGridResults.SelectedItem as DataRowView).Row;
            drugExists = true;
            this.Close();
        }
    }
}
