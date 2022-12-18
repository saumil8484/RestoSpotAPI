using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestoSpotAPI.Models
{
    public class cuisine
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CuisineId { get; set; }
        public string Cuisine { get; set; }
    }
}
