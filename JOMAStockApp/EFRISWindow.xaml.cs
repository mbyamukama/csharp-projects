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
using System.Threading;

namespace StockApp
{
	/// <summary>
	/// Interaction logic for AdminWindow.xaml
	/// </summary>
	public partial class EFRISWindow : Window
	{
		APIExecutor apiExecutor;
		private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

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
			WindowStartupLocation = WindowStartupLocation.CenterOwner;
			progressBar.Visibility = txtProgress.Visibility = Visibility.Hidden;
			Window_LoadedAsync(sender, e);
		}



		private void btnSyncInventory_Click(object sender, RoutedEventArgs e)
		{
			progressBar.Visibility = txtProgress.Visibility = Visibility.Visible;
			Task task = Task.Run(() =>
			{
				int pageCount = 150;// 15652; 
				int pageSize = 10;
				double percent = 0;
				DateTime startTime = DateTime.Now;

				for (int pageNo = 1; pageNo <= pageCount; pageNo++)
				{
					if (cancellationTokenSource.Token.IsCancellationRequested)
						break;
					// Fetch commodity codes for the current page
					string jsonContent = apiExecutor.GetCommodityCodes(pageNo, pageSize).Result;
					Payload jsonData = JsonSerializer.Deserialize<Payload>(jsonContent, options);
					jsonContent = Encoders.Base64Decode(jsonData.Data.Content);
					//parse to Response
					CommodityCategoryResponse response = JsonSerializer.Deserialize<CommodityCategoryResponse>(jsonContent, options);
					FBDataHelper.AddCommodityCategoryRecords(response.Records);

					percent = Math.Round(pageNo * 100.0/ pageCount, 1);

					// Calculate IPS
					double secondsElapsed = (DateTime.Now - startTime).TotalSeconds;
					int itemsPerSecond = (int)(pageNo / secondsElapsed);

					// Calculate ETA
					double itemsRemaining = pageCount - pageNo;
					double etaSeconds = itemsRemaining / itemsPerSecond;
					TimeSpan etaTimeSpan = TimeSpan.FromSeconds(etaSeconds);
					string etaFormatted = $"{etaTimeSpan:mm\\:ss}";

					//UI update
					Application.Current.Dispatcher.Invoke(() =>
					{
						txtProgress.Content = pageNo + "/" + pageCount + "  (" + percent + "%)  " + itemsPerSecond+ " items/s  ETA=" + etaFormatted ;
						progressBar.Value = percent;
					});

				}
				Thread.Sleep(10);
			}, cancellationTokenSource.Token);

			task.ContinueWith(t =>
			{
				// This part of the code will execute when the task completes
				if (t.IsFaulted)
				{
					// Handle any exceptions
				}
				else if (t.IsCanceled)
				{
					// Handle cancellation
				}
				else
				{
					MessageBox.Show("Import Completed", "Complete", MessageBoxButton.OK, MessageBoxImage.Information);
				}
			});
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
		   cancellationTokenSource.Cancel();
		}
	}
}