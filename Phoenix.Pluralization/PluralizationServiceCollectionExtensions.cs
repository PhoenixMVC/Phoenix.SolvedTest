using Phoenix.Pluralization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PluralizationServiceCollectionExtensions
    {
        public static IServiceCollection AddPluralization(this IServiceCollection services)
        {
            services.TryAddSingleton<PluralizationService, EnglishPluralizationService>();
            return services;
        }
    }
}
