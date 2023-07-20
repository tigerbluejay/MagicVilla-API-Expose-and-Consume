namespace MagicVillaAPI.Model.DTO
{
	public class LoginResponseDTO
	{
		// User will have all the details of the logged in User
		public LocalUser User { get; set; }
		// The Token is a string that authenticates the user
		// that is, it proves that the user is who they say they are
		public string Token { get; set; }
	}
}
