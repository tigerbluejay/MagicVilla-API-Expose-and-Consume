using System.ComponentModel.DataAnnotations;

namespace MagicVillaWeb.Models.DTO
{
	// In .NET 6 the form from the view you display thinks all the fields are required
	// by default, even if we don't specify it in data annotations.
	// for this reason we need to add the questionmark to make the Properties nullable
	public class VillaCreateDTO
	{
		[Required]
		[MaxLength(30)]
		public string Name { get; set; }
		public string? Details { get; set; }
		[Required]
		public double Rate { get; set; }
		public int Occupancy { get; set; }
		public int Sqft { get; set; }
		public string? ImageUrl { get; set; }
		public string? Amenity { get; set; }
	}
}
