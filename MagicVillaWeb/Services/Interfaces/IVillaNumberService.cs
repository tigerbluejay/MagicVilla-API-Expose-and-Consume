using MagicVillaWeb.Models.DTO;

namespace MagicVillaWeb.Services.Interfaces
{
	public interface IVillaNumberService
	{
		Task<T> GetAllAsync<T>();
		Task<T> GetAsync<T>(int id);
		Task<T> CreateAsync<T>(VillaNumberCreateDTO dto);
		Task<T> UpdateAsync<T>(VillaNumberUpdateDTO dto);
		Task<T> DeleteAsync<T>(int id);
		
	}
}