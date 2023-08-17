using API.Controllers.CommonController;
using Application.Contracts;
using Application.DataTransferObjects;
using Application.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auths")]
    public class AuthsController : BaseController
    {
        private readonly IServiceManager _service;
        public AuthsController(IServiceManager service)
        {
            _service = service;
        }

        // <summary>Endpoint to login a user</summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(SuccessResponse<AuthDto>), 200)]
        public async Task<IActionResult> LoginUser(UserLoginDto model)
        {
            var response = await _service.AuthenticationService.Login(model);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to generate a new access and refresh token
        /// </summary>
        /// <param name="mdoel"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(SuccessResponse<RefreshTokenResponse>), 200)]
        public async Task<IActionResult> RefreshToken(RefreshTokenDto mdoel)
        {
            var response = await _service.AuthenticationService.GetRefreshToken(mdoel);
            return Ok(response);
        }


        /// <summary>
        /// Endpoint to reset password
        /// </summary>
        /// <param name="mdoel"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(SuccessResponse<object>), 200)]
        public async Task<IActionResult> ResetPassword([FromQuery]string token, ResetPasswordDto mdoel)
        {
            var response = await _service.AuthenticationService.ResetPassword(token, mdoel);

            return Ok(response);
        }



        /// <summary>
        /// Endpoint to verify otp
        /// </summary>
        /// <param name="otp"></param>
        /// <returns></returns>

        [AllowAnonymous]
        [HttpPost("verify-otp")]
        [ProducesResponseType(typeof(SuccessResponse<UserDto>), 200)]
        public async Task<IActionResult> ValidateOTP([FromForm] string otp)
        {
            var response = await _service.AuthenticationService.ValidateOTP(otp);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to initializes password reset
        /// </summary>
        /// <param name="mdoel"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        [ProducesResponseType(typeof(SuccessResponse<string>), 200)]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto mdoel)
        {
            var response = await _service.AuthenticationService.ForgotPassword(mdoel);

            return Ok(response);
        }
    }
}
