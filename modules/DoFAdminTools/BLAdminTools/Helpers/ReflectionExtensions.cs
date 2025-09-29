using System;
using System.Reflection;

namespace DoFAdminTools.Helpers;

public static class ReflectionExtensions
{
    public static T GetFieldValue<T>(this object obj, string name) {
        var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        var field = obj.GetType().GetField(name, bindingFlags);
        return (T)field?.GetValue(obj);
    }
    
    public static void SetFieldValue<T>(this object obj, string name, T value) {
        var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        var field = obj.GetType().GetField(name, bindingFlags);
        field?.SetValue(obj, value);
    }
    
    public static T GetPropertyValue<T>(this object obj, string name)
    {
        return (T)(GetPropertyInfo(obj, name)?.GetValue(obj));
    }

    public static PropertyInfo GetPropertyInfo(this object obj, string name)
    {
        return obj.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    }

    public static MethodInfo GetMethodInfo(this object obj, string methodName)
    {
        return obj.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
    }

    public static MethodInfo GetStaticMethodInfo(this Type type, string methodName)
    {
        return type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
    }
}