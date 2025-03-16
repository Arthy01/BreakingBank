using BreakingBank.Hubs;
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
            services.AddSingleton<ISaveGameService, SaveGameServiceMemory>();
            services.AddSingleton<SessionService>();

            // Hosted (With Dependency Injection)
            services.AddSingleton<GameService>();
            services.AddHostedService(provider => provider.GetRequiredService<GameService>());

            return services;
        }
    }
}
