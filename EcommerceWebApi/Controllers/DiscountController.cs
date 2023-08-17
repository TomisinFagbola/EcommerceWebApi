using API.Controllers.CommonController;
using Application.Contracts;
using Application.DataTransferObjects;
using Application.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/discounts")]
    public class DiscountController : ControllerBase
    {
        private readonly IServiceManager _service;

        public DiscountController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>
        /// Endpoint to create discount
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "ADMIN")]
        [HttpPost()]
        [ProducesResponseType(typeof(SuccessResponse<DiscountDto>), 200)]
        public async Task<IActionResult> CreateDiscount([FromBody] DiscountCreateDto model)
        {
            var response = await _service.DiscountService.CreateDiscount(model);
            return Ok(response);
        }

        /// <summary>
        /// Endpoint to Update discount
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<DiscountDto>), 200)]
        public async Task<IActionResult> UpdateDiscount(Guid id, [FromBody] DiscountUpdateDto model)
        {
            var response = await _service.DiscountService.Update(id, model);
            return Ok(response);
        }



        // <summary>
        /// Endpoint to remove discount
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> RemoveDiscount(Guid Id)
        {
            await _service.ProductService.Remove(Id);
            return NoContent();
        }

    }
}