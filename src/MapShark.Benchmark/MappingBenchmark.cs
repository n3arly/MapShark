using AutoMapper;
using BenchmarkDotNet.Attributes;

namespace MapShark.Benchmark
{
    [MemoryDiagnoser]
    public class MappingBenchmark
    {
        private readonly Models.Class.Source classSource;
        private readonly Models.Record.Source recordSource;
        private readonly Models.RecordStruct.Source recordStructSource;

        private readonly Abstractions.IMapper mapShark;
        private readonly IMapper autoMapper;
        private readonly IMapper autoMapperCustom;

        public MappingBenchmark()
        {
            classSource = new Models.Class.Source { Id = 1, Name = "Test" };
            recordSource = new Models.Record.Source(1, "Test", "Description", new DateTime(), 0, "string", 4.1, 4, true, new DateTime(), Guid.NewGuid(), 123123123, 3.14f, 'a');
            recordStructSource = new Models.RecordStruct.Source(1, "Test", "Description", new DateTime(), 0, "string", 4.1, 4, true, new DateTime(), Guid.NewGuid(), 123123123, 3.14f, 'a');

            mapShark = new Implementations.Mapper();

            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Models.Class.Source, Models.Class.Destination>();
                cfg.CreateMap<Models.Record.Source, Models.Record.Destination>();
                cfg.CreateMap<Models.RecordStruct.Source, Models.RecordStruct.Destination>();
            });

            autoMapper = config.CreateMapper();
            MapperConfiguration customConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Models.Class.Source, Models.Class.Destination>().ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Name));
                cfg.CreateMap<Models.Record.Source, Models.Record.Destination>().ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Name));
                cfg.CreateMap<Models.RecordStruct.Source, Models.RecordStruct.Destination>().ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Name));
            });
            autoMapperCustom = customConfig.CreateMapper();
        }

        [Benchmark]
        public Models.Class.Destination MapSharkClass()
        {
            return mapShark.Map<Models.Class.Source, Models.Class.Destination>(classSource);
        }

        [Benchmark]
        public Models.Class.Destination AutoMapperClass()
        {
            return autoMapper.Map<Models.Class.Source, Models.Class.Destination>(classSource);
        }

        [Benchmark]
        public Models.Record.Destination MapSharkRecord()
        {
            return mapShark.Map<Models.Record.Source, Models.Record.Destination>(recordSource);
        }

        [Benchmark]
        public Models.Record.Destination AutoMapperRecord()
        {
            return autoMapper.Map<Models.Record.Source, Models.Record.Destination>(recordSource);
        }

        [Benchmark]
        public Models.RecordStruct.Destination MapSharkRecordStruct()
        {
            return mapShark.Map<Models.RecordStruct.Source, Models.RecordStruct.Destination>(recordStructSource);
        }

        [Benchmark]
        public Models.RecordStruct.Destination AutoMapperRecordStruct()
        {
            return autoMapper.Map<Models.RecordStruct.Source, Models.RecordStruct.Destination>(recordStructSource);
        }

        [Benchmark]
        public Models.Class.Destination MapSharkCustomMappingClass()
        {
            return mapShark.Map<Models.Class.Source, Models.Class.Destination>(classSource);
        }

        [Benchmark]
        public Models.Class.Destination AutoMapperCustomMappingClass()
        {
            return autoMapperCustom.Map<Models.Class.Source, Models.Class.Destination>(classSource);
        }

        [Benchmark]
        public Models.Record.Destination MapSharkCustomMappingRecord()
        {
            return mapShark.Map<Models.Record.Source, Models.Record.Destination>(recordSource);
        }

        [Benchmark]
        public Models.Record.Destination AutoMapperCustomMappingRecord()
        {
            return autoMapperCustom.Map<Models.Record.Source, Models.Record.Destination>(recordSource);
        }

        [Benchmark]
        public Models.RecordStruct.Destination MapSharkCustomMappingRecordStruct()
        {
            return mapShark.Map<Models.RecordStruct.Source, Models.RecordStruct.Destination>(recordStructSource);
        }

        [Benchmark]
        public Models.RecordStruct.Destination AutoMapperCustomMappingRecordStruct()
        {
            return autoMapperCustom.Map<Models.RecordStruct.Source, Models.RecordStruct.Destination>(recordStructSource);
        }
    }
}
