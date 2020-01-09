using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace iBlog
{
    public class AppRole:IdentityRole
    {
        public string RoleDescription { get; set; }
    }
}
