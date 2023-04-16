namespace Phoenix.RouteAuthorization
{
    public class RoleAuthorizedRoute
    {
        public int RoleId { get; set; }

        public int RouteId { get; set; }

        public bool Allow { get; set; }

        public virtual Route Route { get; set; }

    }
}

