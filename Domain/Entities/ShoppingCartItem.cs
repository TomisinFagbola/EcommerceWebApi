using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ShoppingCartItem : AuditableEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Quantity { get; set; }
    
        //public DateTimeOffset AddedAt { get; set; } = DateTimeOffset.Now;

        //Foreign Key
        public Guid ShoppingCartId { get; set; }
        public Guid ProductId { get; set; }
        //Navigational Properties
        public ShoppingCart ShoppingCart { get; set; }
        public Product Product { get; set; }
    }
}
