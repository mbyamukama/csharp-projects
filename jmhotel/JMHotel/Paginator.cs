using System;
using System.Collections.Generic;
using System.Windows;
using System.Data;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Globalization;
using GenericUtilities;

namespace JMHotel
{
    public static class Paginator
    {
        //NB: A4 paper size is 797 * 1123 pixels on 96DPI
        public static PageContent [] Paginate(DataTable dt, double fontSize, int [] columnWidths, int [] custom)
        {
            Size pageSize = new Size();
            Size a4Portrait = new Size(797, 1123); 
            Size a4Landscape = new Size(1123, 797);

            if (custom == null)
            {
                if (dt.Columns.Count > 8)
                    pageSize = a4Landscape;
                else pageSize = a4Portrait;
            }
            else
            {
                pageSize = new Size(custom[0], custom[1]);
            }

            DataTable origTable = dt.Copy();
            double rowHeight = fontSize * 1.5;
            int rowsPerPage = (int)(350 * 1.0 / fontSize);
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
                //3. Get the grid
                Grid grid = Utilities.LabelTable(tempTable, new Thickness(0.5), fontSize, columnWidths);

                //4. Add it to the page content
                FixedPage tempPage = new FixedPage();

                tempPage.Children.Add(grid);
                pages[i] = new PageContent();
                pages[i].Child = tempPage;
            }

            return pages;
        }

        static void grid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if ((e.PropertyType == typeof(Int32) || e.PropertyType == typeof(Int64))
                && e.Column.Header.ToString() != "ENTRYID" && e.Column.Header.ToString() != "VISITID")
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "N0";
            }
        }
    }
}
