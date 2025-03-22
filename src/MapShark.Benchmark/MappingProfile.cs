using MapShark.Abstractions;
using MapShark.Configuration;

namespace MapShark.Benchmark
{
    public class MappingProfile : IMappingProfile
    {
        public void ConfigureMapping()
        {
            MapperConfigurationRegistry.For<Models.Class.Source, Models.Class.Destination>().Map(src => src.Name, dest => dest.Description);
            MapperConfigurationRegistry.For<Models.Record.Source, Models.Record.Destination>().Map(src => src.Name, dest => dest.Description);
            MapperConfigurationRegistry.For<Models.RecordStruct.Source, Models.RecordStruct.Destination>().Map(src => src.Name, dest => dest.Description);
        }
    }
}
