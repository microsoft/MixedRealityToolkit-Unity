// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class PerformanceBucket
{
    [SerializeField]
    [Tooltip("Quality level value 0-max. Max depends on the defined Unity quality settings")]
    public int QualityLevel;
    [SerializeField]
    [Tooltip("Viewport scale value 0.05-1.0.")]
    public float ViewportScale;
    [SerializeField]
    [Tooltip("Shader level as consumed by ShaderControl component")]
    public int ShaderLevel = 0;
}

namespace HoloToolkit.Unity
{
    public class AdaptivePerformance : Singleton<AdaptivePerformance>
    {
        /// <summary>
        /// Event that is flagged when a performance bucket changes
        /// </summary>
        public class PerformanceBucketChangedEvent : UnityEvent<PerformanceBucket> { }

        /// <summary>
        /// Class that contains serializable fields to save the last used performance bucket.
        /// Next time the component initializes we read the last used performance bucket.
        /// The idea is that when adaptive performance finds good settings we use that
        /// value every time on the specific machine.
        /// </summary>
        [XmlRoot("AdaptivePerformanceSaveData")]
        public class SaveData
        {
            [XmlElement]
            public int BucketIndex = -1;

            public void Save()
            {
                try
                {
                    var serializer = new XmlSerializer(typeof(SaveData));
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        serializer.Serialize(stream, this);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("Deserializing saved bucket failed \r\n{0}", e.Message);
                }
            }

            public static SaveData Load()
            {
                XmlSerializer serializer = null;
                bool skipSerialization = false;
                // workaround for bug in IL2CPP where we get file not found for kernel32
                try
                {
                    serializer = new XmlSerializer(typeof(SaveData));
                }
                catch
                {
                    skipSerialization = true;
                }

                if (skipSerialization)
                {
                    return new SaveData();
                }

                try
                {
                    Debug.LogFormat("Save file name: {0}", filePath);
                    using (var stream = new FileStream(filePath, FileMode.Open))
                    {
                        return serializer.Deserialize(stream) as SaveData;
                    }
                }
                catch (FileNotFoundException)
                {
                    return new SaveData();
                }
            }

            private static string filePath = Path.Combine(Application.persistentDataPath, "AdaptivePerfSaveData.xml");
        }

        private AdaptiveAnalyzer analyzer = null;
        private AdaptiveAnalyzer.AnalyzerType _currentAnalyzerType = AdaptiveAnalyzer.AnalyzerType.FrameRateBased;

        public AdaptiveAnalyzer.AnalyzerType CurrentAnalyzerType
        {
            get { return _currentAnalyzerType; }
            set
            {
                _currentAnalyzerType = value;
                InitializeAnalyzer();
            }
        }

        public bool EnableDebugKeys = true;

        private bool _adaptivePerformanceEnabled = false;
        public bool AdaptivePerformanceEnabled
        {
            get { return _adaptivePerformanceEnabled; }
        }

        /// <summary>
        /// Instance of the event that clients can subscribe to and receive notifications
        /// when a performance bucket changes.
        /// to subscribe call OnPerformanceBucketChanged.AddListener(callbackFunc)
        /// when done call OnPerformanceBucketChanged.RemoveListener(callbackFunc) to unsubscribe.
        /// </summary>
        public PerformanceBucketChangedEvent OnPerformanceBucketChanged =
            new PerformanceBucketChangedEvent();

        /// <summary>
        /// The array below defines the list of available buckets for adaptive performance,
        /// for example:
        ///     private PerformanceBucket[] perfBucketList =
        ///     {
        ///         new PerformanceBucket()
        ///         {
        ///             QualityLevel = 0,
        ///             ViewportScale = 0.5f,
        ///             ShaderLevel = 1
        ///         },
        ///         new PerformanceBucket()
        ///         {
        ///             QualityLevel = 5,
        ///             ViewportScale = 1f,
        ///             ShaderLevel = 0
        ///         }
        ///     };
        ///
        /// By default the field will show in the Unity editor so you can
        /// define the buckets there. Alternative is to remote the SerializeField
        /// attribute below and hard code the files in this script. Values set in 
        /// the Unity editor take precedence over values defined in this script.
        /// </summary>
        [SerializeField]
        private PerformanceBucket[] perfBucketList;

        [SerializeField]
        private int startBucket = 0;
        private int currentBucket = 0;
        private SaveData saves;

        private float adaptivePerfStart = 0;
        private float adaptivePerfTimeout = 0;

        private void SwitchToBucket(int bucketId)
        {
            bucketId = Mathf.Clamp(bucketId, 0, perfBucketList.Length - 1);

            if (bucketId == currentBucket)
            {
                return;
            }

            ApplyBucketSettings(bucketId);
            currentBucket = bucketId;

            saves.BucketIndex = bucketId;
            saves.Save();
        }

        /// <summary>
        /// Public method for explicitly moving a bucket up (lower perf, higher quality).
        /// </summary>
        /// <returns>New bucket index</returns>
        public int SwitchToHigherBucket()
        {
            SwitchToBucket(currentBucket + 1);
            return currentBucket;
        }

        /// <summary>
        /// Public method for explicitly moving a bucket down (higher perf, lower quality).
        /// </summary>
        /// <returns>New bucket index</returns>
        public int SwitchToLowerBucket()
        {
            SwitchToBucket(currentBucket - 1);
            return currentBucket;
        }

        /// <summary>
        /// Public property for retrieving the current bucket index.
        /// </summary>
        public int CurrentBucketId
        {
            get
            {
                return currentBucket;
            }
        }

        /// <summary>
        /// Public method for starting adaptive performance management
        /// during adaptive performance, performance settings will automatically changed depending
        /// on the current performance. Bad performance moves to lower bucket, exceeded performance
        /// moves to higher bucket.
        /// Adaptive performance will run until StopAdaptivePerformance() is called.
        /// </summary>
        public void StartAdaptivePerformance()
        {
            StartAdaptivePerformance(0);
        }

        /// <summary>
        /// Public method for starting adaptive performance with a timeout.
        /// Adaptive performance stops running after the given amount of seconds.
        /// </summary>
        /// <param name="timeout">Adaptive performance stops running after the given amount of seconds</param>
        public void StartAdaptivePerformance(float timeout = 0.0f)
        {
            if (_adaptivePerformanceEnabled)
            {
                return;
            }

            ApplyBucketSettings(currentBucket);
            _adaptivePerformanceEnabled = true;

            if (timeout > 0)
            {
                adaptivePerfStart = Time.realtimeSinceStartup;
                adaptivePerfTimeout = timeout;
            }

            Debug.Log("Starting Adaptive Performance");
        }

        /// <summary>
        /// Public method for stopping adaptive performance management.
        /// </summary>
        public void StopAdaptivePerformance()
        {
            _adaptivePerformanceEnabled = false;
            adaptivePerfTimeout = 0;
            adaptivePerfStart = 0;

            Debug.Log("Stopping Adaptive Performance");
        }

        private void InitializeAnalyzer()
        {
            if (analyzer != null)
            {
                analyzer.OnDestroy();
            }

            if (_currentAnalyzerType == AdaptiveAnalyzer.AnalyzerType.FrameRateBased)
            {
                analyzer = new FrameRateAdaptiveAnalyzer();
            }
            else if (_currentAnalyzerType == AdaptiveAnalyzer.AnalyzerType.GpuTimeBased)
            {
                analyzer = new GpuTimeAdaptiveAnalyzer();
            }
            else
            {
                throw new ArgumentException("Unknown performance analyzer type");
            }
            analyzer.OnCreate();
        }

        protected override void InitializeInternal()
        {
            var performanceLevel = GpuWhitelist.EnsurePerformanceLevelFound();
            saves = SaveData.Load();
            if (saves.BucketIndex >= 0 && saves.BucketIndex < perfBucketList.Length)
            {
                currentBucket = saves.BucketIndex;
            }
            else if (performanceLevel != GpuWhitelist.SystemPerformanceLevel.Undefined)
            {
                switch (performanceLevel)
                {
                    /// Add starting buckets based on the classification of the GPU you are running on
                    /// Also feel free to adjust these and the whitelists lists based on what you find

                    //case GpuWhitelist.SystemPerformanceLevel.HighEndUltra:
                    //    currentBucket = startBucket + 1;
                    //    break;
                    //case GpuWhitelist.SystemPerformanceLevel.LowEndUltra:
                    //    currentBucket = startBucket - 1;
                    //    break;
                    //case GpuWhitelist.SystemPerformanceLevel.Mainstream:
                    //    currentBucket = startBucket - 2;
                    //    break;
                    case GpuWhitelist.SystemPerformanceLevel.Ultra:
                    default:
                        currentBucket = startBucket;
                        break;
                }
            }
            else if (startBucket >= 0 && startBucket < perfBucketList.Length)
            {
                currentBucket = startBucket;
            }
            else
            {
                currentBucket = 0;
            }

            // if we're in the editor use the value from the inspector as default
            if (Application.isEditor)
            {
                currentBucket = startBucket;
            }

            // Ensure there is a Quality Manager
            if (QualityManager.Instance == null)
            {
                Debug.LogWarning("No QualityManager found by adaptive performance adding one");
                gameObject.AddComponent<QualityManager>();
            }

            // Ensure there is a Viewport Scale Manager
            if (ViewportScaleManager.Instance == null)
            {
                Debug.LogWarning("No ViewportScaleManager found by adaptive performance adding one");
                gameObject.AddComponent<ViewportScaleManager>();
            }

            // Ensure the performance counters are initialized
            if (PerformanceCounters.Instance == null)
            {
                Debug.LogWarning("No PerformanceCounters found by adaptive performance adding one");
                gameObject.AddComponent<PerformanceCounters>();
            }

            if (_adaptivePerformanceEnabled)
            {
                ApplyBucketSettings(currentBucket);
            }

            InitializeAnalyzer();
        }

        protected override void Awake()
        {
            base.Awake();

            GpuWhitelist.EnsurePerformanceLevelFound();
        }

        protected void Update()
        {
            if (UpdateInput())
            {
                UpdatePerformance();
            }
        }


        /// <summary>
        /// This function applies all the settings specified in the bucket
        /// If a bucket is extended to include more parameters, changes to
        /// those should take effect in this function
        /// </summary>
        /// <param name="bucketId"></param>
        private void ApplyBucketSettings(int bucketId)
        {
            //Debug.LogFormat("Applying performance settings for bucket id {0}", bucketId);
            var perfParams = perfBucketList[bucketId];
            QualityManager.Instance.SaveQualityLevel(perfParams.QualityLevel);
            ViewportScaleManager.Instance.SetViewport(perfParams.ViewportScale);

            OnPerformanceBucketChanged.Invoke(perfParams);

            /// You may use and extend the following function to send telemetry,
            /// to logging bucket changes.  You can collect this info to update
            /// things like the GPUWhitelist classifications, and your bucket 
            /// parameters.
            // GpuWhitelist.LogSystemInfo(bucketId);
        }

        /// <summary>
        /// Public method for getting the performance parameters of the current bucket
        /// </summary>
        /// <returns>Bucket with current performance settings</returns>
        public PerformanceBucket GetCurrentBucket()
        {
            return perfBucketList[currentBucket];
        }

        private bool UpdateInput()
        {
            if (EnableDebugKeys)
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    if (_adaptivePerformanceEnabled)
                    {
                        StopAdaptivePerformance();
                    }
                    else
                    {
                        StartAdaptivePerformance();
                    }
                }

                if (Input.GetKeyDown(KeyCode.T))
                {
                    var newAnalyzerId = (int)_currentAnalyzerType + 1;
                    if (Enum.IsDefined(typeof(AdaptiveAnalyzer.AnalyzerType), newAnalyzerId))
                    {
                        CurrentAnalyzerType = (AdaptiveAnalyzer.AnalyzerType)newAnalyzerId;
                    }
                    else
                    {
                        CurrentAnalyzerType = 0;
                    }
                }

                if (Input.GetKeyDown(KeyCode.S))
                {
                    SwitchToLowerBucket();
                    return false;
                }

                if (Input.GetKeyDown(KeyCode.D))
                {
                    SwitchToHigherBucket();
                    return false;
                }

                if (Input.GetKeyDown(KeyCode.W))
                {
                    StartAdaptivePerformance(5);
                    return false;
                }
            }

            return true;
        }

        private void UpdatePerformance()
        {
            if (!_adaptivePerformanceEnabled)
            {
                return;
            }

            var result = analyzer.AdaptPerformance();

            if (result == AdaptiveAnalyzer.AnalyzerResult.BucketDown)
            {
                SwitchToLowerBucket();
            }
            else if (result == AdaptiveAnalyzer.AnalyzerResult.BucketUp)
            {
                SwitchToHigherBucket();
            }

            if ((adaptivePerfTimeout > 0) && (Time.realtimeSinceStartup - adaptivePerfStart > adaptivePerfTimeout))
            {
                StopAdaptivePerformance();
            }
        }
    }
}
