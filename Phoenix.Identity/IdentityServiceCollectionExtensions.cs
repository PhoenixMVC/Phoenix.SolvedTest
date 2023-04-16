using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Phoenix.Data.Repository;
using Phoenix.Identity;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServiceCollectionExtensions
    {
        public static IdentityBuilder AddIdentity<TUser, TRole, TContext>(this IServiceCollection services)
            where TUser : UserBase
            where TRole : RoleBase
            where TContext : DbContext
        {
            // var config = services.GetService<IConfiguration>();
            services.AddMvcRouteAuthorization<RouteAuthorizationEvaluvator<TContext>>();

            services.AddScoped<IdentityErrorDescriber, CustomIdentityErrorDescriber>();
            var builder = services.AddIdentity<TUser, TRole>();
            builder.AddEntityFrameworkStores<TUser, TRole, TContext>();
            builder.AddDefaultTokenProviders();

            builder.Services.TryAddEnumerable(ServiceDescriptor.Scoped<IModelConfiguration, IdentityModelConfiguration<TUser, TRole>>());

            //  services.Configure<IdentityOptions>(config.GetSection(nameof(IdentityOptions)));

            return builder;
        }

        //private static T GetService<T>(this IServiceCollection services)
        //{
        //    return (T)services.FirstOrDefault(x => x.ServiceType == typeof(T))?.ImplementationInstance;
        //}
    }
}
