using System.ComponentModel.DataAnnotations;

namespace RestoSpotAPI.Models
{
    public class restaurants
    {
        [Key]
        public Guid RestaurantId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public Guid CityId { get; set; }
        public Guid CuisineId { get; set; }
        public decimal Rating { get; set; }
        public long Reviews { get; set; }
    }
}
