using ApiCorrectlation.Configuration.Interface;

namespace ApiCorrectlation.Helpers
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private const string _correlationHeader = "X-Correlation-Id";

        public CorrelationIdMiddleware(RequestDelegate next) => _next = next;
        
        public async Task Invoke(HttpContext context, ICorrelationIdGenerator generator)
        {
            var correlationId = GetCorrelation(context, generator);
            AddCorrelationIdToResponse(context, correlationId);

            await _next(context);
        }

        private static void AddCorrelationIdToResponse(HttpContext context, string correlationId)
        {
            context.Response.OnStarting(() =>
            {
                context.Response.Headers.Add(_correlationHeader, new[] { correlationId});
                return Task.CompletedTask;
            });
        }

        private static string GetCorrelation(
            HttpContext context, 
            ICorrelationIdGenerator generator)
        {
            if(context.Request.Headers.TryGetValue(_correlationHeader, out var correlationId))
            {
                generator.Set(correlationId!);
                return correlationId;
            }

            return generator.Get();
        }
    }
}
