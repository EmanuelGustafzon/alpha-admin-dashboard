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
                var value = sourceProp.GetValue(sourceObject);
                destinationProp.SetValue(destinationObjectInstance, value);
            }
        }

        return destinationObjectInstance;
    }
}
