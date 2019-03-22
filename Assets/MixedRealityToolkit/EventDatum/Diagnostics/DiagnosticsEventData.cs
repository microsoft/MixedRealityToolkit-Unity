// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Diagnostics
{
    public class DiagnosticsEventData : GenericBaseEventData
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventSystem"></param>
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
