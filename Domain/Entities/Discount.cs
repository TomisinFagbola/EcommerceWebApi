using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Discount : AuditableEntity
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }


        //Foreign key Property
        public Guid ProductId { get; set; }

        //Navigation Property
        public Product Product { get; set; }
    }
}
