using AutoMapper;
using MagicVillaAPI.Model;
using MagicVillaAPI.Model.DTO;

namespace MagicVillaAPI
{
	public class MappingConfig : Profile
	{
		public MappingConfig() 
		{
			CreateMap<Villa, VillaDTO>();
			CreateMap<VillaDTO, Villa>();
			// You can do custom mappings - read the docs
			
			CreateMap<Villa, VillaCreateDTO>().ReverseMap();
			CreateMap<Villa, VillaUpdateDTO>().ReverseMap();

			CreateMap<VillaNumber, VillaNumberDTO>().ReverseMap();
			CreateMap<VillaNumber, VillaNumberCreateDTO>().ReverseMap();
			CreateMap<VillaNumber, VillaNumberUpdateDTO>().ReverseMap();
		}
	}
	
}
