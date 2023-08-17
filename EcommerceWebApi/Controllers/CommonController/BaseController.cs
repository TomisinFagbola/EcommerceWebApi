using Application.DataTransferObjects;
using Application.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.CommonController
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        public LoggedinUserDto LoggedInUser => new()
        {
            UserId = WebHelper.UserId,
            FirstName = WebHelper.FirstName,
            LastName = WebHelper.LastName,
            Email = WebHelper.Email,
            PhoneNumber = WebHelper.PhoneNumber,
            Roles = WebHelper.Roles,
        };
    }
}
