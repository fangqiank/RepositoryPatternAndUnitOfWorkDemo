using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WebApiForMongoDB.Configurations;
using WebApiForMongoDB.Models;

namespace WebApiForMongoDB.Services
{
    public class DriverService : IDriverService
    {
        private readonly IMongoCollection<Driver> _drivers;

        public DriverService(IOptions<DatabaseSettings> dbSettings)
        {
            var mongoClient = new MongoClient(dbSettings.Value.ConnString);

            var mongoDb = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);

            _drivers = mongoDb.GetCollection<Driver>(dbSettings.Value.CollectionName);
        }

        public async Task<List<Driver>> GetAsync() => await _drivers
            .Find(_ => true)
            .ToListAsync();
    }
}
