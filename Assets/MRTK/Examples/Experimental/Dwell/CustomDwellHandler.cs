// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Dwell
{
    /// <summary>
    /// Example to demonstrate DwellHandler override when a custom profile is used
    /// This example script works with the DwellProfileWithDecay custom profile.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/CustomDwellHandler")]
    public class CustomDwellHandler : DwellHandler
    {
        protected override void UpdateFillTimer()
        {
            switch (CurrentDwellState)
            {
                case DwellStateType.DwellCanceled:
                    if (dwellProfile is DwellProfileWithDecay profileWithDecay)
                    {
                        FillTimer -= Time.deltaTime * (float)dwellProfile.TimeToCompleteDwell.TotalSeconds / profileWithDecay.TimeToAllowDwellDecay;
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
    }
}
