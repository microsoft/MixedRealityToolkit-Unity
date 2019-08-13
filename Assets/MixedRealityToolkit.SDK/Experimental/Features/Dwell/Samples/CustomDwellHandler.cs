// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Dwell
{
    /// <summary>
    /// Example to demonstrate DwellHandler override
    /// </summary>
    public class CustomDwellHandler : DwellHandler
    {
        protected override void UpdateFillTimer()
        {
            switch (CurrentDwellState)
            {
                case DwellStateType.None:
                case DwellStateType.FocusGained:
                    FillTimer = 0;
                    break;
                case DwellStateType.DwellStarted:
                    FillTimer -= Time.deltaTime;
                    break;
                case DwellStateType.DwellCompleted:
                    FillTimer = 0;
                    break;
                case DwellStateType.DwellCanceled:
                    var customDwellProfile = dwellProfile as DwellProfileWithDecay;
                    if (customDwellProfile.AllowDwellDecayOnCancel)
                    {
                        FillTimer += Time.deltaTime;
                    }
                    break;
                default:
                    FillTimer = 0;
                    break;
            }
        }
    }
}