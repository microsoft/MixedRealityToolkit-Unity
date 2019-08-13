// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
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
                case DwellStateType.DwellCanceled when dwellProfile is DwellProfileWithDecay customDwellProfile && customDwellProfile.AllowDwellDecayOnCancel:
                    FillTimer += Time.deltaTime;
                    if (DwellProgress <= 0)
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
