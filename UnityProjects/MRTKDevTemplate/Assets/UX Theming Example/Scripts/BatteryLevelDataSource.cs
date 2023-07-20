// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for samples. While nice to have, this XML documentation is not required for samples.
#pragma warning disable CS1591

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// A data source that manages a battery level and provides information that can be
    /// used by any data consumer related to battery level.
    /// </summary>
    /// <remarks>
    /// <para>
    /// As the battery level and charging state change, the desired icon may change.
    /// This class contains the logic to take the actual reported analog charge level
    /// and properly calculate the correct icon.
    /// </para>
    /// <para>
    /// The icons are retrieved from a separate data source. This allows the actual icon
    /// assets to be organized and managed in a theme profile along with other theme profile
    /// assets.
    /// </para>
    /// <para>
    /// The information this data source makes available are managed in a dictionary with the
    /// following key paths available for data consumers:
    /// </para>
    /// <list type="table">
    ///     <listheader>
    ///         <term>Date Type</term>
    ///         <term>Key Path</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term>Sprite</term>
    ///         <term><c>batteryIcon</c></term>
    ///         <description>The current sprite.</description>
    ///     </item>
    ///     <item>
    ///         <term>bool</term>
    ///         <term><c>isCharging</c></term>
    ///         <description>Whether currently in a charging state.</description>
    ///     </item>
    ///     <item>
    ///         <term>string</term>
    ///         <term><c>label</c></term>
    ///         <description>Status text, usually a percentage of charge.</description>
    ///     </item>
    ///     <item>
    ///         <term>int</term>
    ///         <term><c>intLevel</c></term>
    ///         <description>Integral level from 0 to the number of sprites - 1.</description>
    ///     </item>
    ///     <item>
    ///         <term>float</term>
    ///         <term><c>realLevel</c></term>
    ///         <description>Analog level from 0 to 1.</description>
    ///     </item>
    /// </list>        
    /// </remarks>
    [Serializable]
    [AddComponentMenu("MRTK/Examples/Battery Level Data Source")]
    public class BatteryLevelDataSource : DataSourceGOBase
    {
        [SerializeField]
        [Tooltip("Data source that can provide normal and charging sprite sets, such as a theme profile data source.")]
        private DataSourceGOBase spriteDataSource;

        [SerializeField]
        [Tooltip("Keypath for normal battery sprites.")]
        private string normalBatterySpritesKeypath = "BatteryLevel.NormalBatteryIcons";

        [SerializeField]
        [Tooltip("Keypath for charging battery sprites.")]
        private string chargingBatterySpritesKeypath = "BatteryLevel.ChargingBatteryIcons";

        [SerializeField]
        [Tooltip("Convenience access to battery level for testing from 0.0 to 1.0.")]
        private float batteryLevel0To1 = 0;

        private bool _valueChanged = false;

        /// <summary>
        /// Publicly accessible battery level in range of 0..1
        /// </summary>
        public float BatteryLevel
        {
            get
            {
                return batteryLevel0To1;
            }
            
            set
            {
                batteryLevel0To1 = ValidateBatteryLevel(value);
                UpdateBatteryLevelInformation();
            }
        }

        [SerializeField]
        [Tooltip("Convenience access to battery charging state for testing.")]
        private bool isCharging = false;
        
        /// <summary>
        /// Get or set if the battery currently being charged.
        /// </summary>
        public bool IsCharging
        {
            get
            {
                return isCharging;
            }

            set
            {
                isCharging = value;
                UpdateBatteryLevelInformation();
            }
        }

        /// <summary>
        /// A Unity event function that is called every frame, if this object is enabled.
        /// </summary>
        private void Update()
        {
            if (_valueChanged )
            {
                UpdateBatteryLevelInformation();
                _valueChanged = false;
            }
        }

        /// <summary>
        /// This is really a data source proxy to make it placeable
        /// in the Unity inspector. This method allocates the actual
        /// non-Unity specific data source the proxy will route
        /// requests to.
        /// </summary>
        /// <returns>The data source we allocate and provide.</returns>
        public override IDataSource AllocateDataSource()
        {
            return new DataSourceDictionary();
        }

        /// <inheritdoc/>
        protected override void InitializeDataSource()
        {
            if (DataSourceType == null || DataSourceType == "")
            {
                DataSourceType = "batteryData";
            }
            UpdateBatteryLevelInformation();
        }

        /// <summary>
        /// All paths that change either the charging state or the level should lead
        /// to here to update all state.
        /// </summary>
        protected void UpdateBatteryLevelInformation()
        {
            string spriteListKeypath = GetSpriteListKeypath();

            int spriteCount = spriteDataSource.GetCollectionCount(spriteListKeypath);
            int whichSprite = CalculateIntegralLevelFromAnalogLevel(BatteryLevel, IsCharging, spriteCount);
            int level = (int)Math.Round((BatteryLevel * 10) + 0.5) * 10;        // 0, 10, 20, 30, ... 100

            string label = String.Format("{0}%\n{1}", level, IsCharging ? "Charging" : "");
            string finalSpritePath = spriteDataSource.GetNthCollectionKeyPathAt(spriteListKeypath, whichSprite);

            Sprite finalSprite = spriteDataSource.GetValue(finalSpritePath) as Sprite;

            DataChangeSetBegin();

            SetValue("batteryIcon", finalSprite);
            SetValue("intLevel", whichSprite);
            SetValue("realLevel", BatteryLevel);
            SetValue("isCharging", IsCharging);
            SetValue("label", label);

            DataChangeSetEnd();
        }

        /// <summary>
        /// Returns the keypath for the appropriate set of sprites based
        /// on whether charging or not.
        /// </summary>
        /// <returns>Keypath for correct sprite collection.</returns>
        protected string GetSpriteListKeypath()
        {
            if (IsCharging)
            {
                return chargingBatterySpritesKeypath;
            }
            else
            {
                return normalBatterySpritesKeypath;
            }
        }

        /// <summary>
        /// Calculate an integral level from 0 to &lt; <paramref name="numIntegralLevels"/> based on charging state and <paramref name="level"/>.
        /// </summary>
        /// <param name="level">Current battery level from 0 to 1 inclusive.</param>
        /// <param name="charging">Whether it is currently in the state of being charged.</param>
        /// <param name="numIntegralLevels">The number of allowed integral levels, typically equal to the # of unique icons.</param>
        /// <returns>The integral level based on analog level and number of available sprites.</returns>
        protected virtual int CalculateIntegralLevelFromAnalogLevel(float level, bool charging, int numIntegralLevels)
        {
            int integralLevel = 0;

            if (charging)
            {
                // any algorithm desired for charging
                integralLevel = Math.Min((int)((0.1 + level) * numIntegralLevels), numIntegralLevels - 1);
            }
            else
            {
                integralLevel = Math.Min((int)((0.05 + level) * numIntegralLevels), numIntegralLevels - 1);
            }
            return integralLevel;
        }
    
        /// <summary>
        /// A Unity Editor only event function that is called when the script is loaded or a value changes in the Unity Inspector.
        /// </summary>
        private void OnValidate()
        {
            batteryLevel0To1 = ValidateBatteryLevel(batteryLevel0To1);
            _valueChanged = true;
        }

        /// <summary>
        /// Clamp the provided battery level to an acceptable range.
        /// </summary>
        /// <param name="level">The battery level to clamp.</param>
        /// <returns>The clamped battery level value.</returns>
        protected float ValidateBatteryLevel(float level)
        {
            return Mathf.Clamp01(level);
        }
    }
}
#pragma warning restore CS1591