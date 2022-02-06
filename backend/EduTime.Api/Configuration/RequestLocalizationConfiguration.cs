using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.Internal;

namespace EduTime.Api.Configuration
{
    public static class RequestLocalizationConfiguration
    {
        public static IServiceCollection AddLocalizationConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures =
                    ApplicationRequestCultureProvider.SupportedCultures.Select(x => new CultureInfo(x)).ToList();
                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders = new IRequestCultureProvider[]
                    {new ApplicationRequestCultureProvider()};

            });

            return services;
        }
    }

    /// <summary>
    /// Determines the request locale by using Accept-Language header.
    /// You can define your own logic here.
    /// </summary>
    internal class ApplicationRequestCultureProvider : RequestCultureProvider
    {
        public static readonly HashSet<string> SupportedCultures = new()
        {
            "ru", "en"
        };

        private const string DefaultCulture = "en";

        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            var acceptedLanguages = httpContext.Request.Headers.GetOrDefault("Accept-Language").ToString().Split(',');
            var cultureName = acceptedLanguages.Any() && SupportedCultures.Contains(acceptedLanguages.First())
                ? acceptedLanguages.First()
                : DefaultCulture;
            var providerResultCulture = new ProviderCultureResult(cultureName, cultureName);
            return Task.FromResult(providerResultCulture);
        }
    }
}
