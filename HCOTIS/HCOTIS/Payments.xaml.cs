using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections.ObjectModel;
using System.Data;

namespace HCOTIS
{
   
    public partial class Payments: Window
    {
        private string orderId = "";
        SDataTable dt = null;
        public Payments(string orderId)
        {
            InitializeComponent();
           this.orderId = orderId;
        }

        private void menuDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRow selectedRow = (dataGridPayments.SelectedItem as DataRowView).Row;
                if (MessageBox.Show("Are you sure you want to delete this item?\n" +
                    selectedRow[0], "DELETE?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    selectedRow.Delete();
                    dt.UpdateSource();
                    MessageBox.Show("Item deleted.", "SUCCESS");

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred and the delete operation failed.\n" +
                    "DETAILS:" + ex.Message, "DELETE ERROR");
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int affrows = dt.UpdateSource();
                MessageBox.Show("Update Successful.\n" + affrows + " items were changed.", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred during the update.\nDETAILS: " + ex.Message, "ERROR");
            }
        }

        private void menuAddNew_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            dt = FBDataHelper.GetPayments(fromdtPicker.SelectedDate.Value, todtPicker.SelectedDate.Value);
            dataGridPayments.ItemsSource = dt.DefaultView;
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string dName = txtSearch.Text.Trim();
            try
            {
                EnumerableRowCollection<DataRow> qresult = from rtndRows in dt.AsEnumerable()
                                                           where
                                                           rtndRows[0].ToString().ToUpper().Contains(dName.ToUpper()) ||
                                                           rtndRows[1].ToString().ToUpper().Contains(dName.ToUpper()) ||
                                                           rtndRows[2].ToString().ToUpper().Contains(dName.ToUpper()) 
                                                           select rtndRows;
                if (qresult.Count() > 0)
                {
                    dataGridPayments.ItemsSource = qresult.AsDataView<DataRow>();
                }
                else
                {
                    MessageBox.Show("The search has not returned any results", "ERROR");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unknown error occurred. Searching in this information may not be enabled yet.\n DETAILS:" + ex.Message, "ERROR");
            }
        }

        private void exitMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dt = FBDataHelper.GetPayments(orderId);
            dataGridPayments.ItemsSource = dt.DefaultView;

            fromdtPicker.SelectedDate = DateTime.Now.AddDays(-7);
            todtPicker.SelectedDate = DateTime.Now;

        }

        private void dataGridPayments_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
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
