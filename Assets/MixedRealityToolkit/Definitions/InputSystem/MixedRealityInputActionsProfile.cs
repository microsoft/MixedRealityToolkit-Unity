// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Configuration profile settings for setting up and consuming Input Actions.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Profiles/Mixed Reality Input Actions Profile", fileName = "MixedRealityInputActionsProfile", order = (int)CreateProfileMenuItemIndices.InputActions)]
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/Input/InputActions.html")]
    public class MixedRealityInputActionsProfile : BaseMixedRealityProfile
    {
        private readonly string[] defaultInputActions =
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

        private readonly AxisType[] defaultInputActionsAxis =
        {
            AxisType.Digital,
            AxisType.Digital,
            AxisType.SixDof,
            AxisType.SixDof,
            AxisType.DualAxis,
            AxisType.DualAxis,
            AxisType.DualAxis,
            AxisType.Digital,
            AxisType.DualAxis,
            AxisType.DualAxis
        }; // Examples only, to be refined later

        [SerializeField]
        [Tooltip("The list of actions users can do in your application.")]
        private MixedRealityInputAction[] inputActions =
        {
            // 0 is reserved for "None"
            new MixedRealityInputAction(1, "Select"),
            new MixedRealityInputAction(2, "Menu"),
            new MixedRealityInputAction(3, "Grip")
        }; // Examples only, to be refined later

        /// <summary>
        /// The list of actions users can do in your application.
        /// </summary>
        /// <remarks>Input Actions are device agnostic and can be paired with any number of device inputs across all platforms.</remarks>
        public MixedRealityInputAction[] InputActions => inputActions;

        /// <summary>
        /// Reset the current InputActions definitions to the Mixed Reality Toolkit defaults
        /// If existing mappings exist, they will be preserved and pushed to the end of the array
        /// </summary>
        /// <returns>Default MRTK Actions plus any custom actions (if already configured)</returns>
        public MixedRealityInputAction[] LoadMixedRealityToolKitDefaults()
        {
            var defaultActions = new List<MixedRealityInputAction>();
            bool exists = false;

            for (uint i = 0; i < defaultInputActions.Length; i++)
            {
                defaultActions.Add(new MixedRealityInputAction(i, defaultInputActions[i], defaultInputActionsAxis[i]));
            }

            for (int i = 0; i < inputActions.Length; i++)
            {
                if (defaultActions.Contains(inputActions[i]))
                {
                    exists = true;
                }

                if (!exists)
                {
                    defaultActions.Add(inputActions[i]);
                }

                exists = false;
            }

            return inputActions = defaultActions.ToArray();
        }
    }
}