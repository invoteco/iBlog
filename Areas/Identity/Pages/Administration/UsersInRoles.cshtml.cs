using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using iBlog.Data;

namespace iBlog.Areas.Identity.Pages.Administration
{
    public class UsersInRolesModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        public UsersInRolesModel(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [BindProperty]
        public List<AppRole> AllRolesList { get; set; }
        public List<AppUser> AllUsersList { get; set; }

        public void OnGet()
        {
            AllRolesList = _roleManager.Roles.ToList();
            AllUsersList = _userManager.Users.ToList();
        }

        public async Task<IActionResult> OnPostUpdateAsync(List<string> userroles)
        {
            //Сначала удаляем ВСЕ роли (кроме "Admin") ВСЕХ пользователей.
            AllRolesList = _roleManager.Roles.ToList();
            AllUsersList = _userManager.Users.ToList();

            foreach (var role in AllRolesList)
            {
                string rolename = role.Name;
                if (rolename != "Admin")
                {
                    foreach (var user in AllUsersList)
                    {
                        string userid = user.Id;
                        AppUser tempuser = await _userManager.FindByIdAsync(userid);
                        await _userManager.RemoveFromRoleAsync(tempuser, rolename);
                    }
                }
            }

            //Затем присваиваем новые роли в соответствии с выбором.
            string delimiter = "&";//Символ разделителя, разделяющий Id пользователя и назначенную ему роль в запросе 
            //(например, 546b7c31-cdca-4c51-a5d9-a84460a2df7e&Moderator). 
            //Этот же символ должен быть включен в запрос (см.value="@appuser.Id&@approle.Name" в UsersInRoles.cshtml) 
            foreach (var userdata in userroles)
            {
                string[] parts = userdata.Split(delimiter);
                string useruid = parts[0];
                string userrole = parts[1];
                AppUser tempappuser = await _userManager.FindByIdAsync(useruid);
                await _userManager.AddToRoleAsync(tempappuser, userrole);
            }
            return RedirectToPage("./UsersInRoles");
        }      
    }
}

