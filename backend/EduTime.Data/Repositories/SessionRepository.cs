using AutoMapper;
using DigitalSkynet.Boilerplate.Data.Interfaces;
using EduTime.Domain.Entities;
using EduTime.Domain.Entities.Identity;
using EduTime.ViewModels.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalSkynet.Boilerplate.Data.Repositories
{
    public class SessionRepository : BaseRepository<Session>, ISessionRepository
    {
        public SessionRepository(AppDbContext dbContext, IMapper mapper) : base(dbContext, mapper) { }
    }
}
