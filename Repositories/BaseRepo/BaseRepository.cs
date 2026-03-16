using System.Linq.Expressions;
using AppGroupCar.Data;
using AppGroupCar.DTO;
using AppGroupCar.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace AppGroupCar.Repositories.BaseRepo
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _dbSet;
        public BaseRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }
        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            entity.LastUpdateAt = DateTime.UtcNow;
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            entity.DeleteAt = DateTime.UtcNow;

            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
        public async Task<int> SaveChangesAsync()
        {
            // Trả về số lượng bản ghi bị ảnh hưởng trong Database
            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, int pageNumber, int pageSize)
        {
            return await _dbSet
                .Where(predicate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<PagedResult<T>> FindAndFilter(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, int pageNumber = 1, int pageSize = 10)
        {
            // 1. Validation chống mập mờ: Chặn mấy gã ngáo ngơ truyền số âm
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            // 2. AsNoTracking(): Tăng hiệu suất đọc dữ liệu (cực kỳ quan trọng)
            IQueryable<T> query = _dbSet.AsNoTracking().Where(predicate);

            // 3. Đếm TỔNG SỐ bản ghi thỏa mãn điều kiện trước khi cắt trang
            int totalCount = await query.CountAsync();

            // 4. Bắt buộc phải sắp xếp (OrderBy) trước khi Skip/Take để tránh lỗi SQL
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            else
            {
                // Nếu lười không truyền OrderBy, tự động fallback sắp xếp theo Id của BaseEntity
                query = query.OrderBy(e => e.Id);
            }

            // 5. Cắt trang (Pagination)
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // 6. Đóng gói và trả về
            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public IQueryable<T> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }


    }
}
