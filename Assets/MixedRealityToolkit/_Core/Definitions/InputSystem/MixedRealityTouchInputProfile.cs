// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Touch Input Profile", fileName = "MixedRealityTouchInputProfile", order = (int)CreateProfileMenuItemIndices.TouchInput)]
    public class MixedRealityTouchInputProfile : ScriptableObject
    {
        [SerializeField]
        [Tooltip("Action to use for pointer events.")]
        private MixedRealityInputAction pointerAction = MixedRealityInputAction.None;

        /// <summary>
        /// Action to use for pointer events.
        /// </summary>
        public MixedRealityInputAction PointerAction => pointerAction;

        [SerializeField]
        [Tooltip("Action to use for hold events.")]
        private MixedRealityInputAction holdAction = MixedRealityInputAction.None;

        /// <summary>
        /// Action to use for hold events.
        /// </summary>
        public MixedRealityInputAction HoldAction => holdAction;
    }
}