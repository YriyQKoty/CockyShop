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

            var orders = await _appDbContext.Orders.Where(o => o.UserId == user.Id).ToListAsync();
            
            return _autoMapper.Map<List<OrderDto>>(orders);
        }

        public async Task<List<OrderDto>> GetAllOrdersByUserIdAsync(string userId)
        {
            var user = await _userService.FindUserByUserIdAsync(userId);
            
            if (user == null)
            {
                throw new EntityNotFoundException($"User with such id {userId} was not found!");
            }

            return _autoMapper.Map<List<OrderDto>>(
                _appDbContext.Orders.Where(o => o.UserId == user.Id)
                .Include(o => o.OrderDetails)
                .ToListAsync());
        }

        public async Task<OrderDto> GetOrderByIdAsync(int id)
        {
            var order = await ValidateOnExist(id);

            return _autoMapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> CreateOrderAsync(OrderRequest request)
        {
            var user = await _userService.FindUserByEmailAsync(request.UserEmail);

            if (user == null)
            {
                throw new EntityNotFoundException($"User with such email {request.UserEmail} was not found!");
            }

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
                OrderDetailsDto = new OrderDetailsDto()
                {
                    DateOrdered = order.OrderDetails.DateOrdered,
                    OrderedProductsDtos = _autoMapper.Map<List<OrderedProductDto>>(order.OrderDetails.OrderedProducts),
                    Status = new OrderStatusDto{Name = order.OrderDetails.Status.StatusName, Id = order.OrderDetails.Status.Id },
                    Id = order.OrderDetails.Id
                }
            };
        }



        public async Task<OrderDto> ChangeOrderStatusByIdAsync(int id, OrderStatusRequest request)
        {
            var order = await ValidateOnExist(id);
            
            var status = await ValidateStatusOnExist(request);

            order.OrderDetails.Status = new OrderStatus() {StatusName = status.StatusName, Id = status.Id};
            await _appDbContext.SaveChangesAsync();
            
            return _autoMapper.Map<OrderDto>(order);
        }
        

        public async Task<List<OrderDto>> ChangeAllUserOrderStatusesAsync(string userId, OrderStatusRequest request)
        {
            var user = await _userService.FindUserByUserIdAsync(userId);
            
            if (user == null)
            {
                throw new EntityNotFoundException($"User with such id {userId} was not found!");
            }
            
            var status = await ValidateStatusOnExist(request);
            
            foreach (var order in user.Orders)
            {
                order.OrderDetails.Status = new OrderStatus() {StatusName = status.StatusName, Id = status.Id};
                order.OrderDetails.EstimatedTime = TimeSpan.Zero;
            }
            await _appDbContext.SaveChangesAsync();

            return _autoMapper.Map<List<OrderDto>>(user.Orders);

        }
        
        private async Task<Order> ValidateOnExist(int id)
        {
            var order = await _appDbContext.Orders.SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                throw new EntityNotFoundException($"Order with such id {id} was not found!");
            }

            return order;
        }
        
        private async Task<OrderStatus> ValidateStatusOnExist(OrderStatusRequest request)
        {
            var status = await _appDbContext.OrderStatuses.SingleOrDefaultAsync(s => s.StatusName == request.Name);

            if (status == null)
            {
                throw new EntityNotFoundException($"Status with such name {request.Name} was not found!");
            }

            return status;
        }
        
        
    }
}