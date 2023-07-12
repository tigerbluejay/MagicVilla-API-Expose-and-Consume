using MagicVillaAPI.Model.DTO;

namespace MagicVillaAPI.Data
{
	// this static class is part of the data layer,
	// it simulates a database, although the data is not persisted anywhere
	public static class VillaStore
	{
		public static List<VillaDTO> villaList = new List<VillaDTO>()
		{
			new VillaDTO() { Id=1, Name="Pool View", Occupancy= 4, Sqft = 100},
			new VillaDTO() { Id=2, Name="Beach View", Occupancy= 3, Sqft = 300}
		};
	}
}
