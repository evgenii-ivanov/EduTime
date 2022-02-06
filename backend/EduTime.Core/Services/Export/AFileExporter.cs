using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EduTime.Core.Interfaces.Export;

namespace EduTime.Core.Services.Export
{
    public abstract class AFileExporter<TData> : IFileExporter
    {
        protected AFileExporter(IEnumerable<TData> dataCollection)
        {
            Data = dataCollection;
        }

        public abstract string FileName { get; }
        public abstract string ContentType { get; }

        public abstract Task<byte[]> GetBytes(CancellationToken ct);

        protected IEnumerable<TData> Data { get; }
    }
}
