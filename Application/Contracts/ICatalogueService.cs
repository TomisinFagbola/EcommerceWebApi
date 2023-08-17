using Application.DataTransferObjects;
using Application.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts
{
    public interface ICatalogueService
    {
        Task<SuccessResponse<CatalogueDto>> Create(CatalogueCreateDto model);
        Task<PagedResponse<IEnumerable<CatalogueDto>>> GetAllCatalogues(CatalogueParameter parameters, string actionName, IUrlHelper urlHelper);
        Task<SuccessResponse<CatalogueDto>> Update(CatalogueUpdateDto model, Guid catalogueId);
        Task<SuccessResponse<CatalogueDto>> GetById(Guid catalogueId);
        Task Remove(Guid catalogueId);
    }
}
