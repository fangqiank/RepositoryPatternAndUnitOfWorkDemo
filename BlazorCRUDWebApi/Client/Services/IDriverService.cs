using BlazorCRUDWebApi.Shared.Models;

namespace BlazorCRUDWebApi.Client.Services
{
    public interface IDriverService
    {
        List<Driver> Drivers { get; set; }

        Task<Driver?> AddDriver(Driver newDriver);
        Task<IEnumerable<Driver?>> All();
        Task CreateDriver(Driver newDriver);
        Task<bool> Delete(int driverId);
        Task GetAllDrivers();
        Task<Driver?> GetDriver(int driverId);
        Task<Driver> GetSingleDriver(int id);
        Task SetDrivers(HttpResponseMessage result);
        Task<bool> Update(Driver updDriver);
        Task UpdateDriver(Driver updDriver);
    }
}