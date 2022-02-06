using EduTime.Core.Identity;
using DigitalSkynet.Boilerplate.Data.Interfaces;
using DigitalSkynet.Boilerplate.Data.Repositories;
using EduTime.Domain.Entities.Identity;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EduTime.Core.Interfaces;

namespace EduTime.Core.Services
{
    public class DebugService : IDebugService
    {
        private readonly IDebugRepository _debugRepository;
        private readonly IStringLocalizer<DebugService> _stringLocalizer;
        public DebugService(IDebugRepository debugRepository, IStringLocalizer<DebugService> stringLocalizer)
        {
            _debugRepository = debugRepository;
            _stringLocalizer = stringLocalizer;
        }

        public async Task<DebugModel> GetDebugModelAsync()
        {
            var debugModel = _debugRepository.GetDebugModel();
            await Task.CompletedTask;
            return debugModel;
        }

        public string GetDebugRepositoryException()
        {
            throw new InvalidOperationException(_stringLocalizer["This operation is incorrect"].Value);
        }

    }
}
