using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DataTransferObjects
{
    public record ProductDto : AuditableDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string ImageFile { get; set; }
        public decimal Price { get; set; }
        public DiscountDto Discount {get; set;}
   
        public CategoryDto Category { get; set;} 
    }

    public record ProductCreateDto
    {
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public IFormFile ImageFile { get; set; }
        public decimal Price { get; set; }
        public Guid CategoryId { get; set; }
    }

    public record ProductUpdateDto : ProductCreateDto
    {
    }

}
