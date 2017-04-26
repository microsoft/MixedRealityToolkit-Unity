// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Structure that defines a timer. A timer can be scheduled through the TimerScheduler
    /// </summary>
    public struct Timer
    {
        public int Id;

        public static Timer Invalid = new Timer(0);

        public Timer(int id)
        {
            Id = id;
        }

        public bool IsActive
        {
            get
            {
                return (Id != Invalid.Id) && TimerScheduler.Instance.IsTimerActive(this);
            }
        }

        public static Timer Start(float timeSeconds, TimerScheduler.Callback callback, bool loop = false)
        {
            if (TimerScheduler.IsInitialized)
            {
                return TimerScheduler.Instance.StartTimer(timeSeconds, callback, loop);
            }

            return Timer.Invalid;
        }

        public static Timer StartNextFrame(TimerScheduler.Callback callback)
        {
            if (TimerScheduler.IsInitialized)
            {
                return TimerScheduler.Instance.StartTimer(0.0f, callback, false, true);
            }

            return Timer.Invalid;
        }

        public void Stop()
        {
            if (TimerScheduler.IsInitialized)
            {
                TimerScheduler.Instance.StopTimer(this);
                Id = Invalid.Id;
            }
        }
    }
}