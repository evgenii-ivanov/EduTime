using DigitalSkynet.Boilerplate.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTime.Core.Interfaces
{
    public interface IDebugService
    {
        Task<DebugModel> GetDebugModelAsync();
        string GetDebugRepositoryException();
    }
}
