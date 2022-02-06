using System;
using AutoMapper;
using EduTime.Domain.Infrastructure;
using DigitalSkynet.DotnetCore.DataAccess.Repository;

namespace DigitalSkynet.Boilerplate.Data.Repositories
{
    public abstract class BaseRepository<T> : GenericRepository<AppDbContext, T, Guid>
        where T : BaseGuidEntity
    {
        protected BaseRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }
    }
}
