using EduTime.Domain.Entities.Identity;
using DigitalSkynet.DotnetCore.DataStructures.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using EduTime.Domain.Entities;
using EduTime.Domain.Entities.FileStorage;

namespace DigitalSkynet.Boilerplate.Data
{
    public class AppDbContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Setting>()
                .HasIndex(x => x.Name);

            builder.Entity<Session>()
                .HasIndex(x => x.ExpiresAtUtc);

            builder.Entity<StorageObject>();
        }

        #region Modification Tracking

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        /// <summary> Track entites with modification dates </summary>
        private void OnBeforeSaving()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is ITimestamped baseEntity)
                {
                    var now = DateTime.UtcNow;

                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            baseEntity.UpdatedDate = now;
                            break;

                        case EntityState.Added:
                            baseEntity.CreatedDate = now;
                            baseEntity.UpdatedDate = now;
                            break;
                    }
                }
            }
        }

        #endregion
    }
}
