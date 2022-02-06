using AutoMapper;
using DigitalSkynet.Boilerplate.Data.Interfaces.FileStorage;
using EduTime.Domain.Entities.FileStorage;

namespace DigitalSkynet.Boilerplate.Data.Repositories
{
    public class StorageObjectRepository : BaseRepository<StorageObject>, IStorageObjectRepository
    {
        public StorageObjectRepository(AppDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }
    }
}
