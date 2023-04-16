using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Phoenix.Identity
{
    public class UserBase : IdentityUser<int>
    {
        public virtual ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
    }
    public class RoleBase : IdentityRole<int>
    {

        public RoleBase() { }
        public RoleBase(string roleName) : base(roleName)
        {

        }
    }

    public class UserRole : IdentityUserRole<int>
    {

    }

    public class UserClaim : IdentityUserClaim<int> { }

    public class RoleClaim : IdentityRoleClaim<int> { }

    public class UserToken : IdentityUserToken<int> { }
    public class UserLogin : IdentityUserLogin<int> { }
}