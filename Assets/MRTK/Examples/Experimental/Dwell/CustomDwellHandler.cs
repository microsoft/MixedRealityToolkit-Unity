// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Dwell
{
    /// <summary>
    /// Example to demonstrate DwellHandler override
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/CustomDwellHandler")]
    public class CustomDwellHandler : DwellHandler
    {
        protected override void UpdateFillTimer()
        {
            switch (CurrentDwellState)
            {
                case DwellStateType.DwellCanceled:
                    var customDwellProfile = dwellProfile as DwellProfileWithDecay;
                    if (customDwellProfile != null && customDwellProfile.AllowDwellDecayOnCancel)
                    {
                        FillTimer -= Time.deltaTime;
                    }
                    if (FillTimer <= 0)
                    {
                        FillTimer = 0;
                        CurrentDwellState = DwellStateType.None;
                    }
                    break;
                default:
                    base.UpdateFillTimer();
                    break;
            }
        }
    }
}
