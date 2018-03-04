// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Utilities.Attributes;
using UnityEngine;

namespace MixedRealityToolkit.UX.Buttons
{
    /// <summary>
    /// Concrete version of Button class used with other CompoundButton scripts (e.g., CompoundButtonMesh)
    /// Also contains fields for commonly referenced components
    /// </summary>
    public class CompoundButton : Button
    {
        [Tooltip("The button's 'main' collider - not required, but useful for judging scale and enabling / disabling")]
        [DropDownComponent]
        public Collider MainCollider = null;

        [Tooltip("The button's 'main' renderer - not required, but useful for judging material properties")]
        [DropDownComponent]
        public MeshRenderer MainRenderer = null;
    }
}