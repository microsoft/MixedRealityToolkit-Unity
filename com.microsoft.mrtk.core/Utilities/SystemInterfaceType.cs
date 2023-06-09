// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Reference to a <see cref="System.Type"/>, that may or may not be instantiable.
    /// Similar to <see cref="SystemType"/>, but without the concrete/non-interface-type
    /// constraints.
    /// </summary>
    [Serializable]
    public sealed class SystemInterfaceType : SystemType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemInterfaceType"/> class.
        /// </summary>
        /// <param name="assemblyQualifiedClassName">Assembly qualified class name.</param>
        public SystemInterfaceType(string assemblyQualifiedClassName) : base(assemblyQualifiedClassName) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemInterfaceType"/> class.
        /// </summary>
        /// <param name="type">Class type.</param>
        /// <exception cref="System.ArgumentException">
        /// If <paramref name="type"/> is not a class type.
        /// </exception>
        public SystemInterfaceType(Type type) : base(type) { }

        /// <inheritdoc/>
        protected override bool ValidConstraint(Type t)
        {
            return t != null;
        }

        public static implicit operator Type(SystemInterfaceType type)
        {
            return type.Type;
        }

        public static implicit operator SystemInterfaceType(Type type)
        {
            return new SystemInterfaceType(type);
        }
    }
}
