using System;

namespace Application.Exceptions;

public class DuplicateEntityException: Exception
{
    public string EntityName { get; }
    public string PropertyName { get; }
    public string PropertyValue { get; }

    public DuplicateEntityException(string entityName, string propertyName, string propertyValue)
        : base($"A {entityName} with {propertyName} '{propertyValue}' already exists.")
    {
        EntityName = entityName;
        PropertyName = propertyName;
        PropertyValue = propertyValue;
    }

    public DuplicateEntityException(string entityName, string propertyName, string propertyValue, Exception innerException)
        : base($"A {entityName} with {propertyName} '{propertyValue}' already exists.", innerException)
    {
        EntityName = entityName;
        PropertyName = propertyName;
        PropertyValue = propertyValue;
    } 
}
