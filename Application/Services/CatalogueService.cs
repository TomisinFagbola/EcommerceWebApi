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
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CatalogueService : ICatalogueService
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;
     
    

        public CatalogueService(IRepositoryManager repository,
            IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PagedResponse<IEnumerable<CatalogueDto>>> GetAllCatalogues(CatalogueParameter parameters, string actionName, IUrlHelper urlHelper)
        {
            try
            {
                IQueryable<Catalogue> catalogueQuery = _repository.Catalogue.QueryAll()
                    .Include(x => x.Categories)
                    .OrderBy(x => x.CreatedAt);

                if (!string.IsNullOrEmpty(parameters.Search))
                {
                    catalogueQuery = catalogueQuery.Where(x =>
                    x.Name.Contains(parameters.Search.ToLower()) ||
                    x.Description.Contains(parameters.Search.ToLower()));
                }

                var catalogueDtos = catalogueQuery.ProjectTo<CatalogueDto>(_mapper.ConfigurationProvider);

                var pagedCatalogues = await PagedList<CatalogueDto>.Create(catalogueDtos, parameters.PageNumber, parameters.PageSize, parameters.Sort);
                var dynamicParameters = PageUtility<CatalogueDto>.GenerateResourceParameters(parameters, pagedCatalogues);

                var page = PageUtility<CatalogueDto>.CreateResourcePageUrl(dynamicParameters, actionName, pagedCatalogues, urlHelper);

                return new PagedResponse<IEnumerable<CatalogueDto>>
                {
                    Message = "Catalogue data retrieved successfully",
                    Data = pagedCatalogues,
                    Success = true,
                    Meta = new Meta
                    {
                        Pagination = page
                    }
                };
            }
            catch(Exception ex)
            {
                throw new RestException(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        
        public async Task<SuccessResponse<CatalogueDto>> Create(CatalogueCreateDto model)
        {
            try
            {
                await Validate(model);
                var catalogue = _mapper.Map<Catalogue>(model);
                await _repository.Catalogue.AddAsync(catalogue);
                await _repository.SaveChangesAsync();
                var catalogueCreated = _mapper.Map<CatalogueDto>(catalogue);

                return new SuccessResponse<CatalogueDto>
                {
                    Data = catalogueCreated,
                    Message = "Catalogue successfully Created",
                    Success = true,
                };
            }
            catch(Exception ex)
            {
                throw new RestException(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public async Task<SuccessResponse<CatalogueDto>> GetById(Guid catalogueId)
        {
            var catalogue = await _repository.Catalogue.FirstOrDefaultAsync(x => x.Id == catalogueId);
            Guard.AgainstNull(catalogue);
            var response = _mapper.Map<CatalogueDto>(catalogue);
            return new SuccessResponse<CatalogueDto>
            {
                Data = response,
                Message = "Product successfully retrieved",
                Success = true,
            };
        }


        public async Task<SuccessResponse<CatalogueDto>> Update(CatalogueUpdateDto model, Guid catalogueId)
        {
            try
            {
                var catalogue = await _repository.Catalogue.Get(x => x.Id == catalogueId).FirstOrDefaultAsync();
                Guard.AgainstNull(catalogue);

                var catalogueToUpdate = _mapper.Map(model, catalogue);

                _repository.Catalogue.Update(catalogueToUpdate);
                await _repository.SaveChangesAsync();

                var catalogueUpdated = _mapper.Map<CatalogueDto>(catalogueToUpdate);
                return new SuccessResponse<CatalogueDto>
                {
                    Data = catalogueUpdated,
                    Message = "Catalogue successfully Updated",
                    Success = true,
                };
            }
            catch (Exception ex)
            {
                throw new RestException(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        public async Task Remove(Guid catalogueId)
        {
            var catalogue = await _repository.Catalogue.Get(x => x.Id == catalogueId).FirstOrDefaultAsync();
            Guard.AgainstNull(catalogue);

            _repository.Catalogue.Remove(catalogue);
            await _repository.SaveChangesAsync();
        }


        #region private methods
        private async Task<CatalogueCreateDto> Validate(CatalogueCreateDto model)
        {
            var catalogueExist = await _repository.Catalogue.ExistsAsync(x => x.Name.ToLower() == model.Name.ToLower());
            if (catalogueExist)
                throw new RestException(HttpStatusCode.BadRequest, "Catalogue Name already exist");

            return model;
        }
        #endregion
    }
}
