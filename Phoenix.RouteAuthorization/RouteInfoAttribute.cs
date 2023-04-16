using System;

namespace Phoenix.RouteAuthorization
{
    public  class RouteInfoAttribute : Attribute
    {
        public String Name { get; set; }

        public String Icon { get; set; }
    }
}
