using EduTime.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;

namespace DigitalSkynet.Boilerplate.Data.Identity
{
    public class AppRoleStore : RoleStore<Role, AppDbContext, Guid, UserRole, RoleClaim>
    {
        public AppRoleStore(AppDbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
        }
    }
}
