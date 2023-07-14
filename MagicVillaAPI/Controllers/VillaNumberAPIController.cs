using AutoMapper;
using Azure;
using MagicVillaAPI.Customlog;
using MagicVillaAPI.Data;
using MagicVillaAPI.Model;
using MagicVillaAPI.Model.DTO;
using MagicVillaAPI.Repository.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MagicVillaAPI.Controllers
{
	[ApiController] // helps with enabling data annotations from model and dto in controllers
	[Route("api/VillaNumberAPI")]
	public class VillaNumberAPIController : ControllerBase
	{
		private readonly IVillaNumberRepository _dbVillaNumber;
		private readonly IMapper _mapper;
		protected APIResponse _response;
		// we inject the VillaRepository into this controller
		private readonly IVillaRepository _dbVilla;

		// use dependency injection to implement logging
		public VillaNumberAPIController(ApplicationDbContext db, IMapper mapper,
			IVillaNumberRepository dbVillaNumber, IVillaRepository dbVilla)
		{
			_dbVillaNumber = dbVillaNumber;
			_mapper = mapper;
			this._response = new APIResponse();
			_dbVilla = dbVilla;
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<APIResponse>> GetVillas()
		{
			try
			{
				IEnumerable<VillaNumber> villaNumberList = await _dbVillaNumber.GetAllAsync();
				_response.Result = _mapper.Map<List<VillaNumberDTO>>(villaNumberList);
				_response.StatusCode = HttpStatusCode.OK;
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string> { ex.ToString() };
			}

			return _response;
		}

		[HttpGet("{id:int}", Name = "GetVillaNumber")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
		{
			try
			{
				if (id == 0)
				{
					_response.StatusCode = HttpStatusCode.BadRequest;
					return BadRequest(_response);
				}

				var villaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNo == id);

				if (villaNumber == null)
				{
					_response.StatusCode = HttpStatusCode.NotFound;
					return NotFound(_response);
				}

				_response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
				_response.StatusCode = HttpStatusCode.OK;
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string> { ex.ToString() };
			}
			return _response;

		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDTO villaNumberCreateDTO)
		{
			try
			{
				if (await _dbVillaNumber.GetAsync
					(u => u.VillaNo == villaNumberCreateDTO.VillaNo)
					!= null)
				{
					ModelState.AddModelError("CustomError", "Villa Number already Exists");
					return BadRequest(ModelState);
				}

				// we injected the VillaRepository for this:
				// we need to check if the VillaId,
				// which is passed in the Request is valid or not
				if (await _dbVilla.GetAsync(u => u.Id == villaNumberCreateDTO.VillaID) == null)
				{
					ModelState.AddModelError("CustomError", "Villa ID is Invalid!");
					return BadRequest(ModelState);
				}

				if (villaNumberCreateDTO == null)
				{
					return BadRequest(villaNumberCreateDTO);
				}

				VillaNumber villaNumber = _mapper.Map<VillaNumber>(villaNumberCreateDTO);


				await _dbVillaNumber.CreateAsync(villaNumber);

				_response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
				_response.StatusCode = HttpStatusCode.Created;

				return CreatedAtRoute("GetVilla", new { id = villaNumber.VillaNo }, _response);

			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string> { ex.ToString() };
			}
			return _response;
		}

		[HttpDelete("{id:int}", Name = "DeleteVillaNumber")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
		{
			try
			{
				if (id == 0)
				{
					return BadRequest();
				}
				var villaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNo == id);
				if (villaNumber == null)
				{
					return NotFound();
				}
				await _dbVillaNumber.RemoveAsync(villaNumber);
				_response.StatusCode = HttpStatusCode.NoContent;
				_response.IsSuccess = true;
				return Ok(_response);

			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string> { ex.ToString() };
			}
			return _response;

		}

		[HttpPut("{id:int}", Name = "UpdateVillaNumber")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id, [FromBody] VillaNumberUpdateDTO villaNumberUpdateDTO)
		{
			try
			{

				if (villaNumberUpdateDTO == null || id != villaNumberUpdateDTO.VillaNo)
				{
					return BadRequest();
				}

				// we injected the VillaRepository for this:
				// we need to check if the VillaId,
				// which is passed in the Request is valid or not
				if (await _dbVilla.GetAsync(u => u.Id == villaNumberUpdateDTO.VillaID) == null)
				{
					ModelState.AddModelError("CustomError", "Villa ID is Invalid!");
					return BadRequest(ModelState);
				}


				VillaNumber model = _mapper.Map<VillaNumber>(villaNumberUpdateDTO);

				await _dbVillaNumber.UpdateAsync(model);

				_response.StatusCode = HttpStatusCode.NoContent;
				_response.IsSuccess = true;
				return Ok(_response);

			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string> { ex.ToString() };
			}
			return _response;

		}

	}
}
