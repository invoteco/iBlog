using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using iBlog.Data;
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
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;//Для реализации метода CreateRoles

        public RolesModel(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext conText)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = conText;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public string ReturnUrl { get; set; }
        public List<string> Roles { get; set; }
        public string RoleName { get; set; }
        public class InputModel
        {
            [Required]
            public string RoleName { get; set; }
        }


        /// <summary>
        /// Получает все роли приложения
        /// </summary>
        /// <param name="dbcontext"></param>
        /// <returns></returns>
        public List<string> GetAllAppRoles(ApplicationDbContext dbcontext)
        {
            var allRoles = (from Role in _context.Roles.ToList()
                            select Role.Name).ToList();
            return allRoles;
        }

        public async Task CreateAppRole(string rolename)
        {
            bool x = await _roleManager.RoleExistsAsync(rolename);
            if (!x)
            {
                var role = new IdentityRole();
                role.Name = rolename;
                await _roleManager.CreateAsync(role);
            }
        }

        public void OnGet()
        {
            Roles = GetAllAppRoles(_context);
        }

        public async Task<IActionResult> OnPostAsync(string newrolename)
        {           
            await CreateAppRole(newrolename);
            return RedirectToPage("./Roles");
        }
    }
}