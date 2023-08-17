using Domain.Common;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities.Identity
{
    public class User : IdentityUser<Guid>, IAuditableEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Verified { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public bool Disabled { get; set; } = false;
        public DateTimeOffset LastLogin { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public ICollection<UserActivity> UserActivities { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public Guid? CreatedById { get; set; }
    }
}
