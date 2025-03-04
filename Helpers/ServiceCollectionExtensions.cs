using BreakingBank.Services;

namespace BreakingBank.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBreakinBankServices(this IServiceCollection services)
        {
            // Transistent

            // Scoped
            services.AddScoped<JWTService>();

            // Singleton
            services.AddSingleton<ISaveGameService, SaveGameService>();

            return services;
        }
    }
}
