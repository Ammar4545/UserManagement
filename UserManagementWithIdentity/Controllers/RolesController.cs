using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagementWithIdentity.ViewModels;

namespace UserManagementWithIdentity.Controllers
{
    [Authorize(Roles ="Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(RoleFormViewModel roleFormViewModel)
        {
            if (!ModelState.IsValid)
            {
                //i want to return to index"in the same page" but i must reload all data again 
                return View("Index", _roleManager.Roles.ToListAsync());
            }
            //now he send me a role i want to know if this role exists 
            // i should use [RoleExistsAsync] 

            var result = await _roleManager.RoleExistsAsync(roleFormViewModel.Name);

            if (result)
            {
                ModelState.AddModelError("Name", "Role Is Exists!!");
                return View("Index", _roleManager.Roles.ToListAsync());
            }

            await _roleManager.CreateAsync(new IdentityRole(roleFormViewModel.Name.Trim()));

            return RedirectToAction(nameof(Index));
        }
    }
}
