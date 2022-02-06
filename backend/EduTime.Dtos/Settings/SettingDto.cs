using System.Text.Json;
using EduTime.Foundation.Constants;

namespace EduTime.Dtos.Settings
{
    public class SettingDto : BaseDto
    {
        public string Name { get; set; }
        public bool IsPublic { get; set; }
        public string Value { get; set; }

        // TODO: Add any kind of discriminator from execution context if needed. E.g., tenant uid.

        public T Get<T>() => JsonSerializer.Deserialize<T>(Value, JsonSerializerOptionsKeeper.Options);
    }
}
