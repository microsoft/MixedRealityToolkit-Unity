// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.EventDatum.Boundary;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Diagnostics;
using System;
using System.Text;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.DiagnosticsSystem
{
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

        private string displayText;
        private int numberOfLines = 0;
        private bool isShowingInformation;

        private CpuUseTracker cpuUseTracker = new CpuUseTracker();
        private MemoryUseTracker memoryUseTracker = new MemoryUseTracker();
        private FpsUseTracker fpsUseTracker = new FpsUseTracker();

        private GUIStyle style = new GUIStyle()
        {
            alignment = TextAnchor.UpperLeft,
            normal = new GUIStyleState()
            {
                textColor = new Color(0, 0, 0.5f, 1)
            }
        };

        private Rect rect = new Rect();

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

            StringBuilder sb = new StringBuilder();
            numberOfLines = 0;

            if (ShowFps)
            {
                var timeInSeconds = fpsUseTracker.GetFpsInSeconds();
                sb.AppendLine($"Fps: {Math.Round(1.0f / timeInSeconds, 2)}");
                sb.AppendLine($"Frame Time: {Math.Round(timeInSeconds * 1000, 2)} ms");
                numberOfLines++;
            }

            if (ShowCpu)
            {
                var reading = cpuUseTracker.GetReadingInMs();
                sb.AppendLine($"CPU Time: {reading} ms");
                numberOfLines++;
            }

            if (ShowMemory)
            {
                var reading = memoryUseTracker.GetReading();
                sb.AppendLine($"Memory: {Math.Round(BytesToMB(reading.GCMemoryInBytes), 2)} MB");
                numberOfLines++;
            }

            if (numberOfLines > 0)
            {
                displayText = sb.ToString();
            }
            else
            {
                displayText = null;
            }
        }

        private void OnGUI()
        {
            if (!isShowingInformation || displayText == null)
            {
                return;
            }

            int w = Screen.width, h = Screen.height;

            rect.Set(0, 0, w, h * 2 / 100);

            style.fontSize = h * 2 / 100;
            GUI.Label(rect, displayText, style); 
        }

        private static float BytesToMB(long bytes)
        {
            return bytes / (float)(1024 * 1024);
        }
    }
}
