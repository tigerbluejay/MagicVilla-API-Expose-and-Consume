namespace MagicVillaAPI.Model.DTO
{
    // this version is for the .NET scaffolded identity default implementation
    public class LoginResponseDTO
    {
        // User will have all the details of the logged in User
        public UserDTO User { get; set; }
        // The Token is a string that authenticates the user
        // that is, it proves that the user is who they say they are
        public string Token { get; set; }
        //public string Role { get; set; } // we store the role here. actually this was a mistake
        // the role is stored in the token - encrypted
    }


    //public class LoginResponseDTO
    //{
    //	// User will have all the details of the logged in User
    //	public LocalUser User { get; set; }
    //	// The Token is a string that authenticates the user
    //	// that is, it proves that the user is who they say they are
    //	public string Token { get; set; }
    //}
}
