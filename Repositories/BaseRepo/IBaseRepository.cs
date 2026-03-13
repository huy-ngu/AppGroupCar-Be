using AppGroupCar.DTO;
using AppGroupCar.Entities.Base;
using System.Linq.Expressions;

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
        IQueryable<T> GetQueryable();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, int pageNumber, int pageSize);
        Task<PagedResult<T>> FindAndFilter(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>,
            IOrderedQueryable<T>>? orderBy = null,
            int pageNumber = 1,
            int pageSize = 10);
    }
}
