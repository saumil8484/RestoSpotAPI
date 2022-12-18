using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestoSpotAPI.Data;
using RestoSpotAPI.Models;

namespace RestoSpotAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CityController : Controller
    {
        private readonly RestoSpotDbContext _restoSpotDbContext;

        public CityController(RestoSpotDbContext restoSpotDbContext)
        {
            _restoSpotDbContext = restoSpotDbContext;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllCities()
        {
            var cities = await _restoSpotDbContext.city.ToListAsync();

            return Ok(cities);
        }

        [HttpPost]
        public async Task<IActionResult> AddCity([FromBody] city cityRequest)
        {
            cityRequest.CityId = Guid.NewGuid();
            await _restoSpotDbContext.city.AddAsync(cityRequest);
            await _restoSpotDbContext.SaveChangesAsync();

            return Ok(cityRequest);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetCity([FromRoute] Guid id)
        {
            var city = await _restoSpotDbContext.city.FirstOrDefaultAsync(x => x.CityId == id);

            if (city == null)
            {
                return NotFound();
            }

            return Ok(city);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateCity([FromRoute] Guid id, city updateCityRequest)
        {
            var city = await _restoSpotDbContext.city.FindAsync(id);

            if (city == null)
            {
                return NotFound();
            }


            city.City = updateCityRequest.City;

            await _restoSpotDbContext.SaveChangesAsync();

            return Ok(city);
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteCity([FromRoute] Guid id)
        {
            var city = await _restoSpotDbContext.city.FindAsync(id);

            if (city == null)
            {
                return NotFound();
            }

            _restoSpotDbContext.city.Remove(city);

            await _restoSpotDbContext.SaveChangesAsync();

            return Ok(city);
        }
    }
}
