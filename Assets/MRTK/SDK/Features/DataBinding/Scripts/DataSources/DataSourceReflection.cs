﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;


using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Data
{

    /// <summary>
    /// A data source that uses reflection to determine the structure of an object, 
    /// either a struct, a class or a primitive, and makes objects in that structure
    /// available using a keypath structure similar to what is used for the DataSourceObjects
    /// and DataSourceJson classes.
    /// 
    /// </summary>
    /// 
    public class DataSourceReflection : DataSourceBase
    {
        protected static readonly string CollectionElementkeyPathPrefixFormat = "{0}[{1:d}]";
        protected static readonly string DictionaryElementkeyPathPrefixFormat = "{0}[{1}]";

#if UNITY_EDITOR || ENABLE_IL2CPP
        protected static readonly MemberFilter FieldOrPropertyByNameMemberFilter = new MemberFilter(FieldOrPropertyNameCompare);
#endif


        protected static readonly string ArrayTokenPattern = @"^\s*\[\s*([a-zA-Z0-9\-_]*?)\s*\]";
        protected static readonly string KeyTokenPattern = @"^\s*([a-zA-Z0-9\-_]+?)(?:[.\[]|$)";

        protected static readonly Regex ArrayTokenRegex = new Regex(ArrayTokenPattern);
        protected static readonly Regex KeyTokenRegex = new Regex(KeyTokenPattern);

        protected object _dataSourceObject = null;

        public DataSourceReflection() { }

        public DataSourceReflection( object dataSourceObject )
        {
            _dataSourceObject = dataSourceObject;
        }

        public void SetDataSourceObject( object dataSourceObject )
        {
            _dataSourceObject = dataSourceObject;
        }


        public override bool IsCollectionAtKeyPath(string resolvedKeyPath)
        {
            return IsArrayOrList(KeyPathToObject(resolvedKeyPath));
        }

        public override int GetCollectionCount(string resolvedKeyPath)
        {
            object value = KeyPathToObject(resolvedKeyPath);
            if (IsArray(value)) {
                return (value as Array).Length;
            } else if (IsList(value)) {
                return (value as IList).Count;
            } else
            {
                return 0;
            }
        }


        public override void SetValueInternal(string resolvedKeyPath, object value)
        {
            MemberInfo memberInfo;
            object containingObject;

            object currentValue = KeyPathToValueWithMemberInfo(resolvedKeyPath, out memberInfo, out containingObject);
            if (memberInfo != null && containingObject != null)
            {
                SetValueFromFieldOrProperty(containingObject, memberInfo, value);
            }
        }

        public override IEnumerable<string> GetCollectionKeyPathRange(string resolvedKeyPath, int rangeStart, int rangeCount)
        {
            if (IsDataSourceAvailable())
            {
                object objectAtKeyPath = KeyPathToObject(resolvedKeyPath);
                if (objectAtKeyPath != null)
                {
                    if (IsArrayOrList(objectAtKeyPath))
                    {
                        return GetValueAsArrayKeyPathsOptimized(objectAtKeyPath, resolvedKeyPath, rangeStart, rangeCount);
                    }
                }
            }
            return null;
        }

        public override object GetValueInternal(string resolvedKeyPath)
        {
            if (IsDataSourceAvailable())
            {
                object objectAtKeyPath = KeyPathToObject(resolvedKeyPath);
                if (objectAtKeyPath != null)
                {
                    if (objectAtKeyPath is IList)
                    {
                        IList collection = objectAtKeyPath as IList;
                        return GetValueAsArrayKeyPaths(collection, resolvedKeyPath, 0, collection.Count);
                    }
                    else if (objectAtKeyPath is IDictionary)
                    {
                        return GetValueAsDictionaryKeyPaths(objectAtKeyPath as IDictionary, resolvedKeyPath);
                    }
                    else
                    {
                        return objectAtKeyPath;
                    }
                }
            }

            return null;
        }


        protected IEnumerable<string> GetValueAsArrayKeyPathsOptimized(object arrayOrList, string resolvedKeyPath, int rangeStart, int rangeCount)
        {
            return new KeyPathEnumerable(resolvedKeyPath, rangeStart, rangeCount) as IEnumerable<string>;
        }


        protected IEnumerable<string> GetValueAsArrayKeyPaths(IList list, string resolvedKeyPath, int rangeStart, int rangeCount)
        {
            int rangeMax = rangeStart + rangeCount;

            if (rangeMax > list.Count)
            {
                rangeMax = list.Count;
            }


            List<string> keyPaths = new List<string>();
            for (int idx = rangeStart; idx < rangeMax; idx++)
            {
                keyPaths.Add(string.Format(CollectionElementkeyPathPrefixFormat, resolvedKeyPath, idx));
            }
            return keyPaths as IEnumerable<string>;

        }

        protected IEnumerable<string> GetValueAsDictionaryKeyPaths(IDictionary dict, string resolvedKeyPath)
        {
            List<string> keyPaths = new List<string>();
            foreach (string key in dict.Keys )
            {
                keyPaths.Add(string.Format(DictionaryElementkeyPathPrefixFormat, resolvedKeyPath, key));
            }
            return keyPaths as IEnumerable<string>;
        }


        protected object KeyPathToObject(string resolvedKeyPath)
        {
            return KeyPathToValueWithMemberInfo(resolvedKeyPath, out _, out _ );
        }


        protected object KeyPathToValueWithMemberInfo(string resolvedKeyPath, out MemberInfo memberInfoOut, out object containingObjectOut )
        {
            object currentObject = _dataSourceObject;
            string keyPath = resolvedKeyPath;

            containingObjectOut = null;
            memberInfoOut = null;

            while (currentObject != null && keyPath != null && keyPath != "")
            {
                int amountToSkip = 0;
                MatchCollection arrayMatches = ArrayTokenRegex.Matches(keyPath);
                if (arrayMatches.Count > 0)
                {
                    string arrayIndexText = arrayMatches[0].Groups[1].Value;
                    int arrayIndex = int.Parse(arrayIndexText);

                    memberInfoOut = null;

                    if (IsList(currentObject))
                    {
                        IList list = currentObject as IList;
                        currentObject = list[arrayIndex];
                    }
                    else if (IsArray(currentObject))
                    {
                        Array array = currentObject as Array;
                        currentObject = array.GetValue(arrayIndex);
                    }
                    else
                    {
                        currentObject = null;
                    }
                    amountToSkip = arrayMatches[0].Value.Length;

                }
                else
                {
                    MatchCollection keyMatches = KeyTokenRegex.Matches(keyPath);
                    if (keyMatches.Count > 0)
                    {
                        string key = keyMatches[0].Groups[1].Value;

                        if (IsStructOrClass(currentObject))
                        {
                            containingObjectOut = currentObject;
                            currentObject = GetNamedFieldOrPropertyValue(containingObjectOut, key, out memberInfoOut);
                        }

                        amountToSkip = key.Length;
                    }
                }

                if (keyPath.Length > amountToSkip && keyPath[amountToSkip] == '.')
                {
                    amountToSkip++;
                }
                if (amountToSkip > 0)
                {
                    keyPath = keyPath.Substring(amountToSkip);
                }
                else
                {
                    memberInfoOut = null;
                    currentObject = null;
                    containingObjectOut = null;
                    break;      // was not a valid path piece
                }

            }
           
            return currentObject;
        }


        protected object GetNamedFieldOrPropertyValue( object containingObject, string key, out MemberInfo foundMemberInfoOut ) {

            foundMemberInfoOut = null;

            try
            {
                MemberInfo foundMember = null;

#if UNITY_EDITOR || ENABLE_IL2CPP
                Type objType = containingObject.GetType();

               MemberInfo[] members;
               members = objType.FindMembers(
                    MemberTypes.Field | MemberTypes.Property,
                    BindingFlags.Public | BindingFlags.Instance,
                    FieldOrPropertyByNameMemberFilter,
                    key);

                if (members.Length > 0)
                {
                    foundMember = members[0];
                }
#else
                foundMember = TryFindMemberInfo(containingObject, key);
#endif
                if (foundMember != null)
                {
                    // We can assume there is only one item with the exact name we searched for.
                    foundMemberInfoOut = foundMember;
                    return GetValueFromFieldOrProperty(containingObject, foundMemberInfoOut);
                } 
            }
            catch (Exception)
            {
            }

            return null;
        }

        private MemberInfo TryFindMemberInfo(object obj, string keyToFind)
        {
            var objType = obj.GetType();
            var objTypeInfo = objType.GetTypeInfo();
            try
            {
                var propertyInfos = objTypeInfo.DeclaredProperties;
                foreach (var propertyInfo in propertyInfos)
                {
                    if (propertyInfo.Name == keyToFind && !propertyInfo.GetMethod.IsStatic && propertyInfo.GetMethod.IsPublic)
                    {
                        return propertyInfo;
                    }
                }

                var fieldInfos = objTypeInfo.DeclaredFields;
                foreach (var fieldInfo in fieldInfos)
                {
                    if (fieldInfo.Name == keyToFind && !fieldInfo.IsStatic && fieldInfo.IsPublic)
                    {
                        return fieldInfo;
                    }
                }
            }
            catch (Exception)
            {
            }

            return null;
        }

        protected object GetValueFromFieldOrProperty( object containingObject, MemberInfo memberInfo )
        {
            if (memberInfo is PropertyInfo)
            {
                PropertyInfo propertyInfo = memberInfo as PropertyInfo;
                return propertyInfo.GetValue(containingObject);
            }
            else if (memberInfo is FieldInfo)
            {
                FieldInfo fieldInfo = memberInfo as FieldInfo;
                return fieldInfo.GetValue(containingObject);
            } else
            {
                return null;
            }

        }


        protected void SetValueFromFieldOrProperty(object containingObject, MemberInfo memberInfo, object value )
        {
            if (memberInfo is PropertyInfo)
            {
                PropertyInfo propertyInfo = memberInfo as PropertyInfo;
                propertyInfo.SetValue(containingObject, value );
            }
            else if (memberInfo is FieldInfo)
            {
                FieldInfo fieldInfo = memberInfo as FieldInfo;
                fieldInfo.SetValue(containingObject, value );
            }
        }


        protected static bool FieldOrPropertyNameCompare(MemberInfo objMemberInfo, Object key)
        {
            return objMemberInfo.Name.ToString() == key.ToString();
        }
    

        public bool IsList(object source)
        {
            return source != null && source is IList;
        }

        public bool IsArray(object source)
        {
            if (source != null )
            {
                return source.GetType().IsArray;
            }
            else
            {
                return false;
            }
        }

        public bool IsArrayOrList(object source)
        {
            return IsArray(source) || IsList(source);
        }

        public bool IsStructOrClass(object source)
        {
            if (source != null)
            {

                Type type = source.GetType();

                // NOTE: using TypeInfo for backwards compatibility with .NET (pre IL2CPP)
                TypeInfo typeInfo = type.GetTypeInfo();

                return typeInfo.IsClass || (typeInfo.IsValueType && !typeInfo.IsPrimitive && !typeInfo.IsEnum);
            }
            else
            {
                return false;
            }
        }


        protected override bool IsDataSourceAvailable()
        {
            return _dataSourceObject != null;
        }
    } // End of class DataSourceObjects

} // End of namespace Microsoft.MixedReality.Toolkit.Data

