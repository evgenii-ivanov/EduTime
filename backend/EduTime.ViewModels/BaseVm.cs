using System;
using DigitalSkynet.DotnetCore.DataStructures.Interfaces;

namespace EduTime.ViewModels
{
    public class BaseVm : IHasKey<Guid>, ISoftDeletable, ITimestamped
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
