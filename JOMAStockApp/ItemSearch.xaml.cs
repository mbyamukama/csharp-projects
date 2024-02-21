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
    /// Interaction logic for ItemSearch.xaml
    /// </summary>
    public partial class ItemSearch : Window
    {
        SDataTable dtStock = null;
        public string dName = null;
        public DataRow SelectedRow = null;

        public ItemSearch()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dtStock = FBDataHelper.GetStock(FBDataHelper.StockType.Basic, DateTime.Now,DateTime.Now);
            dataGridResults.ItemsSource = dtStock.DefaultView;
            txtSearch.Focus();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            dName = txtSearch.Text.Trim();
            IEnumerable<DataRow> qresult = from rtndRows in dtStock.AsEnumerable()
                                           where rtndRows.Field<string>("ITEMNAME").ToUpper().Contains(dName.ToUpper())
                                           select rtndRows;
            if (qresult.Count() > 0)
            {
                dataGridResults.ItemsSource = qresult.CopyToDataTable().DefaultView;
                if (GlobalSystemData.Session.CurrentUser.IsStandardUser())
                {
                    (dataGridResults.Columns[2] as DataGridColumn).Visibility = System.Windows.Visibility.Hidden;
                    (dataGridResults.Columns[3] as DataGridColumn).Visibility = System.Windows.Visibility.Hidden;
                }
            }
            else
            {
                MessageBox.Show("The search has not returned any results.\n Verify that you have spelt the item name correctly.\n"+
                    "If this is a new item, please add it from the inventory window and click the Update button.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void dataGridResults_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectedRow = (dataGridResults.SelectedItem as DataRowView).Row;
            this.Close();
        }
    }
}
