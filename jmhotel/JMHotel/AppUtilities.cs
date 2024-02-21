using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Media;
using GenericUtilities;
using FirebirdSql.Data.FirebirdClient;

namespace JMHotel
{
    static class AppUtilities
    {
        public static Session Session
        {
            get;
            set;
        }

        public static void PrintDocument(FixedDocument doc, string docName)
        {
            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog() == true)
            {
                pd.PrintDocument(doc.DocumentPaginator, docName);
                MessageBox.Show("The operation completed successfully.", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("The operation was aborted.", "ABORT", MessageBoxButton.OK, MessageBoxImage.Warning);
            }  
        }
        public static void ShowPrintDataGrid(DataGrid datagrid)
        {
            try
            {
                int[] columnWidths = new int[datagrid.Columns.Count];

                foreach (DataGridColumn col in datagrid.Columns)
                {
                    columnWidths[col.DisplayIndex] = (int)col.ActualWidth;
                }

                PageContent[] pages =
                    Paginator.Paginate(((System.Data.DataView)(datagrid.ItemsSource)).ToTable(), 11, columnWidths, null);

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

        public static FixedDocument AppendFirstPage(this PageContent[] pages, FixedPage page1)
        {

            FixedDocument newDoc = new FixedDocument();
            newDoc.Name = "Report";

            if (page1 != null)
            {
                PageContent p = new PageContent();
                p.Child = page1;
                newDoc.Pages.Add(p);
            }

            foreach (PageContent pc in pages)
            {
                newDoc.Pages.Add(pc);
            }
            return newDoc; ;
        }

        public static FixedDocument GenerateTableReceipt(DataTable statement, int tendered, int balance, string recNo)
        {
            FixedDocument doc = new FixedDocument();
            try
            {
                DataTable stmntCopy = statement.Copy();

                stmntCopy.Columns.RemoveAt(0);
                stmntCopy.Rows.Add(null, null);
                stmntCopy.Rows.Add("TENDERED", tendered);
                stmntCopy.Rows.Add("CHANGE", balance);

                int columns = stmntCopy.Columns.Count;
                int rows = stmntCopy.Rows.Count;
                const int LEFT_MARGIN = 5, TOP_MARGIN = 5;

                double left = LEFT_MARGIN, top = TOP_MARGIN, height = 25;

                Grid grid = new Grid()
                {
                    Name = "root",
                    Width = 300,
                    Margin = new Thickness(0, 0, 0, 0)
                };

                //logo
                Label title = new Label();
                title.Width = 280;
                title.Height = 70;
                title.FontFamily = new FontFamily("Arial");
                title.FontSize = 14;
                title.Content = "J&M Hotel\n" + "Entebbe Airport Road, Bwebajja.\n" +
                    "TIN: 1000086956\nTel: +256 701 809 283\n\n";
                title.Margin = new Thickness(LEFT_MARGIN, top, 0, 0);
                title.BorderThickness = new Thickness(1);
                title.BorderBrush = Brushes.Black;
                title.HorizontalAlignment = HorizontalAlignment.Left;
                title.VerticalAlignment = VerticalAlignment.Top;
                title.HorizontalContentAlignment = HorizontalAlignment.Left;
                grid.Children.Add(title);
                top += title.Height;

                //receipt
                Label receipt = new Label();
                receipt.Width = 280;
                receipt.Height = 25;
                receipt.FontFamily = new FontFamily("Arial");
                title.FontSize = 11;
                receipt.Content = "Receipt No: " + recNo + "   Date: " + DateTime.Now.ToString();
                receipt.Margin = new Thickness(LEFT_MARGIN, top, 0, 0);
                receipt.BorderThickness = new Thickness(1);
                receipt.BorderBrush = Brushes.Gray;
                receipt.HorizontalAlignment = HorizontalAlignment.Left;
                receipt.VerticalAlignment = VerticalAlignment.Top;
                receipt.HorizontalContentAlignment = HorizontalAlignment.Left;
                grid.Children.Add(receipt);
                top += receipt.Height;

                //header
                Label header = new Label();
                header.Width = 280;
                header.Height = 25;
                header.FontFamily = new FontFamily("Arial");
                header.FontSize = 11;

                header.Content = "ITEM\t\t\t\tAMOUNT";

                header.Margin = new Thickness(LEFT_MARGIN, top, 0, 0);
                header.BorderThickness = new Thickness(1);
                header.BorderBrush = Brushes.Black;
                header.HorizontalAlignment = HorizontalAlignment.Left;
                header.VerticalAlignment = VerticalAlignment.Top;
                header.HorizontalContentAlignment = HorizontalAlignment.Center;
                grid.Children.Add(header);
                top += header.Height;



                foreach (DataRow row in stmntCopy.Rows)
                {
                    foreach (DataColumn col in stmntCopy.Columns)
                    {
                        Label label = new Label();
                        label.Margin = new Thickness(left, top, 0, 0);
                        label.BorderBrush = Brushes.Gray;
                        label.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                        label.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                        label.BorderThickness = new Thickness(0.5);

                        label.Content = col.ColumnName == "ITEM" && row[col] != DBNull.Value ? row[col].ToString().PadRight(100).Substring(0, 15) :
                                      col.ColumnName == "AMOUNT" && row[col] != DBNull.Value ? Convert.ToInt32(row[col]).ToString("N0") :
                                      row[col].ToString();

                        label.Width = col.Ordinal == 0 ? 200 : 80;
                        label.Height = height;
                        label.FontFamily = new FontFamily("Arial");
                        label.FontSize = 11;
                        label.VerticalContentAlignment = VerticalAlignment.Top;
                        label.HorizontalContentAlignment = HorizontalAlignment.Left;

                        grid.Children.Add(label);
                        left += label.Width;
                    }
                    top += height;
                    left = LEFT_MARGIN;
                }

                //footer
                Label footer = new Label();
                footer.Width = 280;
                footer.Height = 50;
                footer.FontFamily = new FontFamily("Arial");
                footer.FontSize = 11;
                footer.Content = "You were served by: " + Session.UserName +
                    "\nPrices include VAT where applicable. Thank You!\n" +
                    "POS Software by +256-712-256168";
                footer.Margin = new Thickness(LEFT_MARGIN, top, 0, 0);
                footer.BorderThickness = new Thickness(1);
                footer.BorderBrush = Brushes.Gray;
                footer.HorizontalAlignment = HorizontalAlignment.Left;
                footer.VerticalAlignment = VerticalAlignment.Top;
                footer.HorizontalContentAlignment = HorizontalAlignment.Left;
                grid.Children.Add(footer);
                //  top += footer.Height + 2;

                FixedPage page = new FixedPage();
                page.Children.Add(grid);
                PageContent pc = new PageContent();
                pc.Child = page;
                pc.Margin = new Thickness(0, 0, 0, 0);
                doc.Pages.Add(pc);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured.\n DETAILS: " + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return doc;
        }
        public static FixedDocument GenerateRoomReceipt(DataTable statement, int tendered, int balance, string recNo, DataRow details)
        {
            FixedDocument doc = new FixedDocument();
                DataTable stmntCopy = statement.Copy();
                //add change rows
                stmntCopy.Rows.Add(null, null, null);
                stmntCopy.Rows.Add(null, "TENDERED", tendered);
                stmntCopy.Rows.Add(null, "CHANGE", balance);            

                int columns = stmntCopy.Columns.Count;
                int rows = stmntCopy.Rows.Count;
                const int LEFT_MARGIN = 20, TOP_MARGIN = 5;

                double left = LEFT_MARGIN, top = TOP_MARGIN, height = 30;

                Grid grid = new Grid()
                {
                    Name = "root",
                    Width = 780,
                    Margin = new Thickness(0, 0, 0, 0)
                };

                int controlWidth = (int)(grid.Width - 23);
                //logo
                Image logo = new Image();
                logo.Source = Properties.Resources.logo1.ToBitmapSource();
                logo.Width = Properties.Resources.logo1.Width;
                logo.Height = Properties.Resources.logo1.Height;
                logo.Margin = new Thickness(left, top, 0, 0);
                logo.HorizontalAlignment = HorizontalAlignment.Left;
                logo.VerticalAlignment = VerticalAlignment.Top;
                grid.Children.Add(logo);
                top += logo.Height;

                //receipt
                Label receipt = new Label();
                receipt.Width = controlWidth;
                receipt.Height = height * 2;
                receipt.FontFamily = new FontFamily("Arial Unicode MS");
                receipt.FontSize = 12;
                receipt.Content = "Receipt No: " + recNo + "\t\t\t\t\t\t\t\t\t\tReceipt Date: " + DateTime.Now.ToString("dd-MMM-yy HH:mm") + 
                                "\nName        : " + details["FULLNAME"] + "\t\t Room No. " + details["ROOMTABLE"]
                                + "\t\t\t\t         Period:  " + Convert.ToDateTime(details["VTIMESTAMP"]).ToShortDateString() + " to " + DateTime.Now.ToShortDateString();
                receipt.Margin = new Thickness(left, top, 0, 0);
                receipt.BorderThickness = new Thickness(0.5);
                receipt.BorderBrush = Brushes.Gray;
                receipt.HorizontalAlignment = HorizontalAlignment.Left;
                receipt.VerticalAlignment = VerticalAlignment.Top;
                receipt.HorizontalContentAlignment = HorizontalAlignment.Left;
                receipt.VerticalContentAlignment = VerticalAlignment.Center;
                grid.Children.Add(receipt);
                top += receipt.Height;

                //header
                Label header = new Label();
                header.Width = controlWidth;
                header.Height = height;
                header.FontFamily = new FontFamily("Arial Unicode MS");
                header.FontSize = 12;
                header.Content = "DATE\t\t\t\t\t\tITEM\t\t\t\t\t\t\t\tAMOUNT";
                header.Margin = new Thickness(left, top, 0, 0);
                header.BorderThickness = new Thickness(1);
                header.BorderBrush = Brushes.Black;
                header.HorizontalAlignment = HorizontalAlignment.Left;
                header.VerticalAlignment = VerticalAlignment.Top;
                header.HorizontalContentAlignment = HorizontalAlignment.Left;
                header.VerticalContentAlignment = VerticalAlignment.Center;
                grid.Children.Add(header);
                top += header.Height;



                foreach (DataRow row in stmntCopy.Rows)
                {
                    foreach (DataColumn col in stmntCopy.Columns)
                    {
                        Label label = new Label();
                        label.Margin = new Thickness(left, top, 0, 0);
                        label.BorderBrush = Brushes.Gray;
                        label.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                        label.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                        label.BorderThickness = new Thickness(0.3);

                        label.Content = col.ColumnName == "ITEM" && row[col] != DBNull.Value ? row[col].ToString().PadRight(100).Substring(0, 50) :
                                      col.ColumnName == "AMOUNT" && row[col] != DBNull.Value ? Convert.ToInt32(row[col]).ToString("N0") :
                                      col.ColumnName == "DATE" && row[col].ToString() != "" ? Convert.ToDateTime(row[col]).ToString("dd-MMM-yy HH:mm") :
                                      row[col];

                        label.Width = col.Ordinal == 0 ? 150 : col.Ordinal == 1 ? 500 : 107;
                        label.Height = height;
                        label.FontFamily = new FontFamily("Arial Unicode MS");
                        label.FontSize = 12;
                        label.VerticalContentAlignment = VerticalAlignment.Center;
                        label.HorizontalContentAlignment = HorizontalAlignment.Left;

                        grid.Children.Add(label);
                        left += label.Width;
                    }
                    top += height;
                    left = LEFT_MARGIN;
                }

                //footer
                Label footer = new Label();
                footer.Width = controlWidth;
                footer.Height = height;
                footer.FontFamily = new FontFamily("Arial Unicode MS");
                footer.FontSize = 12;
                footer.Content = "You were served by: " + Session.UserName + ".\tPrices include VAT where applicable.\tThank You!\t" +
                    "POS Software by +256-712-256168";
                footer.Margin = new Thickness(LEFT_MARGIN, top, 0, 0);
                footer.BorderThickness = new Thickness(0.5);
                footer.BorderBrush = Brushes.Gray;
                footer.HorizontalAlignment = HorizontalAlignment.Left;
                footer.VerticalAlignment = VerticalAlignment.Top;
                footer.HorizontalContentAlignment = HorizontalAlignment.Left;
                grid.Children.Add(footer);

                FixedPage page = new FixedPage();
                page.Children.Add(grid);
                PageContent pc = new PageContent();
                pc.Child = page;
                pc.Margin = new Thickness(0, 0, 0, 0);
                doc.Pages.Add(pc);
            return doc;
        }
    }



}
