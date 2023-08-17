using API.Controllers.CommonController;
using Application.Contracts;
using Application.DataTransferObjects;
using Application.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/products")]
    public class ProductController : BaseController
    {
        private readonly IServiceManager _service;

        public ProductController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>
        /// Endpoint to create a product
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "ADMIN")]
        [HttpPost()]
        [ProducesResponseType(typeof(SuccessResponse<ProductDto>), 200)]
        public async Task<IActionResult> CreateProduct([FromForm] ProductCreateDto model)
        {
            var response = await _service.ProductService.Create(model);
            return Ok(response);
        }



        /// <summary>
        /// Endpoint to get product by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<ProductDto>), 200)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _service.ProductService.GetById(id);
            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get all products
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet(Name = nameof(GetAllProducts))]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<ProductDto>>), 200)]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductParameter parameter)
        {
            var response = await _service.ProductService.GetAll(parameter, nameof(GetAllProducts), Url);
            return Ok(response);
        }


        /// <summary>
        /// Endpoint to Update product
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<ProductDto>), 200)]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromForm] ProductUpdateDto model)
        {
            var response = await _service.ProductService.Update(id, model);
            return Ok(response);
        }


        // <summary>
        /// Endpoint to Remove product
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> RemoveProduct(Guid Id)
        {
            await _service.ProductService.Remove(Id);
            return NoContent();
        }



    }
}
