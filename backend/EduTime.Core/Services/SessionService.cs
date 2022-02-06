using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using DigitalSkynet.Boilerplate.Data.Interfaces;
using EduTime.Domain.Entities;
using EduTime.Domain.Entities.Identity;
using EduTime.ViewModels.Session;
using EduTime.ViewModels.Token;
using DigitalSkynet.DotnetCore.DataAccess.UnitOfWork;
using DigitalSkynet.DotnetCore.DataStructures.Exceptions.Api;
using DigitalSkynet.DotnetCore.DataStructures.Validation;
using EduTime.Core.Identity;
using EduTime.Core.Interfaces;
using EduTime.Foundation.Constants;
using EduTime.Foundation.Enums;
using EduTime.Foundation.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;

namespace EduTime.Core.Services
{
    public class SessionService : ISessionService
    {
        private readonly AppSignInManager _signInManager;
        private readonly AppUserManager _appUserManager;
        private readonly ITokenService _tokenService;
        private readonly JwtOptions _jwtOptions;
        private readonly ISessionRepository _sessionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFeatureManager _featureManager;

        public SessionService(AppSignInManager signInManager,
            AppUserManager appUserManager,
            ITokenService tokenService,
            IOptions<JwtOptions> jwtOptions,
            ISessionRepository sessionRepository,
            IUnitOfWork unitOfWork,
            IFeatureManager featureManager)
        {
            _signInManager = signInManager;
            _appUserManager = appUserManager;
            _tokenService = tokenService;
            _jwtOptions = jwtOptions.Value;
            _unitOfWork = unitOfWork;
            _featureManager = featureManager;
            _sessionRepository = sessionRepository;
        }

        public async Task<TokenVm> SignInAsync(SignInModel signInModel, CancellationToken ct)
        {
            var user = await _appUserManager.FindByNameAsync(signInModel.Username);
            if (user == null)
                throw new ApiNotAuthenticatedException("Username or password are incorrect");

            var isPasswordCorrect = await _appUserManager.CheckPasswordAsync(user, signInModel.Password);
            if (!isPasswordCorrect)
                throw new ApiNotAuthenticatedException("Username or password are incorrect");

            var sessionId = await CreateSession(user, ct);
            var tokens = await GetTokenPair(user, sessionId, ct);
            return tokens;
        }

        public AuthenticationProperties SignInExternal(ExternalAuthProviders provider, string redirectUrl)
        {
            return _signInManager.ConfigureExternalAuthenticationProperties(provider.ToString(), redirectUrl);
        }

        public async Task<TokenVm> HandleExternalSignInAsync(CancellationToken ct)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info?.Principal == null)
                return null;

            var email = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var user = await _appUserManager.FindByEmailAsync(email);
            if (user == null)
            {
                if (!await _featureManager.IsEnabledAsync(EnabledFeatures.AutoRegistration))
                    return null;

                user = new User
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };
                // TODO: add to roles
                var creationResult = await _appUserManager.CreateAsync(user);
                if (!creationResult.Succeeded)
                    return null;
                await _appUserManager.AddLoginAsync(user, info);
            }

            var signin = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            if (!signin.Succeeded && !await _featureManager.IsEnabledAsync(EnabledFeatures.AutoLogin))
                return null;

            var sessionId = await CreateSession(user, ct);
            var tokenVm = await GetTokenPair(user, sessionId, ct);
            return tokenVm;
        }

        public async Task<TokenVm> RegisterAsync(RegisterModel model, CancellationToken ct)
        {
            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
            };
            var userCreationResult = await _appUserManager.CreateAsync(user, model.Password);
            if (!userCreationResult.Succeeded)
            {
                var validationResult = new ValidationResult();
                foreach (var error in userCreationResult.Errors)
                    validationResult.AddError(error.Description);
                validationResult.ThrowIfInvalid();
            }

            // TODO: Add user to roles
            // TODO: send confirmation email

            var sessionId = await CreateSession(user, ct);
            var tokenVm = await GetTokenPair(user, sessionId, ct);
            return tokenVm;
        }

        public async Task<TokenVm> RefreshAsync(string refreshToken, CancellationToken ct)
        {
            ClaimsPrincipal principal;
            try
            {
                principal = _tokenService.GetPrincipalFromToken(refreshToken);
            }
            catch
            {
                throw new ApiNotAuthenticatedException("User is unauthorized");
            }

            var sessionId = Guid.Parse(principal.FindFirstValue(JwtRegisteredClaimNames.Sid));
            var userSession = await _sessionRepository.FindFirstAsync(x => x.Id == sessionId, ct: ct);

            if (userSession == null || userSession.IsDeleted)
                throw new ApiNotAuthenticatedException("User is out of session");

            ++userSession.TokenVersion;
            await _unitOfWork.SaveChangesAsync(ct);

            var user = await _appUserManager.FindByIdAsync(userSession.UserId.ToString());
            if (user == null)
                throw new ApiNotAuthenticatedException("There is not such user registered");

            var tokens = await GetTokenPair(user, userSession.Id, ct);

            return tokens;
        }

        public async Task LogoutAsync(string refreshToken, CancellationToken ct)
        {
            var principal = _tokenService.GetPrincipalFromToken(refreshToken);
            var sessionId = Guid.Parse(principal.FindFirstValue(JwtRegisteredClaimNames.Sid));
            var userSession = await _sessionRepository.FindFirstAsync(x => x.Id == sessionId, ct: ct);

            if (userSession == null)
                throw new ApiNotAuthenticatedException("User is out of session");

            await _sessionRepository.DeleteHardAsync(userSession.Id, ct: ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task CleanUpExpiredSessionsAsync(Guid userUid, CancellationToken ct)
        {
            var expiredSessions = await _sessionRepository.FilterAsync(x => x.UserId == userUid && x.ExpiresAtUtc >= DateTime.UtcNow, ct: ct);
            var session = expiredSessions.FirstOrDefault();
            if (session != null)
            {
                await _sessionRepository.DeleteHardAsync(session.Id, ct: ct);
                await _unitOfWork.SaveChangesAsync(ct);
            }
        }

        private async Task<TokenVm> GetTokenPair(User user, Guid sessionId, CancellationToken ct)
        {
            var userClaims = await GetClaimsAsync(user, sessionId, ct).ToListAsync(ct);
            var sessionClaims = GetRefreshClaims(user, sessionId);

            var accessToken = _tokenService.GenerateToken(userClaims, _jwtOptions.GetAccessLifetime());
            var refreshToken = _tokenService.GenerateToken(sessionClaims, _jwtOptions.GetRefreshLifetime());

            return new TokenVm
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        private async IAsyncEnumerable<Claim> GetClaimsAsync(User user, Guid sessionId, [EnumeratorCancellation] CancellationToken ct)
        {
            yield return new Claim(JwtRegisteredClaimNames.Sid, sessionId.ToString());
            yield return new Claim(JwtRegisteredClaimNames.Email, user.Email);
            yield return new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName);
            yield return new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString());

            var roles = await _appUserManager.GetRolesAsync(user);
            foreach (var role in roles)
                yield return new Claim(JwtAdditionalClaimNames.Role, role);
        }

        private IEnumerable<Claim> GetRefreshClaims(User user, Guid sessionId)
        {
            yield return new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString());
            yield return new Claim(JwtRegisteredClaimNames.Sid, sessionId.ToString());
        }

        private async Task<Guid> CreateSession(User user, CancellationToken ct)
        {
            await CleanUpExpiredSessionsAsync(user.Id, ct);
            var session = new Session
            {
                ExpiresAtUtc = DateTime.UtcNow.Add(_jwtOptions.GetRefreshLifetime()),
                UserId = user.Id,
                TokenVersion = 1
            };
            _sessionRepository.Create(session);

            await _unitOfWork.SaveChangesAsync(ct);
            return session.Id;
        }
    }
}
