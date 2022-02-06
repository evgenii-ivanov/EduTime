using EduTime.ViewModels.Token;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Threading;
using System.Threading.Tasks;
using EduTime.Foundation.Enums;
using EduTime.ViewModels.Session;

namespace EduTime.Core.Interfaces
{
    public interface ISessionService
    {
        Task<TokenVm> SignInAsync(SignInModel signInModel, CancellationToken ct);
        Task<TokenVm> RefreshAsync(string refreshToken, CancellationToken ct);
        Task LogoutAsync(string refreshToken, CancellationToken ct);
        Task CleanUpExpiredSessionsAsync(Guid userUid, CancellationToken ct);
        AuthenticationProperties SignInExternal(ExternalAuthProviders provider, string redirectUrl);
        Task<TokenVm> HandleExternalSignInAsync(CancellationToken ct);
        Task<TokenVm> RegisterAsync(RegisterModel model, CancellationToken ct);
    }
}
