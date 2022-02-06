using EduTime.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;

namespace DigitalSkynet.Boilerplate.Data.Identity
{
    public class AppUserStore : UserStore<User, Role, AppDbContext, Guid, UserClaim, UserRole, UserLogin, UserToken, RoleClaim>
    {
        public AppUserStore(AppDbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
        }
    }
}
