using System;

namespace Application.Exceptions;

public class EntityNotFoundException : Exception
{
    public string EntityName { get; }
    public string PropertyName { get; }
    public string PropertyValue { get; }

    public EntityNotFoundException(string entityName, string propertyName, string propertyValue)
        : base($"A {entityName} with {propertyName} '{propertyValue}' was not found.")
    {
        EntityName = entityName;
        PropertyName = propertyName;
        PropertyValue = propertyValue;
    }

    public EntityNotFoundException(string entityName, string propertyName, string propertyValue, Exception innerException)
        : base($"A {entityName} with {propertyName} '{propertyValue}' was not found.", innerException)
    {
        EntityName = entityName;
        PropertyName = propertyName;
        PropertyValue = propertyValue;
    }
}
