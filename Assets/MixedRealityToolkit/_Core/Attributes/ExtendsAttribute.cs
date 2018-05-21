﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;

namespace Microsoft.MixedReality.Toolkit.Internal.Attributes
{
    /// <summary>
    /// Constraint that allows selection of classes that extend a specific class when
    /// selecting a <see cref="SystemType"/> with the Unity inspector.
    /// </summary>
    public sealed class ExtendsAttribute : SystemTypeAttribute
    {
        /// <summary>
        /// Gets the type of class that selectable classes must derive from.
        /// </summary>
        public Type BaseType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendsAttribute"/> class.
        /// </summary>
        /// <param name="baseType">Type of class that selectable classes must derive from.</param>
        /// <param name="grouping">Gets or sets grouping of selectable classes. Defaults to <see cref="TypeGrouping.ByNamespaceFlat"/> unless explicitly specified.</param>
        public ExtendsAttribute(Type baseType, TypeGrouping grouping) : base(baseType, grouping)
        {
            BaseType = baseType;
        }

        /// <inheritdoc/>
        public override bool IsConstraintSatisfied(Type type)
        {
            return base.IsConstraintSatisfied(type) &&
                   BaseType.IsAssignableFrom(type) &&
                   type != BaseType;
        }
    }
}