using EduTime.Domain.Infrastructure;
using DigitalSkynet.DotnetCore.DataStructures.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;

namespace EduTime.Domain.Entities.Identity
{
    public class User : IdentityUser<Guid>, IHasKey<Guid>, ITimestamped
    {
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
