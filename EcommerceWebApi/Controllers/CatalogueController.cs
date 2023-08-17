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
    [Route("api/v{version:apiVersion}/catalogues")]
    public class CatalogueController : ControllerBase
    {
        private readonly IServiceManager _service;
        public CatalogueController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>
        /// Endpoint to get all catalogues
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet(Name = nameof(GetAllCatalogues))]
        [ProducesResponseType(typeof(SuccessResponse<ICollection<CatalogueDto>>), 200)]
        public async Task<IActionResult> GetAllCatalogues([FromQuery] CatalogueParameter parameter)
        {
            var response = await _service.CatalogueService.GetAllCatalogues(parameter, nameof(GetAllCatalogues), Url);
            return Ok(response);
        }


        /// <summary>
        /// Endpoint to create catalogue
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "ADMIN")]
        [HttpPost()]
        [ProducesResponseType(typeof(SuccessResponse<CatalogueDto>), 200)]
        public async Task<IActionResult> CreateCatalogue([FromBody] CatalogueCreateDto model)
        {
            var response = await _service.CatalogueService.Create(model);
            return Ok(response);
        }


        /// <summary>
        /// Endpoint to get catalogue by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<CatalogueDto>), 200)]
        public async Task<IActionResult> GetCatalogueById(Guid id)
        {
            var response = await _service.CatalogueService.GetById(id);
            return Ok(response);
        }

        /// <summary>
        /// Endpoint to Update Catalogue
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<CatalogueDto>), 200)]
        public async Task<IActionResult> UpdateCatalgue([FromForm] CatalogueUpdateDto model, Guid id)
        {
            var response = await _service.CatalogueService.Update(model, id);
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
        public async Task<IActionResult> RemoveCatalogue(Guid id)
        {
            await _service.CatalogueService.Remove(id);
            return NoContent();
        }
    }
}
