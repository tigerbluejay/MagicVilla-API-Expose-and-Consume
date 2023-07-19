using AutoMapper;
using MagicVillaWeb.Models;
using MagicVillaWeb.Models.DTO;

namespace MagicVillaWeb
{
	public class MappingConfig : Profile
	{
		public MappingConfig() 
		{
			CreateMap<VillaDTO, VillaCreateDTO>().ReverseMap();
			CreateMap<VillaDTO, VillaUpdateDTO>().ReverseMap();

			CreateMap<VillaNumberDTO, VillaNumberCreateDTO>().ReverseMap();
			CreateMap<VillaNumberDTO, VillaNumberUpdateDTO>().ReverseMap();
		}
	}
	
}
