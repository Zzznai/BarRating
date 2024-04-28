using BarRating.Commons;
using BarRating.Data;
using BarRating.Data.Entities;
using BarRating.Models;
using BarRating.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarRating.Controllers
{

    public class ReviewsController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;

        private readonly BarRatingDbContext dbContext;

        public ReviewsController(BarRatingDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            ApplicationUser currentUser = await userManager.GetUserAsync(User);

            var reviews = new List<Review>();

            if (User.IsInRole(GlobalConstants.AdministratorRoleName))
            {
                reviews = dbContext.Reviews.Include(r => r.User).Include(r => r.Bar).ToList();
            }
            else
            {
                reviews = dbContext.Reviews.Include(r => r.User).Include(r => r.Bar).Where(r => r.User == currentUser).ToList();
            }

            return View(reviews);
        }

        [HttpPost]
        [Authorize]
        [Route("/Reviews/Create")]
        public async Task<IActionResult> Create(int barId, string comment)
        {
            ApplicationUser currentUser = await userManager.GetUserAsync(User);

            var bar = await dbContext.Bars.FirstOrDefaultAsync(x => x.Id == barId);

            var review = new Review()
            {
                Comment = comment,
                Bar = bar,
                User = currentUser
            };

            await this.dbContext.Reviews.AddAsync(review);
            await this.dbContext.SaveChangesAsync();

            return RedirectToAction("Details", "Bars", new { id = barId });
        }

        [HttpGet]
        [Authorize]
        [Route("/Reviews/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            ApplicationUser currentUser = await userManager.GetUserAsync(User);

            var review = await dbContext.Reviews
                .Include(r=>r.Bar)
                .Include(r=>r.User)
                .FirstOrDefaultAsync(r => r.Id == id);


            if (currentUser == review.User)
            {
                dbContext.Reviews.Remove(review);
                await dbContext.SaveChangesAsync();
            }


            return RedirectToAction("Details", "Bars", new { id = review.Bar.Id });
        }

        [Authorize]
        // GET: Reviews/GetReview?id=5
        [HttpGet]
        [Route("/Reviews/GetReview/{id}")]
        public async Task<IActionResult> GetReview(int? id)
        {
            var review = await dbContext.Reviews
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
            {
                return NotFound(); // If review is not found, return NotFound
            }

            return Ok(review);
        }

        [Authorize]
        [HttpPost]
        [Route("/Reviews/UpdateReview")]
        public async Task<IActionResult> UpdateReview([FromBody] ReviewCreateViewModel review)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            ApplicationUser currentUser = await userManager.GetUserAsync(User);

            // Check if the review exists
            var existingReview = await dbContext.Reviews.FindAsync(review.Id);
            if (existingReview == null)
            {
                return NotFound(); // If review does not exist, return NotFound
            }

            existingReview.Comment = review.Comment;
            existingReview.PostedDate = DateTime.Now;
            existingReview.User = currentUser;

            try
            {
                await dbContext.SaveChangesAsync(); 
                return Ok(existingReview); 
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
