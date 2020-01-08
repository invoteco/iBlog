using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using iBlog.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace iBlog.Areas.Identity.Pages.Administration
{
    public class RolesModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ApplicationDbContext _context;//Для реализации метода CreateRoles

        public RolesModel(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager, ApplicationDbContext conText)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = conText;
        }

        public List<AppRole> Roles { get; set; }

        /// <summary>
        /// Получает все роли приложения
        /// </summary>
        /// <param name="dbcontext"></param>
        /// <returns></returns>
        public List<string> GetAllAppRoles(ApplicationDbContext dbcontext)
        {
            var allRoles = (from Role in dbcontext.Roles.ToList()
                            select Role.Name).ToList();
            return allRoles;
        }

        public void OnGet()
        {
            Roles = _roleManager.Roles.ToList();
        }

        public async Task<IActionResult> OnPostCreateAsync(string rolename, string roledescription)
        {

            bool x = await _roleManager.RoleExistsAsync(rolename);
            if (!x)
            {
                var role = new AppRole();
                role.Name = rolename;
                role.RoleDescription = roledescription;
                await _roleManager.CreateAsync(role);
            }
            return RedirectToPage("./Roles");
        }

        public async Task<IActionResult> OnPostDeleteAsync(string rolename)
        {
            AppRole role = await _roleManager.FindByNameAsync(rolename);
            await _roleManager.DeleteAsync(role);
            return RedirectToPage("./Roles");
        }
    }
}