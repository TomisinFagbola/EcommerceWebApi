using API.Controllers.CommonController;
using Application.Contracts;
using Application.DataTransferObjects;
using Application.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly IServiceManager _service;
        public CategoryController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>
        /// Endpoint to get all categories
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet(Name = nameof(GetAllCategories))]
        [ProducesResponseType(typeof(SuccessResponse<ICollection<CategoryDto>>), 200)]
        public async Task<IActionResult> GetAllCategories([FromQuery] CategoryParameter parameter)
        {
            var response = await _service.CategoryService.GetAllCategories(parameter, nameof(GetAllCategories), Url);
            return Ok(response);
        }


        /// <summary>
        /// Endpoint to create category
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "ADMIN")]
        [HttpPost()]
        [ProducesResponseType(typeof(SuccessResponse<CategoryDto>), 200)]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDto model)
        {
            var response = await _service.CategoryService.Create(model);
            return Ok(response);
        }


        /// <summary>
        /// Endpoint to get category by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<CategoryDto>), 200)]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var response = await _service.CategoryService.GetById(id);
            return Ok(response);
        }

        /// <summary>
        /// Endpoint to Update Category
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<CategoryDto>), 200)]
        public async Task<IActionResult> UpdateCategory([FromForm] CategoryUpdateDto model, Guid id)
        {
            var response = await _service.CategoryService.Update(model, id);
            return Ok(response);
        }


        // <summary>
        /// Endpoint to Remove Category
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> RemoveCategory(Guid Id)
        {
            await _service.CategoryService.Remove(Id);
            return NoContent();
        }
    }
}
