using EduTime.Domain.Entities.FileStorage;
using DigitalSkynet.DotnetCore.DataAccess.Repository;
using System;

namespace DigitalSkynet.Boilerplate.Data.Interfaces.FileStorage
{
    public interface IStorageObjectRepository : IGenericRepository<StorageObject, Guid>
    {
    }
}
