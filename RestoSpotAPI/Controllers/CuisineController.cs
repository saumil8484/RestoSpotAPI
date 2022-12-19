using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestoSpotAPI.Data;
using RestoSpotAPI.Models;

namespace RestoSpotAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CuisineController : Controller
    {
        private readonly RestoSpotDbContext _restoSpotDbContext;

        public CuisineController(RestoSpotDbContext restoSpotDbContext)
        {
            _restoSpotDbContext = restoSpotDbContext;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllCuisines()
        {
            var cuisines = await _restoSpotDbContext.cuisine.ToListAsync();

            return Ok(cuisines);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddCuisine([FromBody] cuisine cuisineRequest)
        {
            cuisineRequest.CuisineId = Guid.NewGuid();
            await _restoSpotDbContext.cuisine.AddAsync(cuisineRequest);
            await _restoSpotDbContext.SaveChangesAsync();

            return Ok(cuisineRequest);
        }

        [Authorize]
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetCuisine([FromRoute] Guid id)
        {
            var cuisine = await _restoSpotDbContext.cuisine.FirstOrDefaultAsync(x => x.CuisineId == id);

            if (cuisine == null)
            {
                return NotFound();
            }

            return Ok(cuisine);
        }

        [Authorize]
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateCuisine([FromRoute] Guid id, cuisine updateCuisineRequest)
        {
            var cuisine = await _restoSpotDbContext.cuisine.FindAsync(id);

            if (cuisine == null)
            {
                return NotFound();
            }

            
            cuisine.Cuisine = updateCuisineRequest.Cuisine;

            await _restoSpotDbContext.SaveChangesAsync();

            return Ok(cuisine);
        }

        [Authorize]
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteCuisine([FromRoute] Guid id)
        {
            var cuisine = await _restoSpotDbContext.cuisine.FindAsync(id);

            if (cuisine == null)
            {
                return NotFound();
            }

            _restoSpotDbContext.cuisine.Remove(cuisine);

            await _restoSpotDbContext.SaveChangesAsync();

            return Ok(cuisine);
        }
    }
}
