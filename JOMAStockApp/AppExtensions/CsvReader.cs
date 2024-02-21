using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.AppExtensions
{
	public class CsvReader
	{
		private StreamReader sr = null;
		private string currentline = "";
		private char delimiter;

		private string[] fieldHeaders;
		private String[] rowContent;

		public Int32 FieldCount
		{
			get;
			private set;
		}

		public CsvReader(string filename, char delimiter)
		{
			sr = new StreamReader(filename);
			currentline = sr.ReadLine();  //index 0
			this.delimiter = delimiter;
			fieldHeaders = currentline.Split(delimiter);
			FieldCount = fieldHeaders.Length;
		}

		public string[] GetFieldHeaders()
		{
			return fieldHeaders;
		}
		public bool ReadNextRecord()
		{
			currentline = sr.ReadLine();
			if (currentline != null)
			{
				rowContent = currentline.Split(delimiter);
				return true;
			}
			else
				return false;
		}
		public string GetFieldValue(int index)
		{
			return rowContent[index];
		}
	}
}
