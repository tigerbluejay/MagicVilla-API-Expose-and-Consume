namespace MagicVillaWeb.Models.DTO
{
	public class UserDTO
	{
        //public int Id { get; set; } // Id was implemented as an int when we were using the custom login
        public string Id { get; set; } // now that we are using .NET identity, the Id is stored as a GUID
		// so we require that it be a string here
        public string UserName { get; set; }
		public string Name { get; set; }
		public string Password { get; set; }
		//public string Role { get; set; }
	}
}
