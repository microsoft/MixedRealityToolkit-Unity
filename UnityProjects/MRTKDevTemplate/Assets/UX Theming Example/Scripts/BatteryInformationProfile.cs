// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    [CreateAssetMenu(fileName = "Example_BatteryInformation_Profile", menuName = "MRTK/Examples/Battery Information Profile")]
    [Serializable]
    public class BatteryInformationProfile : ScriptableObject
    {
        /// <summary>
        /// All sprites needed to show normal and charging states in a particular UX style.
        /// </summary>
        [Serializable]
        public class BatteryLevelProfile
        {
            [Tooltip("A list of battery level status sprites to show the battery level when not charging.")]
            [SerializeField]
            private Sprite[] normalStateIcons;
            public Sprite[] NormalStateIcons => normalStateIcons;

            [Tooltip("A list of battery level status sprites to show the battery level when device is charging. Note that this can be a different number of sprites than the non-charging list.")]
            [SerializeField]
            private Sprite[] chargingStateIcons;
            public Sprite[] ChargingStateIcons => chargingStateIcons;
        }

        /// <summary>
        /// A simple battery level theming profile in the form of a ScriptableObject,
        /// that can be used as a DataSource via DataSourceReflection.
        /// </summary>
        /// <remarks>
        /// By creating multiple profiles out of this ScriptableObject, each profile can contain a different
        /// sprites with a different aesthetic, like a "dark" and a "light" theme.
        ///
        /// These profiles can then be easily swapped via helper scripts such as the ThemeSelector.
        /// </remarks>
        [Tooltip("All battery level status theme assets.")]
        [SerializeField]
        private BatteryLevelProfile batteryLevel;
        public BatteryLevelProfile BatteryLevel => batteryLevel;

        // Other battery related assets could go here such as for lifetime battery charge/discharge cycles, or battery malfunction warning icons.
    }
}
