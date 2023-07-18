using MagicVillaUtilities;
using MagicVillaWeb.Models;
using MagicVillaWeb.Models.DTO;
using MagicVillaWeb.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MagicVillaWeb.Services
{
	public class VillaService : BaseService, IVillaService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private string villaUrl;
		public VillaService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
		{
			_httpClientFactory = httpClient;
			this.villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
		}

		public Task<T> CreateAsync<T>(VillaCreateDTO dto)
		{
			return SendAsync<T>(new APIRequest()
			{
				ApiType = SD.ApiType.POST,
				Data = dto,
				Url = villaUrl + "/api/villaAPI" // here villaUrl contains the
												 // values specified in appSettings.json, what we append is the route
												 // defined in the API Project's Controller route.
			});
		}

		public Task<T> DeleteAsync<T>(int id)
		{
			return SendAsync<T>(new APIRequest()
			{
				ApiType = SD.ApiType.DELETE,
				Url = villaUrl + "/api/villaAPI/" + id // here we modify the route and append id
			});
		}

		public Task<T> GetAllAsync<T>()
		{
			return SendAsync<T>(new APIRequest()
			{
				ApiType = SD.ApiType.GET,
				Url = villaUrl + "/api/villaAPI" 
			});
		}

		public Task<T> GetAsync<T>(int id)
		{
			return SendAsync<T>(new APIRequest()
			{
				ApiType = SD.ApiType.GET,
				Url = villaUrl + "/api/villaAPI/" + id // here we modify the route and append id
			});
		}

		public Task<T> UpdateAsync<T>(VillaUpdateDTO dto)
		{
			return SendAsync<T>(new APIRequest()
			{
				ApiType = SD.ApiType.PUT,
				Data = dto,
				Url = villaUrl + "/api/villaAPI/" + dto.Id
			});
		}
	}
}
