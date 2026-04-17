using NUnit.Framework;

namespace TestUtilities;

public static class ObjectTestingUtilities
{
    public static void HasNoNulls<T>(this T obj) where T : class
    {
        var properties = obj.GetType().GetProperties();

        foreach (var property in properties)
        {
            var value = property.GetValue(obj);

            Assert.That(value, Is.Not.Null, "Property {0} is null", property.Name);
        }
    }
}