using MagicVillaUtilities;
using MagicVillaWeb.Models;
using MagicVillaWeb.Models.DTO;
using MagicVillaWeb.Services.Interfaces;

namespace MagicVillaWeb.Services
{
	// this is the auth service to call register and login in the API
	public class AuthService : BaseService, IAuthService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private string villaUrl;
		public AuthService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
		{
			_httpClientFactory = httpClient;
			this.villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
		}
		public Task<T> LoginAsync<T>(LoginRequestDTO obj)
		{
			return SendAsync<T>(new APIRequest()
			{
				ApiType = SD.ApiType.POST,
				Data = obj,
				Url = villaUrl + "/api/v1/UsersAuth/login" // the magic string portioin of the url
													    // is defined in the
													    // UsersController of the API
														// plus the method name which is "login"
			});
		}

		public Task<T> RegisterAsync<T>(RegistrationRequestDTO obj)
		{
			return SendAsync<T>(new APIRequest()
			{
				ApiType = SD.ApiType.POST,
				Data = obj,
				Url = villaUrl + "/api/v1/UsersAuth/register" // the magic string portioin of the url
                                                             // is defined in the
                                                             // UsersController of the API
                                                             // plus the method name which is "login"
            });
		}
	}
}
