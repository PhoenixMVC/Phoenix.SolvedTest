using System.Collections.Generic;
using System.Reflection;

namespace Phoenix.Data.Repository
{
    public class RepositoryOptions
    {
        public List<Assembly> Assemblies { get; } = new List<Assembly>();
    }
}
