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
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration.AzureKeyVault;

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

            services.AddHttpContextAccessor();//
            //services.AddAuthentication()
            //services.AddAuthentication(AzureADDefaults.AuthenticationScheme)

            services.AddAuthentication()
                    //.AddFacebook(facebookOptions =>
                    //{
                    //    facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                    //    facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                    //})
                    //.AddGoogle(options =>
                    //{
                    //    IConfigurationSection googleAuthNSection =
                    //    Configuration.GetSection("Authentication:Google");
                    //    options.ClientId = googleAuthNSection["ClientId"];
                    //    options.ClientSecret = googleAuthNSection["ClientSecret"];
                    //})
                   
                    .AddMicrosoftAccount(microsoftOptions =>
                    {
                        //microsoftOptions.ClientId = Configuration["Authentication:Microsoft:ClientId"];
                        microsoftOptions.ClientId = new KeyIdentifier(AzureKeyVaultConfigurationExtensions.;
                        microsoftOptions.ClientSecret = Configuration["Authentication:Microsoft:ClientSecret"];
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

            app.Run(async context =>
            {
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync($@"SecretName (Name in Key Vault: 'SecretName'){Environment.NewLine}Obtained from Configuration with Configuration[""SecretName""]{Environment.NewLine}Value: {Configuration["SecretName"]}{Environment.NewLine}{Environment.NewLine}Section:SecretName (Name in Key Vault: 'Section--SecretName'){Environment.NewLine}Obtained from Configuration with Configuration[""Section:SecretName""]{Environment.NewLine}Value: {Configuration["Section:SecretName"]}{Environment.NewLine}{Environment.NewLine}Section:SecretName (Name in Key Vault: 'Section--SecretName'){Environment.NewLine}Obtained from Configuration with Configuration.GetSection(""Section"")[""SecretName""]{Environment.NewLine}Value: {Configuration.GetSection("Section")["SecretName"]}");
            });
        }

    }
}
