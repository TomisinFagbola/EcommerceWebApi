using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DataTransferObjects
{
    public record ShoppingCartItemDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Quantity { get; set; }

        public ProductDto Product { get; set; }
    }


    public record ShoppingCartDto
    {
        public Guid Id { get; set; }

        public decimal TotalAmount { get; set; }

        public List<ShoppingCartItemDto> Items { get; set; }

        public Guid UserId { get; set; }
    }
}
