using AutoMapper;
using DigitalSkynet.Boilerplate.Data.Interfaces;
using EduTime.Domain.Entities;

namespace DigitalSkynet.Boilerplate.Data.Repositories
{
    public class SettingRepository : BaseRepository<Setting>, ISettingRepository
    {
        public SettingRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {}
    }
}
