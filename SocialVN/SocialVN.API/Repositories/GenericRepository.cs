using Microsoft.EntityFrameworkCore;
using SocialVN.API.Data;

namespace SocialVN.API.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly SocialVNDbContext dbContext;
        private readonly DbSet<T> _dbSet;
        public GenericRepository(SocialVNDbContext context)
        {
            dbContext = context;
            _dbSet = context.Set<T>();
        }
        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        public async Task<T> CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await SaveAsync();
            return entity;
        }
        public async Task<T> UpdateAsync(T updatedEntity)
        {
            dbContext.Update(updatedEntity);
            await SaveAsync();
            return updatedEntity;

        }
        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await SaveAsync();
            }
        }
        public async Task SaveAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
