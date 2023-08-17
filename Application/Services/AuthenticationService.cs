
using Application.Contracts;
using Application.DataTransferObjects;
using Application.Helpers;
using AutoMapper;
using Domain.ConfigurationModels;
using Domain.Entities;
using Domain.Entities.Identity;
using Domain.Enums;
using Infrastructure.Contracts;
using Infrastructure.Entities.Identity;
using Infrastructure.Utils.Email;
using Infrastructure.Utils.Logger;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IRepositoryManager _repository;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly string otpLifeSpan;
        private readonly IEmailManager _emailManager;
        private readonly JwtConfiguration _jwtConfiguration;
        public AuthenticationService(IRepositoryManager repository,
       UserManager<User> userManager,
        IEmailManager emailManager,
       IMapper mapper,
       ILoggerManager logger,
       IConfiguration configuration,
        IServiceManager service)
        {
            _configuration = configuration;
            _userManager = userManager;
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _emailManager = emailManager;
            _jwtConfiguration = new JwtConfiguration();
            _configuration.Bind(_jwtConfiguration.Section, _jwtConfiguration);
            var jwtSettings = configuration.GetSection("JwtSettings");
            otpLifeSpan = jwtSettings.GetSection("OtpLifeSpan").Value;
        }


        public async Task<SuccessResponse<AuthDto>> Login(UserLoginDto model)
        {
            //query organization by subdomain
            var email = model.Email.Trim().ToLower();
            var user = await _userManager.FindByEmailAsync(email);
            var authenticated = await ValidateUser(user, model.Password);

            if (!authenticated)
                throw new RestException(HttpStatusCode.Unauthorized, "Wrong Email or Password");

            if (!user.Verified || !user.IsActive)
                throw new RestException(HttpStatusCode.Unauthorized, "User is inactive");

            user.LastLogin = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            var userActivity = new UserActivity
            {
                EventType = "User Login",
                UserId = user.Id,
                ObjectClass = "USER",
                Details = "logged in",
                ObjectId = user.Id
            };

            var roles = await _userManager.GetRolesAsync(user);

            await _repository.UserActivity.AddAsync(userActivity);
            await _repository.SaveChangesAsync();

            var tokenResponse = Authenticate(user, roles);

            return new SuccessResponse<AuthDto>
            {
                Message = "Login successful",
                Data = new AuthDto
                {
                    AccessToken = tokenResponse.AccessToken,
                    ExpiresIn = tokenResponse.ExpiresIn,
                    RefreshToken = GenerateRefreshToken(user.Id),
                }
            };
        }

        public async Task<SuccessResponse<AuthDto>> GetRefreshToken(RefreshTokenDto model)
        {
            var userId = GetUserIdFromAccessToken(model.AccessToken);

            var user = await _repository.User.GetByIdAsync(userId);
            if (user == null)
                throw new RestException(HttpStatusCode.NotFound, "User not found");

            var isRefreshTokenValid = ValidateRefreshToken(model.RefreshToken);
            if (!isRefreshTokenValid)
                throw new RestException(HttpStatusCode.NotFound, "Invalid token");

            var roles = await _userManager.GetRolesAsync(user);
            var tokenResponse = Authenticate(user, roles);

            var newRefreshToken = GenerateRefreshToken(user.Id);

            var tokenViewModel = new AuthDto
            {
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = newRefreshToken,
                ExpiresIn = tokenResponse.ExpiresIn
            };

            return new SuccessResponse<AuthDto>
            {
                Message = "Data retrieved successfully",
                Data = tokenViewModel
            };
        }

        public async Task<SuccessResponse<UserDto>> ValidateOTP(string otp)
        {

            var token = await _repository.Token.QueryAll(x => x.Value == otp && x.TokenType == ETokenType.InviteUser.ToString()).Include(x => x.User).FirstOrDefaultAsync();
            if (token is null)
                throw new RestException(HttpStatusCode.NotFound, "Invalid token");

            var expiryTime = double.Parse(otpLifeSpan);

            var isValid = CustomToken.IsTokenValid(token);
            if (!isValid)
            {
                _repository.Token.Remove(token);
                throw new RestException(HttpStatusCode.NotFound, "Invalid token");

            }

            if (token.TokenType == ETokenType.InviteUser.ToString())
            {
                token.User.IsActive = true;
                token.User.Status = EUserStatus.ACTIVE.ToString();
                token.User.EmailConfirmed = true;
                token.User.Verified = true;
            }

            _repository.User.Update(token.User);
            var userActivity = new UserActivity
            {
                EventType = "Otp Validated",
                UserId = token.User.Id,
                Details = "Otp Validated",
                ObjectClass = "USER",
                ObjectId = token.User.Id
            };
            await _repository.UserActivity.AddAsync(userActivity);

            var userDTO = _mapper.Map<UserDto>(token.User);

            _repository.Token.Remove(token);
            await _repository.SaveChangesAsync();


            return new SuccessResponse<UserDto>
            {
                Message = "Data retrieved successfully",
                Data = userDTO
            };
        }
        public async Task<SuccessResponse<string>> ForgotPassword(ForgotPasswordDto model)
        {

            var user = await _repository.User.FirstOrDefaultAsync(x => x.Email == model.Email);
            if (user == null)
                return new SuccessResponse<string>
                {
                    Message = "Email to reset password sent successfully",
                };

            var token = CustomToken.GenerateRandomString(128);
            var tokenEntity = new Token
            {
                UserId = user.Id,
                TokenType = ETokenType.ResetPassword.ToString(),
                Value = token
            };
            await _repository.Token.AddAsync(tokenEntity);

            var userActivity = new UserActivity
            {
                EventType = "Verify Account",
                UserId = tokenEntity.UserId,
                ObjectClass = "USER",
                Details = "Verify Account",
                ObjectId = tokenEntity.UserId
            };
            await _repository.UserActivity.AddAsync(userActivity);

            await _repository.SaveChangesAsync();

            string emailLink = $"{_configuration["CLIENT_URL"]}/reset-password?token={token}";
            var message = _emailManager.GetResetPasswordEmailTemplate(emailLink, user.Email);
            string subject = "ResetPassword";

             await _emailManager.SendSingleMail(user.Email, message, subject);

            return new SuccessResponse<string>
            {
                Message = "Email to reset password sent successfully",
            };
        }

        public async Task<SuccessResponse<string>> ResetPassword(string Token, ResetPasswordDto model)
        {
            var user = await ConfirmToken(Token);

            int comparison = string.Compare(model.Password, model.ConfirmPassword, StringComparison.OrdinalIgnoreCase);
            if (comparison != 0)
                throw new RestException(HttpStatusCode.BadRequest, "Passwords do not match");

            if (user == null)
                return new SuccessResponse<string>
                {
                    Message = "Password Reset Successful"
                };
            user.UpdatedAt = DateTime.UtcNow;
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.Password);

            var userActivity = new UserActivity
            {
                EventType = "Reset Password",
                UserId = user.Id,
                Details = "Reset password",
                ObjectClass = "USER",
                ObjectId = user.Id
            };
            await _repository.UserActivity.AddAsync(userActivity);

            await _repository.SaveChangesAsync();

            return new SuccessResponse<string>
            {
                Message = "Password Reset Successful",
            };
        }


        #region private methods
        private TokenReturnHelper Authenticate(User user, IList<string> roles)
        {
            var roleClaims = new List<Claim>();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypeHelper.Email, user.Email),
                new Claim(ClaimTypeHelper.UserId, user.Id.ToString()),
                new Claim(ClaimTypeHelper.FirstName, user.FirstName),
                new Claim(ClaimTypeHelper.LastName, user.LastName),
            };

            foreach (var role in roles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            claims.AddRange(roleClaims);

            var jwtSettings = _configuration.GetSection("JwtSettings");
            var jwtUserSecret = jwtSettings.GetSection("Secret").Value;
            var tokenExpireIn = string.IsNullOrEmpty(jwtSettings.GetSection("TokenLifespan").Value) ? int.Parse(jwtSettings.GetSection("TokenLifespan").Value) : 7;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtUserSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(tokenExpireIn),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            return new TokenReturnHelper
            {
                ExpiresIn = tokenDescriptor.Expires,
                AccessToken = jwt
            };
        }

        private async Task<bool> ValidateUser(User user, string password)
        {
            var result = (user != null && await _userManager.CheckPasswordAsync(user, password));
            if (!result)
                _logger.LogWarn($"{nameof(ValidateUser)}: Authentication failed, wrong email or password");

            if (user != null && !user.Verified)
            {
                _logger.LogWarn($"{nameof(ValidateUser)}: Authentication failed, User is not verified");
                return false;
            }
            return result;
        }

        private bool ValidateRefreshToken(string refreshToken)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var jwtUserSecret = jwtSettings.GetSection("Secret").Value;

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtUserSecret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(refreshToken, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            var expiryAt = jwtSecurityToken.ValidTo;
            if (DateTime.UtcNow > expiryAt)
                return false;
            return true;
        }

        private async Task<User> ConfirmToken(string Token)
        {
            var token = await _repository.Token.Get(x => x.Value == Token && x.TokenType == ETokenType.ResetPassword.ToString()).Include(x => x.User).FirstOrDefaultAsync();
            if (token == null)
                throw new RestException(HttpStatusCode.NotFound, "The token is invalid or has expired");

            if (DateTime.UtcNow >= token.ExpiresAt)
            {
                _repository.Token.Remove(token);
                await _repository.SaveChangesAsync();

                throw new RestException(HttpStatusCode.BadRequest, "Token is expired");
            }

            var user = await _repository.User.FirstOrDefaultAsync(x => x.Id == token.UserId);
            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest, "User not found");
            await _repository.SaveChangesAsync();
            return user;
        }

        private string GenerateRefreshToken(Guid userId)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var jwtUserSecret = jwtSettings.GetSection("Secret").Value;
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(jwtUserSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypeHelper.UserId, userId.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            return jwt;
        }

        private Guid GetUserIdFromAccessToken(string accessToken)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var jwtUserSecret = jwtSettings.GetSection("Secret").Value;

            var tokenValidationParamters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtUserSecret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParamters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                                                    StringComparison.InvariantCultureIgnoreCase))
            {
                throw new RestException(HttpStatusCode.BadRequest, "Invalid token");
            }

            var userId = principal.FindFirst(ClaimTypeHelper.UserId)?.Value;

            if (userId == null)
                throw new RestException(HttpStatusCode.BadRequest, $"MissingClaim: {ClaimTypeHelper.UserId}");

            return Guid.Parse(userId);
        }
        #endregion
    }
}
