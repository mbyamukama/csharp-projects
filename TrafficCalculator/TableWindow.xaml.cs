using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;

namespace TrafficCalculator
{
    /// <summary>
    /// Interaction logic for TableWindow.xaml
    /// </summary>
    public partial class TableWindow : Window
    {
        DataTable dt = null;
        public TableWindow(DataTable dataTable)
        {
            InitializeComponent();
            dt = dataTable;
            dtGridData.ItemsSource = dataTable.DefaultView;
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StreamWriter sw = new StreamWriter("results.csv", false);
                String toWrite = "";
                foreach (DataColumn dc in dt.Columns)
                {
                    toWrite += dc.ColumnName + ",";
                }
                toWrite.Remove(toWrite.Length - 1);
                sw.WriteLine(toWrite);

                foreach (DataRow row in dt.Rows)
                {
                    foreach (DataColumn dc in dt.Columns)
                    {
                        sw.Write(row[dc] + ",");
                    }
                    sw.WriteLine();
                }
                sw.Flush();
            }
            catch(Exception ex)
            {
                MessageBox.Show("An error occured.\n + DETAILS: " + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
