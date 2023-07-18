﻿using System.ComponentModel.DataAnnotations;

namespace MagicVillaWeb.Models.DTO
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
