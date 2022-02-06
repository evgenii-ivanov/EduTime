using EduTime.Api.Configuration;
using EduTime.Core.Configuration;
using EduTime.Core.Context;
using EduTime.Core.Identity;
using DigitalSkynet.Boilerplate.Data;
using DigitalSkynet.Boilerplate.Data.Configuration;
using DigitalSkynet.Boilerplate.Data.Identity;
using EduTime.Domain.Entities.Identity;
using DigitalSkynet.DotnetCore.Api.Middleware;
using DigitalSkynet.Website.Core.Identity;
using EduTime.Api.Healthchecks;
using EduTime.Foundation.Constants;
using EduTime.Foundation.Enums;
using EduTime.Foundation.Helpers.Time;
using EduTime.Foundation.Options;
using Hangfire;
using Hangfire.Console;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;

namespace EduTime.Api
{
    public class Startup
    {
	    public Startup(IConfiguration configuration)
	    {
		    Configuration = configuration;
	    }

	    public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization();

            services.AddFeatureManagement();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EduTime.Api", Version = "v1" });
            });

            services.Configure<VersionOptions>(Configuration.GetSection("Version"));
            services.Configure<CorsOptions>(Configuration.GetSection("Cors"));
            services.Configure<MessageSenderOptions>(Configuration.GetSection("Messaging:Sender"));
            services.Configure<SmtpConnectionOptions>(Configuration.GetSection("Messaging:Smtp"));
            services.Configure<FileStorageOptions>(Configuration.GetSection("Storage:FileStorage"));

            services.AddScoped<IContextService, HttpContextService>();
            services.AddScoped<IDateTimeHelper, DateTimeHelper>();
            services.AddHttpContextAccessor();
            services.AddAutoMapper(typeof(DataMappingProfile), typeof(CoreMappingProfile));

            services.AddHangfire(c =>
            {
                c.UseConsole();
                c.UseMemoryStorage(); // Make sure to use SQL or Redis in production
            });

            services
                .AddDbContext<AppDbContext>(options =>
                {
                    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"), o => o.UseNetTopologySuite());
                    options.EnableSensitiveDataLogging();
                });

            services.AddIdentity<User, Role>(opts =>
            {
                // TODO: add your Identity settings here.
            })
                .AddUserManager<AppUserManager>()
                .AddUserStore<AppUserStore>()
                .AddRoleStore<AppRoleStore>()
                .AddRoleManager<AppRoleManager>()
                .AddSignInManager<AppSignInManager>()
                .AddDefaultTokenProviders();

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                })
                .AddGoogle(options =>
                {
                    SetupOAuthBase(options, ExternalAuthProviders.Google);
                })
                .AddFacebook(options =>
                {
                    SetupOAuthBase(options, ExternalAuthProviders.Facebook);
                });

            services
                .AddAuthorizationCore();

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services
                .AddHealthChecks()
                .AddCheck<LivenessCheck>("liveness", tags: new[] { "liveness" })
                .AddCheck<ReadinessCheck>("readiness", tags: new[] { "readiness" });

            services
                .RegisterRepositories()
                .RegisterServices()
                .AddConfiguration(Configuration)
                .AddLocalizationConfiguration(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IFeatureManager featureManager)
	    {
            var corsOptions = app.ApplicationServices.GetRequiredService<IOptions<CorsOptions>>().Value;

            var forwardedHeadersOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            };
            forwardedHeadersOptions.KnownNetworks.Clear();
            forwardedHeadersOptions.KnownProxies.Clear();
            app.UseForwardedHeaders(forwardedHeadersOptions);

            app.UseGlobalExceptionHandler();


            app.UseCors(builder => builder
                .WithOrigins(corsOptions.AllowedOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseRequestLocalization(app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

            if (featureManager.IsEnabledAsync(EnabledFeatures.Swagger).GetAwaiter().GetResult())
		    {
			    app.UseSwagger();
			    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EduTime.Api v1"));
		    }

            if (featureManager.IsEnabledAsync(EnabledFeatures.Hangfire).GetAwaiter().GetResult())
            {
                app.UseHangfireServer();
                if (featureManager.IsEnabledAsync(EnabledFeatures.HangfireDashboard).GetAwaiter().GetResult())
                {
                    app.UseHangfireDashboard();
                }
            }

            app.UseRequestLocalization();
		    app.UseRouting();
		    app.UseAuthorization();

		    app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health/liveness",
                    new HealthCheckOptions {Predicate = x => x.Name == "liveness"});
                endpoints.MapHealthChecks("/health/readiness",
                    new HealthCheckOptions {Predicate = x => x.Name == "readiness"});
			    endpoints.MapControllers();
		    });
	    }

        /// <summary>
        /// Sets the OAuth up
        /// </summary>
        /// <param name="options"></param>
        /// <param name="provider"></param>
        private void SetupOAuthBase(OAuthOptions options, ExternalAuthProviders provider)
        {
            var section = Configuration.GetSection("OAuth").GetSection(provider.ToString());

            options.ClientId = section["ClientId"];
            options.ClientSecret = section["ClientSecret"];
            options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.CorrelationCookie.SameSite = SameSiteMode.Lax;
        }

    }
}
