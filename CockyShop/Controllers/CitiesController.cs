using System.Linq;
using System.Threading.Tasks;
using CockyShop.Exceptions;
using CockyShop.Infrastucture;
using CockyShop.Models.App;
using CockyShop.Models.DTO;
using CockyShop.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CockyShop.Controllers
{
    [Authorize]
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

      
        [HttpPost]
        [Authorize(Policy = "RequireAdministratorRole")]
        public async Task<ActionResult<CityDto>> CreateCity([FromBody] CityRequest request)
        {
            var city = await _appDbContext.Cities.Where(c => c.Name.Equals(request.Name)).FirstOrDefaultAsync();
            
            if (city != null)
            {
                throw new DomainException($"City with name '{request.Name}' already exists!");
            }
            
            city = (await  _appDbContext.Cities.AddAsync(new City() {Name = request.Name})).Entity;
            await _appDbContext.SaveChangesAsync();
            
            return Ok(new CityDto()
            {
                Id = city.Id,
                Name = city.Name
            });
        }
    }
}