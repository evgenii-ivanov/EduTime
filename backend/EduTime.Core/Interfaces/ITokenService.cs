using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace EduTime.Core.Interfaces
{
    public interface ITokenService
    {
        ClaimsPrincipal GetPrincipalFromToken(string token, bool validateLifetime = true);
        string GenerateToken(IEnumerable<Claim> claims, TimeSpan lifetime);

        string GenerateRefreshToken();

        TimeSpan AccessTokenLifeTime { get; }
        TimeSpan RefreshTokenLifeTime { get; }
    }
}
