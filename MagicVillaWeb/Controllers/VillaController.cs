using AutoMapper;
using MagicVillaWeb.Models;
using MagicVillaWeb.Models.DTO;
using MagicVillaWeb.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MagicVillaWeb.Controllers
{
	public class VillaController : Controller
	{
		private readonly IVillaService _villaService;
		private readonly IMapper _mapper;

        public VillaController(IVillaService villaService, IMapper mapper)
        {
			_villaService = villaService;
			_mapper = mapper;
            
        }
        public async Task<IActionResult> IndexVilla()
		{
			List<VillaDTO> list = new();

			var response = await _villaService.GetAllAsync<APIResponse>();
			if (response != null && response.IsSuccess)
			{
				list = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result));
			}
			return View(list);
		}

		public async Task<IActionResult> CreateVilla()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateVilla(VillaCreateDTO model)
		{
			if (ModelState.IsValid) // ModelState refers to all Data Annotation validations
			{
				var response = await _villaService.CreateAsync<APIResponse>(model);
				if (response != null && response.IsSuccess)
				{
					TempData["success"] = "Villa created successfully";
					return RedirectToAction(nameof(IndexVilla));
				}
			}
			TempData["error"] = "Error encountered";
			return View(model);
		}

		public async Task<IActionResult> UpdateVilla(int villaId)
		{
			// before we update we need to get the object
			var response = await _villaService.GetAsync<APIResponse>(villaId);
			if (response != null && response.IsSuccess)
			{
				VillaDTO model = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Result));
				return View(_mapper.Map<VillaUpdateDTO>(model)); // we are here mapping VillaDTO to VillaUpdateDTO
				// since we need VillaUpdateDTO in the Post method of the Update Villa
			}
			return NotFound();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UpdateVilla(VillaUpdateDTO model)
		{
			if (ModelState.IsValid) // ModelState refers to all Data Annotation validations
			{
				var response = await _villaService.UpdateAsync<APIResponse>(model);
				if (response != null && response.IsSuccess)
				{
					TempData["success"] = "Villa updated successfully";
					return RedirectToAction(nameof(IndexVilla));
				}
			}
			TempData["error"] = "Error encountered";
			return View(model);
		}

		public async Task<IActionResult> DeleteVilla(int villaId)
		{
			// before we update we need to get the object
			var response = await _villaService.GetAsync<APIResponse>(villaId);
			if (response != null && response.IsSuccess)
			{
				VillaDTO model = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Result));
				return View(model); 
			}
			return NotFound();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteVilla(VillaDTO model)
		{
			var response = await _villaService.DeleteAsync<APIResponse>(model.Id);
			if (response != null && response.IsSuccess)
			{
				TempData["success"] = "Villa deleted successfully";
				return RedirectToAction(nameof(IndexVilla));
			}
			TempData["error"] = "Error encountered";
			return View(model);
		}


	}
}
