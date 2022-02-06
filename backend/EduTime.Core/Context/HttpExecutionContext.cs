using System.Globalization;
using System.Text.Json.Serialization;
using EduTime.Foundation.Context;

namespace EduTime.Core.Context
{
    public class HttpExecutionContext : IExecutionContext
    {
        [JsonIgnore]
        public CultureInfo Culture { get; set; }
        public string CultureCode => Culture?.Name;
    }
}
