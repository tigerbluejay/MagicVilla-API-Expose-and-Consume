using System.ComponentModel.DataAnnotations;

namespace MagicVillaWeb.Models.DTO
{
	public class VillaNumberDTO
	{
		[Required]
		public int VillaNo { get; set; }

		public string SpecialDetails { get; set; }
		[Required]
		public int VillaID { get; set; }
		// Navigation Property
		public VillaDTO Villa { get; set; }

	}
}
