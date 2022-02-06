using DigitalSkynet.Boilerplate.Data.Interfaces;
using DigitalSkynet.Boilerplate.Data.Interfaces.FileStorage;
using DigitalSkynet.Boilerplate.Data.Repositories;
using DigitalSkynet.DotnetCore.DataAccess.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;

namespace DigitalSkynet.Boilerplate.Data.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterRepositories(this IServiceCollection services)
        {
            #region Register repositories

            services.AddScoped<ISettingRepository, SettingRepository>();
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddScoped<IStorageObjectRepository, StorageObjectRepository>();

            //just for test localization
            services.AddTransient<IDebugRepository, DebugRepository>();

            //to test localization
            services.AddTransient<IDebugRepository, DebugRepository>();

            #endregion

            services.AddScoped<IUnitOfWork, UnitOfWork<AppDbContext>>();
            return services;
        }
    }
}
