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
    public class DiscountMapper : Profile
    {
        public DiscountMapper()
        {
            CreateMap<DiscountCreateDto, Discount>();
            CreateMap<Discount, DiscountDto>();
        }
    }
}
