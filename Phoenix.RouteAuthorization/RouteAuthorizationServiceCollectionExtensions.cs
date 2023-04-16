using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Phoenix.RouteAuthorization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RouteAuthorizationServiceCollectionExtensions
    {
        public static IServiceCollection AddMvcRouteAuthorization<TEvaluvator>(this IServiceCollection services)
            where TEvaluvator : class, IRouteAuthorizationEvaluvator
        {
            services.AddScoped<IRouteAuthorizationEvaluvator, TEvaluvator>();
            services.TryAddScoped<IAuthorizationHandler, RouteAuthorizationHandler>();

            services.Configure<MvcOptions>(options =>
            {

                var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(new RouteAuthorizationRequirement())
                .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });
            return services;
        }
    }
}
