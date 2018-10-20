// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.EventDatum.Diagnostics;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Diagnostics;
using Microsoft.MixedReality.Toolkit.Core.Services;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.SDK.DiagnosticsSystem
{
    public class MixedRealityDiagnosticsManager : BaseEventSystem, IMixedRealityDiagnosticsSystem
    {
        #region IMixedRealityService
        private DiagnosticsEventData eventData;
        private GameObject diagnosticVisualization;

        public override void Initialize()
        {
            base.Initialize();
            InitializeInternal();
        }

        private void InitializeInternal()
        {
            eventData = new DiagnosticsEventData(EventSystem.current);

            Visible = MixedRealityToolkit.Instance.ActiveProfile.DiagnosticsSystemProfile.Visible;
            ShowCpu = MixedRealityToolkit.Instance.ActiveProfile.DiagnosticsSystemProfile.ShowCpu;
            ShowFps = MixedRealityToolkit.Instance.ActiveProfile.DiagnosticsSystemProfile.ShowFps;
            ShowMemory = MixedRealityToolkit.Instance.ActiveProfile.DiagnosticsSystemProfile.ShowMemory;

            RaiseDiagnosticsChanged();
        }

        /// <inheritdoc />
        public override void Reset()
        {
            base.Reset();
            InitializeInternal();
        }

        public override void Destroy()
        {
            base.Destroy();

            if (Application.isPlaying)
            {
                if (diagnosticVisualization != null)
                {
                    Unregister(diagnosticVisualization);

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
                showCpu = false;
                showFps = false;
                showMemory = false;

                RaiseDiagnosticsChanged();
            }
        }

        private void RaiseDiagnosticsChanged()
        {
            eventData.Initialize(this,
                visible: Visible,
                showCpu: ShowCpu,
                showFps: ShowFps,
                showMemory: ShowMemory
                );

            HandleEvent(eventData, OnDiagnosticsChanged);
        }

        /// <summary>
        /// Event sent whenever the diagnostics visualization changes.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealityDiagnosticsHandler> OnDiagnosticsChanged =
            delegate (IMixedRealityDiagnosticsHandler handler, BaseEventData eventData)
            {
                DiagnosticsEventData diagnosticsEventsData = ExecuteEvents.ValidateEventData<DiagnosticsEventData>(eventData);
                handler.OnDiagnosticSettingsChanged(diagnosticsEventsData);
            };

        #endregion IMixedRealityService

        #region IMixedRealityDiagnosticsManager
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

        private bool showCpu;

        /// <inheritdoc />
        public bool ShowCpu
        {
            get
            {
                return showCpu;
            }

            set
            {
                if (value != showCpu)
                {
                    showCpu = value;
                    RaiseDiagnosticsChanged();
                }
            }
        }

        private bool showFps;

        /// <inheritdoc />
        public bool ShowFps
        {
            get
            {
                return showFps;
            }
            set
            {
                if (value != showFps)
                {
                    showFps = value;
                    RaiseDiagnosticsChanged();
                }
            }
        }

        private bool showMemory;

        /// <inheritdoc />
        public bool ShowMemory
        {
            get
            {
                return showMemory;
            }
            set
            {
                if (value != showMemory)
                {
                    showMemory = value;
                    RaiseDiagnosticsChanged();
                }
            }
        }

        public GameObject DiagnosticVisualization
        {
            get
            {
                if (diagnosticVisualization != null)
                {
                    return diagnosticVisualization;
                }

                if (!Visible)
                {
                    // Don't create a gameobject if it's not needed
                    return null;
                }

                diagnosticVisualization = GameObject.CreatePrimitive(PrimitiveType.Quad);
                diagnosticVisualization.name = "Diagnostics";
                diagnosticVisualization.layer = Physics.IgnoreRaycastLayer;

                diagnosticVisualization.AddComponent<DiagnosticsHandler>();
                Register(diagnosticVisualization);

                return diagnosticVisualization;
            }
        }
        #endregion IMixedRealityDiagnosticsManager

        #region IMixedRealityEventSource
        /// <inheritdoc />
        public uint SourceId => (uint)SourceName.GetHashCode();

        /// <inheritdoc />
        public string SourceName => "Mixed Reality Diagnostics System";

        /// <inheritdoc />
        public new bool Equals(object x, object y) => false;

        /// <inheritdoc />
        public int GetHashCode(object obj) => SourceName.GetHashCode();
        #endregion IMixedRealityEventSource
    }
}
