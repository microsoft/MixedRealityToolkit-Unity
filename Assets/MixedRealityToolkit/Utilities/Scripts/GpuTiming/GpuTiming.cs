// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace MixedRealityToolkit.Utilities
{
    /// <summary>
    /// Encapsulates access to GPU timing methods.
    /// </summary>
    public static class GpuTiming
    {
        [DllImport("GpuTiming")]
        private static extern IntPtr GetRenderEventFunc();

        [DllImport("GpuTiming")]
        private static extern double GetLastFrameGPUTime(int eventId);

        private const int BaseBeginEventId = 1000;
        private const int BaseEndEventId = 2000;

        private static int nextAvailableEventId = 0;
        private static Stack<int> currentEventId = new Stack<int>();
        private static Dictionary<string, int> eventIds = new Dictionary<string, int>();
        /// <summary>
        /// Gets the latest available sample time for the given event. 
        /// </summary>
        /// <param name="eventId">Name of the event.</param>
        /// <returns>Time in milliseconds.</returns>
        public static double GetTime(string eventId)
        {
            int eventValue;
            if (eventIds.TryGetValue(eventId, out eventValue))
            {
                return GetLastFrameGPUTime(eventValue);
            }

            return -1;
        }
        /// <summary>
        /// Begins sampling GPU time. 
        /// </summary>
        /// <param name="eventId">Name of the event.</param>
        /// <returns>Returns true if a BeginSample with the same event name was last added.</returns>
        public static bool BeginSample(string eventId)
        {
            int eventValue;
            if (!eventIds.TryGetValue(eventId, out eventValue))
            {
                if (nextAvailableEventId == BaseEndEventId)
                {
                    return false;
                }

                eventValue = nextAvailableEventId;
                eventIds.Add(eventId, nextAvailableEventId++);
            }

            if (currentEventId.Contains(eventValue))
            {
                Debug.LogWarning("BeginSample() is being called without a corresponding EndSample() call.");
                return false;
            }

            currentEventId.Push(eventValue);

            // Begin measuring GPU time
            int eventFunctionId = eventValue + BaseBeginEventId;
            GL.IssuePluginEvent(GetRenderEventFunc(), eventFunctionId);
            return true;
        }
        /// <summary>
        /// Ends the GPU sample currently in flight. 
        /// </summary>
        public static void EndSample()
        {
            if (currentEventId.Count > 0)
            {
                // End measuring GPU frame time
                int eventId = currentEventId.Pop() + BaseEndEventId;
                GL.IssuePluginEvent(GetRenderEventFunc(), eventId);
            }
        }
    }
}
