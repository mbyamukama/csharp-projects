using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.EFRIS
{
	public static class Constants
	{
		public static string OFFLINE_ENABLER_POST_URL = "http://127.0.0.1:9880/efristcs/ws/tcsapp/getInformation";
		public static string EFRIS_PROD_URL = "https://efrisws.ura.go.ug/ws/taapp/getInformation";
		public static string EFRIS_TEST_URL = "https://efristest.ura.go.ug/efrisws/ws/taapp/getInformation";
		public static string OFFLINE_ENABLER_SERVICE_URL = "http://127.0.0.1:9880/efristcs/tcs/info/getInfo";

	}
}
