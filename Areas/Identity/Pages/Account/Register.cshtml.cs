using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invoteco.Smtp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using iBlog.Data;
using Microsoft.Extensions.Configuration;

namespace iBlog.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;//Для реализации метода CreateRoles
        private readonly RoleManager<AppRole> _roleManager;//Для реализации метода CreateRoles
        private readonly IConfiguration _config;
        private readonly string AdminRoleDescription = "Может удалять пользователей";

        public RegisterModel(
               UserManager<AppUser> userManager,
               SignInManager<AppUser> signInManager,
               ILogger<RegisterModel> logger,
               IEmailSender emailSender,
               RoleManager<AppRole> roleManager,
               ApplicationDbContext conText, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _context = conText;
            _config = config;
        }
        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var callbackUrl = Url.Page(
                     "/Account/ConfirmEmail",
                     pageHandler: null,
                     values: new { area = "Identity", userId = user.Id, code = code },
                     protocol: "https");
#if DEBUG
                    #region Симуляция smtp-провайдера
                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                    #endregion Симуляция smtp-провайдера
#else
                    #region Реализация собственного smtp-провайдера
                    SmtpProvider smtpp = new SmtpProvider();
                    smtpp.IsBodyHtml = true;
                    smtpp.IsSSL = true;
                    smtpp.MessageBody = "<p>Вы получили это письмо потому что кто-то зарегистрировался на сайте invoteco.com и при регистрации указал этот адрес электронной почты.</p>" +
                        "<p>Если это были Вы, то подтвердите свой e-mail, нажав на ссылку 'Подтвердить'. В противном случае не предпринимайте никаких действий.</p>" +
                        "<p>После подтверждения e-mail Вы будете перенаправлены на сайт и после входа сможете воспользоваться всей его функциональностью.</p>" + "<a href=" + callbackUrl + ">ПОДТВЕРДИТЬ</a>";
                    smtpp.MessageSubject = "Подтверждение E-mail";
                    smtpp.SenderEmail = _config.GetValue<string>("SmtpSettings:Senderemail");
                    smtpp.SenderName = _config.GetValue<string>("SmtpSettings:Sendername");
                    smtpp.SmtpDomain = _config.GetValue<string>("SmtpSettings:Smtpdomain");
                    smtpp.SmtpPort = _config.GetValue<int>("SmtpSettings:Smtpport");
                    smtpp.UserEmail = user.Email;
                    if (smtpp.IsValidPatch())//Если путь к корневой директории соответствует установленному хостинг-провайдером,
                    {
                        //то отправляем пользователю письмо со ссылкой для подтверждения e-mail.
                        await smtpp.AsyncSendMailWithEncodeUrlPassworFree(smtpp.SenderName, smtpp.SenderEmail, smtpp.UserEmail, smtpp.MessageSubject, smtpp.MessageBody, smtpp.IsBodyHtml, smtpp.SmtpDomain, smtpp.SmtpPort, smtpp.IsSSL);
                    }
                    else
                    {
                        //В противном слуае отправляем письмо о проблеме администратору.
                        string WarningSubject = "Внимание!";
                        string AdminEmail = _config.GetValue<string>("SmtpSettings:Adminemail");
                        string WarningMessage = "Письма для подтверждения e-mail не доставляются. Проверьте настройки.";

                        await smtpp.AsyncSendMailWithEncodeUrlPassworFree(smtpp.SenderName, smtpp.SenderEmail, AdminEmail, WarningSubject, WarningMessage, smtpp.IsBodyHtml, smtpp.SmtpDomain, smtpp.SmtpPort, smtpp.IsSSL);
                    }
                    #endregion Реализация собственного smtp-провайдера
#endif //DEBUG
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        #region Присвоение первому пользователю admin@domain.tld роли "Admin"
                        //Метод CreateRolesAndAssignToUser в данном случае вызывается для того, чтобы создать перврго пользователя с ролью Admin.
                        //Этот пользователь создается при регистрации с учетными данными (email), указанными в аргументах метода. До его создания (и после) 
                        //в приложении может регистрироваться сколько угодно пользователей.
                        await CreateRolesAndAssignToUser(
                            user,
                            _context,
                            _roleManager,
                            _userManager,
                            _config.GetValue<string>("SmtpSettings:Adminname"),
                            _config.GetValue<string>("SmtpSettings:Adminemail"),
                            AdminRoleDescription);
                        #endregion Присвоение первому пользователю admin@domain.tld роли "Admin"

                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return Page();
        }

        /// <summary>
        /// Создает роль с именем rolename и присваивает ее пользователю appuser, использовавшему при регистрации Email email 
        /// Пример реализации: await CreateRolesAndAssignToUser(user, _context, _roleManager,_userManager, "Admin", "admin@domain.tld");
        /// </summary>
        /// <param name="appuser">Пользователь AppUser : IdentityUser</param>
        /// <param name="dbcontext">Контекст данных приложения</param>
        /// <param name="rolemanager">Менеджер ролей</param>
        /// <param name="usermanager">Менеджер пользователей</param>
        /// <param name="rolename">Имя роли, которая будет присвоена пользователю appuser</param>
        /// <param name="email">E-mail, использованный пользователем appuser при регистрации</param>
        /// <returns></returns>
            public async Task CreateRolesAndAssignToUser(
                AppUser appuser, 
                ApplicationDbContext dbcontext, 
                RoleManager<AppRole> rolemanager, 
                UserManager<AppUser> usermanager, 
                string rolename, 
                string email, 
                string adminroledescription)

        {
            bool x = await rolemanager.RoleExistsAsync(rolename);
            if (!x)
            {
                var role = new AppRole();
                role.Name = rolename;
                role.RoleDescription = adminroledescription;
                await rolemanager.CreateAsync(role);
            }
            var userId = appuser.Id;

            var allRoles = (from userRole in dbcontext.UserRoles.Where(ur => ur.UserId == userId).ToList()
                            join r in dbcontext.Roles
                            on userRole.RoleId equals r.Id
                            select r.Name).ToList();

            if (!allRoles.Contains(rolename))
            {
                var user = await usermanager.FindByIdAsync(userId);
                string em = user.Email;
                if (em == email)
                {
                    await usermanager.AddToRoleAsync(user, rolename);
                }
            }
        }
    }
}
