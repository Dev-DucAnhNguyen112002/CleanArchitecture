using System;
using System.Reflection;

public static class EnumExtensions
{
    public static string GetPropertyName(this Enum value)
    {
        // Get the field information for the enum value
        var fieldInfo = value.GetType().GetField(value.ToString());

        // Get the PropertyNameAttribute, if it exists
        var attribute = fieldInfo?.GetCustomAttribute<PropertyNameAttribute>();

        // Return the property name or the enum name if the attribute is missing
        return attribute?.Name ?? value.ToString();
    }
}

public class PropertyNameAttribute : Attribute
{
    public string Name { get; }
    public PropertyNameAttribute(string name)
    {
        Name = name;
    }
}
