// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem
{
    /// <summary>
    /// Configuration profile settings for setting up and consuming Input Actions.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Input Actions Profile", fileName = "MixedRealityInputActionsProfile", order = 1)]
    public class MixedRealityInputActionsProfile : ScriptableObject
    {
        private readonly string[] inputActionsMRTKDefaults =
{
            "Select",
            "Menu",
            "Grip",
            "Pointer",
            "Walk",
            "Look",
            "Interact",
            "Pickup",
            "Inventory",
            "ConversationSelect"
        }; // Examples only, to be refined later.

        private readonly AxisType[] inputActionsMRTKDefaultsAxis =
        {
            AxisType.Digital,
            AxisType.Digital,
            AxisType.SixDoF,
            AxisType.SixDoF,
            AxisType.DualAxis,
            AxisType.DualAxis,
            AxisType.DualAxis,
            AxisType.Digital,
            AxisType.DualAxis,
            AxisType.DualAxis
}; // Examples only, to be refined later

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

        /// <summary>
        /// Reset the current InputActions definitions to the Mixed Reality Toolkit defaults
        /// If existing mappings exist, they will be preserved and pushed to the end of the array
        /// </summary>
        /// <returns>Default MRTK Actions plus any custom actions (if already configured)</returns>
        public InputAction[] LoadMixedRealityToolKitDefaults()
        {
            List<InputAction> defaultMRTKActions = new List<InputAction>();
            bool exists = false;

            for (uint i = 0; i < inputActionsMRTKDefaults.Length; i++)
            {
                //defaultMRTKActions.Add(new InputAction(i, inputActionsMRTKDefaults[i], inputActionsMRTKDefaultsAxis[i]));
            }

            foreach (var currentAction in inputActions)
            {
                for (int i = 0; i < inputActions?.Length; i++)
                {
                    if (defaultMRTKActions.Contains(inputActions[i]))
                    {
                        exists = true;
                    }
                    if (!exists)
                    {
                        defaultMRTKActions.Add(inputActions[i]);
                    }
                    exists = false;
                }
            }

            inputActions = defaultMRTKActions.ToArray();
            defaultMRTKActions = null;

            return inputActions;
        }
    }
}