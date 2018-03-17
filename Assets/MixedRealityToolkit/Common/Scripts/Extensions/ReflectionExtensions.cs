// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace MixedRealityToolkit.Common.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class ReflectionExtensions
    {
        public static EventInfo GetEvent(this Type type, string eventName)
        {
            return type.GetRuntimeEvent(eventName);
        }

        public static MethodInfo GetMethod(this Type type, string methodName, BindingFlags flags = 0x0)
        {
            while (true)
            {
                var result = type.GetTypeInfo().GetDeclaredMethod(methodName);
                if ((flags & BindingFlags.FlattenHierarchy) != 0 && result == null)
                {
                    var baseType = type.GetBaseType();
                    if (baseType != null)
                    {
                        type = baseType;
                        continue;
                    }
                }

                return result;
            }
        }

        public static MethodInfo GetMethod(this Type type, string methodName, Type[] parameters)
        {
            return GetMethods(type)
                .Where(methodInfo => methodInfo.Name == methodName)
                .FirstOrDefault(methodInfo =>
                {
                    var types = methodInfo.GetParameters().Select(parameterInfo => parameterInfo.ParameterType).ToArray();
                    if (types.Length == parameters.Length)
                    {
                        for (int i = 0; i < types.Length; i++)
                        {
                            if (types[i] != parameters[i])
                            {
                                return false;
                            }
                        }

                        return true;
                    }

                    return false;
                }
            );
        }

        public static IEnumerable<MethodInfo> GetMethods(this Type type, BindingFlags flags = 0x0)
        {
            return type.GetTypeInfo().GetMethods(flags);
        }

        public static IEnumerable<MethodInfo> GetMethods(this TypeInfo type, BindingFlags flags = 0x0)
        {
            return type.DeclaredMethods;
        }

        public static IEnumerable<FieldInfo> GetFields(this Type type, BindingFlags flags = 0x0)
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

        public static PropertyInfo GetProperty(this Type type, string propertyName, BindingFlags flags = 0x0)
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
