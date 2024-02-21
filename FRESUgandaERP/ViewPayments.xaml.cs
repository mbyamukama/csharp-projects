using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Data;
using FRESUGERP.AppUtilities;
using FirebirdSql.Data.FirebirdClient;
using CEDAT.MathLab;

namespace FRESUgandaERP
{
    /// <summary>
    /// Interaction logic for InventoryWindow.xaml
    /// </summary>
    public partial class ViewPayments: Window
    {
        SDataTable dt = null;
        EnumerableRowCollection <DataRow> results = null;
        string currentlySelectedClientId = "", currentlySelectedReceiptNo = "";
        DateTime? payDate = null;

        public ViewPayments()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dt = FRESUGDBHelper.GetPayments(DateTime.Now.AddDays(-30), DateTime.Now, "ALL");
            dataGridResults.ItemsSource = dt.DefaultView;
            dataGridResults.Columns[0].IsReadOnly = true;
            dataGridResults.Columns[1].IsReadOnly = true;
            lblCount.Content = dt.Rows.Count + " items";
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (FRESUGERP.AppUtilities.UtilityExtensions.currentSession.CurrentCLR > 2)
            {
                try
                {
                    if (MessageBox.Show("Are you sure you want to update this detail?",
                        "CONFIRM",MessageBoxButton.YesNo)== MessageBoxResult.Yes)
                    {
                        int affRows = dt.UpdateSource();
                        if (affRows > 0)
                        {
                            FRESUGDBHelper.AddLog(this.Title, UtilityExtensions.currentSession.USER, "update on client," + currentlySelectedClientId +
                                ", payments was performed", UtilityExtensions.currentSession.STATION);
                            MessageBox.Show("Update Successful.\n" + affRows + " rows were update.");
                        }
                    }
                }
                catch (FbException ex)
                {
                    MessageBox.Show("An error occurred.\n" + ex.Message, "ERROR");
                }
            }
            else
            {
                MessageBox.Show("You do not have sufficient permissions to perform this action.", "ERROR");
            }
        }

        private void menuAddNew_Click(object sender, RoutedEventArgs e)
        {
            new AddServicePayment(currentlySelectedClientId).ShowDialog();
        }

        private void menuClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtSearchParam_TextChanged(object sender, TextChangedEventArgs e)
        {
            int rowCount =0;
            string searchText = txtSearchParam.Text.Trim().ToUpper();
            try
            {
                results = from rows in dt.AsEnumerable()
                          where rows.Field<string>("CLIENTID").ToUpper().StartsWith(searchText)||
                                rows.Field<string>("RECEIPTNO").ToUpper().StartsWith(searchText)
                          select rows;
                rowCount = results.Count();
                lblCount.Content = rowCount + " items found";
                if (rowCount > 0)
                {
                    dataGridResults.ItemsSource = results.AsDataView<DataRow>();
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
            dataGridResults.Columns[0].IsReadOnly = true; //enforce read only
            dataGridResults.Columns[1].IsReadOnly = true;
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = "CSV File|*.csv";
            if (sfd.ShowDialog(this) == true)
            {
                if (results == null)
                {
                    CEDAT.MathLab.Utilities.WriteCSVFile(dt, sfd.FileName + ".csv");
                }
                else
                {
                    CEDAT.MathLab.Utilities.WriteCSVFile(results.CopyToDataTable(), sfd.FileName + ".csv");
                }
            }
        }

        private void dataGridResults_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
             if (e.PropertyType == typeof(DateTime))
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd-MMM-yy";
            }
        }

        private void dataGridResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                currentlySelectedClientId =
                    ((DataRowView)(dataGridResults.CurrentItem)).Row["CLIENTID"].ToString();
                currentlySelectedReceiptNo =
                    ((DataRowView)(dataGridResults.CurrentItem)).Row["RECEIPTNO"].ToString();
                payDate = Convert.ToDateTime(((DataRowView)(dataGridResults.CurrentItem)).Row["PAYDATE"]);
            }
            catch
            {

            }
        }


        private void menuDeletePayment_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this payment?", "CONFIRM", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                bool result = FRESUGDBHelper.DeletePayment(currentlySelectedReceiptNo,
                    currentlySelectedClientId, payDate.Value);
               if (result)
               {
                   MessageBox.Show("The payment has been deleted", "SUCCESS");
               }
                else
                   MessageBox.Show("An error has occured while deleting the payment", "ERROR");

            }
            else
            MessageBox.Show("The delete operation has been aborted", "ERROR");
        }
    }
}
