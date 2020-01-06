using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using iBlog.Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace iBlog.Areas.Identity.Pages.Administration
{
    public class UsersInRolesModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public UsersInRolesModel(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext conText)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = conText;
        }
        [BindProperty]
        public List<IdentityRole> UsersRolesList { get; set; }
        public List<AppUser> UsersList { get; set; }

        public void OnGet()
        {
            UsersRolesList = _roleManager.Roles.ToList();
            UsersList = _userManager.Users.ToList();
        }

        public async Task<IActionResult> OnPostUpdateAsync(List<string> userroles)
        {
            string delimiter = "&";
            foreach (var userdata in userroles)
            {
                string[] parts = userdata.Split(delimiter);
                string useruid = parts[0];
                string userrole = parts[1];

                AppUser tappuser = await _userManager.FindByIdAsync(useruid);
                await AssignRoleToUser(tappuser, _context, _userManager, userrole, tappuser.Email);
            }
            return RedirectToPage("./UsersInRoles");
        }

        public async Task AssignRoleToUser(AppUser appuser, ApplicationDbContext dbcontext, UserManager<AppUser> usermanager, string rolename, string email)
        {
            var userId = appuser.Id;
            //Get all roles of the user by userId
            var allRoles = (from userRole in dbcontext.UserRoles.Where(ur => ur.UserId == userId).ToList()
                            join r in dbcontext.Roles
                            on userRole.RoleId equals r.Id
                            select r.Name).ToList();
            //assign role only if he does not have this roleName before            
            if (!allRoles.Contains(rolename))
            {
                var user = await usermanager.FindByIdAsync(userId);
                string em = user.Email;
                if (em == email)
                {
                    await usermanager.AddToRoleAsync(user, rolename);
                }
                //For removing a role, use await _userManager.RemoveFromRoleAsync(user, rolename);
            }
        }
    }
}

