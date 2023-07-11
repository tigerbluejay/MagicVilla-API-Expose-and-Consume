using MagicVillaAPI.Data;
using MagicVillaAPI.Model;
using MagicVillaAPI.Model.DTO;
using Microsoft.AspNetCore.Mvc;

namespace MagicVillaAPI.Controllers
{
	[ApiController]
	[Route("api/VillaAPI")]
	public class VillaAPIController : ControllerBase
	{
		[HttpGet]
		public IEnumerable<VillaDTO> GetVillas()
		{
			return VillaStore.villaList;
		}

		[HttpGet("id:int")]
		public VillaDTO GetVilla(int id)
		{
			return VillaStore.villaList.FirstOrDefault(u => u.Id == id);
		}
	}
}
