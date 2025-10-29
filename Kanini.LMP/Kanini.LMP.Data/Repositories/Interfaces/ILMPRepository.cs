namespace Kanini.LMP.Data.Repositories.Interfaces
{
    public interface ILMPRepository<T, TId> where T : class
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T?> GetByIdAsync(TId id);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(TId id);
    }
}
