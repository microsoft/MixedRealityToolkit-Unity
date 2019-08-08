// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Dwell
{
    public class CustomDwellHandler : DwellHandler
    {
        private float lastDwellProgress = 0;

        public override float CalculateDwellProgress(float lastProgressValue)
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
                    dwellProgress = Mathf.Clamp((float)(DateTime.UtcNow - focusEnterTime.AddSeconds(dwellProfile.DwellIntentDelay + dwellProfile.DwellStartDelay)).TotalSeconds
                        / dwellProfile.TimeToCompleteDwell,
                        0f, 1f);
                    break;
                case DwellState.DwellCompleted:
                    dwellProgress = 1;
                    break;
                case DwellState.DwellCanceled:
                    var myDwellProfile = dwellProfile as DwellProfileWithDecay;
                    if (myDwellProfile.AllowDwellDecayOnCancel)
                    {
                        dwellProgress = lastProgressValue - (lastDwellProgress / myDwellProfile.TimeToAllowDwellDecay);
                    }
                    break;
                case DwellState.Invalid:
                default:
                    return dwellProgress;
            }

            lastDwellProgress = dwellProgress;
            return dwellProgress;
        }
    }
}