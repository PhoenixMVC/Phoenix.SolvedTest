using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Phoenix.RouteAuthorization
{
    public class RouteAuthorizationHandler : AuthorizationHandler<RouteAuthorizationRequirement>
     
    {
        private readonly IEnumerable<IRouteAuthorizationEvaluvator> _evaluvators;
        public RouteAuthorizationHandler(IEnumerable<IRouteAuthorizationEvaluvator> evaluvators)
        {
            _evaluvators = evaluvators;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RouteAuthorizationRequirement requirement)
        {
            foreach (var evaluvator in _evaluvators)
            {
                if(await evaluvator.IsAllowed(context))
                {
                     context.Succeed(requirement);
                    break;
                }
            }        
           
        }
    }
}
