using BreakingBank.Services;

namespace BreakingBank.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBreakingBankServices(this IServiceCollection services)
        {
            // Transistent

            // Scoped
            services.AddScoped<JWTService>();

            // Singleton
            services.AddSingleton<ISaveGameService, SaveGameService>();

            // Hosted
            services.AddHostedService<GameHubTickService>();

            return services;
        }
    }
}
