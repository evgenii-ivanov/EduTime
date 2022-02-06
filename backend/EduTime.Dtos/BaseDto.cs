using System;
using DigitalSkynet.DotnetCore.DataStructures.Interfaces;

namespace EduTime.Dtos
{
    public class BaseDto : IHasKey<Guid>, ISoftDeletable, ITimestamped
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
