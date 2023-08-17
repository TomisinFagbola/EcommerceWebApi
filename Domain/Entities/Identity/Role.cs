using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Identity
{
    public class Role : IdentityRole<Guid>
    {
        public Role() : base()
        { }

        // add extra special column/properties here

    }
}
