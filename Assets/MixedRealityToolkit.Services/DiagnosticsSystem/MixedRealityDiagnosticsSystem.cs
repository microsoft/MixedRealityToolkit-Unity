// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.EventDatum.Diagnostics;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Diagnostics;
using Microsoft.MixedReality.Toolkit.Core.Services;
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
            IsProfilerVisible = MixedRealityToolkit.Instance.ActiveProfile.DiagnosticsSystemProfile.IsProfilerVisible;

            RaiseDiagnosticsChanged();
        }

        /// <inheritdoc />
        public override void Destroy()
        {
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
        public bool IsProfilerVisible
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

        private GameObject diagnosticVisualization;

        /// <inheritdoc />
        public GameObject DiagnosticVisualization
        {
            get
            {
                if (diagnosticVisualization == null)
                {
                    diagnosticVisualization = new GameObject("Diagnostics");
                    diagnosticVisualization.transform.parent = MixedRealityToolkit.Instance.MixedRealityPlayspace.transform;
                    MixedRealityToolkitVisualProfiler visualProfiler = diagnosticVisualization.AddComponent<MixedRealityToolkitVisualProfiler>();
                    visualProfiler.InitiallyActive = MixedRealityToolkit.Instance.ActiveProfile.DiagnosticsSystemProfile.IsProfilerVisible;
                    visualProfiler.WindowParent = diagnosticVisualization.transform;
                }

                return diagnosticVisualization;
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
            eventData.Initialize(this, IsProfilerVisible);
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