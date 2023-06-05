using RepositoryPatternAndUnitOfWork.Core.IRepositories;

namespace RepositoryPatternAndUnitOfWork.Core.IConfiguration
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        Task CompleteAsync();

    }
}
