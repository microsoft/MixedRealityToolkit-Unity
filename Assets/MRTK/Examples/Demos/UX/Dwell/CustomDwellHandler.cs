// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Dwell
{
    /// <summary>
    /// Example to demonstrate DwellHandler override when a custom profile is used
    /// This example script works with the DwellProfileWithDecay custom profile.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/CustomDwellHandler")]
    public class CustomDwellHandler : DwellHandler
    {
        /// <summary>
        /// Override implementation to handle the decay
        /// </summary>
        protected override void UpdateFillTimer()
        {
            switch (CurrentDwellState)
            {
                case DwellStateType.DwellCanceled:
                    if (dwellProfile is DwellProfileWithDecay profileWithDecay)
                    {
                        FillTimer -= Time.deltaTime * dwellProfile.TimeToCompleteDwell / profileWithDecay.TimeToAllowDwellDecay;
                        if (FillTimer <= 0)
                        {
                            FillTimer = 0;
                            CurrentDwellState = DwellStateType.None;
                        }
                    }
                    else
                    {
                        Debug.LogError("The assigned profile is not DwellProfileWithDecay!");
                    }
                    break;
                default:
                    base.UpdateFillTimer();
                    break;
            }
        }

        /// <summary>
        /// Override implementation to handle the decay
        /// </summary>
        public override float DwellProgress
        {
            get
            {
                switch (CurrentDwellState)
                {
                    case DwellStateType.DwellCanceled:
                        if (dwellProfile is DwellProfileWithDecay profileWithDecay)
                        {
                            if (profileWithDecay.TimeToAllowDwellDecay > 0)
                            {
                                return GetCurrentDwellProgress();
                            }
                            else
                            {
                                return 0;
                            }
                        }
                        else
                        {
                            Debug.LogError("The assigned profile is not DwellProfileWithDecay!");
                            return base.DwellProgress;
                        }
                    default:
                        return base.DwellProgress;
                }
            }
        }
    }
}
