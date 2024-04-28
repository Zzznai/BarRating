using System.ComponentModel.DataAnnotations;

namespace BarRating.Data.Entities
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Comment { get; set; }

        public DateTime PostedDate = DateTime.Now;

        public Bar Bar { get; set; }
        public ApplicationUser User { get; set; }
    }
}
