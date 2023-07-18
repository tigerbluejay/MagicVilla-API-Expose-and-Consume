using MagicVillaWeb.Models;
using MagicVillaWeb.Services.Interfaces;
using Newtonsoft.Json;
using MagicVillaUtilities;
using System.Text;

namespace MagicVillaWeb.Services
{
	// base service for all of the requests to the API
	public class BaseService : IBaseService
	{
		public APIResponse responseModel { get; set; }

		public IHttpClientFactory httpClient { get; set; }

        public BaseService(IHttpClientFactory httpClient)
        {
			this.responseModel = new (); // this is relevant for when we receive the response
			this.httpClient = httpClient; // this will be relevant to call the api
        }
		
        public async Task<T> SendAsync<T>(APIRequest apiRequest)
		{
			try
			{
				/*---- CONFIGURE THE REQUEST --- */

				var client = httpClient.CreateClient("MagicAPI");
				HttpRequestMessage message = new HttpRequestMessage();
				message.Headers.Add("Accept", "application/json"); // configure the message
				message.RequestUri = new Uri(apiRequest.Url); // configure the message, we need
															  // the url where we'll call the api
				// data will not be null in POST and PUT calls
				if (apiRequest.Data != null)
				{
					// this data we will serialize if we CREATE or UPDATING (POST OR PUT)
					message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data),
						Encoding.UTF8, "application/json"); // configure the message
				}

				switch (apiRequest.ApiType) // further configure the message, specifically what is
											// the HTTP type
				{
					case SD.ApiType.POST:
						message.Method = HttpMethod.Post;
						break;
					case SD.ApiType.PUT:
						message.Method = HttpMethod.Put;
						break;
					case SD.ApiType.DELETE:
						message.Method = HttpMethod.Delete;
						break;
					default:
						message.Method = HttpMethod.Get;
						break;
				}

				/*---- PROCESS THE RESPONSE --- */

				HttpResponseMessage apiResponse = null;

				//!!!!!! this is the key point for debugging every call to the API
				apiResponse = await client.SendAsync(message); // we call the API endpoint
																// and we pass the message

				// we receive the response
				var apiContent = await apiResponse.Content.ReadAsStringAsync();
				var APIResponse = JsonConvert.DeserializeObject<T>(apiContent);
				return APIResponse; // return the response
			}
			catch(Exception ex)
			{
				// if there is an error we need to return an APIResponse also
				var dto = new APIResponse
				{
					ErrorMessages = new List<string> { Convert.ToString(ex.Message) },
					IsSuccess = false
				};

				var res = JsonConvert.SerializeObject(dto);
				var APIResponse = JsonConvert.DeserializeObject<T>(res);
				return APIResponse;
			}
		}
	}
}
