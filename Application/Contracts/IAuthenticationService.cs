using Application.DataTransferObjects;
using Application.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts
{
    public interface IAuthenticationService
    {
        Task<SuccessResponse<AuthDto>> Login(UserLoginDto model);
        Task<SuccessResponse<AuthDto>> GetRefreshToken(RefreshTokenDto model);
        Task<SuccessResponse<string>> ResetPassword(string Token, ResetPasswordDto model);
        Task<SuccessResponse<string>> ForgotPassword(ForgotPasswordDto model);
        Task<SuccessResponse<UserDto>> ValidateOTP(string otp);
    }
}
