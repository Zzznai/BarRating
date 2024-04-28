using System.ComponentModel.DataAnnotations;

namespace BarRating.Data.Entities
{
    public class Bar
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(64)]
        public string Name { get; set; }

        [Required]
        public string PhotoUrl { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; }

        public List<Review> Reviews { get; set; }
    }
}
