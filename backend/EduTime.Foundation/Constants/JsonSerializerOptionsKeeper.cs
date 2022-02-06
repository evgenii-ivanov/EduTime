using System.Text.Json;

namespace EduTime.Foundation.Constants
{
    public static class JsonSerializerOptionsKeeper
    {
        public static readonly JsonSerializerOptions Options =
            new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                WriteIndented = false
            };
    }
}
