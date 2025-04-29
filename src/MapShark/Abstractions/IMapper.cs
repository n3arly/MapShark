using System.Collections.Generic;

namespace MapShark.Abstractions
{
    public interface IMapper
    {
        /// <summary>
        /// Creates a new <typeparamref name="TDestination"/> instance by mapping the properties of the
        /// specified <paramref name="source"/> object of type <typeparamref name="TSource"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source object to map from.</typeparam>
        /// <typeparam name="TDestination">The type of the destination object to map to.</typeparam>
        /// <param name="source">The source object whose data will be mapped.</param>
        /// <returns>
        /// A new <typeparamref name="TDestination"/> instance containing the mapped values from
        /// the <paramref name="source"/> object.
        /// </returns>
        TDestination Map<TSource, TDestination>(TSource source);

        /// <summary>
        /// Creates a new <see cref="List{TDestination}"/> instance by mapping each element of the
        /// specified <paramref name="source"/> collection of type <typeparamref name="TSource"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source elements to map from.</typeparam>
        /// <typeparam name="TDestination">The type of the destination elements to map to.</typeparam>
        /// <param name="source">The source collection whose elements will be mapped.</param>
        /// <returns>
        /// A <see cref="List{TDestination}"/> containing the mapped values from
        /// the <paramref name="source"/> collection.
        /// </returns>
        List<TDestination> Map<TSource, TDestination>(IEnumerable<TSource> source);
    }
}
