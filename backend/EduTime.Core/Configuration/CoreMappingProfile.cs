using AutoMapper;
using EduTime.Domain.Entities.FileStorage;
using EduTime.Domain.Entities.Identity;
using EduTime.ViewModels.Session;
using EduTime.ViewModels.Storage;

namespace EduTime.Core.Configuration
{
    public class CoreMappingProfile : Profile
    {
        public CoreMappingProfile()
        {
            // Core mapping profile contains the following types of mappings:
            // DTO -> VM
            // Model -> Entity
            CreateMap<SignInModel, User>()
                .ForMember(x => x.UserName, _ => _.MapFrom(x => x.Username))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<StorageObject, StorageObjectVm>();
        }
    }
}
