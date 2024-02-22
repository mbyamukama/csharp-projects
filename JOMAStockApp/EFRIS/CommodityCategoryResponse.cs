using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.EFRIS
{
	public class Page
	{
		public int PageNo { get; set; }
		public int PageSize { get; set; }
		public int TotalSize { get; set; }
		public int PageCount { get; set; }
	}

	public class Record
	{
		public string CommodityCategoryCode { get; set; }
		public string CommodityCategoryLevel { get; set; }
		public string CommodityCategoryName { get; set; }
		public string EnableStatusCode { get; set; }
		public string Exclusion { get; set; }
		public string ExemptRateStartDate { get; set; }
		public string IsExempt { get; set; }
		public string IsLeafNode { get; set; }
		public string IsZeroRate { get; set; }
		public string ParentCode { get; set; }
		public string Rate { get; set; }
		public string ServiceMark { get; set; }
	}

	public class CommodityCategoryResponse
	{
		public String DateFormat { get; set; }
		public String NowTime { get; set; }
		public Page Page { get; set; }
		public List<Record> Records { get; set; }
	}

}
