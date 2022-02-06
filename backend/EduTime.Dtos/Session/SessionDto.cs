using EduTime.Domain.Entities.Identity;
using System;

namespace EduTime.Dtos.Session
{
    public class SessionDto : BaseDto
    {
        public DateTime ExpiresAtUtc { get; set; }

        public string UserAgent { get; set; }
        public string IpAddress { get; set; }

        public int TokenVersion { get; set; }
        public Guid UserId { get; set; }
        //public virtual User User { get; set; }
    }
}
