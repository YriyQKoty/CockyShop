using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CockyShop.Exceptions;
using CockyShop.Infrastucture;
using CockyShop.Models.App;
using CockyShop.Models.DTO;
using CockyShop.Models.Enums;
using CockyShop.Models.Identity;
using CockyShop.Models.Requests;
using CockyShop.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CockyShop.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _autoMapper;
        private readonly IUserService _userService;
        

        public OrdersService(AppDbContext appDbContext, IMapper autoMapper, IUserService userService)
        {
            _appDbContext = appDbContext;
            _autoMapper = autoMapper;
            _userService = userService;
        }


        public async Task<List<OrderDto>> GetAllOrdersByEmailAsync(string email)
        {
            var user = await _userService.FindUserByEmailAsync(email);

            if (user == null)
            {
                throw new EntityNotFoundException($"User with such email {email} was not found!");
            }

            var orders = await _appDbContext.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(o => o.OrderedProducts)
                .Include(o => o.OrderDetails)
                .ThenInclude(o => o.Status)
                .Where(o => o.UserId == user.Id)
                .ToListAsync();

            return _autoMapper.Map<List<OrderDto>>(orders);
        }

        public async Task<List<OrderDto>> GetAllOrdersByUserIdAsync(string userId)
        {
            var user = await ValidateUserById(userId);

            var orders = await _appDbContext.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(o => o.OrderedProducts)
                .Include(o => o.OrderDetails)
                .ThenInclude(o => o.Status)
                .Where(o => o.UserId == user.Id)
                .ToListAsync();
            
            _autoMapper.ConfigurationProvider.AssertConfigurationIsValid();
            var res = _autoMapper.Map<List<Order>,List<OrderDto>>(orders);
            
            return res;
        }

      
        public async Task<OrderDto> GetOrderByIdAsync(int id)
        {
            var order = await ValidateOrder(id);

            return _autoMapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> CreateOrderAsync(OrderRequest request)
        {
            var user = await ValidateUserByEmail(request.UserEmail);

            var order = new Order
            {
                OrderDetails = new OrderDetails {OrderedProducts = new List<OrderedProduct>()}, UserId = user.Id
            };

            foreach (var orderedProduct in request.OrderDetailsRequest.OrderedProductsRequests)
            {
                var product =
                    await _appDbContext.ProductStocks.SingleOrDefaultAsync(p => p.Id == orderedProduct.ProductStockId);

                if (product == null)
                {
                    throw new EntityNotFoundException(
                        $"Product with such id {orderedProduct.ProductStockId} not found!");
                }


                order.OrderDetails.OrderedProducts.Add(new OrderedProduct()
                {
                    ProductStock = product,
                    PricePerUnit = product.Price,
                    ProductStockId = product.Id,
                    Quantity = orderedProduct.Quantity,
                    OrderDetails = order.OrderDetails,
                });
            }

            order.OrderDetails.Status =
                await _appDbContext.OrderStatuses.SingleAsync(s => s.Id == (int) OrderStatusEnum.Ordered);

            order.OrderDetails.Order = order;
            order.OrderDetails.DateOrdered = DateTime.Now;

            await _appDbContext.Orders.AddAsync(order);
            await _appDbContext.SaveChangesAsync();
            
            return new OrderDto()
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDetails = new OrderDetailsDto()
                {
                    DateOrdered = order.OrderDetails.DateOrdered,
                    OrderedProducts = _autoMapper.Map<List<OrderedProductDto>>(order.OrderDetails.OrderedProducts),
                    Status = new OrderStatusDto{StatusName = order.OrderDetails.Status.StatusName, Id = order.OrderDetails.Status.Id },
                    Id = order.OrderDetails.Id
                }
            };
        }

      
        public async Task<OrderDto> ChangeOrderStatusByIdAsync(string userId,int orderId, OrderStatusRequest request)
        {
            await ValidateUserById(userId);
            
            var order = await ValidateOrder(orderId);
            
            var status = await ValidateOrderStatus(request);

            order.OrderDetails.Status = status;
            
            await _appDbContext.SaveChangesAsync();
            
            return _autoMapper.Map<OrderDto>(order);
        }
        

        public async Task<List<OrderDto>> ChangeAllUserOrderStatusesAsync(string userId, OrderStatusRequest request)
        {
            await ValidateUserById(userId);
            
            var orders = await _appDbContext.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(o => o.OrderedProducts)
                .Include(o => o.OrderDetails)
                .ThenInclude(o => o.Status)
                .Where(o => o.UserId == userId)
                .ToListAsync();
            
            var status = await ValidateOrderStatus(request);
            
            foreach (var order in orders)
            {
                order.OrderDetails.Status = status;
                order.OrderDetails.EstimatedTime = TimeSpan.Zero;
            }
            
            await _appDbContext.SaveChangesAsync();

            return _autoMapper.Map<List<OrderDto>>(orders);

        }

        public async Task<OrderDto> CancelOrder(string email, int id)
        {
            await ValidateUserByEmail(email);

            var order = await ValidateOrder(id);
            
            var status = await ValidateOrderStatus(new OrderStatusRequest() {Name = "Cancelled"});

            order.OrderDetails.Status = status;
            
            await _appDbContext.SaveChangesAsync();
            
            return _autoMapper.Map<OrderDto>(order);
        }


        #region Helpers

        private async Task<Order> ValidateOrder(int id)
        {
            var order = await _appDbContext.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(o => o.OrderedProducts)
                .Include(o=>o.OrderDetails)
                .ThenInclude(o => o.Status)
                .SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                throw new EntityNotFoundException($"Order with such id {id} was not found!");
            }

            return order;
        }
        
        private async Task<OrderStatus> ValidateOrderStatus(OrderStatusRequest request)
        {
            var status = await _appDbContext.OrderStatuses.SingleOrDefaultAsync(s => s.StatusName == request.Name);

            if (status == null)
            {
                throw new EntityNotFoundException($"Status with such name {request.Name} was not found!");
            }
            return status;
        }
        
        private async Task<AppUser> ValidateUserById(string userId)
        {
            var user = await _userService.FindUserByUserIdAsync(userId);

            if (user == null)
            {
                throw new EntityNotFoundException($"User with such id {userId} was not found!");
            }

            return user;
        }
        
        private async Task<AppUser> ValidateUserByEmail(string email)
        {
            var user = await _userService.FindUserByEmailAsync(email);

            if (user == null)
            {
                throw new EntityNotFoundException($"User with such email {email} was not found!");
            }

            return user;
        }


        #endregion
       


        
        
    }
}