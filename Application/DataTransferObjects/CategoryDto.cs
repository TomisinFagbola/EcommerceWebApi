using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DataTransferObjects
{
    public record CategoryDto : AuditableDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<ProductDto> Products { get; set; }
        public CatalogueDto Catalogue { get; set; }

        public Guid CatalogueId { get; set; }
    }

    public record CategoryCreateDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Guid CatalogueId { get; set; }
    }
    
    public record CategoryUpdateDto : CategoryCreateDto
    {
    }
}
