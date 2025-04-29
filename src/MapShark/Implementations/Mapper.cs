using MapShark.Abstractions;
using MapShark.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MapShark.Implementations
{
    public class Mapper : IMapper
    {
        private static readonly ConcurrentDictionary<(Type, Type), object> _cache = new ConcurrentDictionary<(Type, Type), object>();

        public TDestination Map<TSource, TDestination>(TSource source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            (Type, Type) key = (typeof(TSource), typeof(TDestination));
            if (!_cache.TryGetValue(key, out object mapFuncObject))
            {
                Func<TSource, TDestination> mapFunc = CreateMapFunc<TSource, TDestination>();
                _cache.TryAdd(key, mapFunc);
                mapFuncObject = mapFunc;
            }
            Func<TSource, TDestination> mapDelegate = (Func<TSource, TDestination>)mapFuncObject;
            return mapDelegate(source);
        }

        public List<TDestination> MapCollection<TSource, TDestination>(IEnumerable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            List<TDestination> result = new List<TDestination>();

            foreach (TSource item in source)
                result.Add(Map<TSource, TDestination>(item));

            return result;
        }

        private static Func<TSource, TDestination> CreateMapFunc<TSource, TDestination>()
        {
            ParameterExpression sourceParameter = Expression.Parameter(typeof(TSource), "src");

            if (typeof(TDestination).GetConstructor(Type.EmptyTypes) != null)
                return CreateMapFuncWithConstructor<TSource, TDestination>(sourceParameter);
            else
                return CreateMapFuncWithoutConstructor<TSource, TDestination>(sourceParameter);
        }

        private static Func<TSource, TDestination> CreateMapFuncWithConstructor<TSource, TDestination>(ParameterExpression sourceParameter)
        {
            PropertyInfo[] destinationProperties = typeof(TDestination).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            Dictionary<string, PropertyInfo> destinationDictionaries = new Dictionary<string, PropertyInfo>(StringComparer.Ordinal);
            foreach (PropertyInfo destinationProperty in destinationProperties)
            {
                if (destinationProperty.CanWrite)
                    destinationDictionaries[destinationProperty.Name] = destinationProperty;
            }

            PropertyInfo[] sourceProperties = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            int count = 0;
            for (int i = 0; i < sourceProperties.Length; i++)
            {
                PropertyInfo sourceProperty = sourceProperties[i];
                if (!sourceProperty.CanRead)
                    continue;

                if (!MapperConfigurationRegistry.TryGetMapping<TSource, TDestination>(sourceProperty.Name, out string destPropName))
                    destPropName = sourceProperty.Name;

                if (destinationDictionaries.TryGetValue(destPropName, out PropertyInfo dp) && dp.PropertyType == sourceProperty.PropertyType)
                    count++;
            }

            MemberBinding[] bindings = new MemberBinding[count];
            int index = 0;
            for (int i = 0; i < sourceProperties.Length; i++)
            {
                PropertyInfo sourceProperty = sourceProperties[i];
                if (!sourceProperty.CanRead)
                    continue;

                if (!MapperConfigurationRegistry.TryGetMapping<TSource, TDestination>(sourceProperty.Name, out string destPropName))
                    destPropName = sourceProperty.Name;

                if (destinationDictionaries.TryGetValue(destPropName, out PropertyInfo dp) && dp.PropertyType == sourceProperty.PropertyType)
                    bindings[index++] = Expression.Bind(dp, Expression.Property(sourceParameter, sourceProperty));
            }

            return Expression.Lambda<Func<TSource, TDestination>>(
                Expression.MemberInit(Expression.New(typeof(TDestination)), bindings),
                sourceParameter
            ).Compile();
        }

        private static Func<TSource, TDestination> CreateMapFuncWithoutConstructor<TSource, TDestination>(ParameterExpression sourceParameter)
        {
            ConstructorInfo[] constructors = typeof(TDestination).GetConstructors();

            if (constructors.Length == 0)
                throw new InvalidOperationException("No public constructor found for " + typeof(TDestination).FullName);

            ConstructorInfo constructor = constructors.OrderByDescending(c => c.GetParameters().Length).First();
            ParameterInfo[] parameters = constructor.GetParameters();
            Expression[] arguments = new Expression[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo param = parameters[i];

                if (!MapperConfigurationRegistry.TryGetMapping<TSource, TDestination>(param.Name, out string sourcePropName))
                    sourcePropName = param.Name;

                PropertyInfo sourceProp = typeof(TSource).GetProperty(sourcePropName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (sourceProp != null && sourceProp.CanRead && sourceProp.PropertyType == param.ParameterType)
                    arguments[i] = Expression.Property(sourceParameter, sourceProp);
                else
                    arguments[i] = Expression.Default(param.ParameterType);
            }

            return Expression.Lambda<Func<TSource, TDestination>>(Expression.New(constructor, arguments), sourceParameter).Compile();
        }
    }
}
