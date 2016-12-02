//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HoloToolkit.Sharing.SyncModel
{
    /// <summary>
    /// Collection of sharing sync settings, used by the HoloToolkit Sharing sync system
    /// to figure out which data model classes need to be instantiated when receiving
    /// data that inherits from SyncObject.
    /// </summary>
    public class SyncSettings
    {
#if UNITY_METRO && !UNITY_EDITOR
        private readonly Dictionary<TypeInfo, string> dataModelTypeToName = new Dictionary<TypeInfo, string>();
        private readonly Dictionary<string, TypeInfo> dataModelNameToType = new Dictionary<string, TypeInfo>();
#else
        private readonly Dictionary<Type, string> dataModelTypeToName = new Dictionary<Type, string>();
        private readonly Dictionary<string, Type> dataModelNameToType = new Dictionary<string, Type>();
#endif

        private static SyncSettings instance;
        public static SyncSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SyncSettings();
                }
                return instance;
            }
        }

        public string GetDataModelName(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            string retVal;
            dataModelTypeToName.TryGetValue(typeInfo, out retVal);
            return retVal;
        }

#if UNITY_METRO && !UNITY_EDITOR
        public TypeInfo GetDataModelType(string name)
        {
            TypeInfo retVal;
#else
        public Type GetDataModelType(string name)
        {
            Type retVal;
#endif

            dataModelNameToType.TryGetValue(name, out retVal);
            return retVal;
        }

        public void Initialize()
        {
            dataModelNameToType.Clear();
            dataModelTypeToName.Clear();

            foreach (var assembly in GetAssemblies())
            {
                // We currently skip all assemblies except Unity-generated ones
                // This could be modified to be customizable by the user
                if (!assembly.FullName.StartsWith("Assembly-"))
                {
                    continue;
                }

#if UNITY_WSA && !UNITY_EDITOR
                foreach (TypeInfo type in assembly.GetTypes())
#else
                foreach (Type type in assembly.GetTypes())
#endif
                {
                    object customAttribute = type.GetCustomAttributes(typeof(SyncDataClassAttribute), false).FirstOrDefault();
                    SyncDataClassAttribute attribute = customAttribute as SyncDataClassAttribute;

                    if (attribute != null)
                    {
                        string dataModelName = type.Name;

                        // Override the class name if provided
                        if (!string.IsNullOrEmpty(attribute.CustomClassName))
                        {
                            dataModelName = attribute.CustomClassName;
                        }

                        dataModelNameToType.Add(dataModelName, type);
                        dataModelTypeToName.Add(type, dataModelName);
                    }
                }
            }
        }

        private static Assembly[] GetAssemblies()
        {
#if UNITY_WSA && !UNITY_EDITOR
            return new Assembly[]
            {
                typeof(SyncSettings).GetTypeInfo().Assembly
            };
#else
            return AppDomain.CurrentDomain.GetAssemblies();
#endif
        }
    }
}
