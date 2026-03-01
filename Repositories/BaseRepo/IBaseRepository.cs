using System.Linq.Expressions;
using AppGroupCar.Entities.Base;

namespace AppGroupCar.Repositories.BaseRepo
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<int> SaveChangesAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, int pageNumber, int pageSize);
    }
}
