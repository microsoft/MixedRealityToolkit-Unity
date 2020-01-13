// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if WINDOWS_UWP && !ENABLE_IL2CPP
using Microsoft.MixedReality.Toolkit;
#endif // WINDOWS_UWP && !ENABLE_IL2CPP
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Reference to a class <see cref="System.Type"/> with support for Unity serialization.
    /// </summary>
    [Serializable]
    public sealed class SystemType : ISerializationCallbackReceiver
    {
        [SerializeField]
        private string reference = string.Empty;

        private Type type;

        public static string GetReference(Type type)
        {
            if (type == null || string.IsNullOrEmpty(type.AssemblyQualifiedName))
            {
                return string.Empty;
            }

            string[] qualifiedNameComponents = type.AssemblyQualifiedName.Split(',');
            Debug.Assert(qualifiedNameComponents.Length >= 2);
            return $"{qualifiedNameComponents[0]}, {qualifiedNameComponents[1].Trim()}";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemType"/> class.
        /// </summary>
        /// <param name="assemblyQualifiedClassName">Assembly qualified class name.</param>
        public SystemType(string assemblyQualifiedClassName)
        {
            if (!string.IsNullOrEmpty(assemblyQualifiedClassName))
            {
                Type = Type.GetType(assemblyQualifiedClassName);

#if WINDOWS_UWP && !ENABLE_IL2CPP
                if (Type != null && Type.IsAbstract())
#else
                if (Type != null && Type.IsAbstract)
#endif // WINDOWS_UWP && !ENABLE_IL2CPP
                {
                    Type = null;
                }
            }
            else
            {
                Type = null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemType"/> class.
        /// </summary>
        /// <param name="type">Class type.</param>
        /// <exception cref="System.ArgumentException">
        /// If <paramref name="type"/> is not a class type.
        /// </exception>
        public SystemType(Type type)
        {
            Type = type;
        }

        #region ISerializationCallbackReceiver Members

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            // Class references may move between asmdef or be renamed throughout MRTK development
            // Check to see if we need to update our reference value
            reference = TryMigrateReference(reference);

            type = !string.IsNullOrEmpty(reference) ? Type.GetType(reference) : null;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        #endregion ISerializationCallbackReceiver Members

        /// <summary>
        /// Gets or sets type of class reference.
        /// </summary>
        /// <exception cref="System.ArgumentException">
        /// If <paramref name="value"/> is not a class type.
        /// </exception>
        public Type Type
        {
            get => type;
            set
            {
                if (value != null)
                {
#if WINDOWS_UWP && !ENABLE_IL2CPP
                    bool isValid = value.IsValueType() && !value.IsEnum() && !value.IsAbstract() || value.IsClass();
#else
                    bool isValid = value.IsValueType && !value.IsEnum && !value.IsAbstract || value.IsClass;
#endif // WINDOWS_UWP && !ENABLE_IL2CPP
                    if (!isValid)
                    {
                        Debug.LogError($"'{value.FullName}' is not a valid class or struct type.");
                    }
                }

                type = value;
                reference = GetReference(value);
            }
        }

        public static implicit operator string(SystemType type)
        {
            return type.reference;
        }

        public static implicit operator Type(SystemType type)
        {
            return type.Type;
        }

        public static implicit operator SystemType(Type type)
        {
            return new SystemType(type);
        }

        public override string ToString()
        {
            return Type?.FullName ?? "(None)";
        }

        // Key == original reference string entry, value == new migrated placement
        // String values are broken into {namespace.classname, asmdef}
        private static Dictionary<string, string> ReferenceMappings = new Dictionary<string, string>()
        { 
            { "Microsoft.MixedReality.Toolkit.Input.InputSimulationService, Microsoft.MixedReality.Toolkit.Services.InputSimulation.Editor",
            "Microsoft.MixedReality.Toolkit.Input.InputSimulationService, Microsoft.MixedReality.Toolkit.Services.InputSimulation" },
            
            { "Microsoft.MixedReality.Toolkit.Input.InputPlaybackService, Microsoft.MixedReality.Toolkit.Services.InputSimulation.Editor",
            "Microsoft.MixedReality.Toolkit.Input.InputPlaybackService, Microsoft.MixedReality.Toolkit.Services.InputSimulation" },
        };

        /// <summary>
        /// This function checks if there are any known migrations for old class names, namespaces, and/or asmdef files
        /// If so, the new reference string is returned and utilized for editor runtime and will be serialized to disk
        /// </summary>
        private static string TryMigrateReference(string reference)
        {
            if (ReferenceMappings.ContainsKey(reference))
            {
                return ReferenceMappings[reference];
            }

            return reference;
        }
    }
}
