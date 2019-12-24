using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using iBlog.Data;

namespace iBlog.Areas.Identity.Pages.Account
{
    public class RolesModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;

        public RolesModel(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }

        //await createRoles("2a5dede2-cb4c-4da3-b1d2-d8aa52028528","Admin");
        //Создает роль и привязывает к ней пользователя
        public async Task CreateRoles(string userId, string roleName)
        {
            bool x = await _roleManager.RoleExistsAsync(roleName);
            if (!x)//Create new role if not existing
            {
                var role = new IdentityRole();
                role.Name = roleName;
                await _roleManager.CreateAsync(role);
            }

            //Get all roles of the user by userId
            var allRoles = (from userRole in _context.UserRoles.Where(ur => ur.UserId == userId).ToList()
                            join r in _context.Roles
                            on userRole.RoleId equals r.Id
                            select r.Name).ToList();

            //assign role only if he does not have this roleName before            
            if (!allRoles.Contains(roleName))
            {
                var user = await _userManager.FindByIdAsync(userId);
                var result1 = await _userManager.AddToRoleAsync(user, roleName);
                //For removing a role, use await _userManager.RemoveFromRoleAsync(user, "Admin");
            }
        }
    }
}
