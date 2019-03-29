// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// A wrapper for a Type which gives a "friendly name" for the type (i.e.
    /// the class name) along with the assembly qualified name (which can be used
    /// to new instances of this type). 
    /// </summary>
    /// <remarks>
    /// The intent of this wrapper is for use with the various Interactable state, event
    /// and theme classes, which are enumerated using reflection in the editor but must
    /// then be instantiated at runtime (without the usage of reflection due to .NET
    /// backend constraints).
    /// </remarks>
    public class InteractableType
    {
        /// <summary>
        /// The class name of this interactable type (for example, "InteractableActivateTheme").
        /// </summary>
        public string ClassName { get; private set; }

        /// <summary>
        /// The assembly qualified name of the class (for example, 
        /// "Microsoft.MixedReality.Toolkit.UI.InteractableActivateTheme, 
        /// Microsoft.MixedReality.Toolkit.UI")
        /// </summary>
        public string AssemblyQualifiedName { get; private set; }

        /// <summary>
        /// The type of the class (for example, typeof(InteractableActivateTheme)).
        /// </summary>
        public Type Type { get; private set; }

        public InteractableType(Type type)
        {
            ClassName = type.Name;
            AssemblyQualifiedName = SystemType.GetReference(type);
            Type = type;
        }
    }
}
