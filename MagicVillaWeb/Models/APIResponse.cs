using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVillaWeb.Models
{
	// we create this class so that all of our methods can return the
	// standart API response, instead of having all different return types like the
	// previous implementation:
	//public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
	//public async Task<ActionResult<VillaDTO>> GetVilla(int id)
	//public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody] VillaCreateDTO villaCreateDTO)
	//public async Task<IActionResult> DeleteVilla(int id)
	//public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDTO villaUpdateDTO)
	//public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)

	public class APIResponse
	{
		public HttpStatusCode StatusCode { get; set; }
		public bool IsSuccess { get; set; } = true;
		public List<string> ErrorMessages { get; set; }
		public object Result { get; set; }	

	}
}
