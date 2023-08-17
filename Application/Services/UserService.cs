using Application.Contracts;
using Application.DataTransferObjects;
using Application.Helpers;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Identity;
using Domain.Enums;
using Infrastructure.Contracts;
using Infrastructure.Entities.Identity;
using Infrastructure.Utils.Email;
using Infrastructure.Utils.Logger;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
        public class UserService : IUserService
        {
            private readonly IRepositoryManager _repository;
            private readonly UserManager<User> _userManager;
            private readonly RoleManager<Role> _roleManager;
            private readonly IEmailManager _emailManager;
            private readonly IMapper _mapper;
            private readonly ILoggerManager _logger;
            private readonly IConfiguration _configuration;


            public UserService(IRepositoryManager repository,
                UserManager<User> userManager,
                RoleManager<Role> roleManager,
                IEmailManager emailManager,
                IMapper mapper,
                ILoggerManager logger,
                IConfiguration configuration,
                IServiceManager service)
            {
                _repository = repository;
                _userManager = userManager;
                _roleManager = roleManager;
                _emailManager = emailManager;
                _mapper = mapper;
                _logger = logger;
                _configuration = configuration;
                
            }
        public async Task<SuccessResponse<UserDto>> Register(UserCreateDto model)
        {
            var email = model.Email?.Trim().ToLower();
            var isEmailExist = await _repository.User.ExistsAsync(x => x.Email == email);

            if (isEmailExist)
                throw new RestException(HttpStatusCode.BadRequest, "Email address already exists.");

            var user = _mapper.Map<User>(model);
            user.Status = EUserStatus.PENDING.ToString();
            user.IsActive = false;
            user.Verified = false;
            user.EmailConfirmed = false;

            int comparison = string.Compare(model.Password, model.ConfirmPassword, StringComparison.OrdinalIgnoreCase);
            if (comparison != 0)
                throw new RestException(HttpStatusCode.BadRequest, "Passwords do not match");

            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.Password);
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                throw new RestException(HttpStatusCode.BadRequest, "One Uppercase, One Lower case and One Digit Required or Email is Required");

            var userActivity = new UserActivity
            {
                EventType = "Password set",
                UserId = user.Id,
                Details = "set password",
                ObjectClass = "USER",
                ObjectId = user.Id
            };
            await _repository.UserActivity.AddAsync(userActivity);

            await _userManager.AddToRoleAsync(user, ERole.REGULAR.ToString());

            var userWithSentOtp = await SendOtp(user);

            await _repository.SaveChangesAsync();

            var userResponse = _mapper.Map<UserDto>(userWithSentOtp);

            return new SuccessResponse<UserDto>
            {
                Data = userResponse
            };
        }


        public async Task<User> SendOtp(User model)
        {
            //generate otp
            var otp = new Token
            {
                Value = CustomToken.GenerateOtp(),
                UserId = model.Id,
                TokenType = ETokenType.InviteUser.ToString()
            };
            await _repository.Token.AddAsync(otp);


            //send otp to email
            var message = _emailManager.GetSendOtpTemplate(otp.Value);
            string subject = "Send Otp";
            await _emailManager.SendSingleMail(otp.User.Email, message, subject);

            var userActivity = new UserActivity
            {
                EventType = "Send Otp",
                UserId = model.Id,
                ObjectClass = "USER",
                Details = "Send Otp",
                ObjectId = model.Id
            };

            await _repository.UserActivity.AddAsync(userActivity);
            await _repository.SaveChangesAsync();
            return otp.User;
        }
    }
}
