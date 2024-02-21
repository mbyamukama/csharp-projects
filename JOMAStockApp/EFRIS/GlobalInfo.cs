using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.EFRIS
{
	public class GlobalInfo
	{
		public string AgentType { get; set; }
		public string AppId { get; set; }
		public string Version { get; set; }
		public string DataExchangeId { get; set; }
		public string InterfaceCode { get; set; }
		public string RequestCode { get; set; }
		public string RequestTime { get; set; }
		public string ResponseCode { get; set; }
		public string UserName { get; set; }
		public string DeviceMAC { get; set; }
		public string DeviceNo { get; set; }
		public string Tin { get; set; }
		public string TaxpayerID { get; set; }
		public string Longitude { get; set; }
		public string Latitude { get; set; }
		public ExtendField ExtendField { get; set; }
	}
}
