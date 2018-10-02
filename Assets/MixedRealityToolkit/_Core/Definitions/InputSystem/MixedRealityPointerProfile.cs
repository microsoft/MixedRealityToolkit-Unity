// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem
{
    /// <summary>
    /// Configuration profile settings for setting up controller pointers.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Pointer Profile", fileName = "MixedRealityInputPointerProfile", order = (int)CreateProfileMenuItemIndices.Pointer)]
    public class MixedRealityPointerProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        private PointerOption[] pointerOptions = new PointerOption[0];

        /// <summary>
        /// The Pointer options for this profile.
        /// </summary>
        public PointerOption[] PointerOptions => pointerOptions;
    }
}
