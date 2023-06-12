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

        /// <summary>
        /// Create a reference string from the given type's assembly qualified class name. The reference 
        /// string will contain the full name and assembly name from the given type.
        /// </summary>
        /// <param name="type">The class type to track.</param>
        /// <returns>
        /// A new reference string. Will return an empty string if the type is null, 
        /// or if the assembly qualified name is null or empty.
        /// </returns>
        public static string GetReference(Type type)
        {
            if (type == null || string.IsNullOrEmpty(type.AssemblyQualifiedName))
            {
                return string.Empty;
            }

            return GetReference(type.AssemblyQualifiedName);
        }

        /// <summary>
        /// Create a reference string from the given assembly qualified class name. The reference 
        /// string will contain the full name and assembly name from the given assembly qualified name.
        /// </summary>
        /// <param name="assemblyQualifiedClassName">Assembly qualified class name.</param>
        /// <returns>
        /// A new reference string. Will return an empty string if the assembly qualified
        /// name is null or empty.
        /// </returns>
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
        /// <param name="type">The class type to track.</param>
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

        /// <summary>
        /// Gets the hash code for the <see cref="SystemType"/>.
        /// </summary>
        /// <remarks>
        /// Obtains the hash from the underlying reference string.
        /// </remarks>
        /// <returns>
        /// The hash value generated for this <see cref="SystemType"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return reference.GetHashCode();
        }

        /// <summary>
        /// Compares two <see cref="SystemType"/> instances for equality.
        /// </summary>
        /// <remarks>
        /// Obtains the equality from the underlying reference string.
        /// </remarks>
        /// <returns>
        /// `true` if the two instances represent the same <see cref="SystemType"/>; otherwise, `false`.
        /// </returns>
        public override bool Equals(object other)
        {
            var otherType = other as SystemType;

            if (otherType == null)
            {
                return false;
            }

            return reference.Equals(otherType.reference);
        }

        #region ISerializationCallbackReceiver Members

        /// <summary>
        /// Implemented so to receive a callback after Unity deserializes this object.
        /// </summary>
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            // Class references may move between asmdef or be renamed throughout MRTK development
            // Check to see if we need to update our reference value
            reference = TryMigrateReference(reference);

            type = !string.IsNullOrEmpty(reference) ? Type.GetType(reference) : null;
        }

        /// <summary>
        /// Implemented so to receive a callback before Unity deserializes this object.
        /// </summary>
        /// <remarks>
        /// This is currently not utilized, and no operation will be preformed when called.
        /// </remarks>
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
        /// <param name="type">Type to validate.</param>
        /// <returns>True if the type is valid, or false.</returns>
        protected virtual bool ValidConstraint(Type type)
        {
            return type != null && type.IsValueType && !type.IsEnum && !type.IsAbstract || type.IsClass;
        }

        /// <summary>
        /// Returns the reference string for the type represented by this <see cref="SystemType"/>.
        /// </summary>
        public static implicit operator string(SystemType type)
        {
            return type.reference;
        }

        /// <summary>
        /// Returns the type represented by this <see cref="SystemType"/>.
        /// </summary>
        public static implicit operator Type(SystemType type)
        {
            return type.Type;
        }

        /// <summary>
        /// Create an instance of <see cref="SystemType"/> for the given type.
        /// </summary>
        /// <param name="type">The class type to track.</param>
        /// <exception cref="System.ArgumentException">
        /// If <paramref name="type"/> is not a class type.
        /// </exception>
        public static implicit operator SystemType(Type type)
        {
            return new SystemType(type);
        }

        /// <summary>
        /// Returns a string that represents this <see cref="SystemType"/>.
        /// </summary>
        /// <Returns>
        /// The full name for the type represented by this <see cref="SystemType"/>, or 
        /// "(None)" if the type is null.
        /// </Returns>
        public override string ToString()
        {
            return type?.FullName ?? "(None)";
        }

        /// <summary>
        /// A dictionary containing mappings from the original reference string entry to
        /// the new migrated placement value.
        /// </summary>
        /// <remarks>
        /// String values are broken into {Full Name, Assembly Definition}
        /// </remarks>
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
