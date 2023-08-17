using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly AppDbContext _appDbContext;
        private readonly Lazy<IUserRepository> _userRepository;
        private readonly Lazy<IUserActivityRepository> _userActivityRepository;
        private readonly Lazy<ITokenRepository> _tokenRepository;
        private readonly Lazy<IUserRoleRepository> _userRoleRepository;
        private readonly Lazy<IProductRepository> _productRepository;
        private readonly Lazy<ICategoryRepository> _categoryRepository;
        private readonly Lazy<ICatalogueRepository> _catalogueRepository;
        private readonly Lazy<IShoppingCartItemRepository> _shoppingCartItemRepository;
        private readonly Lazy<IShoppingCartRepository> _shoppingCartRepository;
        private readonly Lazy<IDiscountRepository> _discountRepository;

        public RepositoryManager(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _userRepository = new Lazy<IUserRepository>(() => new UserRepository(appDbContext));
            _userRoleRepository = new Lazy<IUserRoleRepository>(() => new UserRoleRepository(appDbContext));
            _userActivityRepository = new Lazy<IUserActivityRepository>(() => new UserActivityRepository(appDbContext));
            _tokenRepository = new Lazy<ITokenRepository>(() => new TokenRepository(appDbContext));
            _productRepository = new Lazy<IProductRepository>(() => new ProductRepository(appDbContext));
            _categoryRepository = new Lazy<ICategoryRepository>(() => new CategoryRepository(appDbContext));
            _catalogueRepository = new Lazy<ICatalogueRepository>(() => new CatalogueRepository(appDbContext));
            _shoppingCartItemRepository = new Lazy<IShoppingCartItemRepository>(() => new ShoppingCartItemRepository(appDbContext));
            _shoppingCartRepository = new Lazy<IShoppingCartRepository>(() => new ShoppingCartRepository(appDbContext));
            _discountRepository = new Lazy<IDiscountRepository>(() => new DiscountRepository(appDbContext));
        }

        public IUserRepository User => _userRepository.Value;
        public IUserRoleRepository UserRole => _userRoleRepository.Value;
        public IUserActivityRepository UserActivity => _userActivityRepository.Value;
        public IProductRepository Product => _productRepository.Value;
        public ITokenRepository Token => _tokenRepository.Value;
        public IDiscountRepository Discount => _discountRepository.Value;
        public ICategoryRepository Category => _categoryRepository.Value;
        public ICatalogueRepository Catalogue => _catalogueRepository.Value;
        public IShoppingCartItemRepository ShoppingCartItem => _shoppingCartItemRepository.Value;
        public IShoppingCartRepository ShoppingCart => _shoppingCartRepository.Value;

        public async Task<int> SaveChangesAsync() => await _appDbContext.SaveChangesAsync();
        public async Task BeginTransaction(Func<Task> action)
        {
            await using var transaction = await _appDbContext.Database.BeginTransactionAsync();
            try
            {
                await action();

                await SaveChangesAsync();
                await transaction.CommitAsync();

            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
