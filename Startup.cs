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

            services.AddHttpContextAccessor();

            services.AddAuthentication()
                    .AddFacebook(facebookOptions =>
                    {
                        //facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                        //facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                        facebookOptions.AppId = "2499576780164175";
                        facebookOptions.AppSecret = "f64232bbd8dc5faf62ce12a181fa679e";
                    })
                    .AddGoogle(options =>
                    {
                        //IConfigurationSection googleAuthNSection = Configuration.GetSection("Authentication:Google");                     
                        //options.ClientId = googleAuthNSection["ClientId"];
                        //options.ClientSecret = googleAuthNSection["ClientSecret"];
                        //IConfigurationSection googleAuthNSection = Configuration.GetSection("Authentication:Google");
                        options.ClientId = "333521974004-8v5cguju3kc176f183s7hm2vh43tofcf.apps.googleusercontent.com";
                        options.ClientSecret = "3DP1ArLZVYEvg1X8-3PL-oBJ";
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

            app.UseHttpsRedirection();
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
