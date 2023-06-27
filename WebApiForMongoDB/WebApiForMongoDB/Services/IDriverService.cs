using WebApiForMongoDB.Models;

namespace WebApiForMongoDB.Services
{
    public interface IDriverService
    {
        Task<List<Driver>> GetAsync();
    }
}