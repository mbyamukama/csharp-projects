using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Data;
using System.Windows.Documents;

namespace StockApp
{
    public static class AppUtilities
    {
        public static Session session = null;
        public static int RoundUpToNearestNote(double number)
        {
            int temp = (int)number;
            while (temp % 100 != 0)
            {
                ++temp;
            }
            return temp;
        }
        public static DataTable addStockdataTable = null;
        public static void InitStockDataTable()
        {
            addStockdataTable = new DataTable();
            addStockdataTable.Columns.Add("ENTRYID");
            addStockdataTable.Columns.Add("DRUGNAME");
            addStockdataTable.Columns.Add("BATCHNO");
            addStockdataTable.Columns.Add("SUPPLIER");
            addStockdataTable.Columns.Add("INVOICENUM");
            addStockdataTable.Columns.Add("PURCHASE");
            addStockdataTable.Columns.Add("QUANTITY");
            addStockdataTable.Columns.Add("COSTPPU");
            addStockdataTable.Columns.Add("COSTTOTAL"); addStockdataTable.Columns["COSTTOTAL"].DataType = typeof(int);
            addStockdataTable.Columns.Add("FACTOR");
            addStockdataTable.Columns.Add("EXPIRYDATE");
            addStockdataTable.Columns.Add("DGROUP");
            addStockdataTable.Columns.Add("DATEOFENTRY");
            addStockdataTable.Columns.Add("EXISTS");

        }
        public static string AsFBDateTime(this DateTime dateTime)
        {
            return dateTime.Day + "." + dateTime.Month + "." + dateTime.Year;
        }
        public static int GetColumnTotal(this DataColumn dataCol)
        {
            int tot = 0;
            foreach (DataRow row in dataCol.Table.Rows)
            {
                object value = row[dataCol];
                if (value.GetType() != typeof(DBNull))
                {
                    tot += Convert.ToInt32(value);
                }
                else
                {
                    tot += 0;
                }
            }
            return tot;
        }
        public static int GetColumnTotal(this EnumerableRowCollection rowCollection, string columnName)
        {
            int tot = 0;
            foreach (DataRow row in rowCollection)
            {
                object value = row[columnName];
                if (value.GetType() != typeof(DBNull))
                {
                    tot += Convert.ToInt32(value);
                }
                else
                {
                    tot += 0;
                }
            }
            return tot;
        }
        public static FixedDocument AppendFirstPage(this PageContent[] pages, FixedPage page1)
        {
            FixedDocument newDoc = new FixedDocument();
            newDoc.Name = "Report";
            PageContent p = new PageContent();
            p.Child = page1;
            newDoc.Pages.Add(p);
            foreach (PageContent pc in pages)
            {
                newDoc.Pages.Add(pc);
            }
            return newDoc;
        }

        public static FixedDocument GenerateReceipt(DataTable itemsBought, int cash, int total, int balance, string recNo, int discount)
        {
            int printerWidth = 210;  //58mm POS, use 300 for 80mm POS
            int controlWidth = printerWidth - 10;
            Grid grid = new Grid()
            {
                Name = "root",
                Width = printerWidth,
                Margin = new Thickness(0, 0, 0, 0)

            };

            double top = 10;
            const int LEFT_MARGIN = 0;

            //title
            Label title = new Label();
            title.Width = controlWidth;
            title.Height = 60;
            title.FontFamily = new FontFamily("Arial");
            title.FontSize = 14;
            title.Content = "EPIPHANIA PHARMACY\n" + "Tank Hill Road, Muyenga.\n" +
                "Tel: 0414697225\n\n";
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
            receipt.Width = controlWidth;
            receipt.Height = 30;
            receipt.FontFamily = new FontFamily("Arial");
            receipt.FontSize = 10;
            receipt.Content = "Rec. No: " + recNo + "   Date: " + DateTime.Now.ToString();
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
            header.Width = controlWidth;
            header.Height = 30;
            header.FontFamily = new FontFamily("Arial");
            header.Content = "ITEM\t            QTY   AMOUNT";
            header.Margin = new Thickness(LEFT_MARGIN, top, 0, 0);
            header.BorderThickness = new Thickness(1);
            header.BorderBrush = Brushes.Black;
            header.HorizontalAlignment = HorizontalAlignment.Left;
            header.VerticalAlignment = VerticalAlignment.Top;
            header.HorizontalContentAlignment = HorizontalAlignment.Center;
            grid.Children.Add(header);
            top += header.Height + 2;

            int columns = itemsBought.Columns.Count;
            int rows = itemsBought.Rows.Count;
            double left = LEFT_MARGIN, height = 25;

            //add change rows
            itemsBought.Rows.Add("", "", "", "", "");
            if (discount > 0) { itemsBought.Rows.Add("Discount", "", "", "", discount); }
            itemsBought.Rows.Add("Total", "", "", "", total);
            itemsBought.Rows.Add("Cash", "", "", "", cash);
            itemsBought.Rows.Add("Change", "", "", "", balance);

            foreach (DataRow row in itemsBought.Rows)
            {
                foreach (DataColumn col in itemsBought.Columns)
                {
                    if (col.ColumnName != "FACTOR" && col.ColumnName != "COSTPPU")
                    {
                        Label label = new Label()
                        {
                            Margin = new Thickness(left, top, 0, 0),
                            BorderBrush = Brushes.Gray,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top,
                            BorderThickness = new Thickness(1),
                            Content = col.ColumnName == "DRUGNAME"? row[col].ToString().PadRight(100).Substring(0, 15):
                                      col.ColumnName == "QUANTITY"? row[col]:
                                      row[col],

                            Width = col.Ordinal == 0 ? 120 : col.Ordinal == 1 ? 30 : 50,
                            Height = height,
                            FontFamily = new FontFamily("Arial"),
                            FontSize = 12,
                            VerticalContentAlignment = VerticalAlignment.Top,
                            HorizontalContentAlignment = HorizontalAlignment.Left
                        };
                        grid.Children.Add(label);
                        left += label.Width;
                    }
                }
                top += height;
                left = LEFT_MARGIN;
            }

            //footer
            Label footer = new Label();
            footer.Width = controlWidth;
            footer.Height = 55;
            footer.FontFamily = new FontFamily("Arial");
            footer.FontSize = 12;
            footer.Content = "Served by: " + session.User.UserName +
                "\nTHANK YOU. PLEASE COME AGAIN.";
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
            FixedDocument doc = new FixedDocument();
            doc.Pages.Add(pc);
            return doc;

        }

        public static SDataTable AsSDataTable(this DataTable dt)
        {
            SDataTable sdt = new SDataTable();
            foreach(DataColumn col in dt.Columns)
            {
                sdt.Columns.Add(col.ColumnName);
                sdt.Columns[col.ColumnName].DataType = col.DataType;
            }
            foreach(DataRow row in dt.Rows)
            {
                sdt.Rows.Add(row.ItemArray);
            }
            return sdt;
        }
    }
}
