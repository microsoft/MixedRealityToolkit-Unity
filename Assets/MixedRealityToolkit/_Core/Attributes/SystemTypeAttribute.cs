// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Attributes
{
    /// <summary>
    /// Base class for class selection constraints that can be applied when selecting
    /// a <see cref="SystemType"/> with the Unity inspector.
    /// </summary>
    public abstract class SystemTypeAttribute : PropertyAttribute
    {
        /// <summary>
        /// Gets or sets grouping of selectable classes. Defaults to <see cref="ClassGrouping.ByNamespaceFlat"/> unless explicitly specified.
        /// </summary>
        public ClassGrouping Grouping { get; protected set; }

        /// <summary>
        /// Gets or sets whether abstract classes can be selected from drop-down.
        /// Defaults to a value of <c>false</c> unless explicitly specified.
        /// </summary>
        public bool AllowAbstract { get; protected set; } = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">Initializes a new instance of the <see cref="SystemTypeAttribute"/> class.</param>
        /// <param name="grouping">Gets or sets grouping of selectable classes. Defaults to <see cref="ClassGrouping.ByNamespaceFlat"/> unless explicitly specified.</param>
        public SystemTypeAttribute(Type type, ClassGrouping grouping = ClassGrouping.ByNamespaceFlat)
        {
            Debug.Assert(type.IsClass || type.IsInterface, $"Invalid Type {type} in attribute.");
            Grouping = grouping;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Type"/> satisfies filter constraint.
        /// </summary>
        /// <param name="type">Type to test.</param>
        /// <returns>
        /// A <see cref="bool"/> value indicating if the type specified by <paramref name="type"/>
        /// satisfies this constraint and should thus be selectable.
        /// </returns>
        public virtual bool IsConstraintSatisfied(Type type)
        {
            return AllowAbstract || !type.IsAbstract;
        }
    }
}