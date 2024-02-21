using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using CSVReader;

namespace GenericUtilities
{
    public static class Utilities
    {
        //paginator

        public static bool AuthenticateUser(string password, string hash)
        {
            return Hasher.ValidatePassword(password, hash); 
        }

        public static DataTable ReadCSVFile(string filePath)
        {
            DataTable dataTable = new DataTable();
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            CsvReader csvReader = new CsvReader(new StreamReader(fs), true);
            csvReader.MissingFieldAction = CSVReader.MissingFieldAction.ReplaceByNull;

            int fieldCount = csvReader.FieldCount;
            string[] row = new string[fieldCount];
            string[] headers = csvReader.GetFieldHeaders();

            for (int i = 0; i < headers.Length; i++) //headers length = field count
            {
                dataTable.Columns.Add(headers[i], typeof(string));
            }
            while (csvReader.ReadNextRecord())
            {
                for (int i = 0; i < fieldCount; i++)
                {
                    row[i] = csvReader[i];
                }
                dataTable.Rows.Add(row);
            }
            fs.Close();
            return dataTable;
        }
        public static bool WriteCSVFile(DataTable dataTable, string path)
        {
            StreamWriter sw = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.Write));
            string firstLine = "";
            foreach (DataColumn col in dataTable.Columns)
            {
                firstLine += col.ColumnName + ",";
            }
            firstLine = firstLine.TrimEnd(new char[] { ',' });
            sw.WriteLine(firstLine);
            foreach (DataRow row in dataTable.Rows)
            {
                string line = "";
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    line += row[j] + ",";
                }
                line = line.TrimEnd(new char[] { ',' });
                sw.WriteLine(line);
            }
            sw.Close();
            return true;
        }
        /// <summary>
        /// appends leading zeros to a single or double digit number to form a 3 digit number
        /// </summary>
        /// <param name="number"> the number to zero-pad</param>
        /// <returns>string</returns>
        public static string ZeroPad(this Int32 number)
        {
            string x = number.ToString();
            if (x.Length == 1)
            {
                return "00" + x;
            }
            if (x.Length == 2)
            {
                return "0" + x;
            }
            else return x;
        }
        /// <summary>
        /// removes all characters such as /, : , etc.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string StripDate(DateTime dateTime, bool includeMs)
        {
            if (includeMs)
                return "" + dateTime.Day + dateTime.Month + dateTime.Year + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond;
            else
                return "" + dateTime.Day + dateTime.Month + dateTime.Year + dateTime.Hour + dateTime.Minute + dateTime.Second;
        }
        public static void VisualizeDataTable(DataTable dataTable, string windowName, int width, int height)
        {
            DataGrid datagrid = new DataGrid();
            datagrid.ItemsSource = dataTable.DefaultView;
            datagrid.AutoGenerateColumns = true;
            //Create a border with the initial height and width of the user control.  
            Border borderWithInitialDimensions = new Border();

            borderWithInitialDimensions.Height = height;
            borderWithInitialDimensions.Width = width;


            //Set the user control's dimensions to double.NaN so that it auto sizes  
            //to fill the window.  
            datagrid.Height = double.NaN;
            datagrid.Width = double.NaN;


            //Create a grid hosting both the border and the user control.  The   
            //border results in the grid and window (created below) having initial  
            //dimensions.  
            Grid hostGrid = new Grid();

            hostGrid.Children.Add(borderWithInitialDimensions);
            hostGrid.Children.Add(datagrid);


            //Create a window that resizes to fit its content with the grid as its   
            //content.  
            Window hostWindow = new Window();

            hostWindow.Content = hostGrid;
            hostWindow.SizeToContent = SizeToContent.WidthAndHeight;
            hostWindow.Title = windowName;
            hostWindow.Show();
        }
        /// <summary>
        /// Converts a dataColumn into an IEnumerable. Null values and empty strings are not added
        /// </summary>
        /// <param name="dataColumn"> the data column whose values to use to fill the array</param>
        /// <returns> the filled array</returns>
        public static List<T> AsEnumerable<T>(this DataColumn dataColumn)
        {
            List<T> values = new List<T>();
            foreach (DataRow row in dataColumn.Table.Rows)
            {
                T item = default(T);
                if (row[dataColumn] is T)
                {
                    try
                    {
                        item = (T)row[dataColumn];
                    }
                    catch (Exception)
                    {
                        item = default(T);
                    }

                }
                else
                    try
                    {
                        item = (T)Convert.ChangeType(row[dataColumn], typeof(T));
                    }
                    catch (InvalidCastException)
                    {
                        item = default(T);
                    }
                    catch (Exception)
                    {
                        item = default(T);
                    }

                if (item != null | item.ToString() != "")
                {
                    values.Add(item);
                }
            }
            return values;
        }

        public static DataTable MergeSchema(this DataTable original, DataTable dataTable, string mergeColumnName)
        {
            DataTable result = original.Clone();
            foreach (DataColumn col in dataTable.Columns)
            {
                if (col.ColumnName != mergeColumnName)
                {
                    result.Columns.Add(col.ColumnName);
                }
            }
            return result;
        }
        public static Grid LabelTable(DataTable dt, System.Windows.Thickness margin, double fontSize, string font, int rowHeight, int[] columnWidths, string [] alignments)
        {
            DataTable dtCloned = dt.Copy();
            Grid grid = new Grid()
            {
                Name = "root",
                Width = 800,
                Margin = new Thickness(5, 5, 5, 5)

            };

            double top = margin.Top, left = margin.Left, height = rowHeight;
            double LEFT_MARGIN = margin.Left;

            HorizontalAlignment[] hAlignment = new HorizontalAlignment[alignments.Length];
            for (int k = 0; k < alignments.Length; k++)
            {
                if (alignments[k] == "L")
                    hAlignment[k] = System.Windows.HorizontalAlignment.Left;
                else if (alignments[k] == "C")
                    hAlignment[k] = HorizontalAlignment.Center;
                else if (alignments[k] == "R")
                    hAlignment[k] = HorizontalAlignment.Right;
                else
                    hAlignment[k] = System.Windows.HorizontalAlignment.Left; //default
            }

            foreach (DataColumn col in dtCloned.Columns)
            {
                Label label = new Label()
                {
                    Margin = new Thickness(left, top, 0, 0),
                    BorderBrush = System.Windows.Media.Brushes.Gray,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    VerticalAlignment = System.Windows.VerticalAlignment.Top,
                    BorderThickness = margin,
                    Content = col.ColumnName,
                    Width = columnWidths[col.Ordinal],
                    Height = height,
                    FontFamily = new System.Windows.Media.FontFamily(font),
                    FontSize = fontSize,
                    VerticalContentAlignment = VerticalAlignment.Top,
                    HorizontalContentAlignment = hAlignment[col.Ordinal]
                };
                grid.Children.Add(label);
                left += label.Width;

            }
            top += height;
            foreach (DataRow row in dtCloned.Rows)
            {
                left = LEFT_MARGIN;
                foreach (DataColumn col in dtCloned.Columns)
                {
                    Label label = new Label()
                    {
                        Margin = new Thickness(left, top, 0, 0),
                        BorderBrush = System.Windows.Media.Brushes.Gray,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                        VerticalAlignment = System.Windows.VerticalAlignment.Top,
                        BorderThickness = margin,

                        Content = (col.DataType == typeof(Int32) && col.Ordinal != 0 && row[col] != DBNull.Value) ? Convert.ToInt32(row[col]).ToString("N0") :
                        (col.DataType == typeof(DateTime) && row[col] != DBNull.Value) ? DateTime.Parse(row[col].ToString()).ToShortDateString() : row[col].ToString(),

                        Width = columnWidths[col.Ordinal],
                        Height = height,
                        FontFamily = new System.Windows.Media.FontFamily(font),
                        FontSize = fontSize,
                        VerticalContentAlignment = VerticalAlignment.Top,
                        HorizontalContentAlignment = HorizontalAlignment.Left
                    };
                    grid.Children.Add(label);
                    left += label.Width;
                }
                top += height;

            }
            return grid;

        }
    }
}
