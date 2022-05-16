// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
#if WINDOWS_UWP && !ENABLE_IL2CPP
using Microsoft.MixedReality.Toolkit;
#endif // WINDOWS_UWP && !ENABLE_IL2CPP
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Base class for class selection constraints that can be applied when selecting
    /// a <see cref="Utilities.SystemType"/> with the Unity inspector.
    /// </summary>
    public abstract class SystemTypeAttribute : PropertyAttribute
    {
        /// <summary>
        /// Gets or sets grouping of selectable classes. Defaults to <see cref="Utilities.TypeGrouping.ByNamespaceFlat"/> unless explicitly specified.
        /// </summary>
        public TypeGrouping Grouping { get; protected set; }

        /// <summary>
        /// Gets or sets whether abstract classes can be selected from drop-down.
        /// Defaults to a value of <c>false</c> unless explicitly specified.
        /// </summary>
        public bool AllowAbstract { get; protected set; } = false;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">Initializes a new instance of the <see cref="SystemTypeAttribute"/> class.</param>
        /// <param name="grouping">Gets or sets grouping of selectable classes. Defaults to <see cref="Utilities.TypeGrouping.ByNamespaceFlat"/> unless explicitly specified.</param>
        protected SystemTypeAttribute(Type type, TypeGrouping grouping = TypeGrouping.ByNamespaceFlat)
        {
#if WINDOWS_UWP && !ENABLE_IL2CPP
            bool isValid = type.IsClass() || type.IsInterface() || type.IsValueType() && !type.IsEnum();
#else
            bool isValid = type.IsClass || type.IsInterface || type.IsValueType && !type.IsEnum;
#endif // WINDOWS_UWP && !ENABLE_IL2CPP
            if (!isValid)
            {
                Debug.Assert(isValid, $"Invalid Type {type} in attribute.");
            }
            Grouping = grouping;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Type"/> satisfies filter constraint.
        /// </summary>
        /// <param name="type">Type to test.</param>
        /// <returns>
        /// A <see cref="bool"/> value indicating if the type specified by <paramref name="type"/>
        /// satisfies this constraint and should thus be selectable.
        /// </returns>
        public virtual bool IsConstraintSatisfied(Type type)
        {
#if WINDOWS_UWP && !ENABLE_IL2CPP
            return AllowAbstract || !type.IsAbstract();
#else
            return AllowAbstract || !type.IsAbstract;
#endif // WINDOWS_UWP && !ENABLE_IL2CPP
        }
    }
}
