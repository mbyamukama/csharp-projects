using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace StockApp.EFRIS
{
	public class Payload
	{
		public Data Data { get; set; }
		public GlobalInfo GlobalInfo { get; set; }
		public ReturnStateInfo ReturnStateInfo { get; set; }

		public Payload() { }

		public Payload(string base64Content, string signature, string interfaceCode)
		{
			Data = new Data()
			{
				Content = base64Content,
				Signature = signature,
				DataDescription = new Description()
				{
					CodeType = "0",
					EncryptCode = "1",
					ZipCode = "Bwebajja"
				}
			};
			GlobalInfo = new GlobalInfo()
			{
				AppId = "AP04",
				AgentType = "0",	
				Version = EFRISInvoiceUtilities.AppVersion,
				DataExchangeId = Guid.NewGuid().ToString().Substring(0, 32),
				InterfaceCode = interfaceCode,
				RequestCode = "TP",
				RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
				ResponseCode = "TA",
				UserName = GlobalSystemData.Session.CurrentUser.ToString(),
				DeviceMAC = EFRISInvoiceUtilities.DeviceMacAddress, //assigned when the form is loaded
				DeviceNo = EFRISInvoiceUtilities.DeviceNo,
				Tin = EFRISInvoiceUtilities.TIN,
				TaxpayerID = EFRISInvoiceUtilities.TaxpayerID,
				Longitude = "32.5380",
				Latitude = "0.175496",
				ExtendField=null
			};

			ReturnStateInfo = new ReturnStateInfo()
			{
				ReturnCode = "0"
			};
		}
	}
}
