using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CockyShop.Exceptions;
using CockyShop.Infrastucture;
using CockyShop.Models.DTO;
using CockyShop.Models.Identity;
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

            return _autoMapper.Map<List<OrderDto>>(user.Orders);
        }

        public async Task<List<OrderDto>> GetAllOrdersByUserIdAsync(string userId)
        {
            var user = await _userService.FindUserByUserIdAsync(userId);

            return _autoMapper.Map<List<OrderDto>>(user.Orders);
        }

        public async Task<OrderDto> GetOrderByIdAsync(int id)
        {
            var order = await _appDbContext.Orders.SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                throw new EntityNotFoundException($"Order with such id {id} was not found!");
            }

            return _autoMapper.Map<OrderDto>(order);
        }
    }
}