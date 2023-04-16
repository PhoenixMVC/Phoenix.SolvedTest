using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Phoenix.Data.Repository;
using Phoenix.SolvedTest.Models;
using Phoenix.RouteAuthorization;
using System;
using System.Linq;

namespace Microsoft.AspNetCore.Builder
{
    public static class SeedDataApplicationBuilderExtension
    {
        public static IApplicationBuilder SeedData(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IDbContext>();
                var adminUser = context.DbSet<User>().FirstOrDefault(x => x.UserName == "09028546415");

                
                if (adminUser == null)
                {
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                    adminUser = new User()
                    {
                        UserName = "09028546415",
                        Email = "phoenix@SolvedTest.com",
                        PhoneNumber = "09028546415",
                        EmailConfirmed = true,
                        PhoneNumberConfirmed=true,
                        FirstName ="مرضیه",
                        LastName="تقدسی",
                    };
                    var userResult = userManager.CreateAsync(adminUser, "123456").GetAwaiter().GetResult();

                    if (userResult.Succeeded)
                    {
                        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
                        var adminRole = new Role()
                        {
                            Name = "superadmin",
                            Description = "ادمین سایت",
                            ConcurrencyStamp = Guid.NewGuid().ToString()
                        };
                        roleManager.CreateAsync(adminRole).GetAwaiter().GetResult();
                        var addToRoleResult = userManager.AddToRoleAsync(adminUser, "superadmin").GetAwaiter().GetResult();
                        var result = context.SaveChangesAsync().GetAwaiter().GetResult();

                        var userRole1 = new Role()
                        {
                            Name = "admin",
                            Description = "مدیران سایت",
                            NormalizedName = "ADMIN"
                        };
                        roleManager.CreateAsync(userRole1).GetAwaiter().GetResult();
                        context.SaveChangesAsync();

                        var userRole2 = new Role()
                        {
                            Name = "user",
                            Description="کاربران عادی سایت - خریداران",
                            NormalizedName="USER"
                        };
                        roleManager.CreateAsync(userRole2).GetAwaiter().GetResult();
                        context.SaveChangesAsync();
                    }

                }
                RegistRoute(context, scope.ServiceProvider.GetRequiredService<IActionDescriptorCollectionProvider>());

             

            }
            return app;
        }
        public static void RegistRoute(IDbContext context, IActionDescriptorCollectionProvider actionDescriptor)
        {
            var oldRoutes = context.DbSet<Route>().ToList();

            var adminRole = context.DbSet<Role>().FirstOrDefault(x => x.Name == "superadmin");

            if (adminRole == null)
            {
                return;
            }

            var currentRoutes = actionDescriptor
                .ActionDescriptors
                .Items
                .Select(x => new
                {
                    Path = x.GetRouteName(),
                    Name = FriendlyName(x)
                })
                .GroupBy(x => x.Path).Select(x => new Route()
                {
                    Path = x.First().Path
                })
                .ToList();

            var roleAuthorizedRoutes = context.DbSet<RoleAuthorizedRoute>().ToList();
            var userAuthorizedRoutes = context.DbSet<UserAuthorizedRoute>().ToList();

            var deletedRoutes = oldRoutes.Where(x => currentRoutes.Any(r => r.Path == x.Path) == false);
            var roleDeletedRoutes = roleAuthorizedRoutes.Where(x => deletedRoutes.Any(r => r.Path == x.Route.Path));
            var userDeletedRoutes = userAuthorizedRoutes.Where(x => deletedRoutes.Any(r => r.Path == x.Route.Path));

            context.RemoveRange(roleDeletedRoutes);
            context.RemoveRange(userDeletedRoutes);
            context.RemoveRange(deletedRoutes);


            var newRoutes = currentRoutes.Where(x => oldRoutes.Any(r => r.Path == x.Path) == false).ToList();
            newRoutes.ForEach(route => route.AuthorizedRoles.Add(new RoleAuthorizedRoute()
            {
                Allow = true,
                RoleId = adminRole.Id
            }));

            context.AddRange(newRoutes);
            context.SaveChanges();
        }

        private static string FriendlyName(ActionDescriptor actionDescriptor)
        {
            var controller = actionDescriptor as ControllerActionDescriptor;
            if (controller != null)
            {
                var attribute = controller.MethodInfo.GetCustomAttributes(typeof(Phoenix.RouteAuthorization.RouteInfoAttribute), true).FirstOrDefault() as RouteInfoAttribute;
                return attribute?.Name ?? actionDescriptor.GetRouteName();

            }
            var page = actionDescriptor as PageActionDescriptor;
            return page.RelativePath;

        }

    }
}
