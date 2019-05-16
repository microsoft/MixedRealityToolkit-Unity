// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Compositor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class TimeSynchronizer : Singleton<TimeSynchronizer>
    {
        private CompositionManager compositionManager;

        protected override void Awake()
        {
            base.Awake();

            compositionManager = FindObjectOfType<CompositionManager>();
        }

        public float GetHologramTime()
        {
            return (compositionManager != null) ? compositionManager.GetHologramTime() : Time.time;
        }
    }
}
