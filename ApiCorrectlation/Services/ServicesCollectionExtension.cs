using ApiCorrectlation.Configuration;
using ApiCorrectlation.Configuration.Interface;

namespace ApiCorrectlation.Services
{
    public static class ServicesCollectionExtension
    {
        public static IServiceCollection AddCorrelationManager(this IServiceCollection services)
        {
            services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();

            return services;
        } 
    }
}
