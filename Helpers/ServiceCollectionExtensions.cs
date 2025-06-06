﻿using BreakingBank.Hubs;
using BreakingBank.Services;
using BreakingBank.Services.Game;

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
            services.AddSingleton<ISaveGameService, SaveGameServiceDatabase>();
            services.AddSingleton<SessionService>();
            services.AddSingleton<DatabaseHelper>();

            // Hosted (With Dependency Injection)
            services.AddSingleton<GameService>();
            services.AddHostedService(provider => provider.GetRequiredService<GameService>());

            return services;
        }
    }
}
