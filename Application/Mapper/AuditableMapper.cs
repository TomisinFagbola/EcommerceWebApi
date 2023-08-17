using Application.DataTransferObjects;
using AutoMapper;
using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapper
{
    public class AuditableMapper : Profile
    {
        public AuditableMapper()
        {
            CreateMap<AuditableEntity, AuditableDto>();
        }
    }
}
