using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using iBlog.Data;
using System.ComponentModel.DataAnnotations;

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

        public List<IdentityRole> UsersRolesList { get; set; }
        public List<AppUser> UsersList { get; set; }

        public void OnGet()
        {
            UsersRolesList = _roleManager.Roles.ToList();
            UsersList = _userManager.Users.ToList();
        }
    }
}

