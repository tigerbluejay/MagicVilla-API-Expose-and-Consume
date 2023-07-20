using MagicVillaAPI.Model;
using MagicVillaAPI.Model.DTO;

namespace MagicVillaAPI.Repository.Interfaces
{
	public interface IUserRepository
	{
		// we need to make sure the user id is unique or not
		bool IsUniqueUser(string username);
		Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
		// we can return the new user that was created in the db.
		// this is the Repository not the actual answer 200 ok we are sending the client
		Task<LocalUser> Register(RegistrationRequestDTO registrationRequestDTO);
	}
}
