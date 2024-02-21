using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Documents;
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
    public partial class ViewClient: Window
    {
        SDataTable dt = null;
        EnumerableRowCollection <DataRow> results = null;
        string currentlySelectedClientId = "";
        string currentlySelectedName = "";

        public ViewClient()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dt = FRESUGDBHelper.GetClients("*");
            dataGridResults.ItemsSource = dt.DefaultView;
            dataGridResults.Columns[0].IsReadOnly = true;
            lblCount.Content = dt.Rows.Count + " items";
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (FRESUGERP.AppUtilities.UtilityExtensions.currentSession.CurrentCLR > 2)
            {
                try
                {
                    if (MessageBox.Show("Are you sure you want to update this detail?",
                        "CONFIRM", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        int affRows = dt.UpdateSource();
                        if (affRows > 0)
                        {
                            FRESUGDBHelper.AddLog(this.Title, UtilityExtensions.currentSession.USER, "update on client was performed", UtilityExtensions.currentSession.STATION);
                            MessageBox.Show("Update Successful.\n" + affRows + " rows were update.");
                        }
                    }
                }
                catch (Exception ex)
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
            new AddClient().ShowDialog();
        }

        private void menuClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtSearchParam_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string searchText = txtSearchParam.Text.Trim().ToUpper();
                results = from rows in dt.AsEnumerable()
                          where rows["CLIENTID"].ToString().ToUpper().StartsWith(searchText)
                          || rows["FULLNAME"].ToString().ToUpper().StartsWith(searchText)
                          || rows["PHONENUM"].ToString().ToUpper().StartsWith(searchText)
                          || rows["ENSTORE"].ToString().ToUpper().StartsWith(searchText)
                          || rows["CONNDATE"].ToString().ToUpper().StartsWith(searchText)
                          select rows;
                int rowCount = results.Count();
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
                MessageBox.Show("An error has occurred.\n" + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            dataGridResults.Columns[0].IsReadOnly = true; //enforce read only
        }

        private void makePayment_Click(object sender, RoutedEventArgs e)
        {
            currentlySelectedClientId =
                ((DataRowView)(dataGridResults.CurrentItem)).Row["CLIENTID"].ToString();
            new AddServicePayment(currentlySelectedClientId).ShowDialog();
        }
        private void viewStatement_Click(object sender, RoutedEventArgs e)
        {
           DataTable statement = FRESUGDBHelper.GetStatement(currentlySelectedClientId);

           Grid grid = CEDAT.MathLab.Utilities.LabelTable(statement, new Thickness(1), 12, new int[] { 150, 150, 150, 150, 50, 50 });
            FixedPage page = new FixedPage();
            page.Children.Add(grid);

            PageContent pc = new PageContent()
            {
                Child = page
            };

            FixedDocument fdoc = new FixedDocument();
            fdoc.Pages.Add(pc);

            new StatementViewer(fdoc).Show();
            
           
        }

        private void dataGridResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                currentlySelectedClientId =
                    ((DataRowView)(dataGridResults.CurrentItem)).Row["CLIENTID"].ToString();
                currentlySelectedName =
                    ((DataRowView)(dataGridResults.CurrentItem)).Row["FULLNAME"].ToString();
            }
            catch (InvalidCastException ex)
            {
                // do nothing
            }
            
        }

        private void dataGridResults_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(DateTime))
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd-MMM-yy";
            }
        }

        private void DeleteClient()
        {
            if (FRESUGERP.AppUtilities.UtilityExtensions.currentSession.CurrentCLR > 2)
            {
                currentlySelectedClientId =
                      ((DataRowView)(dataGridResults.CurrentItem)).Row["CLIENTID"].ToString();

                if (MessageBox.Show("Are you sure you want to delete this client?",
                            "CONFIRM", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (FRESUGDBHelper.DeleteClient(currentlySelectedClientId))
                    {
                        FRESUGDBHelper.AddLog(this.Title, UtilityExtensions.currentSession.USER,
                            "client," + currentlySelectedClientId + ", was deleted at " + DateTime.Now, UtilityExtensions.currentSession.STATION);
                        MessageBox.Show("THE CLIENT ACCOUNT HAS BEEN DELETED");
                    }
                    else
                    {
                        MessageBox.Show("THE CLIENT ACCOUNT HAS NOT BEEN DELETED");
                    }
                }
            }
            else
            {
                MessageBox.Show("You do not have sufficient priviledges to perform this action", "ERROR");
            }
        }
        private void dataGridResults_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DeleteClient();
            }
        }

        private void deleteClient_Click(object sender, RoutedEventArgs e)
        {
            DeleteClient();
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            foreach (DataRow row in dt.Rows)
            {
                row["VILLAGE"] = row["VILLAGE"].ToString().Replace(',', '-');
            }
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
    }
}
