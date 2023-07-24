using MagicVillaWeb.Models.DTO;

namespace MagicVillaWeb.Services.Interfaces
{
	public interface IVillaService
	{
		Task<T> GetAllAsync<T>(string token);
		// we pass on the string token with the token so we are authorized to access
		// the methods with authorize data annotations in the API
		Task<T> GetAsync<T>(int id, string token);
		Task<T> CreateAsync<T>(VillaCreateDTO dto, string token);
		Task<T> UpdateAsync<T>(VillaUpdateDTO dto, string token);
		Task<T> DeleteAsync<T>(int id, string token);

	}
}
