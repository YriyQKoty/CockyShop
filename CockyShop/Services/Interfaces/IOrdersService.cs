using System.Collections.Generic;
using System.Threading.Tasks;
using CockyShop.Models.DTO;

namespace CockyShop.Services.Interfaces
{
    public interface IOrdersService
    {
        Task<List<OrderDto>> GetAllOrdersByEmailAsync(string email);
        
        Task<List<OrderDto>> GetAllOrdersByUserIdAsync(string userId);

        Task<OrderDto> GetOrderByIdAsync(int id);
    }
}