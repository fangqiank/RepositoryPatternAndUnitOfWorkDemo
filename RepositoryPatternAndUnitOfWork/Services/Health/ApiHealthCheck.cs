using Microsoft.Extensions.Diagnostics.HealthChecks;
using RestSharp;

namespace RepositoryPatternAndUnitOfWork.Services.Health
{
    public class ApiHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, 
            CancellationToken cancellationToken = default)
        {
            var url = "http://localhost:5000/api/Setup/GetAllUsers";

            var client = new RestClient();
            var request = new RestRequest(url, Method.Get);
            //request.AddHeader("Authorization",
                //"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6IjJiMmZjODE2LTA5OWUtNDRlNS1iMTAwLTM4MzRiNmRiZmY4ZCIsInN1YiI6InpoYW5nc2FuQG1haWwuY29tIiwiZW1haWwiOiJ6aGFuZ3NhbkBtYWlsLmNvbSIsImp0aSI6IjZhYmYzYmI1LTk3MTMtNDRkMy05MGY4LTFhYWExYzU3Yzg5OSIsImlhdCI6MTY4NjQ0MzQyNCwiZGJhY2Nlc3MiOiJ5ZXMiLCJyb2xlIjoiQXBwVXNlciIsIlByb2plY3QuVmlldyI6InllcyIsIm5iZiI6MTY4NjQ0MzQyNCwiZXhwIjoxNjg2NDQzNDg0fQ.BaHkqY7dITLIRFGlucTnnRkoqu0RhMhrX7JPYO84oCE");
            //request.AddHeader("X-RapidAPI-Key", "SIGN-UP-FOR-KEY");
            //request.AddHeader("X-RapidAPI-Host", "chuck-norris-jokes.p.rapdapi.com");

            var response = client.Execute(request);
            if (response.IsSuccessful)
            {
                return Task.FromResult(HealthCheckResult.Healthy());
            }
            else
            {
                return Task.FromResult(HealthCheckResult.Unhealthy());
            }
        }
    }
}
