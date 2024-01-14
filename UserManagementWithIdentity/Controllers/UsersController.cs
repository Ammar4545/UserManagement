using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagementWithIdentity.Models;
using UserManagementWithIdentity.ViewModels;

namespace UserManagementWithIdentity.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        //i inject this to be able to select data from "user" 'table' and "role" 'table'
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            //this will return list<ApplicationUser> but i want 'UserViewModel' [ var user = await _userManager.Users.ToListAsync(); ] (use Select)
            var user = await _userManager.Users.Select(user => new UsersViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                //i can not asign values with keyword await
                //so i used (Result)

                Roles = _userManager.GetRolesAsync(user).Result

            }).ToListAsync();

            return View(user);
        }

        public async Task<IActionResult> Add()
        {
            var roles = await _roleManager.Roles.Select(r => new RoleViewModel { RoleId = r.Id, RoleName = r.Name }).ToListAsync();

            var viewModel = new AddUserViewModel
            {
                Roles = roles
            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (!model.Roles.Any(r => r.IsSelected))
            {
                ModelState.AddModelError("Roles", "Plz select one role");
                return View(model);
            }
            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                ModelState.AddModelError("Email", "Email is exists");
                return View(model);
            }
            if (await _userManager.FindByNameAsync(model.UserName) != null)
            {
                ModelState.AddModelError("UserName", "User Name is exists");
                return View(model);
            }
            var user = new ApplicationUser
            {

                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            
             if (!result.Succeeded)
             {
                 foreach (var error in result.Errors)
                 {
                     ModelState.AddModelError("Roles", error.Description);
                 }
                 return View(model);
             }
            await _userManager.AddToRolesAsync(user, model.Roles.Where(r=>r.IsSelected).Select(r=>r.RoleName));
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var viewModel = new ProfileFormViewModel
            {
               Id= userId,
               FirstName=user.FirstName,
               LastName=user.LastName,
               UserName=user.UserName,
               Email=user.Email
            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);           
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                return NotFound();

            //if he send me used Eamil i will not save only if this email was for him
            var userWithSameEmail = await _userManager.FindByEmailAsync(model.Email);
            //this means i have email with this name but for another usear "can not save"
            if (userWithSameEmail!=null && userWithSameEmail.Id != model.Id )
            {
                ModelState.AddModelError("Email", "This Email is already used by another user");
                return View(model);
            }

            var userWithSameUserName = await _userManager.FindByNameAsync(model.UserName);

            if (userWithSameUserName != null && userWithSameUserName.Id != model.UserName)
            {
                ModelState.AddModelError("UserName", "This User Name is already used by another user");
                return View(model);
            }
            //updating data 
            user.FirstName = model.UserName;
            user.LastName = model.LastName;
            user.UserName = model.UserName;
            user.Email = model.Email;

            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));

        }

        //now i want to mange user role i will use the buttom 'Mangae Role' with take id 
        // i create two viewModel [UserRoleViewModel][RoleViewModel]
        public async Task<IActionResult> ManageRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _roleManager.Roles.ToListAsync();
            var viewModel = new UserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roless = roles.Select(role => new RoleViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    IsSelected = _userManager.IsInRoleAsync(user, role.Name).Result
                }).ToList()
            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //now i have model with all role [go to UserRolesViewModel]
        public async Task<IActionResult> ManageRoles(UserRolesViewModel model)
        {
            //when tab on button it takes user with its Id
            ////now u have model and data of one user
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return NotFound();
            //now i have roles of this user (GetRolesAsync)return roles of one user
            var userRole = await _userManager.GetRolesAsync(user);

            foreach (var role in model.Roless)
            {
                if (userRole.Any(r => r == role.RoleName) && !role.IsSelected)
                    await _userManager.RemoveFromRoleAsync(user, role.RoleName);
                if (!userRole.Any(r => r == role.RoleName) && role.IsSelected)
                    await _userManager.AddToRoleAsync(user, role.RoleName);
            }
            return RedirectToAction(nameof(Index));

        }

    }
}  













































//    public async Task<IActionResult> ManageRoles(string uesrId)
//    {
//        //if the id is not existed
//        var user = await _userManager.FindByIdAsync(uesrId);

//        if (user == null)
//        {
//            return NotFound();
//        }
//        //return the Role to be able to use it 
//        var roles = await _roleManager.Roles.ToListAsync();

//        var viewModel = new UserRolesViewModel
//        {
//            UserId = user.Id,
//            UserName = user.UserName,
//            //here i used select to show the [IsSelected] field in the RoleViewModel
//            Roless = roles.Select(role => new RoleViewModel
//            {
//                RoleId = role.Id,
//                RoleName = role.Name,
//                //here i want to know if the user in this role or not 
//                IsSelected = _userManager.IsInRoleAsync(user, role.Name).Result

//            }).ToList()
//        };
//        return View(viewModel);
//    }
//}