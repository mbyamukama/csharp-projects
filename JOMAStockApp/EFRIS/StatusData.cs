using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.EFRIS
{
	public class StatusData
	{
		public string OfflineAmount { get; set; }
		public string EfrisUrl { get; set; }
		public string CurrentlyOfflineValue { get; set; }
		public string MysqlTime { get; set; } // nullable DateTimeOffset
		public string LatestLoginTime { get; set; } // nullable DateTimeOffset
		public string LatestTcsVersion { get; set; }
		public bool OnlineStatus { get; set; }
		public string TcsVersion { get; set; }
		public string CurrentlyOfflineAmount { get; set; }
		public string DeviceNo { get; set; }
		public string CommodityCategoryVersion { get; set; }
		public string CertificateIsMatched { get; set; }
		public string NumberChanged { get; set; }
		public string NumberOfOverdueInvoicesNotUploaded { get; set; }
		public string LatestCommodityCategoryVersion { get; set; }
		public string Tin { get; set; }
		public string OfflineDays { get; set; }
		public string WsVersion { get; set; }
		public string SysTime { get; set; } // nullable DateTimeOffset
		public long OfflineInvoiceSavingTime { get; set; }
		public string TcsSymmetricKeyExpiryDate { get; set; } // nullable DateTimeOffset
		public string BranchType { get; set; }
		public string OfflineValue { get; set; }
	}
}
