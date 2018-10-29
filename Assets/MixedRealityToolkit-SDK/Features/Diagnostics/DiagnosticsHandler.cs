// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.EventDatum.Diagnostics;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Diagnostics;
using System;
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
        private bool showCpu;
        private bool ShowCpu
        {
            get { return showCpu; }
            set
            {
                if (showCpu != value)
                {
                    showCpu = value;
                    if (!showCpu)
                    {
                        cpuUseTracker.Reset();
                    }
                }
            }
        }

        private bool ShowFps { get; set; }
        private bool ShowMemory { get; set; }

        private bool isShowingInformation;

        private CpuUseTracker cpuUseTracker = new CpuUseTracker();
        private MemoryUseTracker memoryUseTracker = new MemoryUseTracker();
        private FpsUseTracker fpsUseTracker = new FpsUseTracker();
        StringBuilder displayText = new StringBuilder();

        private GUIStyle style = null;

        private Rect rect = new Rect();

        private void Awake()
        {
            style = new GUIStyle()
            {
                alignment = TextAnchor.UpperLeft,
                normal = new GUIStyleState()
                {
                    textColor = new Color(0, 0, 0.5f, 1)
                }
            };
        }

        /// <summary>
        /// Updates the diagnostic settings
        /// </summary>
        /// <param name="eventData"><see cref="DiagnosticsEventData"/> coming in</param>
        public void OnDiagnosticSettingsChanged(DiagnosticsEventData eventData)
        {
            this.ShowCpu = eventData.ShowCpu;
            this.ShowMemory = eventData.ShowMemory;
            this.ShowFps = eventData.ShowFps;
            this.enabled = eventData.Visible;
        }

        private void UpdateIsShowingInformation()
        {
            isShowingInformation = ShowCpu ||
                                   ShowFps ||
                                   ShowMemory;
        }
        
        private void Update()
        {
            UpdateIsShowingInformation();

            if (!isShowingInformation)
            {
                return;
            }

            displayText.Clear();

            if (ShowFps)
            {
                var timeInSeconds = fpsUseTracker.GetFpsInSeconds();
                displayText.AppendLine($"Fps: {Math.Round(1.0f / timeInSeconds, 2)}");
                displayText.AppendLine($"Frame Time: {Math.Round(timeInSeconds * 1000, 2)} ms");
            }

            if (ShowCpu)
            {
                var reading = cpuUseTracker.GetReadingInMs();
                displayText.AppendLine($"CPU Time: {reading} ms");
            }

            if (ShowMemory)
            {
                var reading = memoryUseTracker.GetReading();
                displayText.AppendLine($"Memory: {Math.Round(BytesToMB(reading.GCMemoryInBytes), 2)} MB");
            }
        }

        private void OnGUI()
        {
            if (!isShowingInformation || displayText.Length == 0)
            {
                return;
            }

            int w = Screen.width, h = Screen.height;

            rect.Set(0, 0, w, h * 2 / 100);

            style.fontSize = h * 2 / 100;
            GUI.Label(rect, displayText.ToString(), style); 
        }

        private static float BytesToMB(long bytes)
        {
            return bytes / (float)(1024 * 1024);
        }
    }
}
