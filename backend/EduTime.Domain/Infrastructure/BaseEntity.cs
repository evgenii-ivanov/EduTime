using DigitalSkynet.DotnetCore.DataStructures.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace EduTime.Domain.Infrastructure
{
    public class BaseEntity<TKey> : IHasKey<TKey>, ISoftDeletable, ITimestamped
        where TKey : struct
    {
        [Key]
        public TKey Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
