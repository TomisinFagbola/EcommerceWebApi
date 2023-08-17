using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Identity
{
    public class RoleClaim : IdentityRoleClaim<Guid>
    {

    }

    public class UserClaim : IdentityUserClaim<Guid>
    {
        public UserClaim() : base()
        { }
    }

    public class UserRole : IdentityUserRole<Guid>
    {
        public UserRole() : base()
        { }
    }
    public class UserLogin : IdentityUserLogin<Guid>
    {
        public UserLogin() : base()
        { }
    }
    public class UserToken : IdentityUserToken<Guid>
    {
    }
}
