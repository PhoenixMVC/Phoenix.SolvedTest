using Microsoft.EntityFrameworkCore;

namespace Phoenix.RouteAuthorization
{
    public class RouteAuthorizationEntityConfigure
    {
        public void Configure(ModelBuilder builder)
        {
            builder.Entity<RoleAuthorizedRoute>(b =>
            {
                 b.HasKey(x => new { x.RoleId, x.RouteId });
                b.ToTable("RoleAuthorizedRoutes");
            });

            builder.Entity<UserAuthorizedRoute>(b =>
            {
                b.HasKey(x => new { x.UserId, x.RouteId });
                b.ToTable("UserAuthorizedRoutes");
            });
            builder.Entity<Route>(b =>
            {
                b.HasIndex(x => x.Path).IsUnique();
            });

        }
    }
}

