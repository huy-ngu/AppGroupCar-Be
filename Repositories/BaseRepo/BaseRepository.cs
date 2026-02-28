using AppGroupCar.Data;
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
            // Lọc ra những bản ghi chưa bị xóa mềm
            // Tuy nhiên, tốt nhất là cấu hình Global Query Filter trong DbContext
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            entity.CreatedAt = DateTime.UtcNow; // Gán lại cho chắc cốp dù bạn đã init ở BaseEntity
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            entity.LastUpdateAt = DateTime.UtcNow; // Tự động cập nhật thời gian sửa

            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            // XÓA MỀM (Soft Delete): Không gọi _dbSet.Remove()
            entity.DeleteAt = DateTime.UtcNow;

            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
        public async Task<int> SaveChangesAsync()
        {
            // Trả về số lượng bản ghi bị ảnh hưởng trong Database
            return await _context.SaveChangesAsync();
        }
    }
}
