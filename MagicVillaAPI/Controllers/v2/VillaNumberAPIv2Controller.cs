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
using System.Data;
using System.Net;

namespace MagicVillaAPI.Controllers.v2
{
    [ApiController] // helps with enabling data annotations from model and dto in controllers
                    //[Route("api/VillaNumberAPI")]
    [Route("api/v{version:apiVersion}/VillaNumberAPI")] // route support for versions
                                                        //[ApiVersion("1.0")] // we typically have one
                                                        // controller per version so we deactivate this
    [ApiVersion("2.0")]
    public class VillaNumberAPIv2Controller : ControllerBase
    {
        private readonly IVillaNumberRepository _dbVillaNumber;
        private readonly IMapper _mapper;
        protected APIResponse _response;
        // we inject the VillaRepository into this controller
        private readonly IVillaRepository _dbVilla;

        // use dependency injection to implement logging
        public VillaNumberAPIv2Controller(ApplicationDbContext db, IMapper mapper,
            IVillaNumberRepository dbVillaNumber, IVillaRepository dbVilla)
        {
            _dbVillaNumber = dbVillaNumber;
            _mapper = mapper;
            _response = new APIResponse();
            _dbVilla = dbVilla;
        }


        [HttpGet("GetString")]
        //[MapToApiVersion("2.0")] // this controller only supports 2.0 so we don't need this.
        // if the controller supported 1.0 and 2.0, then this data annotation would be necessary
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

    }
}
