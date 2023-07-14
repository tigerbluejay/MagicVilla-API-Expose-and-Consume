using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicVillaAPI.Model
{
	public class VillaNumber
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		// it will not be an identity column because
		// numbers can be 101, 102, 201 and so on
		public int VillaNo { get; set; }
		// here this property Special Details is intended
		// to provide specfic information about a VillaNo Villa
		public string SpecialDetails { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime UpdateDate { get; set; }
		

		[ForeignKey("Villa")] // this will add Foreign Key To Villa Numbers Table
		public int VillaID { get; set; } // this references the Id column in the Villa Table
		// and it will be the foreign key
		// Navigation Property:
		public Villa Villa { get; set; }

	}
}
