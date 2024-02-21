using StockApp;
using StockApp.EFRIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using StockApp.AppExtensions;

namespace StockApp.EFRIS
{
	public static class EFRISInvoiceUtilities
	{

		// Properties with default values
		public static string DeviceNo { get; set; }
		public static string TaxpayerID { get; set; }
		public static string TIN { get; set; }
		public static string UserName { get; set; }
		public static string AppVersion { get; set; }
		public static string DeviceMacAddress { get; set; }
		public static string Signature { get;  set; }
		public static string AppToOfflineEnabler { get; internal set; }
		public static string OfflineEnablerToRemote { get; internal set; }

		public static void Initialize()
		{
			DeviceNo = "";
			TaxpayerID = "JomaSuperMarket";
			TIN = "1002783159";
			UserName = "superadmin";
			AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			DeviceMacAddress = NetworkUtilities.GetMacAddress();
		}


		public static string GetPropertyInfo()
		{
			PropertyInfo[] properties = typeof(EFRISInvoiceUtilities).GetProperties(BindingFlags.Public | BindingFlags.Static); String info = "";

			foreach (PropertyInfo property in properties)
			{
				info += property.Name + "\t: " + property.GetValue(null) + "\n";
			}
			return info;
		}
	}

}
