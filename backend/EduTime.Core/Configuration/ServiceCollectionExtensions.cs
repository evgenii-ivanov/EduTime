using EduTime.Core.Interfaces;
using EduTime.Core.Interfaces.FileStorage;
using EduTime.Core.Services;
using EduTime.Core.Services.FileStorage;
using EduTime.Core.Services.Messaging;
using EduTime.Foundation.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EduTime.Core.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IMessageService, SmtpMessageService>();

            //TODO: factory???
            services.AddScoped<IStorageService, LocalStorageService>();

            //just to test localization
            services.AddTransient<IDebugService, DebugService>();

            return services;
        }

        public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtOptions>(options => configuration.GetSection("Jwt"));
            services.Configure<SmtpConnectionOptions>(options => configuration.GetSection("EmailConnect"));
            services.Configure<MessageSenderOptions>(options => configuration.GetSection("Provide"));

            return services;
        }
    }
}
