using System;
using EduTime.Domain.Entities;
using DigitalSkynet.DotnetCore.DataAccess.Repository;

namespace DigitalSkynet.Boilerplate.Data.Interfaces
{
    public interface ISettingRepository : IGenericRepository<Setting, Guid>
    {
    }
}
