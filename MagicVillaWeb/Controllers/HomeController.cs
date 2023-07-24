using AutoMapper;
using MagicVillaUtilities;
using MagicVillaWeb.Models;
using MagicVillaWeb.Models.DTO;
using MagicVillaWeb.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace MagicVillaWeb.Controllers
{
	public class HomeController : Controller
	{
		private readonly IVillaService _villaService;
		private readonly IMapper _mapper;

		public HomeController(IVillaService villaService, IMapper mapper)
		{
			_villaService = villaService;
			_mapper = mapper;

		}
		public async Task<IActionResult> Index()
		{
			List<VillaDTO> list = new();

			var response = await _villaService.GetAllAsync<APIResponse>
				(HttpContext.Session.GetString(SD.SessionToken));
			if (response != null && response.IsSuccess)
			{
				list = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result));
			}
			return View(list);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}