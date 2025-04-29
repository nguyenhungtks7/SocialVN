namespace SocialVN.API.Repositories
{
    public interface IGenericRepository<T> where T: class
    {
        Task<T> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> CreateAsync(T entity);
        Task <T>UpdateAsync(T updatedEntity);
        Task DeleteAsync(Guid id);
        Task SaveAsync();
    }
}
