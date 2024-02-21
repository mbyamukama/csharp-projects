using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.EFRIS
{
	internal class CommodityCategoryRequest
	{
		[JsonProperty("pageNo")]
		public string PageNo { get; set; }
		[JsonProperty("pageSize")]
		public string PageSize { get; set; }

		public CommodityCategoryRequest(int pageNo, int pageSize)
		{
			PageNo = pageNo.ToString();
			PageSize = pageSize.ToString();
		}
	}
}
