using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using FirebirdSql.Data.FirebirdClient;
using System.Data;

namespace StockApp
{
    /// <summary>
    /// Interaction logic for FinStatus.xaml
    /// </summary>
    public partial class FinStatus : Window
    {
        DataTable stock = null;
        DataTable findt = null;
        DataTable sales = null;
        public FinStatus()
        {
            InitializeComponent();
          //  sales = FBDataHelper.GetSales();

            findt = new DataTable();
            findt.Columns.Add("Item");
            findt.Columns.Add("VALUE");

            dataGrid1.ItemsSource = findt.DefaultView;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            stock = FBDataHelper.GetStock(FBDataHelper.StockType.Basic,DateTime.Now,DateTime.Now);
           int sum = 0;
           foreach (DataRow row in stock.Rows)
           {
               sum += (int)(Convert.ToInt32(row["QUANTITY"]) * Convert.ToInt32(row["COSTPPU"]) * Convert.ToDouble(row["FACTOR"]));
           }
           findt.Rows.Add("Stock Value", sum);
           findt.Rows.Add("Sales", 0);
           findt.Rows.Add("Credit", 0);
           findt.Rows.Add("Expenses", 0);
           findt.Rows.Add("Profit", 0);
        }

        private void fromdtPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var relStock = from myRows in sales.AsEnumerable()
                        where (DateTime.Parse(myRows["DATEOFSALE"].ToString()).Date >= fromdtPicker.SelectedDate
                        & DateTime.Parse(myRows["DATEOFSALE"].ToString()).Date <= todtPicker.SelectedDate)
                            select myRows;
            int sum = 0;
            foreach (DataRow row in relStock)
            {
                sum += Convert.ToInt32(row["AMOUNTDUE"]);
            }
            findt.Rows[1]["VALUE"] = sum;
        }

        private void todtPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var relStock = from myRows in sales.AsEnumerable()
                           where (DateTime.Parse(myRows["DATEOFSALE"].ToString()).Date >= fromdtPicker.SelectedDate
                           & DateTime.Parse(myRows["DATEOFSALE"].ToString()).Date <= todtPicker.SelectedDate)
                           select myRows;
            int sum = 0;
            foreach (DataRow row in relStock)
            {
                sum += Convert.ToInt32(row["AMOUNTDUE"]);
            }
            findt.Rows[1]["VALUE"] = sum;
        }
    }
}
