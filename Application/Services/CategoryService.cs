using Application.Contracts;
using Application.DataTransferObjects;
using Application.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Utils.Azure;
using Infrastructure.Utils.Email;
using Infrastructure.Utils.Logger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceManager _serviceManager;
        private readonly IAzureStorage _azureStorage;
        private readonly IEmailManager _emailManager;

        public CategoryService(IRepositoryManager repository,
            IMapper mapper, ILoggerManager logger, IConfiguration configuration, IServiceManager serviceManager, IAzureStorage azureStorage, IEmailManager emailManager)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _serviceManager = serviceManager ?? throw new ArgumentNullException(nameof(serviceManager));
            _emailManager = emailManager ?? throw new ArgumentException(nameof(emailManager));
        }

        public async Task<PagedResponse<IEnumerable<CategoryDto>>> GetAllCategories(CategoryParameter parameters, string actionName, IUrlHelper urlHelper)
        {
            try
            {
                IQueryable<Category> categoryQuery = _repository.Category.QueryAll()
                    .OrderBy(x => x.CreatedAt);

                if (!string.IsNullOrEmpty(parameters.Search))
                {
                    categoryQuery = categoryQuery.Where(x =>
                    x.Name.Contains(parameters.Search.ToLower()) ||
                    x.Description.Contains(parameters.Search.ToLower()));
                }

                var categoryDtos = categoryQuery.ProjectTo<CategoryDto>(_mapper.ConfigurationProvider);

                var pagedCategories = await PagedList<CategoryDto>.Create(categoryDtos, parameters.PageNumber, parameters.PageSize, parameters.Sort);
                var dynamicParameters = PageUtility<CategoryDto>.GenerateResourceParameters(parameters, pagedCategories);

                var page = PageUtility<CategoryDto>.CreateResourcePageUrl(dynamicParameters, actionName, pagedCategories, urlHelper);

                return new PagedResponse<IEnumerable<CategoryDto>>
                {
                    Message = "Categories data retrieved successfully",
                    Data = pagedCategories,
                    Success = true,
                    Meta = new Meta
                    {
                        Pagination = page
                    }
                };
            }
            catch (Exception ex)
            {
                throw new RestException(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        public async Task<SuccessResponse<CategoryDto>> Create(CategoryCreateDto model)
        {
            try
            {
                await CatalogueValidation(model);
                var categoryToCreate = await Validate(model);
                var category = _mapper.Map<Category>(categoryToCreate);
                category.CatalogueId = model.CatalogueId;
                await _repository.Category.AddAsync(category);
                await _repository.SaveChangesAsync();

                var categoryCreated = _mapper.Map<CategoryDto>(category);
                return new SuccessResponse<CategoryDto>
                {
                    Data = categoryCreated,
                    Message = "Category successfully Created",
                    Success = true,
                };
            }
            catch(Exception ex)
            {
                throw new RestException(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public async Task<SuccessResponse<CategoryDto>> GetById(Guid categoryId)
        {

            var category = await _repository.Category.FirstOrDefaultAsync(x => x.Id == categoryId);
            Guard.AgainstNull(category);
            var response = _mapper.Map<CategoryDto>(category);
            return new SuccessResponse<CategoryDto>
            {
                Data = response,
                Message = "Category successfully retrieved",
                Success = true,
            };
        }

        public async Task<SuccessResponse<CategoryDto>> Update(CategoryUpdateDto model, Guid categoryId)
        {
            await CatalogueValidation(model);

            var category = await _repository.Category.Get(x => x.Id == categoryId).FirstOrDefaultAsync();
            Guard.AgainstNull(category);

            var categoryToUpdate = _mapper.Map(model, category);

            _repository.Category.Update(category);
            await _repository.SaveChangesAsync();

            var categoryUpdated = _mapper.Map<CategoryDto>(category);
            return new SuccessResponse<CategoryDto>
            {
                Data = categoryUpdated,
                Message = "Category successfully Updated",
                Success = true,
            };
        }



        public async Task Remove(Guid categoryId)
        {
            var category = await _repository.Category.Get(x => x.Id == categoryId).FirstOrDefaultAsync();
            Guard.AgainstNull(category);

            _repository.Category.Remove(category);
            await _repository.SaveChangesAsync();
        }

        public async Task<PagedResponse<IEnumerable<ProductDto>>> GetAllProductsInACategory(Guid categoryId, ProductParameter parameters, string actionName, IUrlHelper urlHelper)
        {
            try
            {
                var category = await _repository.Category.FirstOrDefaultAsync(x => x.Id == categoryId);
                Guard.AgainstNull(category);

                var productsQuery = _repository.Product.QueryAll(x => x.CategoryId == categoryId);

                if (!string.IsNullOrEmpty(parameters.Search))
                {
                    var search = parameters.Search.ToLower();
                    productsQuery = productsQuery.Where(x =>
                            x.Name.ToLower().Contains(search) || x.Description.ToLower().Contains(search));
                }

                var ProductDtos = productsQuery.ProjectTo<ProductDto>(_mapper.ConfigurationProvider);

                var pagedProducts = await PagedList<ProductDto>.Create(ProductDtos, parameters.PageNumber, parameters.PageSize, parameters.Sort);
                var dynamicParameters = PageUtility<ProductDto>.GenerateResourceParameters(parameters, pagedProducts);

                var page = PageUtility<ProductDto>.CreateResourcePageUrl(dynamicParameters, actionName, pagedProducts, urlHelper);

                return new PagedResponse<IEnumerable<ProductDto>>
                {
                    Message = "Products data retrieved successfully",
                    Data = pagedProducts,
                    Success = true,
                    Meta = new Meta
                    {
                        Pagination = page
                    }
                };
            }
            catch (Exception ex)
            {
                throw new RestException(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        private async Task<CategoryCreateDto> CatalogueValidation(CategoryCreateDto model)
        {
            var catalogueExist = await _repository.Catalogue.ExistsAsync(x => x.Id == model.CatalogueId);
            if (!catalogueExist)
                throw new RestException(HttpStatusCode.BadRequest, "Catalogue already exist");

            return model;
        }

        private async Task<CategoryCreateDto> Validate(CategoryCreateDto model)
        {
            var categoryExist = await _repository.Category.ExistsAsync(x => x.Name.ToLower() == model.Name.ToLower());
            if (categoryExist)
                throw new RestException(HttpStatusCode.BadRequest, "Category Name already exist");

            return model;
        }
    }
}
