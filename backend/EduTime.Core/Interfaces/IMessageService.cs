using System.Threading;
using System.Threading.Tasks;
using EduTime.Dtos.Messaging;

namespace EduTime.Core.Interfaces
{
    public interface IMessageService
    {
        Task<bool> SendAsync(MessageDto model, CancellationToken cancellationToken);
    }
}
