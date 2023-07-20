using AutoMapper;
using Azure;
using MagicVillaAPI.Customlog;
using MagicVillaAPI.Data;
using MagicVillaAPI.Model;
using MagicVillaAPI.Model.DTO;
using MagicVillaAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MagicVillaAPI.Controllers
{
	[ApiController] // helps with enabling data annotations from model and dto in controllers
	[Route("api/VillaAPI")]
	public class VillaAPIController : ControllerBase
	{
		private readonly ILogger<VillaAPIController> _logger;
		private readonly ILogging _customLogger;
		private readonly IVillaRepository _dbVilla;
		private readonly IMapper _mapper;
		protected APIResponse _response;

		// use dependency injection to implement logging
		public VillaAPIController(ILogger<VillaAPIController> logger, ILogging customLogger,
			ApplicationDbContext db, IMapper mapper, IVillaRepository dbVilla)
		{
			_logger = logger;
			_customLogger = customLogger;
			_dbVilla = dbVilla;
			_mapper = mapper;
			this._response = new APIResponse();
		}

		[HttpGet]
		[Authorize] // only an authorized user can access this endpoint 
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<ActionResult<APIResponse>> GetVillas()
		{
			_logger.LogInformation("Getting all Villas");
			_customLogger.Log("Getting all Villas", "");


			try
			{
				//	return Ok(await _db.Villas.ToListAsync());

				// this used to be the implementation without the repository pattern
				//IEnumerable<Villa> villaList = await _db.Villas.ToListAsync();
				IEnumerable<Villa> villaList = await _dbVilla.GetAllAsync();
				_response.Result = _mapper.Map<List<VillaDTO>>(villaList);
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

		[HttpGet("{id:int}", Name = "GetVilla")]
		// 200 is the status code, we can enter it directly as a parameter or use the static details
		// type is the return type (more specific than action result)
		// [ProducesResponseType(200, Type = typeof(VillaDTO)] 
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[Authorize(Roles = "admin")] // only authorized users with the Role of admin can access this endpoint.
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<ActionResult<APIResponse>> GetVilla(int id)
		{
			try
			{
				if (id == 0)
				{
					_logger.LogError("Get Villa Error with Id " + id);
					_customLogger.Log("Get Villa Error with Id " + id, "error");

					_response.StatusCode = HttpStatusCode.BadRequest;
					return BadRequest(_response);
				}

				var villa = await _dbVilla.GetAsync(u => u.Id == id);

				if (villa == null)
				{
					_response.StatusCode = HttpStatusCode.NotFound;
					return NotFound(_response);
				}

				_response.Result = _mapper.Map<VillaDTO>(villa);
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
		//[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDTO villaCreateDTO)
		{
			try
			{

				// if we choose not to use the APIController attribute to enable Data Annotations
				// you can check the ModelState
				// if(!ModelState.IsValid)
				// {
				//	 return BadRequest();
				// }

				// next condition retrieves the first name in data layer that matches the name
				// passed as parameter in the api call and checks if it exists
				// if it is not null it means it exists
				// in other words, if the value we pass through in the api call already exists
				// in data layer/data base, if it exists (if it is not null),
				// the the villa name is not unique
				if (await _dbVilla.GetAsync
					(u => u.Name.ToLower() == villaCreateDTO.Name.ToLower())
					!= null)
				{
					// villa name is not unique
					ModelState.AddModelError("ErrorMessages", "Villa already Exists");
					return BadRequest(ModelState);
				}

				if (villaCreateDTO == null)
				{
					return BadRequest(villaCreateDTO);
				}
				//if (villaDTO.Id > 0)
				//{
				//	// return a status code that does not appear in the Data Annotations
				//	return StatusCode(StatusCodes.Status500InternalServerError);
				//}


				// we stop converting manually and we use automapper
				//Villa model = new Villa()
				//{
				//	//Id = villaCreateDTO.Id,
				//	Name = villaCreateDTO.Name,
				//	Details = villaCreateDTO.Details,
				//	Rate = villaCreateDTO.Rate,
				//	Sqft = villaCreateDTO.Sqft,
				//	Occupancy = villaCreateDTO.Occupancy,
				//	ImageUrl = villaCreateDTO.ImageUrl,
				//	Amenity = villaCreateDTO.Amenity
				//};
				Villa villa = _mapper.Map<Villa>(villaCreateDTO);


				await _dbVilla.CreateAsync(villa);

				_response.Result = _mapper.Map<VillaDTO>(villa);
				_response.StatusCode = HttpStatusCode.Created;

				//return Ok(villaDTO); // this is code 200 
				return CreatedAtRoute("GetVilla", new { id = villa.Id }, _response); // this is code 201

			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string> { ex.ToString() };
			}
			return _response;
		}

		[HttpDelete("{id:int}", Name = "DeleteVilla")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[Authorize(Roles = "custom")] // only authorized users with the Role of custom can access this endpoint.
									  // if a user with the role of admin tries to access this endpoint they will
									  // be blocked and met with 403 Forbidden
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		// we use IActionResult so we don't have to specify what we return
		public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
		{
			try
			{
				if (id == 0)
				{
					return BadRequest();
				}
				var villa = await _dbVilla.GetAsync(u => u.Id == id);
				if (villa == null)
				{
					return NotFound();
				}
				await _dbVilla.RemoveAsync(villa);
				_response.StatusCode = HttpStatusCode.NoContent;
				_response.IsSuccess = true;
				return Ok(_response);
				//return NoContent(); // typically when we delete we don't return anything

			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string> { ex.ToString() };
			}
			return _response;

		}

		[HttpPut("{id:int}", Name = "UpdateVilla")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDTO villaUpdateDTO)
		{
			try
			{


				// if we receive no object as parameter
				// or if the id we receive does not match the id of the object we receive
				// - so we should populate id and the object with the same id on the Request
				if (villaUpdateDTO == null || id != villaUpdateDTO.Id)
				{
					return BadRequest();
				}

				//Villa model = new()
				//{
				//	Id = villaUpdateDTO.Id,
				//	Name = villaUpdateDTO.Name,
				//	Details = villaUpdateDTO.Details,
				//	Rate = villaUpdateDTO.Rate,
				//	Sqft = villaUpdateDTO.Sqft,
				//	Occupancy = villaUpdateDTO.Occupancy,
				//	ImageUrl = villaUpdateDTO.ImageUrl,
				//	Amenity = villaUpdateDTO.Amenity
				//};
				Villa model = _mapper.Map<Villa>(villaUpdateDTO);

				await _dbVilla.UpdateAsync(model);

				_response.StatusCode = HttpStatusCode.NoContent;
				_response.IsSuccess = true;
				return Ok(_response);

				//return NoContent();
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string> { ex.ToString() };
			}
			return _response;

		}

		/***************** PATCH FOR INSTRUCTIONAL PURPOSES ******************
		 ***************** IT IS NOT UP TO DATE, JUST DEMONSTRATION  *********
		 **************************************/


		//// jsonpatch.com has the documentation on how to work with the PATCH action method.
		//[HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
		//[ProducesResponseType(StatusCodes.Status204NoContent)]
		//[ProducesResponseType(StatusCodes.Status400BadRequest)]
		//// we receive a special object JsonPatchDocument of type VillaDTO
		//public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
		//{
		//	if (patchDTO == null || id == 0)
		//	{
		//		return BadRequest();
		//	}

		//	// we add As No tracking so it won't track this villa
		//	// we don't want it to track it because it will enter into conflict
		//	// with the next time we work with an object of the type VillaModel (at the end of this method)
		//	// when you retrieve a record, EF will track it
		//	// to avoid that we use the AsNoTracking() method.
		//	var villa = await _dbVilla.GetAsync(u => u.Id == id, tracked: false);


		//	//VillaUpdateDTO villaUpdateDTO = new()
		//	//{
		//	//	Id = villa.Id,
		//	//	Name = villa.Name,
		//	//	Details = villa.Details,
		//	//	Rate = villa.Rate,
		//	//	Sqft = villa.Sqft,
		//	//	Occupancy = villa.Occupancy,
		//	//	ImageUrl = villa.ImageUrl,
		//	//	Amenity = villa.Amenity
		//	//};
		//	VillaUpdateDTO villaUpdateDTO = _mapper.Map<VillaUpdateDTO>(villa);


		//	if (villa == null)
		//	{
		//		return BadRequest();
		//	}

		//	patchDTO.ApplyTo(villaUpdateDTO, ModelState); // ApplyTo is a special method of the JsonPatchDocument object

		//	if (!ModelState.IsValid)
		//	{
		//		return BadRequest();
		//	}


		//	//Villa model = new()
		//	//{
		//	//	Id = villaUpdateDTO.Id,
		//	//	Name = villaUpdateDTO.Name,
		//	//	Details = villaUpdateDTO.Details,
		//	//	Rate = villaUpdateDTO.Rate,
		//	//	Sqft = villaUpdateDTO.Sqft,
		//	//	Occupancy = villaUpdateDTO.Occupancy,
		//	//	ImageUrl = villaUpdateDTO.ImageUrl,
		//	//	Amenity = villaUpdateDTO.Amenity
		//	//};
		//	Villa model = _mapper.Map<Villa>(villaUpdateDTO);

		//	await _dbVilla.UpdateAsync(model);

		//	return NoContent();
		//}



		/***************** PREVIOUS IMPLEMENTATION USING VILLA STORE DATA CLASS ******************
		 ***************** INSTEAD OF THE DATABASE **********************************************/
		//      [HttpGet]
		//public ActionResult<IEnumerable<VillaDTO>> GetVillas()
		//{
		//	_logger.LogInformation("Getting all Villas");
		//	_customLogger.Log("Getting all Villas", "");
		//	return Ok(VillaStore.villaList);
		//}

		//[HttpGet("{id:int}", Name ="GetVilla")]
		//// 200 is the status code, we can enter it directly as a parameter or use the static details
		//// type is the return type (more specific than action result)
		//// [ProducesResponseType(200, Type = typeof(VillaDTO)] 
		//[ProducesResponseType(StatusCodes.Status200OK)]
		//[ProducesResponseType(StatusCodes.Status400BadRequest)]
		//[ProducesResponseType(StatusCodes.Status404NotFound)]
		//public ActionResult<VillaDTO> GetVilla(int id)
		//{

		//	if (id == 0)
		//	{
		//		_logger.LogError("Get Villa Error with Id " + id);
		//		_customLogger.Log("Get Villa Error with Id " + id, "error");
		//		return BadRequest();
		//	}

		//	var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);

		//	if (villa == null)
		//	{
		//		return NotFound();
		//	}

		//	return Ok(villa);
		//}

		//[HttpPost]
		////[ProducesResponseType(StatusCodes.Status200OK)]
		//[ProducesResponseType(StatusCodes.Status201Created)]
		//[ProducesResponseType(StatusCodes.Status400BadRequest)]
		//[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		//public ActionResult<VillaDTO> CreateVilla([FromBody]VillaDTO villaDTO)
		//{
		//	// if we choose not to use the APIController attribute to enable Data Annotations
		//	// you can check the ModelState
		//	// if(!ModelState.IsValid)
		//	// {
		//	//	 return BadRequest();
		//	// }

		//	// next condition retrieves the first name in data layer that matches the name
		//	// passed as parameter in the api call and checks if it exists
		//	// if it is not null it means it exists
		//	// in other words, if the value we pass through in the api call already exists
		//	// in data layer/data base, if it exists (if it is not null),
		//	// the the villa name is not unique
		//	if (VillaStore.villaList.
		//		FirstOrDefault(u => u.Name.ToLower() == villaDTO.Name.ToLower()) 
		//		!= null)
		//	{
		//		// villa name is not unique
		//		ModelState.AddModelError("CustomError", "Villa already Exists");
		//		return BadRequest(ModelState);
		//	}

		//	if (villaDTO == null)
		//	{
		//		return BadRequest(villaDTO);
		//	}
		//	if (villaDTO.Id > 0)
		//	{
		//		// return a status code that does not appear in the Data Annotations
		//		return StatusCode(StatusCodes.Status500InternalServerError);
		//	}
		//	villaDTO.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
		//	VillaStore.villaList.Add(villaDTO);

		//	//return Ok(villaDTO); // this is code 200 
		//	return CreatedAtRoute("GetVilla", new { id = villaDTO.Id }, villaDTO); // this is code 201

		//}
		//[HttpDelete("{id:int}", Name = "DeleteVilla")]
		//[ProducesResponseType(StatusCodes.Status204NoContent)]
		//[ProducesResponseType(StatusCodes.Status400BadRequest)]
		//[ProducesResponseType(StatusCodes.Status404NotFound)]
		//// we use IActionResult so we don't have to specify what we return
		//public IActionResult DeleteVilla(int id)
		//{
		//	if (id == 0)
		//	{
		//		return BadRequest();
		//	}
		//	var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
		//	if (villa == null)
		//	{
		//		return NotFound();
		//	}
		//	VillaStore.villaList.Remove(villa);
		//	return NoContent(); // typically when we delete we don't return anything
		//}

		//[HttpPut("{id:int}", Name = "UpdateVilla")]
		//[ProducesResponseType(StatusCodes.Status204NoContent)]
		//[ProducesResponseType(StatusCodes.Status400BadRequest)]
		//public IActionResult UpdateVilla(int id, [FromBody]VillaDTO villaDTO)
		//{
		//	// if we receive no object as parameter
		//	// or if the id we receive does not match the id of the object we receive
		//	// - so we should populate id and the object with the same id on the Request
		//	if (villaDTO == null || id != villaDTO.Id)
		//	{
		//		return BadRequest();
		//	}
		//	var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
		//	villa.Name = villaDTO.Name;
		//	villa.Occupancy = villaDTO.Occupancy;
		//	villa.Sqft = villaDTO.Sqft;

		//	return NoContent();
		//}


		//// jsonpatch.com has the documentation on how to work with the PATCH action method.
		//[HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
		//[ProducesResponseType(StatusCodes.Status204NoContent)]
		//[ProducesResponseType(StatusCodes.Status400BadRequest)]
		//// we receive a special object JsonPatchDocument of type VillaDTO
		//public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDTO)
		//{
		//	if (patchDTO == null || id == 0)
		//	{
		//		return BadRequest();
		//	}

		//	var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
		//	if (villa == null)
		//	{
		//		return BadRequest();
		//	}
		//	patchDTO.ApplyTo(villa, ModelState); // ApplyTo is a special method of the JsonPatchDocument object
		//	if (!ModelState.IsValid)
		//	{
		//		return BadRequest();
		//	}
		//	return NoContent();
		//}
	}
}
