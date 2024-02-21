using System.Collections.Generic;
using System.Windows;
using System.Data;

namespace StockApp
{
    /// <summary>
    /// Interaction logic for InventoryWindow.xaml
    /// </summary>
    public partial class Search: Window
    {
        List<DataRow> results = null;
        List<string> tableNames = null;
        int index = -1;
        public Search()
        {
            InitializeComponent();

        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
           index = -1;
           results = FBDataHelper.GetSearchResult(txtSearch.Text.Trim());
           tableNames = new List<string>();
           foreach (DataRow item in results)
           {
               if (!tableNames.Contains(item.Table.TableName)) tableNames.Add(item.Table.TableName);
           }

            MessageBox.Show(results.Count + " results returned");
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            ++index;
            List<DataRow> selected = new List<DataRow>();
            if (index < tableNames.Count)
            {
                string currTableName = tableNames[index];              

                for (int i = index; i < results.Count; i++)
                {
                    if (results[i].Table.TableName == currTableName)
                    {
                        selected.Add(results[i]);
                    }
                }
            }
            if (selected.Count > 0)
            {
                dataGridResults.ItemsSource = selected.CopyToDataTable().DefaultView;
                lblCount.Content = selected.Count;
            }
        }
    }
}
