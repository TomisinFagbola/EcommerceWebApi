using Application.Contracts;
using AutoMapper;
using Domain.Entities.Identity;
using Infrastructure.Contracts;
using Infrastructure.Entities.Identity;
using Infrastructure.Utils.Azure;
using Infrastructure.Utils.Email;
using Infrastructure.Utils.Logger;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;

namespace Application.Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly HttpClient _httpClient;
        private readonly Lazy<IAuthenticationService> _authenticationService;
        private readonly Lazy<IUserService> _userService;
        private readonly Lazy<IProductService> _productService;
        private readonly Lazy<ICategoryService> _categoryService;
        private readonly Lazy<ICatalogueService> _catalogueService;
        private readonly Lazy<IShoppingCartService> _shoppingCartService;
        private readonly Lazy<IDiscountService> _discountService;
        public ServiceManager(IRepositoryManager repositoryManager,
            ILoggerManager logger,
            IMapper mapper,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IConfiguration configuration,
            IAzureStorage azureStorage,
            IEmailManager emailManager)
        {
            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, certChainType, policyErrors) => true
            };
            _httpClient = new HttpClient(handler);
            _authenticationService =
                new Lazy<IAuthenticationService>(
                    () => new AuthenticationService(repositoryManager, userManager, emailManager, mapper, logger, configuration, this));
            _userService = new Lazy<IUserService>(
                    () => new UserService(repositoryManager, userManager, roleManager, emailManager, mapper, logger, configuration, this));
            _productService = new Lazy<IProductService>(
                    () => new ProductService(repositoryManager, mapper, azureStorage));
            _categoryService = new Lazy<ICategoryService>(
                () => new CategoryService(repositoryManager, mapper, logger, configuration, this, azureStorage, emailManager));
            _catalogueService = new Lazy<ICatalogueService>(
                () => new CatalogueService(repositoryManager, mapper));
            _shoppingCartService = new Lazy<IShoppingCartService>(
                () => new ShoppingCartService(repositoryManager, mapper));
            _discountService = new Lazy<IDiscountService>(
                () => new DiscountService(repositoryManager, mapper));
        }
        public IAuthenticationService AuthenticationService => _authenticationService.Value;
        public IUserService UserService => _userService.Value;
        public IProductService ProductService => _productService.Value;
        public ICategoryService CategoryService => _categoryService.Value;
        public ICatalogueService CatalogueService => _catalogueService.Value;
        public IShoppingCartService ShoppingCartService => _shoppingCartService.Value;
        public IDiscountService DiscountService => _discountService.Value;

    }
}

