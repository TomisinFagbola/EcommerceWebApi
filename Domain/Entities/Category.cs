using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Category : AuditableEntity
    {
        public Category()
        {
         Products = new List<Product>();   
        }
        // Properties
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<Product> Products { get; set; }
  

        //Naviational Properties
        public Catalogue Catalogue { get; set; }
        //Foreign Keys
        public Guid CatalogueId { get; set; }


    }
}
