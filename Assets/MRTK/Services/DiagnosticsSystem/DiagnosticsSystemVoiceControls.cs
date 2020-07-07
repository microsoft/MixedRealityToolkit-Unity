// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Diagnostics
{
    /// <summary>
    /// Class that listens for and acts upon diagnostic system voice commands.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Services/DiagnosticsSystemVoiceControls")]
    public class DiagnosticsSystemVoiceControls : MonoBehaviour, IMixedRealitySpeechHandler
    {
        bool registeredForInput = false;

        private void OnEnable()
        {
            if (!registeredForInput)
            {
                if (CoreServices.InputSystem != null)
                {
                    CoreServices.InputSystem.RegisterHandler<IMixedRealitySpeechHandler>(this);
                    registeredForInput = true;
                }
            }
        }

        private void OnDisable()
        {
            if (registeredForInput)
            {
                CoreServices.InputSystem.UnregisterHandler<IMixedRealitySpeechHandler>(this);
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
            if (CoreServices.DiagnosticsSystem != null)
            {
                CoreServices.DiagnosticsSystem.ShowDiagnostics = !CoreServices.DiagnosticsSystem.ShowDiagnostics;
            }
        }

        /// <summary>
        /// Shows or hides the profiler display.
        /// </summary>
        public void ToggleProfiler()
        {
            if (CoreServices.DiagnosticsSystem != null)
            {
                CoreServices.DiagnosticsSystem.ShowProfiler = !CoreServices.DiagnosticsSystem.ShowProfiler;
            }
        }
    }
}
