using MapShark.Abstractions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MapShark.Configuration
{
    public static class MapperConfigurationRegistry
    {
        private static readonly Dictionary<(Type, Type), object> _configurations = new Dictionary<(Type, Type), object>();

        static MapperConfigurationRegistry()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            List<Type> profileTypes = new List<Type>();

            for (int i = 0; i < assemblies.Length; i++)
            {
                Type[] types;
                try
                {
                    types = assemblies[i].GetTypes();
                }
                catch
                {
                    types = Array.Empty<Type>();
                }

                for (int j = 0; j < types.Length; j++)
                {
                    Type t = types[j];

                    if (typeof(IMappingProfile).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
                        profileTypes.Add(t);
                }
            }

            for (int i = 0; i < profileTypes.Count; i++)
            {
                ((IMappingProfile)Activator.CreateInstance(profileTypes[i])).ConfigureMapping();
            }
        }

        /// <summary>
        /// Begins and registers a mapping configuration between the specified
        /// <typeparamref name="TSource"/> and <typeparamref name="TDestination"/> types,
        /// returning a <see cref="MapperConfigurator{TSource,TDestination}"/> for defining mapping rules.
        /// </summary>
        /// <typeparam name="TSource">The type of the source object to map from.</typeparam>
        /// <typeparam name="TDestination">The type of the destination object to map to.</typeparam>
        /// <returns>
        /// A <see cref="MapperConfigurator{TSource,TDestination}"/> instance that allows you
        /// to specify property-to-property mappings between the source and destination types.
        /// </returns>
        public static MapperConfigurator<TSource, TDestination> For<TSource, TDestination>()
        {
            MapperConfigurator<TSource, TDestination> config = new MapperConfigurator<TSource, TDestination>();
            _configurations[(typeof(TSource), typeof(TDestination))] = config;
            return config;
        }

        public static bool TryGetMapping<TSource, TDestination>(string sourcePropertyName, out string destinationPropertyName)
        {
            if (_configurations.TryGetValue((typeof(TSource), typeof(TDestination)), out object configObject))
                return ((MapperConfigurator<TSource, TDestination>)configObject).GetMappings().TryGetValue(sourcePropertyName, out destinationPropertyName);

            destinationPropertyName = null;
            return false;
        }
    }
}