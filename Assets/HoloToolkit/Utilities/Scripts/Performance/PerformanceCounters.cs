#if WINDOWS_UWP && !UNITY_EDITOR
#define STORE_APP_BINARY
#endif

using HoloToolkit.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if STORE_APP_BINARY
using System.Threading.Tasks;
using Windows.Storage;
#endif

namespace HoloToolkit.Unity
{
    public class PerformanceCounters : Singleton<PerformanceCounters>
    {
        public class TimeSample
        {
            private float timeSinceStart;
            private float frameTime;
            private float frameGpuTime;

            public float Time { get { return timeSinceStart; } }
            public float FrameTime { get { return frameTime; } }
            public float FrameGpuTime { get { return frameGpuTime; } }

            public TimeSample(float time, float frame, float gpu)
            {
                timeSinceStart = time;
                frameTime = frame;
                frameGpuTime = gpu;
            }

            public override string ToString()
            {
                return String.Format("{0:0.000}, {1:0.0000}, {2:0.0000}\r\n",
                    timeSinceStart, frameTime, frameGpuTime);
            }
        }

        public class TimeSampleStorage
        {
            public TimeSampleStorage()
            {
#if STORE_APP_BINARY
            storageFileName = String.Format("{0}-{1}.csv", SceneManager.GetActiveScene().name,
                DateTime.Now.ToString("yyyyMMdd-HHmmss"));
#endif
            }

            public bool FlushTimeSamples(IEnumerable<TimeSample> timeSamples)
            {
#if STORE_APP_BINARY
            if (backgroundFlush != null && !backgroundFlush.IsCompleted)
            {
                return false;
            }

            Debug.Log("[PerformanceCounters.TimeSampleStorage.FlushTimeSamples][Call async flush and saving task object]");
            backgroundFlush = FlushAsync(timeSamples);
#endif
                return true;
            }

            public void FlushTimeSamplesImmediate(IEnumerable<TimeSample> timeSamples)
            {
#if STORE_APP_BINARY
            if(backgroundFlush != null && !backgroundFlush.IsCompleted)
            {
                Debug.Log("[PerformanceCounters.TimeSampleStorage.FlushTimeSamplesImmediate][Waiting for taks to complete]");
                backgroundFlush.Wait();
            }

            backgroundFlush = FlushAsync(timeSamples);
            backgroundFlush.Wait();
#endif
            }

#if STORE_APP_BINARY
        private async Task FlushAsync(IEnumerable<TimeSample> timeSamples)
        {
            if(storage == null)
            {
                Debug.Log("[PerformanceCounters.TimeSampleStorage.FlushAsync][Creating file]");
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                storage = await storageFolder.CreateFileAsync(storageFileName, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(storage, "Time, FrameTime, FrameGpuTime\r\n");
            }

            await FileIO.AppendTextAsync(storage, String.Join("", timeSamples));
        }
#endif
#if STORE_APP_BINARY
        private string storageFileName = null;
        private Task backgroundFlush = null;
        private StorageFile storage = null;
#endif
        }

        public bool GpuTimeEnabled
        {
            get
            {
                return profileGpuTime;
            }

            set
            {
                profileGpuTime = value;
                if (!profileGpuTime)
                {
                    LastFrameGpuTime = 0;
                    gpuTimeAccumulator = 0;
                }
            }
        }

        /// <summary>
        /// The amount of time it took to complete the last frame
        /// </summary>
        public float LastFrameTime { get; private set; }
        /// <summary>
        /// The amount of time it took to render the last frame
        /// </summary>
        public float LastFrameGpuTime { get; private set; }

        /// <summary>
        /// The native frame rate as determined by the display refresh rate
        /// </summary>
        public int TargetFrameRate { get; private set; }

        [SerializeField]
        [Tooltip("The number of frames over which the performance counters are averaged")]
        private int averagingInvterval = 10;

        /// <summary>
        /// The number of samples that need to collect before saving performance
        /// data to the file
        /// </summary>
        private int saveDataInterval = 100;

        [SerializeField]
        [Tooltip("Save the performance date to a CSV file")]
        private bool dumpPerformanceData = false;

        [SerializeField]
        [Tooltip("Enable GPU timing performance counter")]
        private bool profileGpuTime = false;

        [SerializeField]
        [Tooltip("Main HMD camera. Needed for GpuTiming")]
        private Camera hmdCamera;

        private GpuTimingCamera gpuCamera;

        private float lastSampleStartTime = 0;
        private float lastFrameStartTime = 0;
        private float gpuTimeAccumulator = 0;
        private int currentFrame = 0;
        private int currentSample = 0;

        private TimeSampleStorage sampleStorage = null;
        private LinkedList<TimeSample> timeSamples = new LinkedList<TimeSample>();

        protected override void InitializeInternal()
        {
            TargetFrameRate = (int)UnityEngine.XR.XRDevice.refreshRate;

            if (TargetFrameRate == 0)
            {
                TargetFrameRate = 60;
                if (!Application.isEditor)
                {
                    Debug.LogWarning("Could not retrieve the HMD's native refresh rate. Assuming " + TargetFrameRate + " Hz.");
                }
            }

            if (null == hmdCamera)
            {
                hmdCamera = Camera.main;
            }

            //Make sure we have the GpuTimingCamera component attached to our camera with the correct timing tag
            gpuCamera = hmdCamera.GetComponent<GpuTimingCamera>();
            if (gpuCamera == null || gpuCamera.TimingTag.CompareTo("Frame") != 0)
            {
                gpuCamera = hmdCamera.gameObject.AddComponent<GpuTimingCamera>();
            }

            var currentTime = Time.realtimeSinceStartup;
            lastFrameStartTime = currentTime;
            lastSampleStartTime = currentTime;
            LastFrameTime = 0;
            LastFrameGpuTime = 0;

            if (dumpPerformanceData)
            {
                sampleStorage = new TimeSampleStorage();
            }
        }

        protected void Update()
        {
            var currentTime = Time.realtimeSinceStartup;
            LastFrameTime = currentTime - lastFrameStartTime;
            lastFrameStartTime = currentTime;

            if (profileGpuTime)
            {
                LastFrameGpuTime = gpuCamera.LastFrameTime;
            }

            if (!dumpPerformanceData)
            {
                return;
            }

            gpuTimeAccumulator += LastFrameGpuTime;
            currentFrame += 1;

            if (currentFrame == averagingInvterval)
            {
                // create the time sample and save
                var currentSampleTime = currentTime - lastSampleStartTime;
                var avgFrameTime = currentSampleTime / averagingInvterval;
                var avgFrameGpuTime = gpuTimeAccumulator / averagingInvterval;

                timeSamples.AddLast(new TimeSample(currentTime, avgFrameTime, avgFrameGpuTime));

                gpuTimeAccumulator = 0;
                currentFrame = 0;
                currentSample += 1;
                lastSampleStartTime = currentTime;
            }

            if (currentSample == saveDataInterval)
            {
                // flag the background thread to save the data to the disk

                if (sampleStorage.FlushTimeSamples(timeSamples))
                {
                    // if we got false from the call above it means a previous write
                    // operation did not get a chance to complete. We only create a new list
                    // if the background operation has completed. Otherwise we leave it as is
                    // and we'll attempt to write during the next interval
                    timeSamples = new LinkedList<TimeSample>();
                }

                currentSample = 0;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (dumpPerformanceData)
            {
                sampleStorage.FlushTimeSamplesImmediate(timeSamples);
            }
        }
    }
}
