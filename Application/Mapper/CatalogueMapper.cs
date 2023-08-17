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
    public class CatalogueMapper : Profile
    {
        public CatalogueMapper()
        {
            CreateMap<CatalogueCreateDto, Catalogue>().ReverseMap();
            CreateMap<CatalogueUpdateDto, Catalogue>().ReverseMap();
            CreateMap<Catalogue, CatalogueDto>().ReverseMap();
           
        }
    }
}
