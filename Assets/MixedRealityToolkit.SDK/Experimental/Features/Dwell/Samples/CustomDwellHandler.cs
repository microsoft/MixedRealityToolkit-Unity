// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Dwell
{
    /// <summary>
    /// Example to demonstrate override
    /// </summary>
    public class CustomDwellHandler : DwellHandler
    {
        public override float CalculateDwellProgress()
        {
            float dwellProgress = 0;

            switch (currentDwellState)
            {
                case DwellState.None:
                    dwellProgress = 0;
                    break;
                case DwellState.FocusGained:
                    dwellProgress = 0;
                    break;
                case DwellState.DwellStarted:
                    fillTimer -= Time.deltaTime;
                    dwellProgress = Mathf.Clamp((float)(dwellProfile.TimeToCompleteDwell - fillTimer) / dwellProfile.TimeToCompleteDwell,
                        0f, 1f);
                    break;
                case DwellState.DwellCompleted:
                    dwellProgress = 1;
                    break;
                case DwellState.DwellCanceled:
                    var customDwellProfile = dwellProfile as DwellProfileWithDecay;
                    if (customDwellProfile.AllowDwellDecayOnCancel)
                    {
                        fillTimer += Time.deltaTime;
                        dwellProgress = Mathf.Clamp((float)(dwellProfile.TimeToCompleteDwell - fillTimer) / dwellProfile.TimeToCompleteDwell,
                                          0f, 1f);
                    }
                    break;
                case DwellState.Invalid:
                default:
                    return dwellProgress;
            }

            return dwellProgress;
        }
    }
}