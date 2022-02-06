using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace EduTime.Foundation.Options
{
    public class JwtOptions
    {
        public string Secret { get; set; }
        public string AccessLifetime { get; set; }
        public string RefreshLifetime { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }

        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
        }

        public TimeSpan GetAccessLifetime() =>
            TimeSpan.TryParse(AccessLifetime, out var result) ? result : TimeSpan.FromMinutes(5);

        public TimeSpan GetRefreshLifetime() =>
            TimeSpan.TryParse(RefreshLifetime, out var result) ? result : TimeSpan.FromDays(14);
    }
}
