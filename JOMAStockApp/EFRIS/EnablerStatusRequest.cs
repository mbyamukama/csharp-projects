
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.EFRIS
{
	public class EnablerStatusRequest
	{
		public int Error { get; set; }
		public string Script { get; set; }
		public StatusData Data { get; set; }
		public bool Success { get; set; }
	}
}
