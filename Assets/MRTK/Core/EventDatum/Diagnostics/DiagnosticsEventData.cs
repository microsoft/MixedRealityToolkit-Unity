// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Diagnostics
{
    public class DiagnosticsEventData : GenericBaseEventData
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DiagnosticsEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="diagnosticsSystem">The instance of the Diagnostic System that raised the event.</param>
        public void Initialize(
            IMixedRealityDiagnosticsSystem diagnosticsSystem)
        {
            BaseInitialize(diagnosticsSystem);
        }
    }
}
