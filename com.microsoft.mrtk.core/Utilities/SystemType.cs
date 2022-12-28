// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Reference to a class <see cref="System.Type"/> with support for Unity serialization.
    /// </summary>
    [Serializable]
    public class SystemType : ISerializationCallbackReceiver
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

            return GetReference(type.AssemblyQualifiedName);
        }

        public static string GetReference(string assemblyQualifiedName)
        {
            if (string.IsNullOrEmpty(assemblyQualifiedName))
            {
                return string.Empty;
            }

            string[] qualifiedNameComponents = assemblyQualifiedName.Split(',');
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

                if (Type != null && Type.IsAbstract)
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
            reference = GetReference(type);
        }

        // Override hash code and equality so we can
        // key dictionaries with the type reference.
        public override int GetHashCode()
        {
            return reference.GetHashCode();
        }

        public override bool Equals(object other)
        {
            var otherType = other as SystemType;

            if (otherType == null)
                return false;

            return reference.Equals(otherType.reference);
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
                    bool isValid = ValidConstraint(value);
                    if (!isValid)
                    {
                        Debug.LogError($"'{value.FullName}' is not a valid class or struct type.");
                    }
                }

                type = value;
                reference = GetReference(value);
            }
        }

        /// <summary>
        /// An overridable constraint to determine whether a type is valid to serialize.
        /// </summary>
        /// <param name="t">Type to validate.</param>
        /// <returns>True if the type is valid, or false.</returns>
        protected virtual bool ValidConstraint(Type t)
        {
            return t != null && t.IsValueType && !t.IsEnum && !t.IsAbstract || t.IsClass;
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
            // Empty for now.
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
