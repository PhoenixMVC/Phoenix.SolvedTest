using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Phoenix.Identity;

namespace Microsoft.AspNetCore.Identity
{
    public static class IdentityBuilderExtensions
    {
        public static IdentityBuilder AddEntityFrameworkStores<TUser, TRole, TContext>(this IdentityBuilder builder)
            where TUser : UserBase
            where TRole : RoleBase
            where TContext : DbContext
        {
            var userStoreType = typeof(UserStore<TUser, TRole, TContext, int, UserClaim, UserRole, UserLogin, UserToken, RoleClaim>);
            var roleStoreType = typeof(RoleStore<TRole, TContext, int, UserRole, RoleClaim>);

            builder.Services.TryAddScoped(typeof(IUserStore<TUser>), userStoreType);
            builder.Services.TryAddScoped(typeof(IRoleStore<TRole>), roleStoreType);

            return builder;
        }
    }
}
