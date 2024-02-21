using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;      
using System.Windows.Controls;
using System.Windows.Documents;      
using System.Data;

namespace StockApp
{
    /// <summary>
    /// Interaction logic for InventoryWindow.xaml
    /// </summary>
    public partial class ViewAlerts : Window
    {
        SDataTable dt = null;
        private int lowstocklevel = 0, expirywarmlevel = 0;
        string[] alignments = new string[] { "L", "L", "L", "L", "L", "L", "L", "L", "L", "L", "L" };

        public ViewAlerts()
        {
            InitializeComponent(); 
        }

        private void dataGridResults_LoadingRow(object sender, DataGridRowEventArgs e)
        {
           

        }

        private void menuPrintSelection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int[] columnWidths = new int[dataGridResults.Columns.Count];

                foreach (DataGridColumn col in dataGridResults.Columns)
                {
                    columnWidths[col.DisplayIndex] = (int)col.ActualWidth;
                }

                IEnumerable<DataRowView> rowViews = dataGridResults.SelectedItems.Cast<DataRowView>();
                DataTable table = rowViews.ElementAt(0).Row.Table.Clone();
                foreach (DataRowView view in rowViews)
                {
                    table.Rows.Add(view.Row.ItemArray);

                }
                PageContent[] pages =
                     Paginator.Paginate(table, 11, columnWidths, alignments);
                FixedDocument doc = new FixedDocument();
                foreach (PageContent p in pages)
                {
                    doc.Pages.Add(p);
                }
                new DocViewer(doc).Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\n DETAILS: " + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void menuPrintAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int[] columnWidths = new int[dataGridResults.Columns.Count];

                foreach (DataGridColumn col in dataGridResults.Columns)
                {
                    columnWidths[col.DisplayIndex] = (int)col.ActualWidth;
                }

                PageContent[] pages =
                    Paginator.Paginate(((System.Data.DataView)(dataGridResults.ItemsSource)).ToTable(), 11, columnWidths,
                    alignments);

                FixedDocument doc = new FixedDocument();
                foreach (PageContent p in pages)
                {
                    doc.Pages.Add(p);
                }
                new DocViewer(doc).Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\n DETAILS: " + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable settings = FBDataHelper.GetSettings();
                foreach (DataRow row in settings.Rows)
                {
                    if(row[1]==DBNull.Value)
                    {
                        row[1] = 1;
                    }
                }
                lowstocklevel = Convert.ToInt32(settings.Rows[3][1]);
                expirywarmlevel = Convert.ToInt32(settings.Rows[4][1]);
                dt = FBDataHelper.GetAlerts(expirywarmlevel);

                IEnumerable <DataRow> rows = from myRows in dt.AsEnumerable()
                                                        where myRows.Field<int>("QUANTITY") > lowstocklevel
                                                        select myRows;
                dataGridResults.ItemsSource = rows.CopyToDataTable().DefaultView;
                lblCount.Content = "Count = " + dataGridResults.Items.Count + "    Low Stock Level = " + lowstocklevel
                    + " Expiry Warn Level = " + expirywarmlevel + " days in advance";
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\n DETAILS: " + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }   
    }
}
