using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FRESUgandaERP
{
    /// <summary>
    /// Interaction logic for RawDataView.xaml
    /// </summary>
    public partial class RawDataView : Window
    {
        SDataTable dt = null;
        
        public RawDataView()
        {
            InitializeComponent();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string searchText = txtSearch.Text.Trim().ToUpper();
                EnumerableRowCollection<DataRow> results = from rows in dt.AsEnumerable()
                                                           where rows[0].ToString().ToUpper().Contains(searchText)
                                                           || rows[1].ToString().ToUpper().Contains(searchText)
                                                           || rows[2].ToString().ToUpper().Contains(searchText)
                                                           select rows;

                if (results.Count() > 0)
                {
                    dtGridResults.ItemsSource = results.AsDataView<DataRow>();              
                }
                else
                {
                    MessageBox.Show("No Matching Results", "ERROR");
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message, "ERROR");
            }
            
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (cbxTableList.SelectedValue.ToString().Trim() != "LOG")
            {
                int r = dt.UpdateSource();
                MessageBox.Show("Update successful.\n" + r + " rows affected.", "UPDATE");
            }
            else
                MessageBox.Show("Log cannot be edited", "ERROR");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {           
            try
            {
                cbxTableList.ItemsSource =
                new System.Collections.ObjectModel.ObservableCollection<string>
                    (FRESUGDBHelper.GetDBTables());
                dt = new SDataTable();
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred\n"+ ex.Message, "ERROR");
            }
        }

        void dt_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Column.ColumnName == "BBF" || e.Column.ColumnName == "AMTDUE")
            {
                e.Row["TOTAL"] = Convert.ToInt32(e.Row["BBF"]) + Convert.ToInt32(e.Row["AMTDUE"]);
               // dt.AcceptChanges();
            }
        }

        private void cbxTableList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dt = FRESUGDBHelper.GetTableData((cbxTableList.SelectedValue.ToString()));          
            dtGridResults.ItemsSource = dt.DefaultView;
            dt.ColumnChanged += dt_ColumnChanged;
        }

        private void dtGridResults_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(DateTime))
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd-MMM-yy";
            }
            if (e.PropertyType == typeof(Int32) || e.PropertyType == typeof(Int64))
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "N0";

            }
        }
    }
}
