// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.EventDatum.Boundary;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Diagnostics;
using System;
using System.Text;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.DiagnosticsSystem
{
    public class DiagnosticBehavior : MonoBehaviour, IMixedRealityDiagnosticsHandler
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

        public void OnDiagnosticSettingsChanged(DiagnosticsEventData eventData)
        {
            this.ShowCpu = eventData.ShowCpu;
            this.ShowMemory = eventData.ShowMemory;
            this.ShowFps = eventData.ShowFps;
            this.enabled = eventData.Visible;
        }

        void UpdateIsShowingInformation()
        {
            isShowingInformation = ShowCpu ||
                                   ShowFps ||
                                   ShowMemory;
        }

        void Update()
        {
            Debug.Log("DiagnosticBehavior update");

            UpdateIsShowingInformation();

            if (!isShowingInformation)
            {
                return;
            }

            StringBuilder sb = new StringBuilder();
            numberOfLines = 0;

            if (ShowCpu)
            {
                var reading = cpuUseTracker.GetReading();
                sb.AppendLine($"CPU Time: {reading.CpuTime.TotalMilliseconds} ms");
                numberOfLines++;
            }

            if (ShowFps)
            {
                sb.AppendLine($"Frame: {Time.unscaledDeltaTime * 1000} ms ({1.0f / Time.unscaledDeltaTime} fps)");
                numberOfLines++;
            }

            if (ShowMemory)
            {
                var reading = memoryUseTracker.GetReading();
                sb.AppendLine($"Memory: {BytesToMB(reading.GCMemory)} MB");
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

        void OnGUI()
        {
            if (displayText == null || !isShowingInformation)
            {
                return;
            }

            Debug.Log(displayText);
        }

        private static float BytesToMB(long bytes)
        {
            return bytes / 1000000;
        }
    }
}
