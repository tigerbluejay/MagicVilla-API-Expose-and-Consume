using MagicVillaAPI.Data;
using MagicVillaAPI.Model;
using MagicVillaAPI.Model.DTO;
using MagicVillaAPI.Repository.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using AutoMapper;

namespace MagicVillaAPI.Repository
{
	// The User Repository class was first designed to work with the LocalUser in the custom implementation
	// of identity. The new and last implementation refers to the implementation for the default
	// .NET identity.
	public class UserRepository : IUserRepository
	{
		private readonly ApplicationDbContext _db;
		private string secretKey;
		private readonly UserManager<ApplicationUser> _userManager; // this is helpful it has built in
																	// helper functions to work with identity with the default scaffolded .NET identity
		private readonly IMapper _mapper; // this is also used in this place for .NET identity
		private readonly RoleManager<IdentityRole> _roleManager;
		public UserRepository(ApplicationDbContext db, IConfiguration configuration, 
			UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager)
		{
			_db = db;
			secretKey = configuration.GetValue<string>("ApiSettings:Secret");
			_userManager = userManager;
			_mapper = mapper;
			_roleManager = roleManager;

		}
		// this was for our custom implementation of identity
		//public bool IsUniqueUser(string username)
		//{
		//	var user = _db.LocalUsers.FirstOrDefault(x => x.UserName == username);
		//	if (user == null)
		//	{
		//		return true; // it is a unique user, since it doesnt exist in the db
		//	}
		//	return false;			
		//}

		// this is for the default scaffolded .NET identity
        public bool IsUniqueUser(string username)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(x => x.UserName == username);
            if (user == null)
            {
                return true; // it is a unique user, since it doesnt exist in the db
            }
            return false;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
		{
			// make sure username and password are legit
			//var user = _db.LocalUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower()
			//&& u.Password == loginRequestDTO.Password); // check if there are existing users
			var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower());

			bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

			if (user == null || isValid == false)
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
			// retrieve roles
			var roles = await _userManager.GetRolesAsync(user);
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(secretKey); // we convert the secret key to bytes
														  // token descriptor contains the claims in a token, the claim identifies role of user, name of the user, etc..
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				// we can pass multiple claims, if you have multiple roles for a user, you can
				// pass multiple claims
				Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim(ClaimTypes.Name, user.UserName.ToString()),
					new Claim(ClaimTypes.Role, roles.FirstOrDefault())

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
				User = _mapper.Map<UserDTO>(user),
				// Role = roles.FirstOrDefault() the Token will be using the Claim and that will have the Role
			};

			return loginResponseDTO;
        }

		//public async Task<LocalUser> Register(RegistrationRequestDTO registrationRequestDTO)
		public async Task<UserDTO> Register(RegistrationRequestDTO registrationRequestDTO)
		{
            // here we map them manually, but we could also have used AutoMapper
            // to map automatically (remember automapper configurations are saved in
            // MappingConfig.cs

            //LocalUser user = new LocalUser()
            //{
            //	UserName = registrationRequestDTO.UserName,
            //	Name = registrationRequestDTO.Name,
            //	Password = registrationRequestDTO.Password,
            //	Role = registrationRequestDTO.Role
            //};

            //// Add the new user to the database
            //_db.LocalUsers.Add(user);
            //_db.SaveChangesAsync();
            //// empty the password so it won't appear on the response
            //user.Password = "";
            //return user;

            ApplicationUser user = new()
			{
				UserName = registrationRequestDTO.UserName,
				Email = registrationRequestDTO.UserName,
				NormalizedEmail = registrationRequestDTO.UserName.ToUpper(),
                Name = registrationRequestDTO.Name
			};

			try
			{
				var result = await _userManager.CreateAsync(user, registrationRequestDTO.Password);
                if (result.Succeeded) // if it succeeded we want to add a role
                {
					if (!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult()) 
						// if the role of admin does not exist, it will create admin and customer
						// roles, but only the first time. The next time the role of admin will exist.
						// but the normal way to implement this is not this, it is by seeding the db
						// with the user roles. here we did this quick fix to create the roles in the db.
					{
						await _roleManager.CreateAsync(new IdentityRole("admin"));
                        await _roleManager.CreateAsync(new IdentityRole("customer"));
                    }
                    await _userManager.AddToRoleAsync(user, "admin");
					var userToReturn = _db.ApplicationUsers
						.FirstOrDefault(u => u.UserName == registrationRequestDTO.UserName);
					return _mapper.Map<UserDTO>(userToReturn);
                }

            } catch(Exception ex)
			{

			}

			return new UserDTO();
		}
	}
}
