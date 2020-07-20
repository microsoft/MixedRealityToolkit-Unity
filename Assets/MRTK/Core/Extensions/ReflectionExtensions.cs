// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if WINDOWS_UWP && !ENABLE_IL2CPP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Extension methods for .Net reflection functions
    /// </summary>
    public static class ReflectionExtensions
    {
        public static EventInfo GetEvent(this Type type, string eventName)
        {
            return type.GetRuntimeEvent(eventName);
        }

        public static MethodInfo GetMethod(this Type type, string methodName)
        {
            return GetMethod(type, methodName, (BindingFlags)0x0);
        }

        public static MethodInfo GetMethod(this Type type, string methodName, BindingFlags flags)
        {
            var result = type.GetTypeInfo().GetDeclaredMethod(methodName);
            if (((flags & BindingFlags.FlattenHierarchy) != 0) && result == null)
            {
                var baseType = type.GetTypeInfo().BaseType;
                if (baseType != null)
                {
                    return GetMethod(baseType, methodName, flags);
                }
            }

            return result;
        }

        public static MethodInfo GetMethod(this Type type, string methodName, BindingFlags bindingAttr, Object binder, Type[] parameters, Object[] modifiers)
        {
            var result = type.GetTypeInfo().GetDeclaredMethod(methodName);
            if (result == null)
            {
                var baseType = type.GetTypeInfo().BaseType;
                if (baseType != null)
                {
                    return GetMethod(baseType, methodName, bindingAttr, binder, parameters, modifiers);
                }
            }

            return result;
        }

        public static MethodInfo GetMethod(this Type type, string methodName, Type[] parameters)
        {
            return GetMethods(type).Where(m => m.Name == methodName).FirstOrDefault(
                m =>
                {
                    var types = m.GetParameters().Select(p => p.ParameterType).ToArray();
                    if (types.Length == parameters.Length)
                    {
                        for (int idx = 0; idx < types.Length; idx++)
                        {
                            if (types[idx] != parameters[idx])
                            {
                                return false;
                            }
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            );
        }

        public static IEnumerable<MethodInfo> GetMethods(this Type type)
        {
            return GetMethods(type, (BindingFlags)0x0);
        }

        public static IEnumerable<MethodInfo> GetMethods(this Type type, BindingFlags flags)
        {
            return type.GetTypeInfo().GetMethods(flags);
        }

        public static IEnumerable<MethodInfo> GetMethods(this TypeInfo type)
        {
            return GetMethods(type, (BindingFlags)0x0);
        }

        public static IEnumerable<MethodInfo> GetMethods(this TypeInfo type, BindingFlags flags)
        {
            return type.DeclaredMethods;
        }

        public static IEnumerable<FieldInfo> GetFields(this Type type)
        {
            return GetFields(type, (BindingFlags)0x0);
        }

        public static IEnumerable<FieldInfo> GetFields(this Type type, BindingFlags flags)
        {
            return type.GetTypeInfo().DeclaredFields;
        }

        public static FieldInfo GetField(this Type type, string fieldName)
        {
            return type.GetRuntimeField(fieldName);
        }

        public static IEnumerable<PropertyInfo> GetProperties(this Type type, BindingFlags flags)
        {
            return type.GetTypeInfo().DeclaredProperties;
        }

        public static PropertyInfo GetProperty(this Type type, string propertyName)
        {
            return GetProperty(type, propertyName, (BindingFlags)0x0);
        }

        public static PropertyInfo GetProperty(this Type type, string propertyName, BindingFlags flags)
        {
            return type.GetRuntimeProperty(propertyName);
        }

        public static PropertyInfo GetProperty(this Type type, string propertyName, Type returnType)
        {
            return type.GetRuntimeProperty(propertyName);
        }

        public static IEnumerable<TypeInfo> GetTypeInfos(this Assembly assembly)
        {
            return assembly.DefinedTypes;
        }

        public static Type[] GetInterfaces(this Type type)
        {
            return type.GetTypeInfo().ImplementedInterfaces.ToArray();
        }

        public static bool IsClass(this Type type)
        {
            return type.GetTypeInfo().IsClass;
        }

        public static bool IsInterface(this Type type)
        {
            return type.GetTypeInfo().IsInterface;
        }

        public static bool IsAbstract(this Type type)
        {
            return type.GetTypeInfo().IsAbstract;
        }

        public static bool IsSubclassOf(this Type type, Type c)
        {
            return type.GetTypeInfo().IsSubclassOf(c);
        }

        public static bool IsAssignableFrom(this Type type, Type c)
        {
            return type.IsAssignableFrom(c.GetTypeInfo());
        }

        public static bool IsEnum(this Type type)
        {
            return type.GetTypeInfo().IsEnum;
        }

        public static bool IsValueType(this Type type)
        {
            return type.GetTypeInfo().IsValueType;
        }

        public static bool IsAssignableFrom(this Type type, TypeInfo typeInfo)
        {
            return type.GetTypeInfo().IsAssignableFrom(typeInfo);
        }

        public static object[] GetCustomAttributes(this Type type, bool inherit)
        {
            return type.GetTypeInfo().GetCustomAttributes(inherit).ToArray();
        }

        public static object[] GetCustomAttributes(this Type type, Type attributeType, bool inherit)
        {
            return type.GetTypeInfo().GetCustomAttributes(attributeType, inherit).ToArray();
        }
    }
}
#endif // WINDOWS_UWP && !ENABLE_IL2CPP
