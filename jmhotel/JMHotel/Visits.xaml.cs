using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Drawing;
using System.Collections.ObjectModel;
using System.Data;
using FirebirdSql.Data.FirebirdClient;
using GenericUtilities;

namespace JMHotel
{
    /// <summary>
    /// Interaction logic for InventoryWindow.xaml
    /// </summary>
    public partial class Visits : Window
    {
        SDataTable sdt = null;
        int amountPaid = 0;
        bool isCheckingOut = false, paymentApproved = false;

        public Visits()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataGridVisits.DataContext = (sdt = JMFBDataHelper.GetVisits()).DefaultView;
            fromdtPicker.SelectedDate = todtPicker.SelectedDate= DateTime.Now;

            this.WindowState = System.Windows.WindowState.Maximized;

            if(AppUtilities.Session.CLRLevel < 2)
            {
                dataGridVisits.CanUserDeleteRows = false;
                dataGridVisits.Columns[0].IsReadOnly = dataGridVisits.Columns[1].IsReadOnly = true;
            }

        }
        private void menuExport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int[] columnWidths = new int[dataGridVisits.Columns.Count];
                foreach (DataGridColumn col in dataGridVisits.Columns)
                {
                    columnWidths[col.DisplayIndex] = (int)col.ActualWidth;
                }
                PageContent[] pages = Paginator.Paginate(sdt,10, columnWidths, new int [] {1500, 790});
                FixedDocument doc = new FixedDocument();
                foreach (PageContent p in pages)
                {
                    doc.Pages.Add(p);
                }
                new DocViewer(doc).Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\n DETAILS: " + ex.Message, "ERROR");
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
                int affRows = sdt.UpdateSource();
                MessageBox.Show("The update affected " + affRows + " rows.", "UPDATE", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            dataGridVisits.ItemsSource = (sdt = JMFBDataHelper.GetVisits
                (fromdtPicker.SelectedDate.Value, todtPicker.SelectedDate.Value)).DefaultView;
        }

        private void dataGridVisits_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(DateTime) && e.Column.Header.ToString()=="DOB")
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "{0:dd/MM/yyyy}";
            }
        }
        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void menuCheckIn_Click(object sender, RoutedEventArgs e)
        {
            new AddVisit(null).Show();
        }

        private void menuCheckOut_Click(object sender, RoutedEventArgs e)
        {
            //get visitID
            DataRow row = (dataGridVisits.SelectedItem as DataRowView).Row;
            string visitId = row["VISITID"].ToString();
            bool isTableGuest = (row["GUESTTYPE"].ToString() == "WALK IN" || row["ROOMTABLE"].ToString()[0] == 'T') ? true : false;
            int totBill = 0, totPay = 0, owed=0;
            isCheckingOut = false;
            paymentApproved = false;

            if (row["STATUS"].ToString() == "CONCLUDED")
            {
                MessageBox.Show("This visit has already been concluded", "ERROR", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                //get amount owed
                DataTable statement = JMFBDataHelper.GetStatement(visitId, out totBill, out totPay, out owed);
                if (owed > 0)
                {
                    if (MessageBox.Show("Customer owes " + owed + ". Proceed to payment?", "CHECK OUT", MessageBoxButton.OKCancel,
                        MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        isCheckingOut = true;
                        menuPay_Click(null, null);
                    }
                }

                if (paymentApproved)
                {
                    if (MessageBox.Show("Confirm checkout?", "CHECK OUT", MessageBoxButton.OKCancel,
                        MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        JMFBDataHelper.CheckOutGuest(visitId);

                        if (MessageBox.Show("Print receipt?", "RECEIPT", MessageBoxButton.OKCancel,
                            MessageBoxImage.Question) == MessageBoxResult.OK)
                        {
                            FixedDocument doc = new FixedDocument();
                            if (isTableGuest)
                            {
                                doc = AppUtilities.GenerateTableReceipt(statement, amountPaid, amountPaid - owed, visitId);
                            }
                            else
                            {
                                doc = AppUtilities.GenerateRoomReceipt(statement, amountPaid, amountPaid - owed, visitId, row);
                            }

                            AppUtilities.PrintDocument(doc, "Receipt for visit " + visitId);
                        }
                    }
                }
            }
        }

        private void menuViewBill_Click(object sender, RoutedEventArgs e)
        {
            DataRow row = (dataGridVisits.SelectedItem as DataRowView).Row;
            string visitId = row["VISITID"].ToString();
            int temp1 = 0, temp2 = 0, temp3 = 0;
            DataTable dt = JMFBDataHelper.GetStatement(visitId, out temp1, out temp2, out temp3);

            int[] thickness = { 150, 160, 150 };

            if (row["GUESTTYPE"].ToString() == "WALK IN" || row["ROOMTABLE"].ToString()[0] == 'T')
            {
                dt.Columns.RemoveAt(0);
                thickness = new int [] {150, 80};
            }

            Grid grid = Utilities.LabelTable(dt, new Thickness(1), 12, thickness);

            PageContent pc = new PageContent();       
            FixedPage page = new FixedPage();
            page.Children.Add(grid);
            pc.Child = page;
            FixedDocument doc = new FixedDocument();
            doc.Pages.Add(pc);
            new DocViewer(doc).Show();

        }

        private void menuAddBill_Click(object sender, RoutedEventArgs e)
        {
            bool discount=false;
            if((sender as MenuItem).Name=="menuAddBillDisc")
            {
                discount = true;
            }
            //get visitID
            DataRow row = (dataGridVisits.SelectedItem as DataRowView).Row;
            if (row["STATUS"].ToString() == "CONCLUDED")
            {
                MessageBox.Show("Bill item cannot be added to concluded visit", "ERROR", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            new ItemsBrief(row["VISITID"].ToString(), discount).Show();
        }

        private void menuPay_Click(object sender, RoutedEventArgs e)
        {
            DataRow row = (dataGridVisits.SelectedItem as DataRowView).Row;
            
            if (row["STATUS"].ToString() == "CONCLUDED")
            {
                MessageBox.Show("Payment cannot be added to concluded visit", "ERROR", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                
                string visitId = row["VISITID"].ToString();
                int owed = JMFBDataHelper.GetOwed(visitId);
                PayWindow window = new PayWindow(owed);
                
                if (window.ShowDialog() == true)
                {
                    amountPaid = window.Amount;
                }
                if ((amountPaid < owed) && isCheckingOut)
                {
                    MessageBox.Show("The amount paid is less than the amount owed.\nPayment cannot proceed during check out.",
                        "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    if (MessageBox.Show("Confirm Payment of " + amountPaid + "?", "PAYMENT", MessageBoxButton.OKCancel,
                            MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        JMFBDataHelper.MakePayment(visitId, owed, amountPaid);
                        paymentApproved = true;
                    }
                }
                
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string dName = txtSearch.Text.Trim().ToUpper();
            try
            {
                EnumerableRowCollection<DataRow> qresult = from rtndRows in sdt.AsEnumerable()
                                                           where 
                                                           rtndRows[0].ToString().ToUpper().Contains(dName) 
                                                           ||rtndRows[1].ToString().ToUpper().Contains(dName)
                                                           || rtndRows[2].ToString().ToUpper().Contains(dName)
                                                           || rtndRows[3].ToString().ToUpper().Contains(dName)
                                                           select rtndRows;
                if (qresult.Count() > 0)
                {
                    dataGridVisits.ItemsSource = qresult.AsDataView<DataRow>();
                    lblCount.Content = qresult.Count() + " items";
                }
                else
                {
                    MessageBox.Show("The search has not returned any results", "ERROR",MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unknown error occurred.\n DETAILS:" + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ContextMenu_Loaded(object sender, RoutedEventArgs e)
        {
            DataRow row = (dataGridVisits.SelectedItem as DataRowView).Row;
            if (row["ROOMTABLE"].ToString()[0] == 'T')
            {
                menuViewBill.Header = "View Bill";
            }
            else
            {
                menuViewBill.Header = "View Statement";
            }

            if (row["STATUS"].ToString() == "CONCLUDED")
            {
                menuReprint.IsEnabled = true;
            }
            else
            {
                menuReprint.IsEnabled = false;
            }
            
        }

        private void menuReprint_Click(object sender, RoutedEventArgs e)
        {
            //get visitID
            try
            {
                DataRow row = (dataGridVisits.SelectedItem as DataRowView).Row;
                string visitId = row["VISITID"].ToString();
                bool isTableGuest = (row["GUESTTYPE"].ToString() == "WALK IN" || row["ROOMTABLE"].ToString()[0] == 'T') ? true : false;
                int totBill = 0, totPay = 0, owed = 0;

                //get amount owed
                DataTable statement = JMFBDataHelper.GetStatement(visitId, out totBill, out totPay, out owed);

                if (MessageBox.Show("Print receipt?", "RECEIPT", MessageBoxButton.OKCancel,
                            MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    FixedDocument doc = new FixedDocument();
                    if (isTableGuest)
                    {
                        doc = AppUtilities.GenerateTableReceipt(statement, amountPaid, amountPaid - owed, visitId);
                    }
                    else
                    {
                        doc = AppUtilities.GenerateRoomReceipt(statement, amountPaid, amountPaid - owed, visitId, row);
                    }
                    AppUtilities.PrintDocument(doc, "Receipt for visit " + visitId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured.\n DETAILS: " + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void menuRefresh_Click(object sender, RoutedEventArgs e)
        {
            dataGridVisits.DataContext = (sdt = JMFBDataHelper.GetVisits()).DefaultView;
        }
       
    }
}