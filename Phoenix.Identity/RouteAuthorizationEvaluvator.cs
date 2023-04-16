using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Phoenix.RouteAuthorization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Phoenix.Identity
{
    public class RouteAuthorizationEvaluvator<TContext> : IRouteAuthorizationEvaluvator
         where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly IdentityOptions _options;
        public RouteAuthorizationEvaluvator(TContext context, IOptions<IdentityOptions> options)
        {
            _context = context;
            _options = options.Value;
        }
        public async Task<bool> IsAllowed(AuthorizationHandlerContext context)
        {
            var prencipal = context.User;
            var isanonymouse = prencipal?.Identity == null || !prencipal.Identities.Any(i => i.IsAuthenticated);
            if (isanonymouse)
            {
                return false;
            }

            if (context.Resource is AuthorizationFilterContext filterContext)
            {

                var actionDescriptor = filterContext.ActionDescriptor;
                var routePath = actionDescriptor.GetRouteName();

                var userId = int.Parse(prencipal.FindFirstValue(_options.ClaimsIdentity.UserIdClaimType));

                return await _context.Set<Route>()
                      .AnyAsync(route =>
                      route.Path == routePath &&
                      (
                      route.AuthorizedUsers.Any(user => user.UserId == userId && user.Allow != false) ||

                     route.AuthorizedRoles
                     .Join(_context.Set<UserRole>(), x => x.RoleId, x => x.RoleId, (authorizedRoles, userRoles) => new
                     {
                         AuthorizedRoles = authorizedRoles,
                         UserRoles = userRoles
                     })
                     .Any(x => x.UserRoles.UserId == userId && x.AuthorizedRoles.Allow != false)


                     ) &&
                     (
                     route.AllowAuthorizedUser ||
                        route.AuthorizedUsers.Any(user => user.UserId == userId && user.Allow == true) ||

                     route.AuthorizedRoles
                     .Join(_context.Set<UserRole>(), x => x.RoleId, x => x.RoleId, (authorizedRoles, userRoles) => new
                     {
                         AuthorizedRoles = authorizedRoles,
                         UserRoles = userRoles
                     }
                     )
                     .Any(x => x.UserRoles.UserId == userId && x.AuthorizedRoles.Allow == true)
                     )
                     );

            }
            return false;
        }
    }

}


