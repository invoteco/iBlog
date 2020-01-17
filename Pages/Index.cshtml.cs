using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace iBlog.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }
        public void OnGet() {
            //Request.Protocol = "https";
            //Response.Redirect("https://invoteco.com",true);
            
        
        }
        //public void OnGet()
        //{
        //    //var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl }, protocol: "https");//Чтобы возврат на страницу был по https
        //    var redirectUrl = Url.PageLink("/", protocol: "https");//Чтобы возврат на страницу был по https
        //    RedirectToPage(redirectUrl);

        //}
        //public IActionResult OnGet()
        //{

        //    //var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl }, protocol: "https");//Чтобы возврат на страницу был по https
        //    string redirecturl = Url.PageLink("", protocol: "https");//Чтобы возврат на страницу был по https
        //    //RedirectToPage(redirectUrl);
        //    //await redirectUrl.
        //    return RedirectToPage(redirecturl);

        //}
    }
}
