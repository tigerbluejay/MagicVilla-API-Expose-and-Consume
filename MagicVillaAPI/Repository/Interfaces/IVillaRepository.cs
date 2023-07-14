using MagicVillaAPI.Model;
using System.Linq.Expressions;

namespace MagicVillaAPI.Repository.Interfaces
{
	// when we use the VillaRepository, T will be Villa
	public interface IVillaRepository : IRepository<Villa>
	{
		// update stays here and not in the superclass
		// because update logic tends to be custom
		Task<Villa> UpdateAsync(Villa entity);

	}
}
