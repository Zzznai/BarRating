using BarRating.Commons;
using BarRating.Data;
using BarRating.Data.Entities;
using BarRating.Models.Bars;
using BarRating.Models.Users;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarRating.Controllers
{
    public class BarsController : Controller
    {
        private readonly BarRatingDbContext dbContext;

        public BarsController(BarRatingDbContext dbContext, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.cloudinarySettings = configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>();
            this.cloudinary = new Cloudinary(new Account(cloudinarySettings.CloudName, cloudinarySettings.ApiKey, cloudinarySettings.ApiSecret));
        }

        private readonly Cloudinary cloudinary;
        private readonly CloudinarySettings cloudinarySettings;

        public async Task<IActionResult> Index()
        {
            var bars = this.dbContext.Bars.Include(b => b.Reviews).ToList();
            return View(bars);
        }

        public IActionResult Create()
        {
            var bar = new BarCreateViewModel();
            return View(bar);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BarCreateViewModel barModel)
        {
            if (!ModelState.IsValid)
            {
                return View(barModel);
            }

            // Check if file size exceeds 2 megabytes
            if (barModel.Photo != null && barModel.Photo.Length > 2 * 1024 * 1024) // 2 MB in bytes
            {
                ModelState.AddModelError("Picture", "The image file size must not exceed 2 megabytes.");
            }

            if (ModelState.IsValid)
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(barModel.Photo.FileName, barModel.Photo.OpenReadStream()),
                };

                var result = this.cloudinary.Upload(uploadParams);

                var imageUrl = result.Url;

                var bar = new Bar()
                {
                    Name = barModel.Name,
                    Description = barModel.Description,
                    PhotoUrl = imageUrl.OriginalString
                };

                await this.dbContext.Bars.AddAsync(bar);
                await this.dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("/Bars/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var bar = await this.dbContext.Bars.Include(b => b.Reviews)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bar == null)
            {
                return NotFound();
            }

            return View(bar);
        }

        [HttpPost]
        [Route("/Bars/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var bar = this.dbContext.Bars.FirstOrDefault(x => x.Id == id);

            if (bar == null)
            {
                return NotFound();
            }

            this.dbContext.Remove(bar);
            await this.dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        [HttpGet]
        [Route("/Bars/Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var bar = await this.dbContext.Bars.FirstOrDefaultAsync(c => c.Id == id);

            if (bar == null)
            {
                return NotFound();
            }

            var carModel = new BarEditViewModel()
            {
                Name = bar.Name,
                Description = bar.Description
            };

            return View(carModel);
        }

        [HttpPost]
        [Route("/Bars/Edit/{id}")]
        public async Task<IActionResult> Edit(int id, BarEditViewModel barModel)
        {
            if (ModelState.IsValid)
            {
                var bar = await this.dbContext.Bars.FirstOrDefaultAsync(c => c.Id == id);

                if (bar == null)
                {
                    return NotFound();
                }

                // Handle image upload if provided
                if (barModel.Photo != null)
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(barModel.Photo.FileName, barModel.Photo.OpenReadStream()),
                    };

                    var result = this.cloudinary.Upload(uploadParams);
                    var imageUrl = result.Url;

                    bar.PhotoUrl = imageUrl.OriginalString;
                }

                bar.Name = barModel.Name;
                bar.Description = barModel.Description;

                this.dbContext.Bars.Update(bar);
                await this.dbContext.SaveChangesAsync();

                return RedirectToAction("Index");

            }

            return View(barModel);
        }
    }
}
