using System.Reflection;

namespace Domain.Extensions;

public static class MappingExtensions
{
    public static TDestinationObject MapTo<TDestinationObject>(this object sourceObject)
    {
        ArgumentNullException.ThrowIfNull(sourceObject, nameof(sourceObject));

        TDestinationObject destinationObjectInstance = Activator.CreateInstance<TDestinationObject>();

        var sourceProps = sourceObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var destinationsProperties = destinationObjectInstance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach ( var destinationProp in destinationsProperties)
        {
            var sourceProp = sourceProps.FirstOrDefault(x => x.Name == destinationProp.Name && x.PropertyType == destinationProp.PropertyType);
            if(sourceProp != null && destinationProp.CanWrite)
            {
                var sourceValue = sourceProp.GetValue(sourceObject);
                destinationProp.SetValue(destinationObjectInstance, sourceValue);
            }

            var sourceClassProp = sourceProps.FirstOrDefault(x => 
                x.Name == destinationProp.Name 
                && x.PropertyType.IsClass 
                && destinationProp.PropertyType.IsClass
                && x.PropertyType != typeof(string)
                && destinationProp.PropertyType != typeof(string));

            if(sourceClassProp != null && destinationProp.CanWrite)
            {
                var sourceValue = sourceClassProp.GetValue(sourceObject);
                if (sourceValue == null)
                    continue;
                var mapMethod = typeof(MappingExtensions)?.GetMethod(nameof(MapTo))?.MakeGenericMethod(destinationProp.PropertyType);
                if (mapMethod == null)
                    continue;
                var mappedValue = mapMethod.Invoke(null, new[] { sourceValue });
                destinationProp.SetValue(destinationObjectInstance, mappedValue);
            }
        }

        return destinationObjectInstance;
    }
}
