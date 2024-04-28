using System.ComponentModel.DataAnnotations;

namespace BarRating.Models.Bars
{
    public class BarCreateViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please select a photo.")]
        public IFormFile Photo { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
