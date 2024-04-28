using BarRating.Commons;
using BarRating.Data;
using BarRating.Models;
using BarRating.Models.Bars;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BarRating.Controllers
{
    public class HomeController : Controller
    {
        private readonly BarRatingDbContext dbContext;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, BarRatingDbContext dbContext)
        {
            _logger = logger;
            this.dbContext = dbContext;
        }

        public IActionResult Index()
        {
            if (User.IsInRole(GlobalConstants.AdministratorRoleName))
            {
                return RedirectToAction("AdminIndex");
            }

            var searchModel = new BarsSearchViewModel();

            var barName = HttpContext.Session.GetString("searchBarName");

            if (String.IsNullOrEmpty(barName))
            {
                searchModel.Bars = dbContext.Bars.Include(b => b.Reviews).ToList();
            }
            else
            {
                searchModel.Bars = dbContext.Bars.Include(b => b.Reviews).Where(b => b.Name == barName).ToList();
            }

            return View(searchModel);
        }

        [HttpPost]
        public IActionResult Index(BarsSearchViewModel searchModel)
        {
            if (String.IsNullOrEmpty(searchModel.Name))
            {
                searchModel.Bars = dbContext.Bars.Include(b => b.Reviews).ToList();
            }
            else
            {
                HttpContext.Session.SetString("searchBarName", searchModel.Name);

                searchModel.Bars = dbContext.Bars.Include(b => b.Reviews).Where(b => b.Name == searchModel.Name).ToList();
            }

            return View(searchModel);
        }

        public async Task<IActionResult> AdminIndexAsync()
        {
            AdminHomePageViewModel adminHomePageViewModel = new AdminHomePageViewModel();
            adminHomePageViewModel.ReviewCounts = await this.dbContext.Reviews.CountAsync();
            adminHomePageViewModel.BarCounts = await this.dbContext.Bars.CountAsync();
            adminHomePageViewModel.UserCounts = await this.dbContext.Users.CountAsync();

            return View(adminHomePageViewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}