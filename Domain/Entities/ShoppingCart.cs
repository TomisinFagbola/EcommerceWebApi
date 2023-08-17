using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ShoppingCart : AuditableEntity
    {
        public ShoppingCart()
        {
            Items = new List<ShoppingCartItem>();
        }
        public Guid Id { get; set; } = Guid.NewGuid();

        public decimal TotalAmount { get; set; }

        public List<ShoppingCartItem> Items { get; set; }
        
        public Guid UserId { get; set; }

    }
}
