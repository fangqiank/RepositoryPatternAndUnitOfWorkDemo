using Microsoft.EntityFrameworkCore;
using RepositoryPatternAndUnitOfWork.Core.IRepositories;
using RepositoryPatternAndUnitOfWork.Data;
using RepositoryPatternAndUnitOfWork.Models;

namespace RepositoryPatternAndUnitOfWork.Core.Repositories
{
    public class UserRepository : GenericRespository<User> ,IUserRepository
    {

        public UserRepository(ApplicationDbContext context, ILogger logger) 
            : base(context, logger)
        {
            
        }

        //public override async Task<bool> Add(User entity)
        //{
        //    await dbSet.AddAsync(entity);
        //    return true;
        //}

        public override async Task<bool> Delete(Guid id)
        {
            try
            {
                var existedUser = await dbSet.Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

                if (existedUser != null)
                {
                    dbSet.Remove(existedUser);
                    return true;
                }

                return false;
                    
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{typeof(UserRepository)} Delete method error");
                return false;
            }
        }

        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            try
            {
                return await dbSet.ToListAsync();
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, $"{typeof(UserRepository)} GetAll method error");
                return new List<User>();
            }
        }

        public override async Task<bool> Upsert(User entity)
        {
            try
            {
                var existedUser = await dbSet.Where(x => x.Id == entity.Id)
                .FirstOrDefaultAsync();

                if (existedUser == null)
                    return await Add(entity);

                existedUser.FirstName = entity.FirstName;
                existedUser.LastName = entity.LastName;
                existedUser.Email = entity.Email;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{typeof(UserRepository)} Upsert method error");
                return false;
            }
        }
    }
}
