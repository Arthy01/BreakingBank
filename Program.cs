
using BreakingBank.Hubs;
using BreakingBank.Models;
using BreakingBank.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

            // Enable CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowEverything",
                    policy => policy.AllowAnyOrigin()  // Erlaubt jede Domain
                                    .AllowAnyMethod()  // Erlaubt GET, POST, PUT, DELETE, etc.
                                    .AllowAnyHeader()); // Erlaubt alle Header
            });

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

                 // Header Authentication
                 options.Events = new JwtBearerEvents
                 {
                     OnMessageReceived = context =>
                     {
                         var accessToken = context.Request.Query["access_token"];

                         if (!string.IsNullOrEmpty(accessToken) &&
                             context.HttpContext.WebSockets.IsWebSocketRequest)
                         {
                             context.Token = accessToken;
                         }

                         return Task.CompletedTask;
                     },

                     OnTokenValidated = context =>
                     {
                         if (context.HttpContext.Request.Path.Value?.Contains("/negotiate") == true)
                         {
                             return Task.CompletedTask;
                         }

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

            // Add Custom Services
            builder.Services.AddScoped<JWTService>();
            
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

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowEverything");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<TestingHub>("/testingHub");

            app.MapControllers();

            app.Run();
        }
    }
}
