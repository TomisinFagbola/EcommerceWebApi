using Application.Contracts;
using Application.DataTransferObjects;
using Application.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Hangfire.Dashboard;
using Infrastructure.Contracts;
using Infrastructure.Utils.Azure;
using Infrastructure.Utils.Email;
using Infrastructure.Utils.Logger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Application.Services
{
    public class ProductService  : IProductService
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;
        private readonly IAzureStorage _azureStorage;

        public ProductService(IRepositoryManager repository,
            IMapper mapper, IAzureStorage azureStorage)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _azureStorage = azureStorage ?? throw new ArgumentNullException(nameof(azureStorage));
        }

        public async Task<PagedResponse<IEnumerable<ProductDto>>> GetAll(ProductParameter parameters, string actionName, IUrlHelper urlHelper)
        {
            IQueryable<Product> productsQuery = _repository.Product.QueryAll().Include(x => x.Discount);
          

            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                var search = parameters.Search.Trim().ToLower();
                productsQuery = productsQuery.Where(x => (x.Name.ToLower().Contains(search))
                   || x.Description.ToLower().Contains(search) || x.Summary.ToLower().Contains(search));
            }

            if (parameters.Category != Guid.Empty)
            {
                var category = await _repository.Category.FirstOrDefaultAsync(x => x.Id == parameters.Category);
                Guard.AgainstNull(category);

                productsQuery = productsQuery.Where(x => x.CategoryId == parameters.Category);
            }

            if (parameters.Date.Count() > 0)
            {
                if (parameters.Date.Any(e => e.Contains("3 days ago")))
                {

                    productsQuery = productsQuery.Where(x => parameters.Date.Any(e => x.CreatedAt >= DateTime.UtcNow.AddDays(-3)));
                }
                else if (parameters.Date.Any(e => e.Contains("Last 24 hours")))
                {
                    productsQuery = productsQuery.Where(x => parameters.Date.Any(e => x.CreatedAt >= DateTime.UtcNow.AddHours(-24)));

                }
                else
                    productsQuery = productsQuery.Where(x => parameters.Date.Any(e => x.CreatedAt >= DateTime.UtcNow.AddMonths(-1)));
            }

            var productDtos = productsQuery.ProjectTo<ProductDto>(_mapper.ConfigurationProvider);

            var pagedProducts = await PagedList<ProductDto>.Create(productDtos, parameters.PageNumber, parameters.PageSize, parameters.Sort);

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

        public async Task<SuccessResponse<ProductDto>> Create(ProductCreateDto model)
        {
            await CategoryValidation(model);
            var productToCreate = await Validate(model);
            var product = _mapper.Map<Product>(productToCreate);
            product.CategoryId = model.CategoryId;


            if (model.ImageFile is not null)
                product.ImageFile = await _azureStorage.UploadAsync(model.ImageFile); 

            await _repository.Product.AddAsync(product);
            await _repository.SaveChangesAsync();

            var productCreated = _mapper.Map<ProductDto>(product);
         
            return new SuccessResponse<ProductDto>
            {
                Data = productCreated,
                Message = "Product successfully Created",
                Success = true,
            };
        }

        public async Task<SuccessResponse<ProductDto>> Update(Guid productId, ProductUpdateDto model)
        {
            await CategoryValidation(model);

            var product = await _repository.Product.Get(x => x.Id == productId).FirstOrDefaultAsync();
            Guard.AgainstNull(product);

            var productToUpdate =_mapper.Map(model, product);
            if (model.ImageFile is not null)
                productToUpdate.ImageFile = await _azureStorage.UploadAsync(model.ImageFile);

            _repository.Product.Update(productToUpdate);
            await _repository.SaveChangesAsync();

            var productUpdated = _mapper.Map<ProductDto>(productToUpdate);
            return new SuccessResponse<ProductDto>
            {
                Data = productUpdated,
                Message = "Product successfully Updated",
                Success = true,
            };
        }

        public async Task<SuccessResponse<ProductDto>> GetById(Guid productId)
        {

            var product = await _repository.Product.FirstOrDefaultAsync(x => x.Id == productId);

            Guard.AgainstNull(product);

            var response = _mapper.Map<ProductDto>(product);

            return new SuccessResponse<ProductDto>
            {
                Data = response,
                Message = "Product successfully retrieved",
                Success = true,
            };
        }

        
        public async Task Remove(Guid productId)
        {
            var product = await _repository.Product.Get(x => x.Id == productId).FirstOrDefaultAsync();
            Guard.AgainstNull(product);

            _repository.Product.Remove(product);
            await _repository.SaveChangesAsync();
        }

        private async Task<ProductCreateDto> Validate(ProductCreateDto model)
        {
            var productExist = await _repository.Product.ExistsAsync(x => x.Name.ToLower() == model.Name.ToLower());
            if (productExist)
                throw new RestException(HttpStatusCode.BadRequest, "Product Name already exist");

            return model;
        }

        private async Task<ProductCreateDto> CategoryValidation(ProductCreateDto model)
        {
                var category = await _repository.Category.Get(x => x.Id == model.CategoryId).FirstOrDefaultAsync();
                Guard.AgainstNull(category);

                return model;
            
        }


    }
}
