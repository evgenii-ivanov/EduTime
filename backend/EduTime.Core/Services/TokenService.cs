using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EduTime.Core.Interfaces;
using EduTime.Foundation.Helpers.Time;
using EduTime.Foundation.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EduTime.Core.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly IDateTimeHelper _dateTimeHelper;

        public TokenService(IOptions<JwtOptions> jwtOptions, IDateTimeHelper dateTimeHelper)
        {
            _jwtOptions = jwtOptions.Value;
            _dateTimeHelper = dateTimeHelper;
        }

        /// <summary>
        /// Gets the access token lifetime
        /// </summary>
        /// <returns></returns>
        public TimeSpan AccessTokenLifeTime => _jwtOptions.GetAccessLifetime();

        /// <summary>
        /// Gets the refresh token lifetime
        /// </summary>
        /// <returns></returns>
        public TimeSpan RefreshTokenLifeTime => _jwtOptions.GetRefreshLifetime();

        public string GenerateToken(IEnumerable<Claim> claims, TimeSpan lifetime)
        {
            var now = _dateTimeHelper.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                notBefore: now,
                claims: claims,
                expires: now.Add(lifetime),
                signingCredentials: new SigningCredentials(_jwtOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }

        public ClaimsPrincipal GetPrincipalFromToken(string token, bool validateLifetime = true)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = true,
                ValidAudience = _jwtOptions.Audience,
                ValidIssuer = _jwtOptions.Issuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _jwtOptions.GetSymmetricSecurityKey(),
                ValidateLifetime = validateLifetime
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Refresh token not found"); // TODO: i18n

            return principal;
        }

        /// <summary>
        /// Generates the refresh token
        /// </summary>
        /// <returns>Refresh token randomized string</returns>
        public string GenerateRefreshToken()
        {
            var refreshToken = GenerateToken(new List<Claim>(), RefreshTokenLifeTime);
            return refreshToken;
        }
    }
}
