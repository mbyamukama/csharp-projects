using StockApp.AppExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StockApp.EFRIS
{
	public class APIExecutor
	{
		private HttpClient httpClient = new HttpClient();

		JsonSerializerOptions options = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
			UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
			IncludeFields = true
		};

		public APIExecutor(){
		}

		private async Task<string> PostInterfaceCodeAsync(string base64Content, string signature, string interfaceCode)
		{
			Payload payload = new Payload(base64Content, signature, interfaceCode);
			// Serialize the object to JSON
			string jsonPayload = JsonSerializer.Serialize(payload);

			// Make the HTTP request
			var response = await httpClient.PostAsync(Constants.OFFLINE_ENABLER_POST_URL, new StringContent(jsonPayload, Encoding.UTF8, "application/json"));

			// Check for successful response
			if (response.IsSuccessStatusCode)
			{
				// Read the JSON content
				return await response.Content.ReadAsStringAsync();
			}
			else
			{
				throw new HttpRequestException($"Error: {response.StatusCode}");
			}

		}

		public async Task<string> GetOfflineEnablerStatusInfo()
		{
			// Make the HTTP request
			var response = await httpClient.PostAsync(Constants.OFFLINE_ENABLER_SERVICE_URL, null);
			// Check for successful response
			if (response.IsSuccessStatusCode)
			{
				// Read the JSON content
				return await response.Content.ReadAsStringAsync();
			}
			else
			{
				throw new HttpRequestException($"Error: {response.StatusCode}");
			}
		}

		public async Task<string> GetServerTime()
		{
			return await PostInterfaceCodeAsync(null, null, InterfaceCodes.LOCAL_SERVER_TIME);

		}

		public async Task<string> GetCommodityCodes(int pageNo, int pageSize)
		{
			CommodityCategoryRequest cmRequest = new CommodityCategoryRequest(pageNo, pageSize);
			string content = JsonSerializer.Serialize(cmRequest);
			string base64Content = Encoders.Base64Encode(content);
			return await PostInterfaceCodeAsync(base64Content, null, InterfaceCodes.QUERY_COMMODITY_CATEGORY);

		}
	}
}
