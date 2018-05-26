// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem
{
    /// <summary>
    /// Configuration profile settings for setting up and consuming Input Actions.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Input Actions Profile", fileName = "MixedRealityInputActionsProfile", order = 1)]
    public class MixedRealityInputActionsProfile : ScriptableObject
    {
        [SerializeField]
        [Tooltip("The list of actions users can do in your application.")]
        private InputAction[] inputActions =
        {
            new InputAction(1, "Select"),
            new InputAction(2, "Menu"),
            new InputAction(3, "Grip")
        };

        /// <summary>
        /// The list of actions users can do in your application.
        /// <remarks>Input Actions are device agnostic and can be paired with any number of device inputs across all platforms.</remarks>
        /// </summary>
        public InputAction[] InputActions => inputActions;

        [SerializeField]
        [Tooltip("The action to raise pointing events against the uGUI Canvas System.")]
        private InputAction pointerAction = null;

        /// <summary>
        /// The action to use for pointing events.
        /// </summary>
        public InputAction PointerAction => pointerAction;
    }
}