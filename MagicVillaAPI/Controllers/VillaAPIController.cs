using MagicVillaAPI.Customlog;
using MagicVillaAPI.Data;
using MagicVillaAPI.Model;
using MagicVillaAPI.Model.DTO;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVillaAPI.Controllers
{
	[ApiController] // helps with enabling data annotations from model and dto in controllers
	[Route("api/VillaAPI")]
	public class VillaAPIController : ControllerBase
	{
		private readonly ILogger<VillaAPIController> _logger;
		private readonly ILogging _customLogger;
		private readonly ApplicationDbContext _db;

		// use dependency injection to implement logging
        public VillaAPIController(ILogger<VillaAPIController> logger, ILogging customLogger,
			ApplicationDbContext db)
        {
			_logger = logger;
			_customLogger = customLogger;
			_db = db;
        }

		[HttpGet]
		public ActionResult<IEnumerable<VillaDTO>> GetVillas()
		{
			_logger.LogInformation("Getting all Villas");
			_customLogger.Log("Getting all Villas", "");
			return Ok(_db.Villas.ToList());
		}

		[HttpGet("{id:int}", Name = "GetVilla")]
		// 200 is the status code, we can enter it directly as a parameter or use the static details
		// type is the return type (more specific than action result)
		// [ProducesResponseType(200, Type = typeof(VillaDTO)] 
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult<VillaDTO> GetVilla(int id)
		{

			if (id == 0)
			{
				_logger.LogError("Get Villa Error with Id " + id);
				_customLogger.Log("Get Villa Error with Id " + id, "error");
				return BadRequest();
			}

			var villa = _db.Villas.FirstOrDefault(u => u.Id == id);

			if (villa == null)
			{
				return NotFound();
			}

			return Ok(villa);
		}

		[HttpPost]
		//[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult<VillaDTO> CreateVilla([FromBody] VillaDTO villaDTO)
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
			if (_db.Villas.
				FirstOrDefault(u => u.Name.ToLower() == villaDTO.Name.ToLower())
				!= null)
			{
				// villa name is not unique
				ModelState.AddModelError("CustomError", "Villa already Exists");
				return BadRequest(ModelState);
			}

			if (villaDTO == null)
			{
				return BadRequest(villaDTO);
			}
			if (villaDTO.Id > 0)
			{
				// return a status code that does not appear in the Data Annotations
				return StatusCode(StatusCodes.Status500InternalServerError);
			}

			Villa model = new Villa()
			{
				Id = villaDTO.Id,
				Name = villaDTO.Name,
				Details = villaDTO.Details,
				Rate = villaDTO.Rate,
				Sqft = villaDTO.Sqft,
				Occupancy = villaDTO.Occupancy,
				ImageUrl = villaDTO.ImageUrl,
				Amenity = villaDTO.Amenity
			};

			_db.Villas.Add(model);
			_db.SaveChanges();

			//return Ok(villaDTO); // this is code 200 
			return CreatedAtRoute("GetVilla", new { id = villaDTO.Id }, villaDTO); // this is code 201

		}
		[HttpDelete("{id:int}", Name = "DeleteVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		// we use IActionResult so we don't have to specify what we return
		public IActionResult DeleteVilla(int id)
		{
			if (id == 0)
			{
				return BadRequest();
			}
			var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
			if (villa == null)
			{
				return NotFound();
			}
			_db.Villas.Remove(villa);
			_db.SaveChanges();
			return NoContent(); // typically when we delete we don't return anything
		}

		[HttpPut("{id:int}", Name = "UpdateVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public IActionResult UpdateVilla(int id, [FromBody] VillaDTO villaDTO)
		{
			// if we receive no object as parameter
			// or if the id we receive does not match the id of the object we receive
			// - so we should populate id and the object with the same id on the Request
			if (villaDTO == null || id != villaDTO.Id)
			{
				return BadRequest();
			}

			Villa model = new()
			{
				Id = villaDTO.Id,
				Name = villaDTO.Name,
				Details = villaDTO.Details,
				Rate = villaDTO.Rate,
				Sqft = villaDTO.Sqft,
				Occupancy = villaDTO.Occupancy,
				ImageUrl = villaDTO.ImageUrl,
				Amenity = villaDTO.Amenity
			};
			_db.Villas.Update(model);
			_db.SaveChanges();

			return NoContent();
		}


		// jsonpatch.com has the documentation on how to work with the PATCH action method.
		[HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		// we receive a special object JsonPatchDocument of type VillaDTO
		public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDTO)
		{
			if (patchDTO == null || id == 0)
			{
				return BadRequest();
			}

			// we add As No tracking so it won't track this villa
			// we don't want it to track it because it will enter into conflict
			// with the next time we work with an object of the type VillaModel (at the end of this method)
			// when you retrieve a record, EF will track it
			// to avoid that we use the AsNoTracking() method.
			var villa = _db.Villas.AsNoTracking().FirstOrDefault(u => u.Id == id);	

			VillaDTO villaDTO = new()
			{
				Id = villa.Id,
				Name = villa.Name,
				Details = villa.Details,
				Rate = villa.Rate,
				Sqft = villa.Sqft,
				Occupancy = villa.Occupancy,
				ImageUrl = villa.ImageUrl,
				Amenity = villa.Amenity
			};
			
			if (villa == null)
			{
				return BadRequest();
			}
			patchDTO.ApplyTo(villaDTO, ModelState); // ApplyTo is a special method of the JsonPatchDocument object
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}

			Villa model = new()
			{
				Id = villaDTO.Id,
				Name = villaDTO.Name,
				Details = villaDTO.Details,
				Rate = villaDTO.Rate,
				Sqft = villaDTO.Sqft,
				Occupancy = villaDTO.Occupancy,
				ImageUrl = villaDTO.ImageUrl,
				Amenity = villaDTO.Amenity
			};

			_db.Villas.Update(model);
			_db.SaveChanges();

			return NoContent();
		}



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
