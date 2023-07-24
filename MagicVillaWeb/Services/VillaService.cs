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

		public Task<T> CreateAsync<T>(VillaCreateDTO dto, string token)
		{
			return SendAsync<T>(new APIRequest()
			{
				ApiType = SD.ApiType.POST,
				Data = dto,
				Url = villaUrl + "/api/villaAPI", // here villaUrl contains the
				Token = token					 // values specified in appSettings.json, what we append is the route
												 // defined in the API Project's Controller route.
			});
		}

		public Task<T> DeleteAsync<T>(int id, string token)
		{
			return SendAsync<T>(new APIRequest()
			{
				ApiType = SD.ApiType.DELETE,
				Url = villaUrl + "/api/villaAPI/" + id, // here we modify the route and append id
				Token = token
			});
		}

		public Task<T> GetAllAsync<T>(string token)
		{
			return SendAsync<T>(new APIRequest()
			{
				ApiType = SD.ApiType.GET,
				Url = villaUrl + "/api/villaAPI",
				Token = token
			});
		}

		public Task<T> GetAsync<T>(int id, string token)
		{
			return SendAsync<T>(new APIRequest()
			{
				ApiType = SD.ApiType.GET,
				Url = villaUrl + "/api/villaAPI/" + id, // here we modify the route and append id
				Token = token
			});
		}

		public Task<T> UpdateAsync<T>(VillaUpdateDTO dto, string token)
		{
			return SendAsync<T>(new APIRequest()
			{
				ApiType = SD.ApiType.PUT,
				Data = dto,
				Url = villaUrl + "/api/villaAPI/" + dto.Id,
				Token = token
			});
		}
	}
}
