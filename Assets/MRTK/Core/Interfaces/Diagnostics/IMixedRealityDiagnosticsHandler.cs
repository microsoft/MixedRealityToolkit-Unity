// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Diagnostics
{
    public interface IMixedRealityDiagnosticsHandler : IEventSystemHandler
    {
        void OnDiagnosticSettingsChanged(DiagnosticsEventData eventData);
    }
}
