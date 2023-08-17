using Application.DataTransferObjects;
using Application.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts
{
    public interface IDiscountService
    {
        Task<SuccessResponse<DiscountDto>> CreateDiscount(DiscountCreateDto model);

        Task<SuccessResponse<DiscountDto>> Update(Guid discountId, DiscountUpdateDto model);

        Task Remove(Guid discountId);
    }
}
