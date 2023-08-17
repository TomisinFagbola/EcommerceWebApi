using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Product : AuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string ImageFile { get; set; }
        public decimal Price { get; set; }

       //Navigational Property
       public Category Category { get; set; }

       public ShoppingCartItem ShoppingCartItem { get; set; }
        public Discount Discount { get; set; }

       //Foreign Key
       public Guid CategoryId { get; set; }

    }
}
