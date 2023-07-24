using AutoMapper;
using MagicVillaUtilities;
using MagicVillaWeb.Models;
using MagicVillaWeb.Models.DTO;
using MagicVillaWeb.Models.ViewModels;
using MagicVillaWeb.Services;
using MagicVillaWeb.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Data;

namespace MagicVillaWeb.Controllers
{
	public class VillaNumberController : Controller
	{
		private readonly IVillaNumberService _villaNumberService;
		private readonly IMapper _mapper;
		// we inject the VillaService because we need to retrieve the Villas
		// associated with a given VillaNumber
		private readonly IVillaService _villaService;
		public VillaNumberController(IVillaNumberService villaNumberService, IMapper mapper,
			IVillaService villaService)
		{
			_villaNumberService = villaNumberService;
			_mapper = mapper;
			_villaService = villaService;
		}

		public async Task<IActionResult> IndexVillaNumber()
		{
			List<VillaNumberDTO> list = new();

			var response = await _villaNumberService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
			if (response != null && response.IsSuccess)
			{
				list = JsonConvert.DeserializeObject<List<VillaNumberDTO>>(Convert.ToString(response.Result));
            }
			return View(list);
		}
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateVillaNumber()
		{
			// we create this because we need to populate the dropdown
			// once we call the view
			VillaNumberCreateVM villaNumberVM = new VillaNumberCreateVM();
			var response = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
			if (response != null && response.IsSuccess)
			{
				// here we save the list into the VillaList Property but since we get
				// a VillaDTO boject we need to project it to a SelectListItem object
				// before we pass it to the view.
				villaNumberVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
					(Convert.ToString(response.Result)).Select(i => new SelectListItem
					{
						Text = i.Name,
						Value = i.Id.ToString()
					});
			}
			return View(villaNumberVM);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateVillaNumber(VillaNumberCreateVM model)
		{
			if (ModelState.IsValid)
			{
				var response = await _villaNumberService.CreateAsync<APIResponse>(model.VillaNumber, HttpContext.Session.GetString(SD.SessionToken));
				if (response != null && response.IsSuccess)
				{
					return RedirectToAction(nameof(IndexVillaNumber));
				} 
				// if there are error messages add them to the ModelState
				else if (response.ErrorMessages.Count > 0)
				{
					ModelState.AddModelError("ErrorMessages", response.ErrorMessages.FirstOrDefault());
				}
			}

			var responseWhenModelIsNotValid = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
			if (responseWhenModelIsNotValid != null && responseWhenModelIsNotValid.IsSuccess)
			{
				model.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
					(Convert.ToString(responseWhenModelIsNotValid.Result)).Select(i => new SelectListItem
					{
						Text = i.Name,
						Value = i.Id.ToString()
					});
			}
			return View(model);
		}

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateVillaNumber(int villaNo)
		{
			VillaNumberUpdateVM villaNumberVM = new VillaNumberUpdateVM();
			// before we update we need to get the object
			var response = await _villaNumberService.GetAsync<APIResponse>(villaNo, HttpContext.Session.GetString(SD.SessionToken));
			if (response != null && response.IsSuccess)
			{
				VillaNumberDTO model = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result));
				villaNumberVM.VillaNumber = _mapper.Map<VillaNumberUpdateDTO>(model);
			}

			response = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
			if (response != null && response.IsSuccess)
			{
				villaNumberVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
					(Convert.ToString(response.Result)).Select(i => new SelectListItem
					{
						Text = i.Name,
						Value = i.Id.ToString()
					});
				return View(villaNumberVM);
			}
			return NotFound();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateVillaNumber(VillaNumberUpdateVM model)
		{
			if (ModelState.IsValid)
			{
				var response = await _villaNumberService.UpdateAsync<APIResponse>(model.VillaNumber, HttpContext.Session.GetString(SD.SessionToken));
				if (response != null && response.IsSuccess)
				{
					return RedirectToAction(nameof(IndexVillaNumber));
				}
				// if there are error messages add them to the ModelState
				else if (response.ErrorMessages.Count > 0)
				{
					ModelState.AddModelError("ErrorMessages", response.ErrorMessages.FirstOrDefault());
				}
			}
			// here we are repopulating the dropdown menu if there are errors
			var responseWhenModelIsNotValid = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
			if (responseWhenModelIsNotValid != null && responseWhenModelIsNotValid.IsSuccess)
			{
				model.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
					(Convert.ToString(responseWhenModelIsNotValid.Result)).Select(i => new SelectListItem
					{
						Text = i.Name,
						Value = i.Id.ToString()
					});
			}
			return View(model);
		}

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteVillaNumber(int villaNo)
		{
			VillaNumberDeleteVM villaNumberVM = new();
			// before we update we need to get the object
			var response = await _villaNumberService.GetAsync<APIResponse>(villaNo, HttpContext.Session.GetString(SD.SessionToken));
			if (response != null && response.IsSuccess)
			{
				VillaNumberDTO model = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result));
				villaNumberVM.VillaNumber = model;
			}

			response = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
			if (response != null && response.IsSuccess)
			{
				villaNumberVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
					(Convert.ToString(response.Result)).Select(i => new SelectListItem
					{
						Text = i.Name,
						Value = i.Id.ToString()
					});
				return View(villaNumberVM);
			}
			return NotFound();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteVillaNumber(VillaNumberDeleteVM model)
		{
			var response = await _villaNumberService.DeleteAsync<APIResponse>(model.VillaNumber.VillaNo, HttpContext.Session.GetString(SD.SessionToken));
			if (response != null && response.IsSuccess)
			{
				return RedirectToAction(nameof(IndexVillaNumber));
			}
			return View(model);
		}

	}
}
