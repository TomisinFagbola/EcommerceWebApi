using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum ERole
    {
        [Description("REGULAR")]
        REGULAR = 1,
        [Description("ADMIN")]
        ADMIN = 2
    }
}
