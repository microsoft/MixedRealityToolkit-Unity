//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

namespace HoloToolkit.Unity.Buttons
{
    /// <summary>
    /// Concrete version of Button class used with other CompoundButton scripts (eg, CompoundButtonMesh)
    /// Also contains fields for commonly referenced components
    /// </summary>
    public class CompoundButton : Button
    {
        /// <summary>
        /// The button's 'main' collider
        /// </summary>
        public Collider MainCollider;

        /// <summary>
        /// The button's 'main' renderer
        /// </summary>
        public MeshRenderer MainRenderer;
    }
}