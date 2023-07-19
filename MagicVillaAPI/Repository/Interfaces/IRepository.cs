using MagicVillaAPI.Model;
using System.Linq.Expressions;

namespace MagicVillaAPI.Repository.Interfaces
{
	// we use T which is the generic form of a class
	// it stands for Villa and any other class or model we wish to pass to it
	public interface IRepository<T> where T : class
	{
		Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
		Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true, string? includeProperties = null);
		Task CreateAsync(T entity);
		Task RemoveAsync(T entity);
		Task SaveAsync();
	}
}
