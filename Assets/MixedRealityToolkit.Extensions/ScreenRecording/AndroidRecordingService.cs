// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System;

namespace Microsoft.MixedReality.Toolkit.Extensions.ScreenRecording
{
    public class AndroidRecordingService : MonoBehaviour,
    IRecordingService
    {
        const string _fileNamePrefix = "spectatorView";
        const string _fileNameExt = ".mp4";
        private bool _initialized = false;
        private bool _recording = false;

        public void Dispose()
        {
        }

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

        private String GetLastErrorMessage()
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

        private String GetUniqueFileName()
        {
            var fileStamp = DateTime.Now.ToString("yyyy'_'MM'_'dd'_'HH':'mm':'ss");
            return _fileNamePrefix + "_" + fileStamp + _fileNameExt;
        }
    }
}
