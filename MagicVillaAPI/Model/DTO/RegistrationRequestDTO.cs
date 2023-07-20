using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MagicVillaAPI.Model.DTO
{
	// We have a Registration Request DTO but not a Registration Response DTO
	// because we just send them a 200 Ok Response, letting them know registration
	// was successful
	public class RegistrationRequestDTO
	{
		public string UserName { get; set; }
		public string Name { get; set; }
		public string Password { get; set; }
		public string Role { get; set; }
	}
}
