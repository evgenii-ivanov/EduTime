using System.Threading;
using System.Threading.Tasks;

namespace EduTime.Core.Interfaces.Export
{
    public interface IFileExporter
    {
        string FileName { get; }
        string ContentType { get; }

        Task<byte[]> GetBytes(CancellationToken ct);
    }
}
