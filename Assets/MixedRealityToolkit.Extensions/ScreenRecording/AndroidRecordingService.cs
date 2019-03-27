// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System;

namespace Microsoft.MixedReality.Toolkit.Extensions.ScreenRecording
{
    /// <summary>
    /// Class implementing <see cref="IRecordingService"/> for the Android platform
    /// </summary>
    public class AndroidRecordingService : MonoBehaviour,
    IRecordingService
    {
        private readonly string _fileNamePrefix = "spectatorView";
        private readonly string _fileNameExt = ".mp4";
        private bool _initialized = false;
        private bool _recording = false;

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public void Initialize()
        {
            if (!_initialized)
            {
                try
                {
                    using (var screenRecorderActivity = GetScreenRecorderActivity())
                    {
                        var initialized = screenRecorderActivity.Call<bool>("Initialize", GetUniqueFileName());
                        if (!initialized)
                        {
                            Debug.Log("Failed to initialize AndroidRecordingService: " + GetLastErrorMessage());
                            _initialized = false;
                            return;
                        }

                        _initialized = true;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Failed to initialize AndroidRecordingService: " + e.ToString());
                }
            }
        }

        /// <inheritdoc />
        public bool IsInitialized()
        {
            if (_initialized)
            {
                try
                {
                    using (var screenRecorderActivity = GetScreenRecorderActivity())
                    {
                        return screenRecorderActivity.Call<bool>("IsInitialized");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Failed to assess whether AndroidRecordingService was initialized: " + e.ToString());
                }
            }

            return false;
        }

        /// <inheritdoc />
        public bool StartRecording()
        {
            if (_initialized && !_recording)
            {
                try
                {
                    using (var screenRecorderActivity = GetScreenRecorderActivity())
                    {
                        var success = screenRecorderActivity.Call<bool>("StartRecording");
                        if (!success)
                        {
                            Debug.LogError("Failed to start recording in AndroidRecordingService: " + GetLastErrorMessage());
                            _recording = false;
                            return false;
                        }

                        _recording = true;
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Failed to start recording in AndroidRecordingService: " + e.ToString());
                }
            }

            return false;
        }

        /// <inheritdoc />
        public void StopRecording()
        {
            if (_initialized && _recording)
            {
                try
                {
                    using (var screenRecorderActivity = GetScreenRecorderActivity())
                    {
                        var success = screenRecorderActivity.Call<bool>("StopRecording");
                        if (!success)
                        {
                            Debug.LogError("Failed to stop recording in AndroidRecordingService: " + GetLastErrorMessage());
                        }

                        _recording = false;
                        _initialized = false;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Failed to stop recording in AndroidRecordingService: " + e.ToString());
                }
            }
        }

        /// <inheritdoc />
        public bool IsRecordingAvailable()
        {
            try
            {
                using (var screenRecorderActivity = GetScreenRecorderActivity())
                {
                    return screenRecorderActivity.Call<bool>("IsRecordingAvailable");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to query whether recording was available for AndroidRecordingService: " + e.ToString());
            }

            return false;
        }

        /// <inheritdoc />
        public void ShowRecording()
        {
            try
            {
                using (var screenRecorderActivity = GetScreenRecorderActivity())
                {
                    screenRecorderActivity.Call<bool>("ShowRecording");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to show recording for AndroidRecordingService: " + e.ToString());
            }
        }

        /// <summary>
        /// Obtains a Unity wrapper object for the Android application's default activity
        /// </summary>
        /// <returns>Unity wrapper object for the Android application's default activity</returns>
        private AndroidJavaObject GetScreenRecorderActivity()
        {
            using (var classType = new AndroidJavaClass("java.lang.Class"))
            {
                using (var screenRecorderClass = classType.CallStatic<AndroidJavaObject>("forName", "com.Microsoft.MixedReality.Toolkit.ScreenRecorderActivity"))
                {
                    using (var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        return screenRecorderClass.Call<AndroidJavaObject>("cast", activity);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the last error message cached by the Android recording service
        /// </summary>
        /// <returns>last error message cached by the Android recording service</returns>
        private string GetLastErrorMessage()
        {
            try
            {
                using (var screenRecorderActivity = GetScreenRecorderActivity())
                {
                    return screenRecorderActivity.Call<string>("GetLastErrorMessage");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to initialize AndroidRecordingService: " + e.ToString());
            }

            return "Failed to obtain last error message";
        }

        /// <summary>
        /// Generates a unique file name based on time
        /// </summary>
        /// <returns>Unique file name based on time</returns>
        private string GetUniqueFileName()
        {
            var fileStamp = DateTime.Now.ToString("yyyy'_'MM'_'dd'_'HH':'mm':'ss");
            return _fileNamePrefix + "_" + fileStamp + _fileNameExt;
        }
    }
}
