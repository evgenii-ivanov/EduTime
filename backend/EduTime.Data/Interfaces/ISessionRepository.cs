using EduTime.Domain.Entities;
using DigitalSkynet.DotnetCore.DataAccess.Repository;
using System;

namespace DigitalSkynet.Boilerplate.Data.Interfaces
{
    public interface ISessionRepository : IGenericRepository<Session, Guid>
    {
    }
}
