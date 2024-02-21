using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Text.Json.Serialization;
using StockApp.EFRIS;
using System.Data;
using static System.Windows.Forms.Design.AxImporter;
using StockApp.AppExtensions;

namespace StockApp
{
	/// <summary>
	/// Interaction logic for AdminWindow.xaml
	/// </summary>
	public partial class EFRISWindow : Window
	{
		APIExecutor apiExecutor;

		JsonSerializerOptions options = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
			UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
			IncludeFields = true
		};

		public EFRISWindow()
		{
			InitializeComponent();
			EFRISInvoiceUtilities.Initialize();
			apiExecutor = new APIExecutor();
		}


		private async Task Window_LoadedAsync(object sender, RoutedEventArgs e)
		{


			// Make the HTTP request
			var jsonContent = await apiExecutor.GetOfflineEnablerStatusInfo();
			dynamic jsonData = JsonSerializer.Deserialize<EnablerStatusRequest>(jsonContent, options);
			var data = jsonData.Data;

			EFRISInvoiceUtilities.DeviceNo = jsonData.Data.DeviceNo;

			if (jsonData.Success) EFRISInvoiceUtilities.AppToOfflineEnabler = "OK";
			else EFRISInvoiceUtilities.AppToOfflineEnabler = "FAILED";

			if (jsonData.Data.OnlineStatus) EFRISInvoiceUtilities.OfflineEnablerToRemote = "OK";
			else EFRISInvoiceUtilities.OfflineEnablerToRemote = "FAILED";


			//get the key and signature info
			jsonContent = await apiExecutor.GetServerTime();
			jsonData = JsonSerializer.Deserialize<Payload>(jsonContent, options);
			EFRISInvoiceUtilities.Signature = ((Payload)jsonData).Data.Signature;

		}



		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.F1)
			{
				MessageBox.Show(EFRISInvoiceUtilities.GetPropertyInfo());
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Window_LoadedAsync(sender, e);
		}



		private void btnSyncInventory_Click(object sender, RoutedEventArgs e)
		{
			Task.Run(() =>
			{
				string jsonContent = apiExecutor.GetCommodityCodes(5, 10).Result;
				Payload jsonData = JsonSerializer.Deserialize<Payload>(jsonContent, options);
				jsonContent = Encoders.Base64Decode(jsonData.Data.Content);

				int x = 1;
			});

			//DataTable stock = FBDataHelper.GetStock(FBDataHelper.StockType.Detailed, null, null);
		}
	}
}