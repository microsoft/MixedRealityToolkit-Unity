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
    /// The default implementation of the <see cref="Microsoft.MixedReality.Toolkit.Core.Interfaces.Diagnostics.IMixedRealityDiagnosticsSystem"/>
    /// </summary>
    public class MixedRealityDiagnosticsSystem : BaseEventSystem, IMixedRealityDiagnosticsSystem
    {
        /// <summary>
        /// The parent object under which all visualization game objects will be placed.
        /// </summary>
        private GameObject diagnosticVisualizationParent = null;

        /// <summary>
        /// Creates the parent for diagnostic visualizations so that the scene hierarchy does not get overly cluttered.
        /// </summary>
        /// <returns>
        /// The <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> to which diagnostic visualizations will be parented.
        /// </returns>
        private GameObject CreateDiagnosticVisualizationParent()
        {
            diagnosticVisualizationParent = new GameObject("Diagnostics");
            diagnosticVisualizationParent.transform.parent = MixedRealityToolkit.Instance.MixedRealityPlayspace.transform;
            diagnosticVisualizationParent.SetActive(MixedRealityToolkit.Instance.ActiveProfile.DiagnosticsSystemProfile.ShowDiagnostics);

            // visual profiler settings
            visualProfiler = diagnosticVisualizationParent.AddComponent<MixedRealityToolkitVisualProfiler>();
            visualProfiler.WindowParent = diagnosticVisualizationParent.transform;
            visualProfiler.IsVisible = MixedRealityToolkit.Instance.ActiveProfile.DiagnosticsSystemProfile.ShowProfiler;

            return diagnosticVisualizationParent;
        }

        private MixedRealityToolkitVisualProfiler visualProfiler = null;

        #region IMixedRealityService

        /// <inheritdoc />
        public override void Initialize()
        {
            if (!Application.isPlaying) { return; }

            eventData = new DiagnosticsEventData(EventSystem.current);

            // Apply profile settings
            ShowDiagnostics = MixedRealityToolkit.Instance.ActiveProfile.DiagnosticsSystemProfile.ShowDiagnostics;
            ShowProfiler = MixedRealityToolkit.Instance.ActiveProfile.DiagnosticsSystemProfile.ShowProfiler;

            diagnosticVisualizationParent = CreateDiagnosticVisualizationParent();
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            if (diagnosticVisualizationParent != null)
            {
                diagnosticVisualizationParent.transform.DetachChildren();
                if (Application.isEditor)
                {
                    Object.DestroyImmediate(diagnosticVisualizationParent);
                }
                else
                {
                    Object.Destroy(diagnosticVisualizationParent);
                }

                diagnosticVisualizationParent = null;
            }
        }

        #endregion IMixedRealityService

        #region IMixedRealityDiagnosticsSystem

        private bool showDiagnostics;

        public bool ShowDiagnostics
        {
            get { return showDiagnostics; }

            set
            {
                if (value != showDiagnostics)
                {
                    showDiagnostics = value;

                    if (diagnosticVisualizationParent != null)
                    {
                        diagnosticVisualizationParent.SetActive(value);
                    }
                }
            }
        }

        private bool showProfiler;

        /// <inheritdoc />
        public bool ShowProfiler
        {
            get
            {
                return showProfiler;
            }

            set
            {
                if (value != showProfiler)
                {
                    showProfiler = value;
                    if (visualProfiler != null)
                    {
                        visualProfiler.IsVisible = value;
                    }
                }
            }
        }

        private float frameRateDuration = 0.1f;
        private readonly float minFrameRateDuration = 0.01f;
        private readonly float maxFrameRateDuration = 1.0f;

        /// <inheritdoc />
        public float FrameRateDuration
        {
            get
            {
                return frameRateDuration;
            }

            set
            {
                if (!Mathf.Approximately(frameRateDuration, value))
                {
                    frameRateDuration = Mathf.Clamp(value, minFrameRateDuration, maxFrameRateDuration);
                    if (visualProfiler != null)
                    {
                        visualProfiler.FrameSampleRate = frameRateDuration;
                    }
                }
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
            eventData.Initialize(this);
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