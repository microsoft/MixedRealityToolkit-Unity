// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class TimeSynchronizer : Singleton<TimeSynchronizer>
    {
        public float GetHologramTime()
        {
            return Time.time;
        }
    }
}
