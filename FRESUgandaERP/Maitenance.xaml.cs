using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Data;
using System.Windows.Documents;
using CEDAT.MathLab;
using FirebirdSql.Data.FirebirdClient;
using System.IO;
using FRESUGERP.AppUtilities;

namespace FRESUgandaERP
{
    /// <summary>
    /// Interaction logic for Bills.xaml
    /// </summary>
    public partial class Maintenance : Window
    {
        DataTable dueClients = null;
        string currentlySelectedClientId = "", log = "";
        DateTime dueDate, mtnDate;
        string centre = "ALL", sLevel = "ALL";
        public Maintenance()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
        }
        

        private void btnPrintAll_Click(object sender, RoutedEventArgs e)
        {          
           IEnumerable<DataRowView> items = dtgridDueClients.SelectedItems.Cast<DataRowView>();
           List<DataRow> billList = new List<DataRow>();
           foreach (DataRowView item in items)
           {
               billList.Add(item.Row);
           }
           DataTable dt = billList.CopyToDataTable();
           Size a4PaperSize = new Size(797, 1123);
           FixedDocument doc = FRESUGERP.AppUtilities.Paginator.Paginate(a4PaperSize,dt,12,null);
           try
           {
               PrintDialog pd = new PrintDialog();
               if (pd.ShowDialog() == true)
               {
                   pd.PrintDocument(doc.DocumentPaginator, "Bills");
               }
               
           }
          
           catch (Exception ex)
           {
               MessageBox.Show("An unknown error occurred.\n TECHNICAL DETAILS: " + ex.Message);
           }

        }

        private void dtDueClients_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
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



        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            centre = UtilityExtensions.MapEnergyCentre(centre);
            if (sLevel == "4+") sLevel = "5";

            dueClients = FRESUGDBHelper.GetUnMaintainedClients(dtPickerDueDate.SelectedDate.Value, centre, sLevel);

            DataTable mtnAgeTable = dueClients.Copy();
            mtnAgeTable.Columns.Add("180 days");
            mtnAgeTable.Columns.Add("180-360 days");
            mtnAgeTable.Columns.Add("over 360 days");

            foreach (DataRow row in mtnAgeTable.Rows)
            {
                int[] ages = FRESUGERP.AppUtilities.UtilityExtensions.DecomposeMtn(Convert.ToInt32(row["AGE"]));

                row["180 days"] = ages[0];
                row["180-360 days"] = ages[1];
                row["over 360 days"] = ages[2];
            }
            dtgridDueClients.ItemsSource = mtnAgeTable.DefaultView;
            dtgridDueClients.FontSize = 10;

            lblCount.Content = dueClients.Rows.Count + " items";
        }

        private void menuAddRecord_Click(object sender, RoutedEventArgs e)
        {
            currentlySelectedClientId =
                  ((DataRowView)(dtgridDueClients.CurrentItem)).Row["CLIENTID"].ToString();

            dueDate =
                  Convert.ToDateTime(((DataRowView)(dtgridDueClients.CurrentItem)).Row["DUEDATE"]);

            MaintenanceDetails window = new MaintenanceDetails();

            if (window.ShowDialog() == true)
            {
                mtnDate = window.MtnDate;
                log = window.MtnLog;
                if (FRESUGDBHelper.UpdateMaintenance(currentlySelectedClientId, log, dueDate, mtnDate))
                {
                    MessageBox.Show("Record Updated.", "SUCCESS");
                }
            }
        }

        private void menuExport_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = "CSV File|*.csv";
            if (sfd.ShowDialog(this) == true)
            {
                CEDAT.MathLab.Utilities.WriteCSVFile(dueClients, sfd.FileName);
            }
        }

        private void cbxEnergyCentre_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            centre = cbxEnergyCentre.GetSelectedItemContent();
        }

        private void cbxServiceLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sLevel = cbxServiceLevel.GetSelectedItemContent();
        }
    }
}
