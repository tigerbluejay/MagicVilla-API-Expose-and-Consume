using System.ComponentModel.DataAnnotations;

namespace MagicVillaAPI.Model.DTO
{
	public class VillaNumberCreateDTO
	{
		[Required]
		public int VillaNo { get; set; }

		public string SpecialDetails { get; set; }
		[Required]
		public int VillaID { get; set; }
	}
}
