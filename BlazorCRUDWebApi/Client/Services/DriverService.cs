using BlazorCRUDWebApi.Shared.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace BlazorCRUDWebApi.Client.Services
{
    public class DriverService : IDriverService
    {
        private readonly HttpClient _client;
        private readonly NavigationManager _manager;

        public List<Driver> Drivers { get; set; } = new();

        public DriverService(HttpClient client, NavigationManager manager)
        {
            _client = client;
            _manager = manager;
        }

        public async Task SetDrivers(HttpResponseMessage result)
        {
            var res = await result.Content.ReadFromJsonAsync<List<Driver>>();
            Drivers = res;
            _manager.NavigateTo("driver");
        }

        public async Task<IEnumerable<Driver?>> All()
        {
            try
            {
                var apiResponse = await _client.GetStreamAsync("api/drivers");

                var result = await JsonSerializer.DeserializeAsync<
                    IEnumerable<Driver>>(apiResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw ex;
            }
        }

        public async Task GetAllDrivers()
        {
            try
            {
                var result = await _client.GetFromJsonAsync<List<Driver>>("api/drivers");

                if (result != null)
                    Drivers = result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw ex;
            }
        }

        public async Task<Driver?> AddDriver(Driver newDriver)
        {
            try
            {
                var itemJson = new StringContent(
                    JsonSerializer.Serialize(newDriver),
                     Encoding.UTF8,
                     "application/json"
                     );

                var response = await _client.PostAsync("api/drivers", itemJson);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStreamAsync();

                    var result = JsonSerializer.Deserialize<Driver>(
                        responseBody,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                    return result;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw ex;
            }
        }

        public async Task CreateDriver(Driver newDriver)
        {
            var result = await _client.PostAsJsonAsync("api/drivers", newDriver);
            await SetDrivers(result);
        }

        public async Task<bool> Delete(int driverId)
        {
            try
            {
                var response = await _client.DeleteAsync($"api/drivers/{driverId}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<Driver?> GetDriver(int driverId)
        {
            try
            {
                var response = await _client.GetStreamAsync($"api/drivers/{driverId}");

                var result = await JsonSerializer.DeserializeAsync<Driver>(
                    response,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw ex;
            }
        }

        public async Task<Driver> GetSingleDriver(int id)
        {
            var result = await _client.GetFromJsonAsync<Driver>($"api/drivers/{id}");
            if (result != null)
                return result;

            throw new Exception("Driver not found");
        }

        public async Task<bool> Update(Driver updDriver)
        {
            try
            {
                var itemJson = new StringContent(
                    JsonSerializer.Serialize(updDriver),
                     Encoding.UTF8,
                     "application/json"
                     );

                var response = await _client.PutAsync($"api/drivers/{updDriver.Id}", itemJson);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw ex;
            }
        }

        public async Task UpdateDriver(Driver updDriver)
        {
            var result = await _client.PutAsJsonAsync($"api/drivers/{updDriver.Id}", updDriver);
            await SetDrivers(result);
        }
    }
}
