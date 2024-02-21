using System;
using System.Windows;
using System.Data;
using System.Windows.Controls;
using System.Windows.Documents;

namespace StockApp
{
    public static class Paginator
    {
        //NB: A4 paper size is 797 * 1123 pixels on 96DPI
        public static PageContent [] Paginate(DataTable dt, double fontSize, int [] columnWidths, string [] alignments)
        {
            Size pageSize = new Size();
            Size a4Portrait = new Size(797, 1123);
            Size a4Landscape = new Size(1123, 797);

            if (dt.Columns.Count > 8)
            {
                pageSize = a4Landscape;
                fontSize = 9;
            }
            else
            {
                pageSize = a4Portrait;
            }
                    

            DataTable origTable = dt.Copy();
            int rowHeight = (int)(fontSize * 2.5);
            int rowsPerPage = (int)Math.Floor(pageSize.Height * 1.0 / rowHeight) - 5;         //allowance
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

                Grid grid = StockApp.AppExtensions.Windows.LabelTable(tempTable, new Thickness(0.5), fontSize, "Segoe UI", rowHeight, columnWidths, alignments);


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
