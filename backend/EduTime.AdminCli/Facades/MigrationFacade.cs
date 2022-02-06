using System;
using System.Linq;
using DigitalSkynet.Boilerplate.Data;
using EduTime.AdminCli.CliOptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EduTime.AdminCli.Facades
{
    public static class MigrationFacade
    {
        public static int Run(MigrationOptions options, IConfiguration configuration)
        {
            var builder = new DbContextOptionsBuilder<AppDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            builder.UseNpgsql(connectionString);

            using var dbContext = new AppDbContext(builder.Options);
            var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();
            if (pendingMigrations.Count > 0)
            {
                Console.WriteLine("The following migrations will be applied:");
                foreach (var pendingMigration in pendingMigrations)
                {
                    Console.Write(" - ");
                    Console.WriteLine(pendingMigration);
                }

                dbContext.Database.Migrate();
            }
            else
            {
                Console.WriteLine("Database is in sync with assembly migrations");
            }

            return 0;
        }
    }
}
