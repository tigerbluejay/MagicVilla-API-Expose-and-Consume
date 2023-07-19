using MagicVillaWeb.Models.DTO;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MagicVillaWeb.Models.ViewModels
{
	public class VillaNumberUpdateVM
	{
		public VillaNumberUpdateDTO VillaNumber { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> VillaList { get; set; }

		public VillaNumberUpdateVM()
        {
            VillaNumber = new VillaNumberUpdateDTO();
        }

        
    }
}
