using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EduTime.Domain.Entities.Identity;
using EduTime.Domain.Infrastructure;

namespace EduTime.Domain.Entities
{
    public class Session : BaseGuidEntity
    {
        public DateTime ExpiresAtUtc { get; set; }

        [MaxLength(100)]
        public string UserAgent { get; set; }
        [MaxLength(15)]
        public string IpAddress { get; set; }

        public int TokenVersion { get; set; }

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }
}
