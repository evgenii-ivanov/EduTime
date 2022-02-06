using DigitalSkynet.Boilerplate.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;

namespace EduTime.Api.Healthchecks
{
    public class ReadinessCheck : IHealthCheck
    {
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Constructor. Initializes the class.
        /// </summary>
        /// <param name="dbContext"></param>
        public ReadinessCheck(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Checks if the app has the access to the database
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            _ = await _dbContext.Database.ExecuteSqlInterpolatedAsync($"select 1;", cancellationToken);
            return HealthCheckResult.Healthy("Database is working");
        }
    }
}
