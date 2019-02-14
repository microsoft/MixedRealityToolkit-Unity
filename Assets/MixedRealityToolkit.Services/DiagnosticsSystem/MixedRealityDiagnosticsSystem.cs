// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Diagnostics;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.Diagnostics;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Diagnostics;
using Microsoft.MixedReality.Toolkit.Core.Services;
using Microsoft.MixedReality.Toolkit.Services.DiagnosticsSystem.Profiling;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Services.DiagnosticsSystem
{
    /// <summary>
    /// The default implementation of the <see cref="IMixedRealityDiagnosticsSystem"/>
    /// </summary>
    public class MixedRealityDiagnosticsSystem : BaseEventSystem, IMixedRealityDiagnosticsSystem
    {
        #region IMixedRealityService

        /// <inheritdoc />
        public override void Initialize()
        {
            if (!Application.isPlaying) { return; }

            eventData = new DiagnosticsEventData(EventSystem.current);

            // Setting the visibility creates our GameObject reference, so set it last after we've configured our settings.
            Visible = MixedRealityToolkit.Instance.ActiveProfile.DiagnosticsSystemProfile.Visible;

            RaiseDiagnosticsChanged();
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            diagnosticsHandler = null;

            if (diagnosticVisualization != null)
            {
                if (Application.isEditor)
                {
                    Object.DestroyImmediate(diagnosticVisualization);
                }
                else
                {
                    Object.Destroy(diagnosticVisualization);
                }

                diagnosticVisualization = null;
            }

            visible = false;

            if (Application.isPlaying)
            {
                RaiseDiagnosticsChanged();
            }
        }

        #endregion IMixedRealityService

        #region IMixedRealityDiagnosticsSystem

        private bool visible;

        /// <inheritdoc />
        public bool Visible
        {
            get
            {
                return visible;
            }

            set
            {
                if (value != visible)
                {
                    visible = value;
                    DiagnosticVisualization.SetActive(value);

                    RaiseDiagnosticsChanged();
                }
            }
        }

        private IMixedRealityDiagnosticsHandler diagnosticsHandler;

        private GameObject diagnosticVisualization;

        /// <inheritdoc />
        public GameObject DiagnosticVisualization
        {
            get
            {
                // todo: reorder these checks?
                if (diagnosticVisualization != null)
                {
                    return diagnosticVisualization;
                }

                if (!Visible)
                {
                    // Don't create a GameObject if it's not needed
                    return null;
                }

                diagnosticVisualization = new GameObject("Diagnostics");
                diagnosticVisualization.transform.parent = MixedRealityToolkit.Instance.MixedRealityPlayspace.transform;
                MixedRealityToolkitVisualProfiler visualProfiler = diagnosticVisualization.AddComponent<MixedRealityToolkitVisualProfiler>();
                // todo: apply properties to the script
                diagnosticVisualization.layer = Physics.IgnoreRaycastLayer;

                var handlerType = MixedRealityToolkit.Instance.ActiveProfile.DiagnosticsSystemProfile.HandlerType;

                if (handlerType.Type != null)
                {
                    diagnosticsHandler = diagnosticVisualization.AddComponent(handlerType.Type) as IMixedRealityDiagnosticsHandler;
                    return diagnosticVisualization;
                }

                Debug.LogError("A handler type must be assigned to the diagnostics profile.");
                return null;
            }
        }

        #endregion IMixedRealityDiagnosticsSystem

        #region IMixedRealityEventSource

        private DiagnosticsEventData eventData;

        /// <inheritdoc />
        public uint SourceId => (uint)SourceName.GetHashCode();

        /// <inheritdoc />
        public string SourceName => "Mixed Reality Diagnostics System";

        /// <inheritdoc />
        public new bool Equals(object x, object y) => false;

        /// <inheritdoc />
        public int GetHashCode(object obj) => SourceName.GetHashCode();

        private void RaiseDiagnosticsChanged()
        {
            eventData.Initialize(this, Visible);

            // Manually send it to our diagnostics handler, no matter who's listening.
            diagnosticsHandler?.OnDiagnosticSettingsChanged(eventData);

            HandleEvent(eventData, OnDiagnosticsChanged);
        }

        /// <summary>
        /// Event sent whenever the diagnostics visualization changes.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealityDiagnosticsHandler> OnDiagnosticsChanged =
            delegate (IMixedRealityDiagnosticsHandler handler, BaseEventData eventData)
            {
                var diagnosticsEventsData = ExecuteEvents.ValidateEventData<DiagnosticsEventData>(eventData);
                handler.OnDiagnosticSettingsChanged(diagnosticsEventsData);
            };

        #endregion IMixedRealityEventSource
    }
}