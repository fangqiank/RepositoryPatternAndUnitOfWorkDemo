namespace RepositoryPatternAndUnitOfWork.Services.Background
{
    public interface IServiceManagement
    {
        Task GenerateMerchandise();
        Task SendMail();
        void SyncData();
        Task UpdateDatabase();
    }
}