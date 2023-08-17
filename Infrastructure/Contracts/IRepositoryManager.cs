namespace Infrastructure.Contracts;

public interface IRepositoryManager
{
    IUserRepository User { get; }
    IUserActivityRepository UserActivity { get; }
    ITokenRepository Token { get; }
    ICategoryRepository Category { get; }
    ICatalogueRepository Catalogue { get; }
    IProductRepository Product { get; }
    IDiscountRepository Discount { get; }
    IShoppingCartItemRepository ShoppingCartItem { get; }
    IShoppingCartRepository ShoppingCart { get; }
    IUserRoleRepository UserRole { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransaction(Func<Task> action);
}