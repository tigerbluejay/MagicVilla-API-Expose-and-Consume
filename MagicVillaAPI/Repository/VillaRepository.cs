using MagicVillaAPI.Data;
using MagicVillaAPI.Model;
using MagicVillaAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace MagicVillaAPI.Repository
{
	public class VillaRepository : Repository<Villa>, IVillaRepository
	{
		private readonly ApplicationDbContext _db;
		public VillaRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}
		public async Task<Villa> UpdateAsync(Villa entity)
		{
			entity.UpdateDate = DateTime.Now;
			_db.Villas.Update(entity);
			await _db.SaveChangesAsync();
			return entity;
		}
	}
}
