using System;
using System.Windows;
using System.Windows.Controls;
using System.Data;

namespace StockApp
{
    /// <summary>
    /// Interaction logic for InventoryWindow.xaml
    /// </summary>
    public partial class QueryWindow: Window
    {
        public QueryWindow()
        {
            InitializeComponent();
        }


        private void dataGridResults_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(DateTime))
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd-MMM-yy";
            }
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = FBDataHelper.RunQuery(txtSearch.Text.Trim());
            dataGridResults.ItemsSource = dt.DefaultView;
        }
    }
}
