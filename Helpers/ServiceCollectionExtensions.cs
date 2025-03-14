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
            services.AddSingleton<SaveGameServiceMemory>();
            services.AddSingleton<SessionService>();

            // Hosted (Background Tasks)
            services.AddHostedService<GameHubTickService>();

            return services;
        }
    }
}
