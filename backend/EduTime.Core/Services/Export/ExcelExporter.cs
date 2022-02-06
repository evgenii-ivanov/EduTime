using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;

namespace EduTime.Core.Services.Export
{
    public class ExcelExporter<TData> : AFileExporter<TData>
    {
        public ExcelExporter(string fileName, IEnumerable<TData> dataCollection)
            : base(dataCollection)
        {
            FileName = fileName;
        }

        public override string FileName { get; }

        public override string ContentType => "application/vnd.ms-excel";

        public override Task<byte[]> GetBytes(CancellationToken ct)
        {
            var data = ConvertToDataTable(Data.ToList());

            using var wb = new XLWorkbook();
            wb.Worksheets.Add(data,"Main");

            using var memoryStream = new MemoryStream();
            wb.SaveAs(memoryStream);
            return Task.FromResult(memoryStream.ToArray());
        }

        private static DataTable ConvertToDataTable<T>(IEnumerable<T> data)
        {
            var dataTable = new DataTable(typeof(T).Name);
            //Get all the properties by using reflection
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (var item in data)
            {
                var values = new object[props.Length];
                for (var i = 0; i < props.Length; i++)
                {

                    values[i] = props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
    }
}
