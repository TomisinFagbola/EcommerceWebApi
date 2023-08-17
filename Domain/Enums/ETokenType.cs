using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum ETokenType
    {

        [Description("Create New User")]
        InviteUser,
        [Description("Reset Password")]
        ResetPassword,
        [Description("Phone Confirmation")]
        PhoneNumberConfirmation
    }
}
