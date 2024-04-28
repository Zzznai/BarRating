using Microsoft.Build.Framework;

namespace BarRating.Models.Authentication
{
    public class UserLoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
