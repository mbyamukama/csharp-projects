using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Controls;
using System.Windows;
using System.Net.NetworkInformation;

namespace StockApp.AppExtensions
{
    public static class FileUtilities
    {
        //paginator                
        public static DataTable ReadCSVFile(string filePath)
        {
            DataTable dataTable = new DataTable();
            CsvReader csvReader = new CsvReader(filePath, ',');

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
                    row[i] = csvReader.GetFieldValue(i);
                }
                dataTable.Rows.Add(row);
            }
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
    }

	public static class NetworkUtilities
	{

		public static string GetMacAddress()
		{
			string macAddressString = "";
			try
			{
				// Get all network interfaces on the machine
				NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

				foreach (NetworkInterface networkInterface in networkInterfaces)
				{
					// Check if the network interface is operational and not a loopback or tunnel
					if (networkInterface.OperationalStatus == OperationalStatus.Up &&
						networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
						networkInterface.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
					{
						// Retrieve the physical address (MAC address) of the network interface
						PhysicalAddress macAddress = networkInterface.GetPhysicalAddress();

						// Convert the physical address to a string
						macAddressString = BitConverter.ToString(macAddress.GetAddressBytes());
					}
				}
			}
			catch (Exception ex)
			{

			}
			return macAddressString;
		}
	}


    public static class DataUtilities
    {

        public static string AsFBDateTime(this DateTime dateTime)
        {
            if (dateTime != null)
                return dateTime.Day + "." + dateTime.Month + "." + dateTime.Year;
            else
                return null;
        }

        public static List<DataTable> Split(this DataTable dt, int rowsPerPage)
        {
            List<DataTable> tables = new List<DataTable>();
            int startPosition = 0,  pageCount = (int)Math.Ceiling(dt.Rows.Count * 1.0 / rowsPerPage);

            if (dt.Rows.Count > rowsPerPage)
            {
                for (int i = 0; i < pageCount; i++)
                {
                    DataTable temp = dt.Clone();

                    for (int j = startPosition; j < startPosition + rowsPerPage; j++)
                    {
                        if (j == dt.Rows.Count) break;
                        temp.ImportRow(dt.Rows[j]);
                    }
                    tables.Add(temp);
                    startPosition += rowsPerPage;
                }
            }

            else
            {
                tables.Add(dt);
            }
            return tables;
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
    }


    /**************** GENERAL UTILITIES*************************/

    public static class General
    {
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
	}


    /*****************WINDOWS UTILITIES*************************/
    public static class Windows
    {
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

        public static Grid LabelTable(DataTable dt, Thickness margin, double fontSize, string font, int rowHeight, int[] columnWidths, string[] alignments)
        {
            if (columnWidths.Length < dt.Columns.Count)
            {
                throw new Exception("Column widths array length (" + columnWidths.Length + ") is less than number of columns (" + dt.Columns.Count + ")");
            }
            if (alignments.Length < dt.Columns.Count)
            {
                throw new Exception("Alignments array length (" + alignments.Length + ") is less than number of columns (" + dt.Columns.Count + ")");
            }
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
                    hAlignment[k] = HorizontalAlignment.Left;
                else if (alignments[k] == "C")
                    hAlignment[k] = HorizontalAlignment.Center;
                else if (alignments[k] == "R")
                    hAlignment[k] = HorizontalAlignment.Right;
                else
                    hAlignment[k] = HorizontalAlignment.Left; //default
            }

            foreach (DataColumn col in dtCloned.Columns)
            {
                Label label = new Label()
                {
                    Margin = new Thickness(left, top, 0, 0),
                    BorderBrush = System.Windows.Media.Brushes.Gray,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    BorderThickness = margin,
                    Content = col.ColumnName,
                    Width = columnWidths[col.Ordinal],
                    Height = height,
                    FontFamily = new System.Windows.Media.FontFamily(font),
                    FontSize = fontSize,
                    VerticalContentAlignment = VerticalAlignment.Center,
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
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
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
