// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// A convenience class that holds arrays of class names, fully qualified assembly names
    /// and their corresponding actual types.
    /// </summary>
    /// <remarks>
    /// This abstraction exists primarily to reduce code duplication among the different
    /// inspectors which use these lists to populate their dropdowns.
    /// 
    /// Note that all of these arrays are the same size and come in the same order
    /// (so for example, ClassName[0] = "InteractableActivateTheme" means that
    /// Types[0] == typeof(InteractableActivateTheme) and AssemblyQualifiedNames
    /// is the assembly qualified name for InteractableActivateTheme.
    /// </remarks>
    public class InteractableTypesContainer
    {
        /// <summary>
        /// An array of class names (for example, "InteractableActivateTheme").
        /// </summary>
        public string[] ClassNames { get; private set; }

        /// <summary>
        /// A array of assembly qualified names (for example, 
        /// "Microsoft.MixedReality.Toolkit.UI.InteractableActivateTheme, 
        /// Microsoft.MixedReality.Toolkit.UI")
        /// </summary>
        public string[] AssemblyQualifiedNames { get; private set; }

        /// <summary>
        /// An array of types. See class remarks for more information on relation to
        /// other fields.
        /// </summary>
        public Type[] Types { get; private set; }

        /// <summary>
        /// A convenience helper that will unwrap a list of InteractableType objects into
        /// a form that is more easy consumed by inspector components.
        /// </summary>
        public InteractableTypesContainer(List<InteractableType> interactableTypes)
        {
            var classNames = new List<string>();
            var assemblyQualifiedNames = new List<string>();
            var types = new List<Type>();

            for (int i = 0; i < interactableTypes.Count; i++)
            {
                classNames.Add(interactableTypes[i].ClassName);
                assemblyQualifiedNames.Add(interactableTypes[i].AssemblyQualifiedName);
                types.Add(interactableTypes[i].Type);
            }

            ClassNames = classNames.ToArray();
            AssemblyQualifiedNames = assemblyQualifiedNames.ToArray();
            Types = types.ToArray();
        }
    }
}
