using MagicVillaWeb.Models.DTO;

namespace MagicVillaWeb.Services.Interfaces
{
	public interface IAuthService
	{
		Task<T> LoginAsync<T>(LoginRequestDTO obj);
		Task<T> RegisterAsync<T>(RegistrationRequestDTO obj);
	}
}
