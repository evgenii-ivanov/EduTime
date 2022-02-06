using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using EduTime.Core.Interfaces.Export;

namespace EduTime.Core.Services.Export
{
    public class ArchiveExporter : IFileExporter
    {
        private readonly ICollection<IFileExporter> _exporters;

        public ArchiveExporter(string filename, params IFileExporter[] exporters)
        {
            _exporters = exporters;
            FileName = filename;
        }

        public string FileName { get; }

        public string ContentType => "application/zip";

        public async Task<byte[]> GetBytes(CancellationToken ct)
        {
            await using var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create))
            {
                foreach (var exporter in _exporters)
                {
                    var file = await exporter.GetBytes(ct);

                    var entry = archive.CreateEntry(exporter.FileName);
                    await using var entryStream = entry.Open();
                    await entryStream.WriteAsync(file, 0, file.Length, ct);
                }
            }

            return memoryStream.ToArray();
        }
    }
}
