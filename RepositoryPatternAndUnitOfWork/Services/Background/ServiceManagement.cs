namespace RepositoryPatternAndUnitOfWork.Services.Background
{
    public class ServiceManagement : IServiceManagement
    {
        public Task GenerateMerchandise()
        {
            Console.WriteLine($"Generate Merchandise: " +
                $"Long running task {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");

            return Task.CompletedTask;
        }

        public Task SendMail()
        {
            Console.WriteLine($"Send email: " +
                $"Long running task {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");

            return Task.CompletedTask;
        }

        public void SyncData()
        {
            Console.WriteLine($"Sync data: " +
                $"Long running task {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");

        }

        public Task UpdateDatabase()
        {
            Console.WriteLine($"Update database: " +
                $"Long running task {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");

            return Task.CompletedTask;
        }
    }
}
