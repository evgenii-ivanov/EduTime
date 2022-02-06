using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EduTime.Foundation.Context;
using EduTime.ViewModels.Options;

namespace EduTime.Core.Interfaces
{
    public interface ISettingService
    {
        Task<List<SettingVm>> GetAllPublicSettings(IExecutionContext executionContext, CancellationToken ct);

        Task<TSetting> Get<TSetting>(IExecutionContext executionContext, string name, TSetting defaultValue = default,
            CancellationToken ct = default);

        Task Set<TSetting>(IExecutionContext executionContext, string name, TSetting value,
            bool isPublic = false, CancellationToken ct = default);
    }
}
