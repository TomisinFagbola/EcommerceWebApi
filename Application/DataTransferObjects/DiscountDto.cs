using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DataTransferObjects
{
    public record DiscountDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public ProductDto Product { get; set; }
        public Guid ProductId { get; set; }
    }

    public record DiscountCreateDto
    {
        public string Description { get; set; }
        public int Amount { get; set; }
        public Guid ProductId { get; set; }
    }
    
    public record DiscountUpdateDto : DiscountCreateDto 
    { 
    }
}
