# What is MapShark?

MapShark is a mapping library that maps one object to another object simply and quickly.

### How do I get started?

Using MapShark is quite simple.

To start using MapShark, simply add the following code to your project's Program.cs file (for Dependency Injection).

```csharp
using MapShark.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMapShark();
```

You can then use it as follows:

```csharp
using MapShark.Abstractions;

[ApiController]
[Route("[controller]")]
public class ExampleController : ControllerBase
{
    private readonly IMapper _mapper;
    public ExampleController(IMapper mapper)
    {
        _mapper = mapper;
    }

    [HttpGet]
    public ActionResult Map()
    {
        Source source = new Source
        {
            Id = 1,
            Name = "Name",
        };

        return Ok(_mapper.Map<Source, Destination>(source));
    }
}
```

### Flex Mapping

MapShark offers a flexible mapping structure for data transformations. Thanks to this structure:

- **Properties with Different Names:** If there is a property like “FirstName” in the source object, you can map it to a different name like “Name” in the target object.

- **Custom Mappings:** Even if the property names are not the same, you can define specific transformations or mappings to achieve the desired data transfer.

This makes it easier to transfer and transform data between objects with different naming conventions.

#### Using Flex Mapping

In order to use Flex Mapping, we need a class created from the IMappingProfile interface. After creating this class as below and adding the ConfigureMapping method to it, the first thing we need to do is to specify which mapping operation will be used in it, we do this with the For method. Then you can specify which source property to assign to which target with the Map method. The Map function can be used in a chained way.

```csharp
using MapShark.Abstractions;
using MapShark.Configuration;

namespace Example
{
    public class MappingProfile : IMappingProfile
    {
        public void ConfigureMapping()
        {
            MapperConfigurationRegistry.For<Models.Class.Source, Models.Class.Destination>()
                    .Map(src => src.FirstName, dest => dest.Name)
                    .Map(src => src.Descr, dest => dest.Description);
        }
    }
}
```
Then just use the mapping for your related objects.

**If you don't want to create a profiler in this way**, you can use this construct instead, which will be executed just before the mapping.

```csharp
using MapShark.Abstractions;

[ApiController]
[Route("[controller]")]
public class ExampleController : ControllerBase
{
    private readonly IMapper _mapper;
    public ExampleController(IMapper mapper)
    {
        _mapper = mapper;
    }

    [HttpGet]
    public ActionResult Map()
    {
        Source source = new Source
        {
            Id = 1,
            Name = "Name",
        };
        MapperConfigurationRegistry.For<Models.Class.Source, Models.Class.Destination>()
                                        .Map(src => src.Name, dest => dest.Firstname)
                                        .Map(src => src.Id, dest => dest.IdentityNumber);
        return Ok(_mapper.Map<Source, Destination>(source));
    }
}
```