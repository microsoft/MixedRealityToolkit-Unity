// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MixedRealityToolkit.Common.Extensions;
using MixedRealityToolkit.Sharing.SyncModel;

namespace MixedRealityToolkit.Sharing
{
    /// <summary>
    /// Collection of sharing sync settings, used by the MixedRealityToolkit Sharing sync system
    /// to figure out which data model classes need to be instantiated when receiving
    /// data that inherits from SyncObject.
    /// </summary>
    public class SyncSettings
    {
#if UNITY_WSA && !UNITY_EDITOR
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
            string retVal;
            dataModelTypeToName.TryGetValue(type, out retVal);
            return retVal;
        }

        public Type GetDataModelType(string name)
        {
            Type retVal;
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

                foreach (TypeInfo type in assembly.GetTypeInfos())
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
            return new[]
            {
                typeof(SyncSettings).GetTypeInfo().Assembly
            };
        }
    }
}
