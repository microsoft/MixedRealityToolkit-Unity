// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal enum SynchronizationPerformanceFeature : byte
    {
        GameObjectComponentCheck = 0x0,
        MaterialPropertyUpdate = 0x1,
        MaterialPropertyBlockUpdate = 0x2,
    }

    internal class SynchronizationPerformanceMonitor : Singleton<SynchronizationPerformanceMonitor>
    {
        private const int PeriodsToAverageOver = 5;
        private const int FeatureCount = 3;
        private Stopwatch[] stopwatches;
        private float[][] previousSpentTimes;
        private float[][] previousActualTimes;
        private int currentPeriod = 0;

        protected override void Awake()
        {
            base.Awake();

            stopwatches = new Stopwatch[FeatureCount];
            previousSpentTimes = new float[PeriodsToAverageOver][];
            previousActualTimes = new float[PeriodsToAverageOver][];
            for (int i = 0; i < FeatureCount; i++)
            {
                stopwatches[i] = new Stopwatch();
            }

            for (int i = 0; i < PeriodsToAverageOver; i++)
            {
                previousSpentTimes[i] = new float[FeatureCount];
                previousActualTimes[i] = new float[FeatureCount];
            }
        }

        public IDisposable MeasureScope(SynchronizationPerformanceFeature feature)
        {
            return new TimeScope(stopwatches[(byte)feature]);
        }

        public void WriteMessage(BinaryWriter message)
        {
            for (int i = 0; i < currentPeriod && i < PeriodsToAverageOver - 1; i++)
            {
                for (int j = 0; j < FeatureCount; j++)
                {
                    previousSpentTimes[i + 1][j] = previousSpentTimes[i][j];
                    previousActualTimes[i + 1][j] = previousActualTimes[i][j];
                }
            }
            currentPeriod++;
            for (int i = 0; i < FeatureCount; i++)
            {
                previousSpentTimes[0][i] = (float)stopwatches[i].Elapsed.TotalMilliseconds;
                previousActualTimes[0][i] = Time.time;
            }

            if (currentPeriod > 1)
            {
                int targetPeriodSlot = Math.Min(currentPeriod, PeriodsToAverageOver - 1);
                message.Write(FeatureCount);
                for (int i = 0; i < FeatureCount; i++)
                {
                    float spentTimeDelta = previousSpentTimes[0][i] - previousSpentTimes[targetPeriodSlot][i];
                    float actualTimeDelta = previousActualTimes[0][i] - previousActualTimes[targetPeriodSlot][i];
                    message.Write(spentTimeDelta / actualTimeDelta);
                }
            }
            else
            {
                message.Write(FeatureCount);
                for (int i = 0; i < FeatureCount; i++)
                {
                    message.Write(0.0f);
                }
            }
        }

        private struct TimeScope : IDisposable
        {
            private Stopwatch stopwatch;

            public TimeScope(Stopwatch stopwatch)
            {
                this.stopwatch = stopwatch;
                stopwatch.Start();
            }

            public void Dispose()
            {
                if (stopwatch != null)
                {
                    stopwatch.Stop();
                    stopwatch = null;
                }
            }
        }
    }
}