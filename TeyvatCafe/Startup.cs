using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using TeyvatCafe.Models;

namespace TeyvatCafe
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        IConfiguration Configuration;

        public Startup (IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddAuthentication(opt => { opt.DefaultScheme = "User_Scheme"; }).
                AddCookie("User_Scheme",opt =>
                {

                    opt.LoginPath = "/auth/login";
                    opt.LogoutPath = "/auth/logout";
                    opt.AccessDeniedPath = "/";
                    opt.ExpireTimeSpan = TimeSpan.FromHours(1);
                    opt.Cookie.Name = "TeyvatCafe19DTHE4";


                }).
                AddCookie("Staff_Scheme",option =>
                {
                    option.LoginPath = "/adminauth/login";
                    option.AccessDeniedPath = "/admin";                   
                    option.LogoutPath = "/adminauth/logout";
                    option.ExpireTimeSpan = TimeSpan.FromMinutes(1);
                    option.Cookie.Name = "TeyvatStaff";
                });
           





            services.AddDbContext<DataContext>(p => p.UseSqlServer(Configuration.GetConnectionString("Data")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            //app.UseSession();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
