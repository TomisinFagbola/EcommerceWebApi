using Domain.Entities;
using Infrastructure.Data.DbContext;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Contracts
{
    public interface IShoppingCartRepository : IRepositoryBase<ShoppingCart>
    {
    }

    public interface IShoppingCartItemRepository : IRepositoryBase<ShoppingCartItem>
    { 
    }
}
