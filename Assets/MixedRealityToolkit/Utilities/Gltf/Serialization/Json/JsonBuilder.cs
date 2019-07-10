// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Microsoft.MixedReality.Toolkit.Utilities.Json
{
    /// <summary>
    /// JSON utility class for structured serialization.
    /// </summary>
    /// <remarks>
    /// This class takes optional [JSON attributes](xref:Microsoft.MixedReality.Toolkit.Utilities.Json.JsonAttribute)
    /// into account to produce valid JSON according to some schema.
    /// The [Unity JsonUtility class](https://docs.unity3d.com/Manual/JSONSerialization.html) is not flexible enough to
    /// follow all aspects of a schema, e.g. minimum values or how to serialize an enum.
    /// </remarks>
    public class JsonBuilder
    {
        /// <summary>
        /// Serialize the given object to a JSON string.
        /// </summary>
        /// <returns>The serialized JSON string.</returns>
        public string Build(object obj)
        {
            return AppendObject(obj);
        }

        /// Append contents of the object to the JSON string.
        private string AppendObject(object obj)
        {
            Type type = obj.GetType();
            var fields = type.GetFields();
            var builder = new StringBuilder();

            int count = 0;
            builder.Append("{");
            if (IsDictionary(type))
            {
                IDictionary dict = obj as IDictionary;
                foreach (DictionaryEntry item in dict)
                {

                    var result = AppendItem(item.Value, null);
                    if (result.Length > 0)
                    {
                        if (count > 0)
                        {
                            builder.Append(",");
                        }

                        builder.Append("\"" + SanitizeString(item.Key.ToString()) + "\":" + result);

                        ++count;
                    }

                }
            }
            else
            {
                foreach (var field in fields)
                {
                    if (field.IsStatic || field.IsNotSerialized || !field.FieldType.IsSerializable)
                    {
                        continue;
                    }

                    var result = AppendItem(field.GetValue(obj), field);
                    if (result.Length > 0)
                    {
                        if (count > 0)
                        {
                            builder.Append(",");
                        }

                        builder.Append("\"" + SanitizeString(field.Name) + "\":" + result);

                        ++count;
                    }
                }
            }
            builder.Append("}");

            if (count == 0)
            {
                return "";
            }
            return builder.ToString();
        }

        /// Append the contents of an array to the JSON string.
        private string AppendArray(Array array, MemberInfo member)
        {
            Type type = array.GetType();
            var builder = new StringBuilder();

            int count = 0;
            builder.Append("[");
            foreach (var item in array)
            {
                string result = AppendItem(item, null);
                if (result.Length > 0)
                {
                    if (count > 0)
                    {
                        builder.Append(",");
                    }

                    builder.Append(result);

                    ++count;
                }
            }
            builder.Append("]");

            var attr = member?.GetCustomAttribute<JSONArrayAttribute>();
            if (attr != null)
            {
                if (count < attr.MinItems)
                {
                    return "";
                }
            }
            return builder.ToString();
        }

        /// Append the value of a field or array item.
        private string AppendItem(object obj, MemberInfo member)
        {
            if (obj == null)
            {
                return "";
            }

            Type type = obj.GetType();
            if (IsString(type))
            {
                return "\"" + obj.ToString() + "\"";
            }
            else if (type.IsEnum)
            {
                var attr = member?.GetCustomAttribute<JSONEnumAttribute>();
                if (attr != null)
                {
                    if (attr.IgnoreValues != null)
                    {
                        foreach (var ignoreValue in attr.IgnoreValues)
                        {
                            if ((int)obj == (int)ignoreValue)
                            {
                                return "";
                            }
                        }
                    }
                    if (attr.UseIntValue)
                    {
                        return ((int)obj).ToString();
                    }
                }

                return "\"" + obj.ToString() + "\"";
            }
            else if (IsInteger(type))
            {
                var attr = member?.GetCustomAttribute<JSONIntegerAttribute>();
                if (attr != null)
                {
                    if ((int)obj < attr.Minimum)
                    {
                        return "";
                    }
                }

                return obj.ToString();
            }
            else if (IsFloat(type))
            {
                return obj.ToString();
            }
            else if (IsBoolean(type))
            {
                return obj.ToString().ToLower();
            }
            else if (type.IsArray)
            {
                return AppendArray(obj as Array, member);
            }
            else
            {
                return AppendObject(obj);
            }
        }

        /// Escape characters in a string so it can be used as a JSON literal.
        private static string SanitizeString(string s)
        {
            return s.Replace("\"", "\\\"");
        }

        /// <summary>
        /// Returns true if a type is an integer type.
        /// </summary>
        public static bool IsInteger(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns true if a type is an floating point type.
        /// </summary>
        public static bool IsFloat(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns true if a type is an string type.
        /// </summary>
        public static bool IsString(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.String:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns true if a type is an boolean type.
        /// </summary>
        public static bool IsBoolean(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsDictionary(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        }
    }
}