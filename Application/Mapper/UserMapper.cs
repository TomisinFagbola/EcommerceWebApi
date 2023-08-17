using Application.DataTransferObjects;
using AutoMapper;
using Infrastructure.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapper
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<UserCreateDto, User>().AfterMap((src, dest) =>
            {
                dest.Email = src.Email.Trim().ToLower();
                dest.UserName = src.Email.Trim().ToLower();
                dest.SecurityStamp = Guid.NewGuid().ToString();
            });
            CreateMap<User, AuthDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}