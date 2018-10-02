// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.EventDatum.Diagnostics;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.Diagnostics
{
    public interface IMixedRealityDiagnosticsHandler : IEventSystemHandler
    {
        void OnDiagnosticSettingsChanged(DiagnosticsEventData eventData);
    }
}
