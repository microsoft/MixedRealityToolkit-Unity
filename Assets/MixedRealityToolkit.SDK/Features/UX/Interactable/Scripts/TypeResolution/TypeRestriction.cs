// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Controls the behavior of the InteractableTypeFinder.FindTypes function. See individual
    /// enum values for more details.
    /// </summary>
    public enum TypeRestriction
    {
        /// <summary>
        /// When this is specified, only classes derived from the specified type will be
        /// returned by the lookup. This means that if you pass InteractableStates, the
        /// lookup will only return classes whose base class is InteractableStates but
        /// will not return InteractableStates itself.
        /// </summary>
        DerivedOnly,

        /// <summary>
        /// When this is specified, classes derived from the specified type AND the class
        /// itself will be returned by the lookup. This means that if you pass 
        /// InteractableStates, the lookup will both classes whose base class is 
        /// InteractableStates and InteractableStates itself.
        /// </summary>
        AllowBase,
    };
}
