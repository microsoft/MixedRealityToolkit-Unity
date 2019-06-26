// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Diagnostics
{
    /// <summary>
    /// Class that listens for and acts upon diagnostic system voice commands.
    /// </summary>
    public class DiagnosticsSystemVoiceControls : MonoBehaviour, IMixedRealitySpeechHandler
    {
        private IMixedRealityDiagnosticsSystem diagnosticsSystem = null;

        private IMixedRealityDiagnosticsSystem DiagnosticsSystem
        {
            get
            {
                if (diagnosticsSystem == null)
                {
                    MixedRealityServiceRegistry.TryGetService<IMixedRealityDiagnosticsSystem>(out diagnosticsSystem);
                }
                return diagnosticsSystem;
            }
        }

        private IMixedRealityInputSystem inputSystem = null;

        private IMixedRealityInputSystem InputSystem
        {
            get
            {
                if (inputSystem == null)
                {
                    MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem);
                }
                return inputSystem;
            }
        }

        bool registeredForInput = false;

        private void OnEnable()
        {
            if (!registeredForInput)
            {
                if (InputSystem != null)
                {
                    InputSystem.RegisterHandler<IMixedRealitySpeechHandler>(this);
                    registeredForInput = true;
                }
            }
        }

        private void OnDisable()
        {
            if (registeredForInput)
            {
                InputSystem.UnregisterHandler<IMixedRealitySpeechHandler>(this);
                registeredForInput = false;
            }
        }

        /// <inheritdoc />
        void IMixedRealitySpeechHandler.OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            switch (eventData.Command.Keyword.ToLower())
            {
                case "toggle diagnostics":
                    ToggleDiagnostics();
                    break;

                case "toggle profiler":
                    ToggleProfiler();
                    break;
            }
        }

        /// <summary>
        /// Shows or hides all enabled diagnostics.
        /// </summary>
        public void ToggleDiagnostics()
        {
            if (DiagnosticsSystem != null)
            {
                DiagnosticsSystem.ShowDiagnostics = !DiagnosticsSystem.ShowDiagnostics;
            }
        }

        /// <summary>
        /// Shows or hides the profiler display.
        /// </summary>
        public void ToggleProfiler()
        {
            if (DiagnosticsSystem != null)
            {
                DiagnosticsSystem.ShowProfiler = !DiagnosticsSystem.ShowProfiler;
            }
        }
    }
}
