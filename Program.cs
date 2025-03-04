
using BreakingBank.Hubs;
using BreakingBank.Models;
using BreakingBank.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BreakingBank.Helpers;

namespace BreakingBank
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add Logging functionality
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            // Add basic services
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSignalR();

            // Configuration of appsettings
            builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JWTSettings"));

            // Add JWT Authentication and Authorizarion
            JWTSettings jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JWTSettings>() ?? throw new NullReferenceException("JwtSettings Section not found!");
            byte[] jwtKeyBytes = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
            
            builder.Services
             .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
             {
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = false,
                     ValidateAudience = false,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = new SymmetricSecurityKey(jwtKeyBytes),
                 };

                 // Authentication Events
                 options.Events = new JwtBearerEvents
                 {
                     OnMessageReceived = context =>
                     {
                         var accessToken = context.Request.Query["access_token"];

                         // Falls die Anfrage von SignalR kommt, JWT-Token aus der URL extrahieren
                         var path = context.HttpContext.Request.Path;
                         if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/testingHub") || path.StartsWithSegments("/gameHub")))
                         {
                             context.Token = accessToken;
                         }

                         return Task.CompletedTask;
                     },
                     OnTokenValidated = context =>
                     {
                         Console.WriteLine($"Token Validated: {context.SecurityToken}");
                         return Task.CompletedTask;
                     },
                     OnChallenge = context =>
                     {
                         Console.WriteLine("Token validation failed: Unauthorized");
                         return Task.CompletedTask;
                     }
                 };

             });

            builder.Services.AddAuthorization();

            // Add BreakingBank Services
            builder.Services.AddBreakinBankServices();
            
            // Configure Https
            CertSettings certSettings = builder.Configuration.GetSection("Certificate").Get<CertSettings>() ?? throw new NullReferenceException("Certificate Section not found!");

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(5000); // HTTP
                options.ListenAnyIP(5001, listenOptions =>
                {
                    listenOptions.UseHttps(certSettings.File, certSettings.Password);
                });
            });

            var app = builder.Build();

            app.UseCors(builder => builder.WithOrigins("http://breakingbank.de:8000", "http://localhost:8000").AllowAnyMethod().AllowAnyHeader().AllowCredentials());

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<TestingHub>("/testingHub");
            app.MapHub<GameHub>("/gameHub");
            
            app.MapControllers();

            app.Run();
        }
    }
}
