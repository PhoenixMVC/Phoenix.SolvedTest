using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Phoenix.Data.Repository;
using Phoenix.SolvedTest.Data;
using Phoenix.SolvedTest.Models;
using System.Linq;

namespace Phoenix.SolvedTest
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

            services.AddIdentity<User, Role, GenericDbContext>();

            services.AddMemoryCache();
            services.Configure<IdentityOptions>(Configuration.GetSection(nameof(IdentityOptions)));

            services.AddEntityFrameworkSqlServer();

            services.AddDbContextPool<GenericDbContext>((serviceProvider, options) =>
            {
                var cnn = Configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(cnn, sqlOptions => sqlOptions.MigrationsAssembly("Phoenix.SolvedTest"));
                options.UseInternalServiceProvider(serviceProvider);
            });

            services.AddRepository(options =>
            {
                options.Assemblies.Add(typeof(DataMarker).Assembly);
            });

            services.AddScoped<IDbContext>(provider => provider.GetRequiredService<GenericDbContext>());
            services.Configure<IdentityOptions>(options => Configuration.GetSection("IdentityOptions"));

            services.AddAntiforgery();

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = System.IO.Compression.CompressionLevel.Optimal;
            });

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                     "image/svg+xml",
                     "application/atom+xml",
                     // General
                     "text/plain",
                    // Static files
                    "text/css",
                    "application/javascript",
                    // MVC
                    "text/html",
                    "application/xml",
                    "text/xml",
                    "application/json",
                    "text/json",
                });
            });
            services.AddControllersWithViews()
            .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseCookiePolicy();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }
            else
            {
                app.UseExceptionHandler("/error/index/500");
                app.UseStatusCodePagesWithReExecute("/error/index/{0}");
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseStaticFiles();

            app.SeedData();

            app.UseRouting();

            app.UseAuthorization();

            app.UseResponseCompression();

            app.UseEndpoints(endpoints =>
            {
              
                endpoints.MapAreaControllerRoute(
                            "admin",
                            "admin",
                            "Admin/{controller=Home}/{action=Index}/{id?}");


                endpoints.MapControllerRoute(
                            name: "default",
                            pattern: "{controller=Home}/{action=Index}/{id?}",
                            defaults: new { controller = "Home", action = "Index" });

            });

        }

    }
}
