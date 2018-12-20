// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.EventDatum.Diagnostics;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Diagnostics;
using Microsoft.MixedReality.Toolkit.Core.Services;
using System;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.DiagnosticsSystem
{
    /// <summary>
    /// Behavior class for showing Diagnostic information. Implements <see cref="IMixedRealityDiagnosticsHandler"/>
    /// to manage setting updates. 
    /// </summary>
    public class DiagnosticsHandler : MonoBehaviour, IMixedRealityDiagnosticsHandler
    {
        private readonly StringBuilder displayText = new StringBuilder();

        private bool showCpu;
        private bool showFps;
        private bool showMemory;
        private bool isShowingInformation;

        private Rect rect = new Rect();
        private GUIStyle style = null;

        private void Awake()
        {
            style = new GUIStyle
            {
                alignment = TextAnchor.UpperLeft,
                normal = new GUIStyleState
                {
                    textColor = new Color(0, 1f, 0, 1)
                }
            };
        }

        /// <summary>
        /// Updates the diagnostic settings
        /// </summary>
        /// <param name="eventData"><see cref="DiagnosticsEventData"/> coming in</param>
        public void OnDiagnosticSettingsChanged(DiagnosticsEventData eventData)
        {
            showCpu = eventData.ShowCpu;
            showMemory = eventData.ShowMemory;
            showFps = eventData.ShowFps;
            enabled = eventData.Visible;
        }

        private void UpdateIsShowingInformation()
        {
            isShowingInformation = showCpu || showFps || showMemory;
        }

        private void Update()
        {
            UpdateIsShowingInformation();

            if (!isShowingInformation)
            {
                return;
            }

            displayText.Clear();

            if (showFps)
            {
                var timeInSeconds = MixedRealityToolkit.DiagnosticsSystem.FpsUseTracker.CurrentReadingInSeconds;
                displayText.AppendLine($"Fps: {Math.Round(1.0f / timeInSeconds, 2).ToString(CultureInfo.InvariantCulture)}");
                displayText.AppendLine($"Frame Time: {Math.Round(timeInSeconds * 1000, 2).ToString(CultureInfo.InvariantCulture)} ms");
            }

            if (showCpu)
            {
                displayText.AppendLine($"CPU Time: {MixedRealityToolkit.DiagnosticsSystem.CpuUseTracker.CurrentReadingInMs.ToString(CultureInfo.InvariantCulture)} ms");
            }

            if (showMemory)
            {
                displayText.AppendLine($"Memory: {Math.Round(BytesToMB(MixedRealityToolkit.DiagnosticsSystem.MemoryUseTracker.CurrentReading.GcMemoryInBytes), 2).ToString(CultureInfo.InvariantCulture)} MB");
            }
        }

        private void OnGUI()
        {
            if (!isShowingInformation || displayText.Length == 0)
            {
                return;
            }

            int screenHeight = Screen.height * 2 / 100;
            rect.Set(0, 0, Screen.width, screenHeight);
            style.fontSize = screenHeight;
            GUI.Label(rect, displayText.ToString(), style);
        }

        private static float BytesToMB(long bytes)
        {
            return bytes / (float)(1024 * 1024);
        }
    }
}
