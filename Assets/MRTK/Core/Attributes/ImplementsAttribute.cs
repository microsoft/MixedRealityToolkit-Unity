// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
#if WINDOWS_UWP && !ENABLE_IL2CPP
using Microsoft.MixedReality.Toolkit;
#endif // WINDOWS_UWP && !ENABLE_IL2CPP
using System;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Constraint that allows selection of classes that implement a specific interface
    /// when selecting a <see cref="Utilities.SystemType"/> with the Unity inspector.
    /// </summary>
    public sealed class ImplementsAttribute : SystemTypeAttribute
    {
        /// <summary>
        /// Gets the type of interface that selectable classes must implement.
        /// </summary>
        public Type InterfaceType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImplementsAttribute"/> class.
        /// </summary>
        /// <param name="interfaceType">Type of interface that selectable classes must implement.</param>
        /// <param name="grouping">Gets or sets grouping of selectable classes. Defaults to <see cref="Utilities.TypeGrouping.ByNamespaceFlat"/> unless explicitly specified.</param>
        public ImplementsAttribute(Type interfaceType, TypeGrouping grouping) : base(interfaceType, grouping)
        {
            InterfaceType = interfaceType;
        }

        /// <inheritdoc />
        public override bool IsConstraintSatisfied(Type type)
        {
            if (base.IsConstraintSatisfied(type))
            {
                var interfaces = type.GetInterfaces();
                for (var i = 0; i < interfaces.Length; i++)
                {
                    if (interfaces[i] == InterfaceType)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}