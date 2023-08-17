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
    public interface IShoppingCartService
    {
        Task<SuccessResponse<ShoppingCartItemDto>> AddToCart(LoggedinUserDto loggedinUser, Guid ProductId);

        Task<PagedResponse<IEnumerable<ShoppingCartItemDto>>> ShoppingCartItems(ShoppingCartParameter parameters, LoggedinUserDto loggedinUser, string actionName, IUrlHelper urlHelper);
        Task<SuccessResponse<ShoppingCartItemDto>> GetItemById(Guid id);

        Task RemoveItemFromCart(LoggedinUserDto loggedinUser, Guid shoppingCartItem);

    }
}
