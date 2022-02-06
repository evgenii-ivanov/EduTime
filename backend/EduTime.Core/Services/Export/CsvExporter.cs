using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;

namespace EduTime.Core.Services.Export
{
    public class CsvExporter<TData> : AFileExporter<TData>
    {
        public CsvExporter(string fileName, IEnumerable<TData> dataCollection)
            : base(dataCollection)
        {
            FileName = fileName;
        }

        public override string FileName { get; }
        public override string ContentType => "text/csv";

        public override Task<byte[]> GetBytes(CancellationToken ct)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, Encoding.UTF8);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(Data);
            writer.Flush();
            return Task.FromResult(memoryStream.ToArray());
        }
    }
}
