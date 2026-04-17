using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace TestUtilities;

public static class AttributeTestUtilities
{
    public static void HasAttribute<T>(this Type type) where T : Attribute
    {
        var attributes = type.GetCustomAttributes(typeof(T), false);

        Assert.That(attributes.Length, Is.GreaterThan(0));
    }

    public static PropertyInfo HasProperty(this Type type, string name)
    {
        var properties = type.GetProperties();

        var property = properties.FirstOrDefault(p => p.Name == name);

        Assert.That(property, Is.Not.Null);
        return property!;
    }

    public static PropertyInfo PropertyHasAttribute<T>(this PropertyInfo property) where T : Attribute
    {
        var attributes = property.GetCustomAttributes(typeof(T), true);

        Assert.That(attributes.Length, Is.GreaterThan(0));
        return property;
    }
}