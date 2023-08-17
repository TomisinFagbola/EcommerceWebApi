using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum EUserStatus
    {
        [Description("ACTIVE")]
        ACTIVE = 1,
        [Description("PENDING")]
        PENDING = 2,
        [Description("DEACTIVATE")]
        DEACTIVATE = 3,
    }
}
