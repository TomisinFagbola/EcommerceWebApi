using Application.Contracts;
using Application.DataTransferObjects;
using Application.Helpers;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Utils.Azure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class DiscountService  : IDiscountService
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;
        

        public DiscountService(IRepositoryManager repository,
            IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      
        }

        public async Task<SuccessResponse<DiscountDto>> CreateDiscount(DiscountCreateDto model)
        {
            var product = await _repository.Product.FirstOrDefaultAsync(x => x.Id == model.ProductId);
            Guard.AgainstNull(product);

            var discount = await _repository.Discount.FirstOrDefaultAsync(x => x.ProductId == model.ProductId);
            if(discount is not null)
                throw new RestException(HttpStatusCode.BadRequest, "Discount already exist for this product");
            var newDiscount = _mapper.Map<Discount>(model);
            await _repository.Discount.AddAsync(newDiscount);
            await _repository.SaveChangesAsync();
            var response = _mapper.Map<DiscountDto>(newDiscount);
            return new SuccessResponse<DiscountDto>
            {
                Data = response,
                Message = "Discount successfully Created",
                Success = true,
            };
        }

        public async Task<SuccessResponse<DiscountDto>> GetById(Guid discountId)
        {

            var discount = await _repository.Discount.Get(x => x.Id == discountId).
                                        Include(x => x.Product)
                                        .FirstOrDefaultAsync();
            Guard.AgainstNull(discount);

            var response = _mapper.Map<DiscountDto>(discount);

            return new SuccessResponse<DiscountDto>
            {
                Data = response,
                Message = "Discount successfully retrieved",
                Success = true,
            };
        }

        public async Task<SuccessResponse<DiscountDto>> Update(Guid discountId, DiscountUpdateDto model)
        {

            var product = await _repository.Product.Get(x => x.Id == model.ProductId).FirstOrDefaultAsync();
            Guard.AgainstNull(product);
            var discount = await _repository.Discount.Get(x => x.Id == discountId)
                                             .Include(x => x.Product)
                                             .FirstOrDefaultAsync();
            var discountToUpdate = _mapper.Map(model, discount);

            _repository.Discount.Update(discountToUpdate);
            await _repository.SaveChangesAsync();

            var discountUpdated = _mapper.Map<DiscountDto>(discountToUpdate);
            return new SuccessResponse<DiscountDto>
            {
                Data = discountUpdated,
                Message = "Discount successfully Updated",
                Success = true,
            };
        }
        public async Task Remove(Guid discountId)
        {
            var discount = await _repository.Discount.Get(x => x.Id == discountId).FirstOrDefaultAsync();
            Guard.AgainstNull(discount);

            _repository.Discount.Remove(discount);
            await _repository.SaveChangesAsync();
        }

    }
}
