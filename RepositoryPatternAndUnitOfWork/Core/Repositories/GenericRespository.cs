using Microsoft.EntityFrameworkCore;
using RepositoryPatternAndUnitOfWork.Data;

namespace RepositoryPatternAndUnitOfWork.Core.Repositories
{
    public class GenericRespository<T> : IGenericRespository<T>
        where T : class
    {
        protected ApplicationDbContext _context;
        protected readonly ILogger _logger;
        protected DbSet<T> dbSet;

        public GenericRespository(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
            dbSet = context.Set<T>();
        }

        public virtual async Task<bool> Add(T entity)
        {
            await dbSet.AddAsync(entity);
            return true;
        }


        public virtual Task<bool> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync() => await dbSet.ToListAsync();

        public async Task<T?> GetByIdAsync(Guid id) => await dbSet.FindAsync(id);


        public virtual Task<bool> Upsert(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
