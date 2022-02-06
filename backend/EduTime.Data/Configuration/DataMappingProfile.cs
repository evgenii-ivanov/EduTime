using AutoMapper;
using EduTime.Domain.Entities;
using EduTime.Domain.Entities.FileStorage;
using EduTime.Domain.Infrastructure;
using EduTime.Dtos;
using EduTime.Dtos.FileStorage;
using EduTime.Dtos.Session;
using EduTime.Dtos.Settings;

namespace DigitalSkynet.Boilerplate.Data.Configuration
{
    public class DataMappingProfile : Profile
    {
        public DataMappingProfile()
        {
            // Data mapping profile contains the following types of mappings:
            // Entity -> Dto
            // Dto -> Entity

            
            CreateMap<BaseGuidEntity, BaseDto>();

            CreateMap<Session, SessionDto>()
                .IncludeBase<BaseGuidEntity, BaseDto>();

            CreateMap<Setting, SettingDto>()
                .IncludeBase<BaseGuidEntity, BaseDto>();

            CreateMap<StorageObject, FileStorageDto>()
                .IncludeBase<BaseGuidEntity, BaseDto>();
        }
    }
}
