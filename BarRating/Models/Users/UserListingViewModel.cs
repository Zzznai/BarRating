using Microsoft.AspNetCore.Mvc;

namespace BarRating.Models.Users
{
    public class UserListingViewModel : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
