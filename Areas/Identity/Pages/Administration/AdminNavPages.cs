using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace iBlog.Areas.Identity.Pages.Administration
{
    public static class AdminNavPages
    {
        public static string Index => "Index";

        public static string Roles => "Roles";

        public static string UsersInRoles => "UsersInRoles";
        public static string IndexNavClass(ViewContext viewContext) => AdminPageNavClass(viewContext, Index);

        public static string RolesNavClass(ViewContext viewContext) => AdminPageNavClass(viewContext, Roles);
        public static string UsersInRolesNavClass(ViewContext viewContext) => AdminPageNavClass(viewContext, UsersInRoles);

        private static string AdminPageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
