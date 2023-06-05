namespace RepositoryPatternAndUnitOfWork.Core.Repositories
{
    public interface IGenericRespository<T>
    {
        Task<bool> Add(T entity);
        Task<bool> Delete(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        Task<bool> Upsert(T entity);
    }
}