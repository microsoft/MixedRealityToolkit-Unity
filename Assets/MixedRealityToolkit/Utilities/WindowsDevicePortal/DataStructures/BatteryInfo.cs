// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.WindowsDevicePortal
{
    [Serializable]
    public class BatteryInfo
    {
        /// <summary>
        /// (0 | 1)
        /// </summary>
        public int AcOnline;
        /// <summary>
        /// (0 | 1)
        /// </summary>
        public int BatteryPresent;
        /// <summary>
        /// (0 | 1)
        /// </summary>
        public int Charging;
        public int DefaultAlert1;
        public int DefaultAlert2;
        public int EstimatedTime;
        public int MaximumCapacity;
        public int RemainingCapacity;

        public bool IsCharging => AcOnline != 0;

        [NonSerialized]
        private float percentRemaining = 0f;
        public float PercentRemaining
        {
            get
            {
                if (percentRemaining > 0f)
                {
                    return percentRemaining;
                }

                return percentRemaining = RemainingCapacity / (float)MaximumCapacity;
            }
        }
    }
}