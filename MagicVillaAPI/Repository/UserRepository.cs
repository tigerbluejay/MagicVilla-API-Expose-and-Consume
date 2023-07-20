using MagicVillaAPI.Data;
using MagicVillaAPI.Model;
using MagicVillaAPI.Model.DTO;
using MagicVillaAPI.Repository.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;

namespace MagicVillaAPI.Repository
{
	public class UserRepository : IUserRepository
	{
		private readonly ApplicationDbContext _db;
		private string secretKey;
		public UserRepository(ApplicationDbContext db, IConfiguration configuration)
		{
			_db = db;
			secretKey = configuration.GetValue<string>("ApiSettings:Secret");

		}
		public bool IsUniqueUser(string username)
		{
			var user = _db.LocalUsers.FirstOrDefault(x => x.UserName == username);
			if (user == null)
			{
				return true; // it is a unique user, since it doesnt exist in the db
			}
			return false;
			
		}

		public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
		{
			// make sure username and password are legit
			var user = _db.LocalUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower()
			&& u.Password == loginRequestDTO.Password); // check if there are existing users

			if (user == null)
			{
				// either password or username is invalid
				// return empty response
				return new LoginResponseDTO()
				{
					Token = "",
					User = null
				};
				
			}

			// if the user is found we generate a JWT Token
			// We need for that a SecretKey to Encrypt our Token - it will be used to see if Token
			// is valid - only the application knows the secret key in appsettings.json

			// Step 1. Generate the JWT Token or Security Token
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(secretKey); // we convert the secret key to bytes
														  // token descriptor contains the claims in a token, the claim identifies role of user, name of the user, etc..
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				// we can pass multiple claims, if you have multiple roles for a user, you can
				// pass multiple claims
				Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim(ClaimTypes.Name, user.Id.ToString()),
					new Claim(ClaimTypes.Role, user.Role)

				}),
				Expires = DateTime.UtcNow.AddDays(7),
				// the SigningCredentials Contains the transformed key (in bytes) using
				// a security Algorithm
				SigningCredentials = new(new SymmetricSecurityKey(key),
				SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor); // generate the token

			LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
			{
				Token = tokenHandler.WriteToken(token), // this will serialize the token
				User = user
			};

			return loginResponseDTO;
		}

		public async Task<LocalUser> Register(RegistrationRequestDTO registrationRequestDTO)
		{
			// here we map them manually, but we could also have used AutoMapper
			// to map automatically (remember automapper configurations are saved in
			// MappingConfig.cs

			LocalUser user = new LocalUser()
			{
				UserName = registrationRequestDTO.UserName,
				Name = registrationRequestDTO.Name,
				Password = registrationRequestDTO.Password,
				Role = registrationRequestDTO.Role
			};

			// Add the new user to the database
			_db.LocalUsers.Add(user);
			_db.SaveChangesAsync();
			// empty the password so it won't appear on the response
			user.Password = "";
			return user;
		}
	}
}
