// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Windows.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Configuration profile settings for setting up and consuming Input Actions.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Profiles/Mixed Reality Gestures Profile", fileName = "MixedRealityGesturesProfile", order = (int)CreateProfileMenuItemIndices.Gestures)]
    [HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/input/gestures")]
    public class MixedRealityGesturesProfile : BaseMixedRealityProfile
    {
        [EnumFlags]
        [SerializeField]
        [Tooltip("The recognizable Manipulation Gestures.")]
        private WindowsGestureSettings manipulationGestures = 0;

        /// <summary>
        /// The recognizable Manipulation Gestures.
        /// </summary>
        public WindowsGestureSettings ManipulationGestures => manipulationGestures;

        [EnumFlags]
        [SerializeField]
        [Tooltip("The recognizable Navigation Gestures.")]
        private WindowsGestureSettings navigationGestures = 0;

        /// <summary>
        /// The recognizable Navigation Gestures.
        /// </summary>
        public WindowsGestureSettings NavigationGestures => navigationGestures;

        [SerializeField]
        [Tooltip("Should the Navigation use Rails on start?\nNote: This can be changed at runtime to switch between the two Navigation settings.")]
        private bool useRailsNavigation = false;

        public bool UseRailsNavigation => useRailsNavigation;

        [EnumFlags]
        [SerializeField]
        [Tooltip("The recognizable Rails Navigation Gestures.")]
        private WindowsGestureSettings railsNavigationGestures = 0;

        /// <summary>
        /// The recognizable Navigation Gestures.
        /// </summary>
        public WindowsGestureSettings RailsNavigationGestures => railsNavigationGestures;

        [SerializeField]
        private AutoStartBehavior windowsGestureAutoStart = AutoStartBehavior.AutoStart;

        public AutoStartBehavior WindowsGestureAutoStart => windowsGestureAutoStart;

        [SerializeField]
        private MixedRealityGestureMapping[] gestures =
        {
            new MixedRealityGestureMapping("Hold", GestureInputType.Hold, MixedRealityInputAction.None),
            new MixedRealityGestureMapping("Navigation", GestureInputType.Navigation, MixedRealityInputAction.None),
            new MixedRealityGestureMapping("Manipulation", GestureInputType.Manipulation, MixedRealityInputAction.None),
        };

        /// <summary>
        /// The currently configured gestures for the application.
        /// </summary>
        public MixedRealityGestureMapping[] Gestures => gestures;
    }
}