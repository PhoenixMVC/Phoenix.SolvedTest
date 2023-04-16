using Microsoft.EntityFrameworkCore;

namespace Phoenix.Data.Repository
{
    public interface IModelConfiguration
    {
        void Configure(ModelBuilder builder);
    }
}
