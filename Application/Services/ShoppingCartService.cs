using Application.Contracts;
using Application.DataTransferObjects;
using Application.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure;
using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Utils.Azure;
using Infrastructure.Utils.Email;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Application.Services
{
    public class ShoppingCartService  : IShoppingCartService
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;

        public ShoppingCartService(IRepositoryManager repository,
            IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<SuccessResponse<ShoppingCartItemDto>> AddToCart(LoggedinUserDto loggedinUser, Guid ProductId)
        {
            var shoppingCart = await _repository.ShoppingCart.Get(x => x.UserId == loggedinUser.UserId)
                .Include(x => x.Items).
                FirstOrDefaultAsync();
            //if (shoppingCart is null)
            //{
            //    shoppingCart = new();
            //}   
            //Guard.AgainstNull(shoppingCart);
            var product = await _repository.Product.FirstOrDefaultAsync(x => x.Id == ProductId);
            Guard.AgainstNull(product);

            ShoppingCart newShoppingCart = new();
            newShoppingCart.UserId = loggedinUser.UserId;
            ShoppingCartItem shoppingCartItem = new();
            ShoppingCartItem existingShoppingCartItem = new();
            if (shoppingCart is not null)
            {
                existingShoppingCartItem = shoppingCart.Items.FirstOrDefault(x => x.ProductId == ProductId);
            }
            // if the shopping cart does not exist, create a new shopping shoppping cart and add item
            if (shoppingCart is null && newShoppingCart is not null)
            {
                ShoppingCartItems(newShoppingCart, shoppingCartItem, product);
                newShoppingCart.TotalAmount = product.Price;
                await _repository.ShoppingCart.AddAsync(newShoppingCart);
                await _repository.ShoppingCartItem.AddAsync(shoppingCartItem);
                await _repository.SaveChangesAsync();
                var responseShoppingCartItem = _mapper.Map<ShoppingCartItemDto>(shoppingCartItem);
                return new SuccessResponse<ShoppingCartItemDto>
                {
                    Data = responseShoppingCartItem,
                    Message = "ShoppingCart Item successfully added",
                    Success = true,
                };
            }

            //if shoppping cart exist and the same item is being added
            else if (existingShoppingCartItem is not null)
            {
                existingShoppingCartItem.Quantity++;
                var responseShoppingCartItem = _mapper.Map<ShoppingCartItemDto>(existingShoppingCartItem);
                await _repository.SaveChangesAsync();
                return new SuccessResponse<ShoppingCartItemDto>
                {
                    Data = responseShoppingCartItem,
                    Message = "ShoppingCart Item successfully added",
                    Success = true,
                };

            }

            //if shoppingCart exist and we just want to add a new item
            else
            {
                ShoppingCartItems(shoppingCart, shoppingCartItem, product);
                await _repository.ShoppingCartItem.AddAsync(shoppingCartItem);
                shoppingCart.TotalAmount += product.Price;
                await _repository.SaveChangesAsync();
                var responseShoppingCartItem = _mapper.Map<ShoppingCartItemDto>(shoppingCartItem);
                return new SuccessResponse<ShoppingCartItemDto>
                {
                    Data = responseShoppingCartItem,
                    Message = "ShoppingCart Item successfully added",
                    Success = true,
                };
            }

            
        }


        public async Task<PagedResponse<IEnumerable<ShoppingCartItemDto>>> ShoppingCartItems(ShoppingCartParameter parameters, LoggedinUserDto loggedinUser, string actionName, IUrlHelper urlHelper) 
        {

            var shoppingCart = await _repository.ShoppingCart.FirstOrDefaultAsync(x => x.UserId == loggedinUser.UserId);

            Guard.AgainstNull(shoppingCart);
            IQueryable<ShoppingCartItem> shoppingCartItems = _repository.ShoppingCartItem.QueryAll(x => x.ShoppingCartId == shoppingCart.Id)
                .Include(x => x.Product)
                .ThenInclude(x => x.Discount);



            Guard.AgainstNull(shoppingCartItems); 

            if(!string.IsNullOrWhiteSpace(parameters.Search))
            {
                var search = parameters.Search.ToLower().Trim();
                shoppingCartItems = shoppingCartItems.Where(x => x.Product.Name.ToLower().Contains(search));
            };

           // var responseShoppingCartDto = _mapper.Map<ShoppingCartDto>(shoppingCart);
            var shoppingCartDto = shoppingCartItems.ProjectTo<ShoppingCartItemDto>(_mapper.ConfigurationProvider);

            var pagedShoppingCartItems = await PagedList<ShoppingCartItemDto>.Create(shoppingCartDto, parameters.PageNumber, parameters.PageSize, parameters.Sort);

            var dynamicParameters = PageUtility<ShoppingCartItemDto>.GenerateResourceParameters(parameters, pagedShoppingCartItems);

            var page = PageUtility<ShoppingCartItemDto>.CreateResourcePageUrl(dynamicParameters, actionName, pagedShoppingCartItems, urlHelper);

            return new PagedResponse<IEnumerable<ShoppingCartItemDto>>
            {
                Message = "Shopping Cart Items data retrieved successfully",
                Data = pagedShoppingCartItems,
                Success = true,
                Meta = new Meta
                {
                    Pagination = page
                }
            };
        }

        public async Task RemoveItemFromCart(LoggedinUserDto loggedinUser, Guid shoppingCartItem)
        {
            var shoppingCart = await _repository.ShoppingCart.FirstOrDefaultAsync(x => x.UserId == loggedinUser.UserId);
            Guard.AgainstNull(shoppingCart);

            var item = await _repository.ShoppingCartItem.FirstOrDefaultAsync(x => x.Id == shoppingCartItem);
            shoppingCart.Items.Remove(item);
            await _repository.SaveChangesAsync();
        }

        public async Task<SuccessResponse<ShoppingCartItemDto>> GetItemById(Guid id)
        {

            var shoppingCartItem = await _repository.ShoppingCartItem.Get(x => x.Id == id)
                                             .Include(x => x.Product)
                                             .FirstOrDefaultAsync();
            Guard.AgainstNull(shoppingCartItem); 

            var response = _mapper.Map<ShoppingCartItemDto>(shoppingCartItem);

            return new SuccessResponse<ShoppingCartItemDto>
            {
                Data = response,
                Message = "Shopping Cart Item successfully retrieved",
                Success = true,
            };
        }

        #region private methods
        private void ShoppingCartItems(ShoppingCart shoppingCart, ShoppingCartItem shoppingCartItem, Product product)
        {
           
                shoppingCartItem.ProductId = product.Id;
                shoppingCartItem.Quantity++;
                shoppingCartItem.ShoppingCartId = shoppingCart.Id;
          
        }
        #endregion

    };

  
 }

