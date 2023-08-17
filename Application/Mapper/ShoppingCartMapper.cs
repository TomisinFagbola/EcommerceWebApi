using Application.DataTransferObjects;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapper
{
    public class ShoppingCartMapper : Profile
    {
        public ShoppingCartMapper()
        {
            CreateMap<ShoppingCartItem, ShoppingCartItemDto>();
            CreateMap<ShoppingCart, ShoppingCartDto>();
        }
    }
}
