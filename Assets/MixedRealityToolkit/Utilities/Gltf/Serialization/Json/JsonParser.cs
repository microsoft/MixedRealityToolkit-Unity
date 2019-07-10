// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Json
{
    /// <summary>
    /// JSON utility class for structured deserialization.
    /// </summary>
    /// <remarks>
    /// This class takes optional [JSON attributes](xref:Microsoft.MixedReality.Toolkit.Utilities.Json.JsonAttribute)
    /// into account to produce valid JSON according to some schema.
    /// The [Unity JsonUtility class](https://docs.unity3d.com/Manual/JSONSerialization.html) is not flexible enough to
    /// follow all aspects of a schema, e.g. minimum values or how to serialize an enum.
    /// </remarks>
    public class JsonParser
    {
        /// <summary>
        /// Deserialize the given object from a JSON string.
        /// </summary>
        public bool Parse(string json, Type type, out object obj)
        {
            return TryParseObject(ref json, type, out obj);
        }

        const int maxLogLength = 120;

        private void LogWarning(string warning)
        {
            Debug.LogWarning(warning);
        }

        private void LogError(string error)
        {
            Debug.LogError("Invalid JSON string:" + error);
        }

        private void LogMissingObject(string json)
        {
            LogMissingObject(json, null);
        }

        private void LogMissingObject(string json, Type expected)
        {
            if (expected != null)
            {
                LogError($"Expected object of type {expected.Name} at {json.Substring(0, maxLogLength)}");
            }
            else
            {
                LogError($"Expected object at {json.Substring(0, maxLogLength)}");
            }
        }

        private void LogMissingLiteral(string json, string expected)
        {
            LogError($"Expected \"{expected}\" at {json.Substring(0, maxLogLength)}");
        }

        private bool TryParseObject(ref string json, Type type, out object obj)
        {
            obj = null;

            if (!TryParseLiteral(ref json, "{"))
            {
                return false;
            }

            if (TryParseLiteral(ref json, "}"))
            {
                // Empty list
                return true;
            }

            IDictionary dict = null;
            Type keyType = null;
            Type valueType = null;
            if (type != null)
            {
                obj = Activator.CreateInstance(type);

                if (IsDictionary(type))
                {
                    dict = obj as IDictionary;
                    Type[] argTypes = type.GetGenericArguments();
                    keyType = argTypes[0];
                    valueType = argTypes[1];
                }
            }

            while (true)
            {
                if (dict != null)
                {
                    if (!TryParseItem(ref json, keyType, null, out object key))
                    {
                        LogMissingObject(json, keyType);
                        return false;
                    }
                    if (!TryParseLiteral(ref json, ":"))
                    {
                        LogMissingLiteral(json, ":");
                        return false;
                    }
                    if (!TryParseItem(ref json, valueType, null, out object value))
                    {
                        LogMissingObject(json, valueType);
                        return false;
                    }

                    dict.Add(key, value);
                }
                else
                {
                    if (!TryParseString(ref json, out string fieldName))
                    {
                        LogMissingObject(json, typeof(string));
                        return false;
                    }

                    if (!TryParseLiteral(ref json, ":"))
                    {
                        LogMissingLiteral(json, ":");
                        return false;
                    }

                    FieldInfo fieldInfo = null;
                    if (type != null)
                    {
                        fieldInfo = type.GetField(fieldName);
                    }

                    if (!TryParseItem(ref json, fieldInfo?.FieldType, fieldInfo, out object value))
                    {
                        LogMissingObject(json, fieldInfo?.FieldType);
                        return false;
                    }

                    if (type != null)
                    {
                        if (fieldInfo != null)
                        {
                            fieldInfo.SetValue(obj, value);
                        }
                        else
                        {
                            LogWarning($"Could not find field \"{fieldName}\" in {type.Name}");
                        }
                    }
                }

                if (!TryParseLiteral(ref json, ","))
                {
                    break;
                }
            }

            if (!TryParseLiteral(ref json, "}"))
            {
                LogMissingLiteral(json, "}");
                return false;
            }

            return true;
        }

        /// Append the contents of an array to the JSON string.
        private bool TryParseArray(ref string json, Type type, MemberInfo member, out object obj)
        {
            obj = null;

            if (!TryParseLiteral(ref json, "["))
            {
                return false;
            }
            if (TryParseLiteral(ref json, "]"))
            {
                // Empty list
                return true;
            }

            Array array = null;
            Type elementType = null;
            if (type != null && type.IsArray)
            {
                elementType = type.GetElementType();
                if (elementType != null)
                {
                    array = Array.CreateInstance(elementType, 0);
                    obj = array;
                }
            }

            List<object> tmp = new List<object>();
            while (true)
            {
                if (!TryParseItem(ref json, elementType, null, out object element))
                {
                    LogMissingObject(json, elementType);
                    return false;
                }

                tmp.Add(element);

                if (!TryParseLiteral(ref json, ","))
                {
                    break;
                }
            }

            if (!TryParseLiteral(ref json, "]"))
            {
                LogMissingLiteral(json, "]");
                return false;
            }

            if (elementType != null)
            {
                array = Array.CreateInstance(elementType, tmp.Count);
                obj = array;

                for (int i = 0; i < tmp.Count; ++i)
                {
                    array.SetValue(tmp[i], i);
                }
            }

            return true;
        }

        /// Parse the value of a field or array item.
        private bool TryParseItem(ref string json, Type type, MemberInfo member, out object item)
        {
            string orig = json;

            if (TryParseObject(ref json, type, out item))
            {
                return true;
            }

            json = orig;
            if (TryParseArray(ref json, type, member, out item))
            {
                return true;
            }

            json = orig;
            if (TryParseEnumByValue(ref json, type, out item))
            {
                return true;
            }

            json = orig;
            if (TryParseEnumByName(ref json, type, out item))
            {
                return true;
            }

            json = orig;
            if (TryParseString(ref json, out item))
            {
                return true;
            }

            json = orig;
            if (TryParseInt(ref json, out item))
            {
                return true;
            }

            json = orig;
            if (TryParseFloat(ref json, out item))
            {
                return true;
            }

            json = orig;
            if (TryParseBool(ref json, out item))
            {
                return true;
            }

            return false;
        }

        private static readonly char[] SpecialChars = new char[] { '{', '}', '[', ']', ',', ':' };

        private static bool TryParseLiteral(ref string json, string literal)
        {
            string t = json.TrimStart();
            if (t.StartsWith(literal))
            {
                json = t.Substring(literal.Length);
                return true;
            }
            return false;
        }

        private static bool TryParseString(ref string json, out object result)
        {
            bool ok = TryParseString(ref json, out string s);
            result = s;
            return ok;
        }

        private static bool TryParseString(ref string json, out string result)
        {
            int end = json.IndexOfAny(SpecialChars);

            var match = Regex.Match(json.Substring(0, end), @"\s*""(([^""\\]|\\.)*)""\s*");
            if (match.Success)
            {
                result = match.Groups[1].Value;
                json = json.Substring(end);
                return true;
            }

            result = "";
            return false;
        }

        private static bool TryParseInt(ref string json, out object result)
        {
            bool ok = TryParseInt(ref json, out int i);
            result = i;
            return ok;
        }

        private static bool TryParseInt(ref string json, out int result)
        {
            int end = json.IndexOfAny(SpecialChars);
            if (int.TryParse(json.Substring(0, end), out result))
            {
                json = json.Substring(end);
                return true;
            }
            return false;
        }

        private static bool TryParseFloat(ref string json, out object result)
        {
            bool ok = TryParseFloat(ref json, out float f);
            result = f;
            return ok;
        }

        private static bool TryParseFloat(ref string json, out float result)
        {
            int end = json.IndexOfAny(SpecialChars);
            if (float.TryParse(json.Substring(0, end), out result))
            {
                json = json.Substring(end);
                return true;
            }
            return false;
        }

        private static bool TryParseBool(ref string json, out object result)
        {
            bool ok = TryParseBool(ref json, out bool b);
            result = b;
            return ok;
        }

        private static bool TryParseBool(ref string json, out bool result)
        {
            int end = json.IndexOfAny(SpecialChars);
            if (bool.TryParse(json.Substring(0, end), out result))
            {
                json = json.Substring(end);
                return true;
            }
            return false;
        }

        private static bool TryParseEnumByValue(ref string json, Type type, out object result)
        {
            if (type != null && type.IsEnum)
            {
                if (TryParseInt(ref json, out int value))
                {
                    result = Enum.ToObject(type, value);
                    return true;
                }
            }
            result = null;
            return false;
        }

        private static bool TryParseEnumByName(ref string json, Type type, out object result)
        {
            if (type != null && type.IsEnum)
            {
                if (TryParseString(ref json, out string name))
                {
                    result = Enum.Parse(type, name);
                    if (result != null)
                    {
                        return true;
                    }
                }
            }
            result = null;
            return false;
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