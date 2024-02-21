using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using StockApp.AppExtensions;

namespace StockApp
{
    /// <summary>
    /// Interaction logic for InventoryWindow.xaml
    /// </summary>
    public partial class SaleHistory : Window
    {
        string itemName = "";
        public SaleHistory(string itemName)
        {
            InitializeComponent();
            this.itemName = itemName;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataTable dt = FBDataHelper.GetSaleHistory(itemName, DateTime.Now.AddDays(-30), DateTime.Now);
            dataGridResults.ItemsSource = dt.DefaultView;
            lblSearch.Content = itemName;
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = FBDataHelper.GetSaleHistory(itemName, fromdtPicker.SelectedDate.Value, todtPicker.SelectedDate.Value);
            if (dt.Rows.Count > 0)
            {
                try
                {
                    int count = dt.Rows.Count;
                    int salecount = dt.Columns["QUANTITY"].AsEnumerable<int>().Sum();
                    int revenue = dt.Columns["AMOUNTDUE"].AsEnumerable<int>().Sum();
                    double perday = 0, perweek = 0;
                    int days = Convert.ToDateTime(dt.Rows[count - 1]["TDATE"]).Subtract(Convert.ToDateTime(dt.Rows[0]["TDATE"])).Days;

                    if (days > 0)
                    {
                        perday = Math.Round(salecount * 1.0 / days, 1);
                        perweek = perday * 7;
                    }

                    lblCount.Content = "Count: " + count + "\tSale Count: " + salecount + "\tSale Revenue: " + revenue.ToString("N0") +
                       "\tSale Duration: " + days + "\tPer Day: " + perday + "\tPer Week: " + perweek;
                    dataGridResults.ItemsSource = dt.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred.\nDETAILS: "
                    + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
                MessageBox.Show("There is no data to return", "ERROR", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        private void dataGridResults_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(DateTime))
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "{0:dd/MM/yyyy}";
            }
            if (e.PropertyType == typeof(Int32) && e.Column.DisplayIndex != 1)
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "N0";
            }
        }
    }
}
