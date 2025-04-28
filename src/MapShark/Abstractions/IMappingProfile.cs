namespace MapShark.Abstractions
{
    public interface IMappingProfile
    {
        /// <summary>
        /// Configures and registers mappings between source and destination types
        /// by calling <see cref="Configuration.MapperConfigurationRegistry.For{TSource,TDestination}"/>
        /// and chaining <see cref="Configuration.MapperConfigurator{TSource,TDestination}.Map"/> calls.
        /// </summary>
        void ConfigureMapping();
    }
}
