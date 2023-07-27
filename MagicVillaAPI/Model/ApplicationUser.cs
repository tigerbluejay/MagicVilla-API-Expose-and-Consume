using Microsoft.AspNetCore.Identity;

namespace MagicVillaAPI.Model
{
    // this will add custom properties to the identity user used
    // in .NET scaffolded identity
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
