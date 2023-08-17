using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DataTransferObjects
{
    public record CatalogueDto : AuditableDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<CategoryDto> Categories { get; set; }
    }

    public record CatalogueCreateDto 
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public record CatalogueUpdateDto : CatalogueCreateDto
    {

    }
    
}
