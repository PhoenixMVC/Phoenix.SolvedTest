using Microsoft.Extensions.DependencyInjection.Extensions;
using Phoenix.Data.Repository;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RepositoryServiceCollectionExtensions
    {
        public static IServiceCollection AddRepository(this IServiceCollection services, Action<RepositoryOptions> configure)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configure is null)
            {
                throw new ArgumentNullException(nameof(configure));
            }
            

            services.Configure(configure);
            return services;
        }
    }
}
