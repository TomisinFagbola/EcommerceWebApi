using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts
{
    public interface IServiceManager
    {
        IAuthenticationService AuthenticationService { get; }
        IUserService UserService { get; }
        IProductService ProductService { get; }
        ICategoryService CategoryService { get; }
        IShoppingCartService ShoppingCartService { get; }

        IDiscountService DiscountService { get; }
        ICatalogueService CatalogueService { get; }

    }
}
