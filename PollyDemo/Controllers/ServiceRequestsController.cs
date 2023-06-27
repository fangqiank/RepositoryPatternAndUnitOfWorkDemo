using Microsoft.AspNetCore.Mvc;
using Polly;
using RestSharp;

namespace PollyDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly ILogger<ServiceRequestsController> _logger;

        public ServiceRequestsController(ILogger<ServiceRequestsController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetData")]
        public async Task<IActionResult> Get()
        {
            var retryPolicy = Policy
                .Handle<Exception>()
                .RetryAsync(5, onRetry: (exception, retryCount) =>
                {
                    Console.WriteLine($"Error: {exception.Message}, Retry Count: {retryCount}");
                });

            //await retryPolicy.ExecuteAsync(async () =>
            //{
            //    await ConnectToApi();
            //});

            var amountToPause = TimeSpan.FromSeconds(15);

            var retryWaitPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(5, x => amountToPause , onRetry: (ex, retryCount) =>
                {
                    Console.WriteLine($"Error: {ex.Message}, Retry count: {retryCount}");
                });

            //await retryWaitPolicy.ExecuteAsync(async () =>
            //{
            //    await ConnectToApi();
            //});

            var tryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetry(5, x => amountToPause, (ex, retryCount) =>
                {
                    Console.WriteLine($"Error: {ex.Message}, Retry count: {retryCount}");
                });

            var circuitBreakPolicy = Policy
                .Handle<Exception>()
                .CircuitBreaker(3, TimeSpan.FromSeconds(30));

            var finalPolicy = tryPolicy.Wrap(circuitBreakPolicy);

            finalPolicy.Execute(() =>
            {
                Console.WriteLine("Execuiting");
                ConnectToApi();
            });

            return Ok();
        }

        private async Task ConnectToApi()
        {
            var url = "https://matchilling-chuck-norris-jokes-v1.p.rapidapi.com/jokes/random";

            var client = new RestClient();

            var request = new RestRequest(url, Method.Get);

            request.AddHeader("accept", "application/json");
            request.AddHeader("X-RapidAPI-Key", 
                "5a598a4679msh5ecafd74e047659p1e7c18jsn7232c864c3a9");
            request.AddHeader("X-RapidAPI-Host", 
                "matchilling-chuck-norris-jokes-v1.p.rapidapi.com");

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                Console.WriteLine(response.Content);
            }
            else
            {
                Console.WriteLine(response.ErrorMessage);
                throw new Exception("Not able to connect the service");
            }
        }
    }
}
