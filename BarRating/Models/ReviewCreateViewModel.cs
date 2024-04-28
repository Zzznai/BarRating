using System.ComponentModel.DataAnnotations;

namespace BarRating.Models
{
    public class ReviewCreateViewModel
    {
        public int Id { get; set; }

        public string Comment { get; set; }
    }
}
