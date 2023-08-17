using Application.DataTransferObjects;
using Application.Helpers;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts
{
    public interface IProductService
    {
        Task<SuccessResponse<ProductDto>> Create(ProductCreateDto model);
        Task<PagedResponse<IEnumerable<ProductDto>>> GetAll(ProductParameter parameters, string actionName, IUrlHelper urlHelper);
        Task<SuccessResponse<ProductDto>> Update(Guid productId, ProductUpdateDto model);
        Task<SuccessResponse<ProductDto>> GetById(Guid productId);
        Task Remove(Guid producId);
      
    }
}
