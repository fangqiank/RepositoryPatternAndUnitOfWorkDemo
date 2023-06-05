using RepositoryPatternAndUnitOfWork.Core.Repositories;
using RepositoryPatternAndUnitOfWork.Models;

namespace RepositoryPatternAndUnitOfWork.Core.IRepositories
{
    public interface IUserRepository: IGenericRespository<User>
    {
    }
}
