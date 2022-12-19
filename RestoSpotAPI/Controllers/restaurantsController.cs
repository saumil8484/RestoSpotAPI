using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestoSpotAPI.Data;
using RestoSpotAPI.Models;

namespace RestoSpotAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class restaurantsController : Controller
    {
        private readonly RestoSpotDbContext _restoSpotDbContext;

        public restaurantsController(RestoSpotDbContext restoSpotDbContext)
        {
            _restoSpotDbContext = restoSpotDbContext;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GettAllRestaurants()
        {
            var restaurants = await (from r in _restoSpotDbContext.restaurants
                                    join cu in _restoSpotDbContext.cuisine on r.CuisineId equals cu.CuisineId
                                    join ci in _restoSpotDbContext.city on r.CityId equals ci.CityId
                                    select new restaurantInformation()
                                    {
                                        RestaurantId = r.RestaurantId,
                                        Name = r.Name,
                                        Address = r.Address,
                                        CityId = r.CityId,
                                        CuisineId = r.CuisineId,
                                        Rating = r.Rating,
                                        Reviews = r.Reviews,
                                        Cuisine = cu.Cuisine,
                                        City = ci.City
                                    }).ToListAsync();

            return Ok(restaurants);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddRestaurant([FromBody] restaurants restaurantsRequest)
        {
            restaurantsRequest.RestaurantId = Guid.NewGuid();
            await _restoSpotDbContext.restaurants.AddAsync(restaurantsRequest);
            await _restoSpotDbContext.SaveChangesAsync();

            return Ok(restaurantsRequest);
        }

        [Authorize]
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetRestaurant([FromRoute] Guid id)
        {
            var restaurant = await (from r in _restoSpotDbContext.restaurants
                                    join cu in _restoSpotDbContext.cuisine on r.CuisineId equals cu.CuisineId
                                    join ci in _restoSpotDbContext.city on r.CityId equals ci.CityId
                                    select new restaurantInformation()
                                    {
                                        RestaurantId = r.RestaurantId,
                                        Name = r.Name,
                                        Address = r.Address,
                                        CityId = r.CityId,
                                        CuisineId = r.CuisineId,
                                        Rating = r.Rating,
                                        Reviews = r.Reviews,
                                        Cuisine = cu.Cuisine,
                                        City = ci.City
                                    }).FirstOrDefaultAsync(x => x.RestaurantId == id);

            if (restaurant == null)
            {
                return NotFound();
            }

            return Ok(restaurant);
        }

        [Authorize]
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateRestaurant([FromRoute] Guid id, restaurants updateRestaurantRequest)
        {
            var restaurant = await _restoSpotDbContext.restaurants.FindAsync(id);

            if(restaurant == null)
            {
                return NotFound();
            }

            restaurant.Name = updateRestaurantRequest.Name;
            restaurant.Address = updateRestaurantRequest.Address;
            restaurant.CityId = updateRestaurantRequest.CityId;
            restaurant.CuisineId = updateRestaurantRequest.CuisineId;
            restaurant.Rating = updateRestaurantRequest.Rating;
            restaurant.Reviews = updateRestaurantRequest.Reviews;

            await _restoSpotDbContext.SaveChangesAsync();

            return Ok(restaurant);
        }

        [Authorize]
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteRestaurant([FromRoute] Guid id)
        {
            var restaurant = await _restoSpotDbContext.restaurants.FindAsync(id);

            if (restaurant == null)
            {
                return NotFound();
            }

            _restoSpotDbContext.restaurants.Remove(restaurant);

            await _restoSpotDbContext.SaveChangesAsync();

            return Ok(restaurant);
        }
    }
}
