using BlazorCRUDWebApi.Shared.Models;

namespace BlazorCRUDWebApi.Client.Services
{
    public interface IDriverService
    {
        Task<Driver?> AddDriver(Driver newDriver);
        Task<IEnumerable<Driver?>> All();
        Task<bool> Delete(int driverId);
        Task<Driver?> GetDriver(int driverId);
        Task<bool> Update(Driver updDriver);
    }
}