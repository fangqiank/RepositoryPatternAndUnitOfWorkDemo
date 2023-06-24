using BlazorCRUDWebApi.Shared.Models;
using System.Text;
using System.Text.Json;

namespace BlazorCRUDWebApi.Client.Services
{
    public class DriverService : IDriverService
    {
        private readonly HttpClient _client;

        public DriverService(HttpClient client)
        {
            _client = client;
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
                await Console.Out.WriteLineAsync($"Error: {ex.Message}");
                throw;
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
                await Console.Out.WriteLineAsync($"Error: {ex.Message}");
                throw;
            }
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
                await Console.Out.WriteLineAsync($"Error: {ex.Message}");
                throw;
            }
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
                await Console.Out.WriteLineAsync($"Error: {ex.Message}");
                throw;
            }
        }
    }
}
