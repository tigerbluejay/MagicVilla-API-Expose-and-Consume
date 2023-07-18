using MagicVillaWeb.Models;

namespace MagicVillaWeb.Services.Interfaces
{
	public interface IBaseService
	{
		APIResponse responseModel {  get; set; }
		// this will be used to call our API
		// every time we call the API we will pass an apiRequest and
		// return type will be generic
		Task<T> SendAsync<T>(APIRequest apiRequest);
	}
}
