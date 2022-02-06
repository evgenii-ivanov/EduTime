using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EduTime.Core.Extensions;
using DigitalSkynet.Boilerplate.Data.Interfaces;
using EduTime.Domain.Entities;
using EduTime.ViewModels.Options;
using DigitalSkynet.DotnetCore.DataAccess.Enums;
using DigitalSkynet.DotnetCore.DataAccess.UnitOfWork;
using EduTime.Core.Interfaces;
using EduTime.Dtos.Settings;
using EduTime.Foundation.Constants;
using EduTime.Foundation.Context;

namespace EduTime.Core.Services
{
    public class SettingService : ISettingService
    {
        private readonly ISettingRepository _settingRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public SettingService(ISettingRepository settingRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _settingRepository = settingRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SettingVm>> GetAllPublicSettings(IExecutionContext executionContext,
            CancellationToken ct = default)
        {
            var result =
                await _settingRepository.ProjectToAsync<SettingDto>(x => x.IsPublic, FetchModes.NoTracking, ct);
            return _mapper.Map<List<SettingVm>>(result.ToMap().Values);
        }

        public async Task<TSetting> Get<TSetting>(IExecutionContext executionContext, string name,
            TSetting defaultValue = default,
            CancellationToken ct = default)
        {
            var dtoList = await _settingRepository.ProjectToAsync<SettingDto>(x => x.Name == name, ct: ct);
            return dtoList.Count == 0 ? defaultValue : dtoList.ToMap()[name].Get<TSetting>();
        }

        public async Task Set<TSetting>(IExecutionContext executionContext, string name, TSetting value,
            bool isPublic = false, CancellationToken ct = default)
        {
            var existingEntity = await _settingRepository.FindFirstAsync(x => x.Name == name, // && executionContext
                FetchModes.Tracking, ct) ?? new Setting {Name = name};

            existingEntity.Value = JsonSerializer.Serialize(value, JsonSerializerOptionsKeeper.Options);
            existingEntity.IsPublic = isPublic;

            _settingRepository.Update(existingEntity);
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
