using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace iBlog.Areas.Identity.Pages.Roles.Manage
{
    public class ManageRolesNavPages
    {
        public static string Index => "Index";
        //public static string Index => "/Identity/Roles/Manage/Index";

        public static string AppRoles => "AppRoles";

        public static string IndexNavClass(ViewContext viewContext) => PageRolesNavClass(viewContext, Index);

        public static string AppRolesNavClass(ViewContext viewContext) => PageRolesNavClass(viewContext, AppRoles);

        //private static string PageNavClass(ViewContext viewContext, string page)
        //{
        //    var activePage = viewContext.ViewData["ActivePage"] as string
        //        ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
        //    return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        //}
        private static string PageRolesNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
