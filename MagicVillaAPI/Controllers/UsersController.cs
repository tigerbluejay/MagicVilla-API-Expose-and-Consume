using MagicVillaAPI.Model;
using MagicVillaAPI.Model.DTO;
using MagicVillaAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVillaAPI.Controllers
{
	[Route("api/UsersAuth")]
	[ApiController]
	public class UsersController : Controller
	{
		private readonly IUserRepository _userRepository;
		protected APIResponse _response;
		public UsersController(IUserRepository userRepository)
		{
			_userRepository = userRepository;
			this._response = new();
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
		{
			var loginResponse = await _userRepository.Login(model);
			if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
			{
				// the username or password is incorrect
				_response.StatusCode = HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
				_response.ErrorMessages.Add("Username or password is incorrect");
				return BadRequest(_response);
			}
			// username can login
			_response.StatusCode = HttpStatusCode.OK;
			_response.IsSuccess = true;
			_response.Result = loginResponse;
			return Ok(_response);
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO model)
		{
			// check if user is Unique User
			bool ifUserNameUnique = _userRepository.IsUniqueUser(model.UserName);
			if (!ifUserNameUnique)
			{
				// user exists
				_response.StatusCode = HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
				_response.ErrorMessages.Add("Username already exists");
				return BadRequest(_response);
			}
			// user is unique proceed with registration
			var user = await _userRepository.Register(model);
			// if Repository register method returns null for some reason:
			if (user == null) 
			{
				_response.StatusCode = HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
				_response.ErrorMessages.Add("Error while Registering");
				return BadRequest(_response);
			}
			// user registered successfully:
			_response.StatusCode = HttpStatusCode.OK;
			_response.IsSuccess = true;
			return Ok(_response);
		}

	}
}
