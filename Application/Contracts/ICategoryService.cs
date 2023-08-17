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
    public interface ICategoryService
    {
        Task<SuccessResponse<CategoryDto>> Create(CategoryCreateDto model);
        Task<PagedResponse<IEnumerable<CategoryDto>>> GetAllCategories(CategoryParameter parameters, string actionName, IUrlHelper urlHelper);
        Task<SuccessResponse<CategoryDto>> Update(CategoryUpdateDto model, Guid categoryId);
        Task<SuccessResponse<CategoryDto>> GetById(Guid categoryId);
        Task Remove(Guid categoryId);
    }
}
