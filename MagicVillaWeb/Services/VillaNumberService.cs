﻿using MagicVillaUtilities;
using MagicVillaWeb.Models;
using MagicVillaWeb.Models.DTO;
using MagicVillaWeb.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MagicVillaWeb.Services
{
	public class VillaNumberService : BaseService, IVillaNumberService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private string villaUrl;
		public VillaNumberService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
		{
			_httpClientFactory = httpClient;
			this.villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
		}

		public Task<T> CreateAsync<T>(VillaNumberCreateDTO dto)
		{
			return SendAsync<T>(new APIRequest()
			{
				ApiType = SD.ApiType.POST,
				Data = dto,
				Url = villaUrl + "/api/villaNumberAPI" // the magic string portioin of the url
													   // is defined in the
													   // VillaNumberController of the API
			});
		}

		public Task<T> DeleteAsync<T>(int id)
		{
			return SendAsync<T>(new APIRequest()
			{
				ApiType = SD.ApiType.DELETE,
				Url = villaUrl + "/api/villaNumberAPI/" + id // here we modify the route and append id
			});
		}

		public Task<T> GetAllAsync<T>()
		{
			return SendAsync<T>(new APIRequest()
			{
				ApiType = SD.ApiType.GET,
				Url = villaUrl + "/api/villaNumberAPI"
			});
		}

		public Task<T> GetAsync<T>(int id)
		{
			return SendAsync<T>(new APIRequest()
			{
				ApiType = SD.ApiType.GET,
				Url = villaUrl + "/api/villaNumberAPI/" + id // here we modify the route and append id
			});
		}

		public Task<T> UpdateAsync<T>(VillaNumberUpdateDTO dto)
		{
			return SendAsync<T>(new APIRequest()
			{
				ApiType = SD.ApiType.PUT,
				Data = dto,
				Url = villaUrl + "/api/villaNumberAPI/" + dto.VillaNo
			});
		}
	}
}
