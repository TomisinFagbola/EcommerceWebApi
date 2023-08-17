using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DataTransferObjects
{
    #region User Request Dtos

    public record UserLoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }

    }

    public record RefreshTokenDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public record ResetPasswordDto
    {
        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

    }

    public record UserCreateDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
    public record ForgotPasswordDto
    {
        public string Email { get; set; }
    }

    #endregion

    public record AuthDto
    {
        public string AccessToken { get; set; }
        public DateTime? ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
    }


    public record UserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public bool Verified { get; set; }
        public string Role { get; set; }
    }

    public class GetConifrmedTokenUserDto
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class GetSetPasswordDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Picture { get; set; }
    }

    public class VerifyTokenDto
    {
        public string Token { get; set; }
    }

    public class RefreshTokenResponse
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public DateTime? ExpiresIn { get; set; }
    }

    public record LoggedinUserDto
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
    public class TokenReturnHelper
    {
        public string AccessToken { get; set; }
        public DateTime? ExpiresIn { get; set; }
    }

}
