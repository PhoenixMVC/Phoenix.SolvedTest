using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Phoenix.RouteAuthorization
{
    public interface IRouteAuthorizationEvaluvator
    {
        Task<bool> IsAllowed(AuthorizationHandlerContext context);
    }
}
