namespace Phoenix.RouteAuthorization
{
    public class UserAuthorizedRoute
    {
        public int UserId { get; set; }

        public int RouteId { get; set; }

        public bool Allow { get; set; }

     public virtual Route Route { get; set; }
    }
}


