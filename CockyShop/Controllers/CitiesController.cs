using System.Linq;
using System.Threading.Tasks;
using CockyShop.Infrastucture;
using CockyShop.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CockyShop.Controllers
{
    public class CitiesController : AppBaseController
    {
        private AppDbContext _appDbContext;

        public CitiesController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpGet]
        public async Task<ActionResult<CityDto>> GetAllCities()
        {
            var cities = await _appDbContext.Cities.Select(c => new CityDto()
            {
                Id = c.Id,
                Name = c.Name
            }).ToListAsync();

            return Ok(cities);
        }
    }
}