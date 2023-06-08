using ApiCorrectlation.Helpers;

namespace ApiCorrectlation.Services
{
    public static class ApplicationBuilderExtension
    {
        public static IApplicationBuilder AddCorrelationIdMiddleware(this IApplicationBuilder app)
            => app.UseMiddleware<CorrelationIdMiddleware>();
        
    }
}
