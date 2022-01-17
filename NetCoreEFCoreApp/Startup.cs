using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCoreEFCoreApp.Persistences.EFCore.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreEFCoreApp
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
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddDbContext<AppDbContext>(); // uygulamanýn veri tabanýnýn instance buradan ioc container vasýtasý ile yöneteceðiz. uygulama bizim için dbContext üzerinden otomatik olarak instance alacaktýr.
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts(); // uygulama isteklerinin headerýna yani request header strict-trasport-security ekler. // http isteklerini https kanalýna yönledirir.
            }
            app.UseHttpsRedirection(); // bu servis ile bütün uygulama https isteklerine uygumlu hale gelir. MVC 5 de bir isteðin sadece Https olarak çalýþabileceðini söylemek için ilgili action üzerine [RequireHttps]  yazardýk.
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization(); // role ve claim based yetkilendirme middleware bu sayede Authorize attribute ile yöntemim saðlayacaðýz.

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
