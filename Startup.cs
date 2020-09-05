using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RoomForRent.Models;
using RoomForRent.Persistence.Contexts;
using RoomForRent.Repositories;

namespace RoomForRent
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var mvcBuilder = services.AddControllersWithViews();
            mvcBuilder.AddRazorRuntimeCompilation();

            services.AddDbContext<RoomForRentDbContext>(opt =>
                {
                    var connectionString = Configuration["ConnectionStrings:RoomForRentConnectionString"];
                    opt.UseSqlServer(connectionString);
                }
            );

            services.AddScoped<ILeaserRepository, EfLeaserRepository>();
            services.AddScoped<IRenterRepository, EfRenterRepository>();

            services.AddScoped<ITransactionRepository,
                EfRenterLeaserTransactionRepository>();

            //services.AddSingleton<ITransactionRepository, 
            //    InMemoryLeaserRenterTransactionRepository > ();

            //services.AddSingleton<ILeaserRepository, EfLeaserRepository>();
            //services.AddSingleton<IRenterRepository, EfRenterRepository>();

            //services.AddSingleton<IRenterRepository, InMemoryRenterRepository>();
            //services.AddSingleton<ILeaserRepository, InMemoryRoomLeaserRepository>();
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
                    new
                    {
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
                    new
                    {
                        Controller = "Renter",
                        action = "Index"
                    });
                endpoints.MapDefaultControllerRoute();
            });

            EnsurePopulated(app);
        }

        public static void EnsurePopulated(IApplicationBuilder app)
        {
            RoomForRentDbContext context = app.ApplicationServices
            .CreateScope().ServiceProvider.GetRequiredService<RoomForRentDbContext>();
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();

                context.SaveChanges();
            }
        }
    }
}
