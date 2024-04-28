using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BarRating.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "The First Name field is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "The First Name must be between {2} and {1} characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "The Last Name field is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "The Last Name must be between {2} and {1} characters.")]
        public string LastName { get; set; }
    }
}
