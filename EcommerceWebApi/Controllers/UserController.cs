using API.Controllers.CommonController;
using Application.Contracts;
using Application.DataTransferObjects;
using Application.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/users")]
    public class UserController : BaseController
    {
        private readonly IServiceManager _service;

        public UserController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>
        /// Endpoint to register a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        [ProducesResponseType(typeof(SuccessResponse<UserDto>), 200)]
        public async Task<IActionResult> RegisterUser([FromBody] UserCreateDto model)
        {
            var response = await _service.UserService.Register(model);

            return Ok(response);

        }

    }
}