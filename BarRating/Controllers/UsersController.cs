using BarRating.Commons;
using BarRating.Data.Entities;
using BarRating.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BarRating.Controllers
{
    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = (await this.userManager.GetUsersInRoleAsync(GlobalConstants.UserRoleName)).ToList();
            return View(users);
        }

      
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            UserCreateViewModel userModel = new UserCreateViewModel();
            return View(userModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userModel);
            }

            if (ModelState.IsValid)
            {
    
                if (userManager.Users.Any(u => u.UserName == userModel.Username))
                {
                    ModelState.AddModelError(string.Empty, "A user with the same username already exists.");
                    return View(userModel);
                }

                var user = new ApplicationUser()
                {
                    UserName = userModel.Username,
                    FirstName = userModel.FirstName,
                    LastName = userModel.LastName
                };

                var result = await userManager.CreateAsync(user, userModel.Password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, GlobalConstants.UserRoleName);
                    return RedirectToAction("Index", "Users");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(userModel);
        }

        [HttpGet]
        [Route("/Users/Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await this.userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var userModel = new UserEditViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName
            };

            return View(userModel);
        }

        [HttpPost]
        [Route("/Users/Edit/{id}")]
        public async Task<IActionResult> Edit(string id, UserEditViewModel userModel)
        {
            var user = await this.userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            if (!this.ModelState.IsValid)
            {
                return this.View(userModel);
            }

            if (userManager.Users.Any(u => u.UserName == userModel.Username && u.Id != userModel.Id))
            {
                ModelState.AddModelError(string.Empty, "A user with the same username already exists.");
                return View(userModel);
            }

            user.UserName = userModel.Username;
            user.FirstName = userModel.FirstName;
            user.LastName = userModel.LastName;
           
            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                if (userModel.Password != null)
                {
                    await userManager.RemovePasswordAsync(user);
                    var passwordCheck = await userManager.AddPasswordAsync(user, userModel.Password);

                    if (!passwordCheck.Succeeded)
                    {
                        foreach (var error in passwordCheck.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                            return View(userModel);
                        }
                    }
                }
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(userModel);
        }

        [HttpPost]
        [Route("/Users/Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var result = await userManager.DeleteAsync(user);


            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return RedirectToAction("Index");
            }
        }
    }
}
