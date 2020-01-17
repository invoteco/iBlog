using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using iBlog.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Authentication;
using System.Text.RegularExpressions;

namespace iBlog
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
 
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<AppUser, AppRole>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddRoles<AppRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultUI()
                    .AddDefaultTokenProviders();
            services.AddMvc();
            services.AddRazorPages();
            //services.AddHttpContextAccessor();
            //==============================================================

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.MaxAge = TimeSpan.FromDays(60);
            });

            services.AddHttpsRedirection(options =>
            {
                options.HttpsPort = 443;
            });
            //=============================================================
            services.AddAuthentication()
                    .AddFacebook(facebookOptions =>
                    {
                        //facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                        //facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                        facebookOptions.AppId = "2499576780164175";
                        facebookOptions.AppSecret = "f64232bbd8dc5faf62ce12a181fa679e";
                        //=======================https://stackoverflow.com/questions/54706987/override-redirect-url-in-addmicrosoftaccount-identity-oauth-for-asp-net-core-w
                        //facebookOptions.Events.OnRedirectToAuthorizationEndpoint = context =>
                        //{
                        //    context.Response.Redirect(Regex.Replace(context.RedirectUri, "redirect_uri=(.)+%2Fsignin-facebook", "redirect_uri=https%3A%2F%2Finvoteco.com%2Fsignin-facebook"));
                        //    return Task.FromResult(0);
                        //};
                        //==============================================================================================
                    })
                    .AddGoogle(options =>
                    {
                        options.ClientId = "333521974004-bobqq9cbgqtctadnq9fv12cuk57ae0tl.apps.googleusercontent.com";
                        options.ClientSecret = "1jCNz9XfnjS2aVwB_XAjT6J7";
                    })

                    .AddMicrosoftAccount(microsoftOptions =>
                    {
                        //microsoftOptions.ClientId = Configuration["Authentication:Microsoft:ClientId"];
                        //microsoftOptions.ClientSecret = Configuration["Authentication:Microsoft:ClientSecret"];
                        microsoftOptions.ClientId = "7336db4d-7bda-48a3-9236-d88736098828";
                        microsoftOptions.ClientSecret = "YKd]byAXDn0bJGbpZ10XVVv?@xTYtk?9";

                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //При использовании UseHttpsRedirection() получаем ошибку о множественном редиректе
            app.UseHttpsRedirection();//
            //==================================================================+
            //app.Use((context, next) =>
            //{
            //    if (context.Request.Headers["x-forwarded-proto"] == "https")
            //    {
            //        context.Request.Scheme = "https";
            //    }
            //    return next();
            //});
            //=================================================================+


            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }

    }
}
