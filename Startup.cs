using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RoomForRent.Models;

namespace RoomForRent
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var mvcBuilder = services.AddControllersWithViews();
            mvcBuilder.AddRazorRuntimeCompilation();

            services.AddSingleton<IRenterRepository, InMemoryRenterRepository>();
            services.AddSingleton<ILeaserRepository, InMemoryRoomLeaserRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseStatusCodePages();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("Leaser",
                    "Leaser/Page{pageCount}",
                    new {
                        Controller = "Leaser",
                        action = "Index"
                    });

                endpoints.MapControllerRoute("LeaserHistory",
                    "Leaser/History/Page{pageCount}"
                    , new
                    {
                        Controller = "Leaser",
                        action = "History",
                    });

                endpoints.MapControllerRoute("Renter",
                    "Renter/Page{pageCount}",
                    new {
                        Controller = "Renter",
                        action = "Index"
                    });
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
