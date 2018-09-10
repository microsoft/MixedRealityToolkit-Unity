// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.EventDatum.Boundary;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Diagnostics;
using UnityEngine;

namespace Assets.MixedRealityToolkit_SDK.Features.Diagnostics
{
    public class DiagnosticBehavior : MonoBehaviour, IMixedRealityDiagnosticsHandler
    {
        public bool ShowCpu { get; set; }
        public bool ShowFps { get; set; }
        public bool ShowMemory { get; set; }

        public void OnDiagnosticSettingsChanged(DiagnosticsEventData eventData)
        {
            this.ShowCpu = eventData.ShowCpu;
            this.ShowMemory = eventData.ShowMemory;
            this.ShowFps = eventData.ShowFps;
            this.enabled = eventData.Visible;
        }
    }
}
