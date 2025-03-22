namespace MapShark.Abstractions
{
    public interface IMapper
    {
        TDestination Map<TSource, TDestination>(TSource source);
    }
}
