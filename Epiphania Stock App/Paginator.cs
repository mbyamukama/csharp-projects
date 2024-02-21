using System;
using System.Collections.Generic;
using System.Windows;
using System.Data;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Globalization;

namespace StockApp
{
    public static class Paginator
    {
        //NB: A4 paper size is 797 * 1123 pixels on 96DPI
        public static PageContent [] Paginate(DataTable dt, double fontSize)
        {
            Size pageSize = new Size();
            Size a4Portrait = new Size(797, 1123);
            Size a4Landscape = new Size(1123, 797);

            if (dt.Columns.Count > 8) pageSize = a4Landscape; else pageSize = a4Portrait;

            DataTable origTable = dt.Copy();
            double rowHeight = fontSize * 1.5;
            int rowsPerPage = (int)(640 * 1.0 / fontSize);
            int pageCount = (int)Math.Ceiling(dt.Rows.Count * 1.0 / rowsPerPage);

            //3. Initialise the page content
            PageContent[] pages = new PageContent[pageCount];
            
            FixedDocument fDoc = new FixedDocument();
            
            //add Pages
            int startrow = 0;

            for (int i = 0; i < pageCount; i++)
            {
                //1. We are now on a page : New DataTable with same schema as original table
                DataTable tempTable = origTable.Clone();
                //2. Add the rows
                for (int j = startrow; j < startrow + rowsPerPage; j++)
                {
                    if (j == origTable.Rows.Count) break;  //all rows have been added
                    tempTable.ImportRow(origTable.Rows[j]);
                }
                startrow += rowsPerPage;
                //3. Make the Grid
                DataGrid grid = new DataGrid();
                grid.AutoGenerateColumns = true;
                grid.AutoGeneratingColumn += new EventHandler<DataGridAutoGeneratingColumnEventArgs>(grid_AutoGeneratingColumn);
                grid.ItemsSource = tempTable.DefaultView;  //set the datasource of the grid
                grid.FontSize = fontSize;
                grid.RowHeight = rowHeight;
                grid.Margin = new Thickness(30);


                //4. Add it to the page content
                FixedPage tempPage = new FixedPage();
                tempPage.Children.Add(grid);
                pages[i] = new PageContent();
                pages[i].Child = tempPage;
            }

           /* foreach (PageContent p in pages)
            {
                p.Child.Width = pageSize.Width;
                p.Child.Height = pageSize.Height;
                fDoc.Pages.Add(p);
            }*/
            return pages;
        }

        static void grid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(DateTime))
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd-MMM-yy";
            }
            if ((e.PropertyType == typeof(Int32) || e.PropertyType == typeof(Int64)) && e.Column.Header.ToString()!="ENTRYID")
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "N0";
            }
        }
    }
}
