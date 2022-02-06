using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EduTime.Dtos.Settings;

namespace EduTime.Core.Extensions
{
    public static class SettingExtensions
    {
        public static Dictionary<string, SettingDto> ToMap(this IEnumerable<SettingDto> settings)
            => settings
                .Where(x => !x.IsDeleted)
                .GroupBy(x => x.Name)
                .Select(x => x
                    // TODO: Put your logic of overriding settings here
                    // E.g. tenant-level settings have greater priority than global ones
                    // .OrderByDescending(x => x.)
                    .First())
                .ToDictionary(x => x.Name, x => x);
    }
}
