using System;
using System.Linq; 
using System.Windows;
using System.Windows.Controls;
using System.Data; 

namespace StockApp
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
                    ( FBDataHelper.GetDBTables());
                dt = new SDataTable();
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred\n"+ ex.Message, "ERROR");
            }
        }


        private void cbxTableList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dt = FBDataHelper.GetTableData((cbxTableList.SelectedValue.ToString()));          
            dtGridResults.ItemsSource = dt.DefaultView;
        }

        private void DtGridResults_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(DateTime))
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd.MM.yyyy";

            }

        }
    }
}
