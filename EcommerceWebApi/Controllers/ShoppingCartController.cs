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
    [Route("api/v{version:apiVersion}/shoppingcart")]
    public class ShoppingCartController : BaseController
    {
        private readonly IServiceManager _service;

        public ShoppingCartController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>
        /// Endpoint to add item to shopping cart
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "REGULAR, ADMIN")]
        [HttpPost("add")]
        [ProducesResponseType(typeof(SuccessResponse<ShoppingCartItemDto>), 200)]
        public async Task<IActionResult> AddToCart(Guid id)
        {
            var response = await _service.ShoppingCartService.AddToCart(LoggedInUser, id);
            return Ok(response);
        }



        /// <summary>
        /// Endpoint to get shoppingcart Item by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "REGULAR, ADMIN")]
        [HttpGet("items/{id}")]
        [ProducesResponseType(typeof(SuccessResponse<ProductDto>), 200)]
        public async Task<IActionResult> GetShoppingCartItem(Guid id)
        {
            var response = await _service.ShoppingCartService.GetItemById(id);
            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get all shopping cart items
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [Authorize(Roles = "REGULAR, ADMIN")]
        [HttpGet("items")]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<ShoppingCartDto>>), 200)]
        public async Task<IActionResult> GetAllShoppingCartItems([FromQuery] ShoppingCartParameter parameters)
        {
            var response = await _service.ShoppingCartService.ShoppingCartItems(parameters, LoggedInUser, nameof(GetAllShoppingCartItems), Url);
            return Ok(response);
        }

        // <summary>
        /// Endpoint to remove shopping cart item
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [Authorize(Roles = "REGULAR, ADMIN")]
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> RemoveItemFromCart(Guid id)
        {
            await _service.ShoppingCartService.RemoveItemFromCart(LoggedInUser, id);
            return NoContent();
        }
    }
}
