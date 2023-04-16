using System.Collections.Generic;

namespace Phoenix.RouteAuthorization
{
    public class Route

    {
        public int Id { get; set; }

        public string Path { get; set; }

        public bool AllowAuthorizedUser { get; set; }

        public virtual ICollection<RoleAuthorizedRoute> AuthorizedRoles { get; set; } 
            = new HashSet<RoleAuthorizedRoute>();

        public virtual ICollection<UserAuthorizedRoute> AuthorizedUsers { get; set; } 
            = new HashSet<UserAuthorizedRoute>();

    }
}

