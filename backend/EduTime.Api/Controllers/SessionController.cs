using EduTime.Core.Interfaces;
using DigitalSkynet.DotnetCore.DataStructures.Models.Response;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using EduTime.Foundation.Enums;
using EduTime.ViewModels.Session;
using EduTime.ViewModels.Token;

namespace EduTime.Api.Controllers
{
    public class SessionController : AppBaseController
    {
        private readonly ISessionService _sessionService;

        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        /// <summary>
        /// Logs the user in
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPost("SignIn")]
        public async Task<ActionResult<ApiResponseEnvelope<TokenVm>>> SignInAsync([FromBody] SignInModel model, CancellationToken ct = default)
        {
            var result = await _sessionService.SignInAsync(model, ct);
            return ResponseModel(result);
        }

        /// <summary>
        /// Retuns token refreshed
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <param name="tokenVm"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPost("Refresh")]
        public async Task<ActionResult<ApiResponseEnvelope<TokenVm>>> RefreshAsync([FromBody] TokenVm tokenVm, CancellationToken ct = default)
        {
            var result = await _sessionService.RefreshAsync(tokenVm.RefreshToken, ct);
            return ResponseModel(result);
        }

        /// <summary>
        /// Logs out
        /// </summary>
        /// <param name="tokenVm"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPost("Logout")]
        public async Task<ActionResult<ApiResponseEnvelope<bool>>> Logout([FromBody] TokenVm tokenVm, CancellationToken ct = default)
        {
            await _sessionService.LogoutAsync(tokenVm.RefreshToken, ct);
            return ResponseModel(true);
        }

        /// <summary>
        /// Redirects the user to the external login provider page
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet("External/{provider}")]
        [Produces(typeof(ChallengeResult))]
        public IActionResult ExternalLogin(ExternalAuthProviders provider, [FromQuery] string returnUrl = "/")
        {
            var redirectUrl = $"{Request.Scheme}://{Request.Host}/api/Session/External/Callback?returnUrl={WebUtility.UrlEncode(returnUrl)}";
            var properties = _sessionService.SignInExternal(provider, redirectUrl);
            return Challenge(properties, provider.ToString());
        }

        [HttpGet("External/Callback")]
        [Produces(typeof(ApiResponseEnvelope<TokenVm>))]
        [ProducesResponseType(302)]
        public async Task<IActionResult> SignInExternalCallback([FromQuery] string returnUrl, CancellationToken ct = default)
        {
            var tokenVm = await _sessionService.HandleExternalSignInAsync(ct);
            if (tokenVm == null)
                return Redirect(returnUrl);

            return Ok(new ApiResponseEnvelope<TokenVm>(tokenVm));
        }

        [HttpPost("Register")]
        public async Task<ActionResult<ApiResponseEnvelope<TokenVm>>> RegisterAsync([FromBody] RegisterModel model,
            CancellationToken ct = default)
        {
            var tokenVm = await _sessionService.RegisterAsync(model, ct);
            return ResponseModel(tokenVm);
        }
    }
}
