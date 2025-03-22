using MapShark.Abstractions;
using MapShark.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace MapShark.DependencyInjection
{
    public static class MapSharkServiceCollectionExtensions
    {
        public static IServiceCollection AddMapShark(this IServiceCollection services)
        {
            services.AddSingleton<IMapper, Mapper>();
            return services;
        }
    }
}
