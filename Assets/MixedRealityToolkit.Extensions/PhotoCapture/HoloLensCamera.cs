// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

// We have this CAN_USE_UNITY_TYPES macro definition so that this file can serve as an example
// for developers building C# windows camera applications outside of Unity.
// Unity specific types should be contained behind this macro with suitable counterparts
// defined when this macro is not available
#define CAN_USE_UNITY_TYPES

// The WINDOWS_UWP macro allows references to WinRT APIs within Unity
#if WINDOWS_UWP
#define CAN_USE_UWP_TYPES
#endif

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#if CAN_USE_UWP_TYPES
using Windows.Media.Devices;
using Windows.Graphics.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using Windows.Perception.Spatial;
using System.Runtime.InteropServices;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;

using WindowsMatrix4x4 = System.Numerics.Matrix4x4;
using WindowsVector3 = System.Numerics.Vector3;
using WindowsQuaternion = System.Numerics.Quaternion;
#endif

#if CAN_USE_UNITY_TYPES
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Quaternion = UnityEngine.Quaternion;
using Debug = UnityEngine.Debug;
#endif

namespace Microsoft.MixedReality.Toolkit.Extensions.PhotoCapture
{
    /// <summary>
    /// The mode to run the camera in
    /// </summary>
    public enum CaptureMode
    {
        /// <summary>
        /// Standard photo mode.
        /// </summary>
        Single,
        /// <summary>
        /// Starts video stream and just grabs latest frame when call made to take photo.
        /// </summary>
        SingleLowLatency,
        /// <summary>
        /// All frames are streamed, uses standard video mode.
        /// </summary>
        Continuous,
    }

    /// <summary>
    /// The pixel format of a frame
    /// </summary>
    public enum PixelFormat
    {
        Invalid,
        BGRA8,
        RGBA8,
        L8,
        L16,
        NV12,
        YUY2,
    }

    /// <summary>
    /// Used for comparisons in stream selection
    /// </summary>
    public enum StreamCompare
    {
        /// <summary>
        /// Will select streams with the property greater than the passed in argument(s)
        /// </summary>
        GreaterThan,

        /// <summary>
        /// Will select streams with the property equal to the passed in argument(s)
        /// </summary>
        EqualTo,

        /// <summary>
        /// Will select streams with the property less than the passed in argument(s)
        /// </summary>
        LessThan,
    }

#if !CAN_USE_UNITY_TYPES
    public struct Vector2
    {
        public float x;
        public float y;

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public struct Vector3
    {
        public float x;
        public float y;
        public float z;

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public struct Vector4
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public Vector4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
    }

    public struct Matrix4x4
    {
        public Vector4 Column0;
        public Vector4 Column1;
        public Vector4 Column2;
        public Vector4 Column3;

        public Matrix4x4(Vector4 col0, Vector4 col1, Vector4 col2, Vector4 col3)
        {
            Column0 = col0;
            Column1 = col1;
            Column2 = col2;
            Column3 = col3;
        }
    }
#endif

    // Devices states for cameras
    public enum CameraState
    {
        Stopping,
        Initializing,
        Initialized,
        Starting,
        Ready,
        CapturingContinuous,
        CapturingSingle,
    }

    /// <summary>
    /// Handler delegate for capturing frames
    /// </summary>
    /// <param name="sender">The camera object that captured the frame</param>
    /// <param name="frame">The captured frame</param>
    public delegate void OnFrameCapturedHandler(HoloLensCamera sender, CameraFrame frame);

    /// <summary>
    /// Handler delegate for initialization complete callback.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="initializeSuccessful"></param>
    public delegate void OnCameraInitializedHandler(HoloLensCamera sender, bool initializeSuccessful);

    /// <summary>
    /// Handler delegate for camera start complete callback.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="startSuccessful"></param>
    public delegate void OnCameraStartedHandler(HoloLensCamera sender, bool startSuccessful);

    /// <summary>
    /// Exposes functionality for all of HoloLens's cameras.
    /// </summary>
    public class HoloLensCamera : IDisposable
    {
        private const double epsilon = 0.00001;

#if CAN_USE_UWP_TYPES
        private class StreamDescriptionInternal : StreamDescription
        {
            public MediaFrameSourceInfo FrameSourceInfo;
            public MediaFrameSourceGroup FrameSourceGroup;
        }

        private class PixelHelpers
        {
            public static int GetDataSize(int width, int height, PixelFormat format)
            {
                int dataSize = -1;

                switch (format)
                {
                    case PixelFormat.BGRA8:
                    case PixelFormat.RGBA8:
                        {
                            dataSize = width * height * 4;
                            break;
                        }
                    case PixelFormat.NV12:
                        {
                            dataSize = (width * height * 6) / 4;
                            break;
                        }
                    case PixelFormat.YUY2:
                        {
                            dataSize = width * height * 2;
                            break;
                        }
                    case PixelFormat.L8:
                        {
                            dataSize = width * height;
                            break;
                        }
                    case PixelFormat.L16:
                        {
                            dataSize = width * height * 2;
                            break;
                        }
                }

                return dataSize;
            }

            public static PixelFormat ConvertFormat(BitmapPixelFormat bmpFormat)
            {
                PixelFormat pixelFormat = PixelFormat.Invalid;

                switch (bmpFormat)
                {
                    case BitmapPixelFormat.Nv12:
                        {
                            pixelFormat = PixelFormat.NV12;
                            break;
                        }
                    case BitmapPixelFormat.Yuy2:
                        {
                            pixelFormat = PixelFormat.YUY2;
                            break;
                        }
                    case BitmapPixelFormat.Bgra8:
                        {
                            pixelFormat = PixelFormat.BGRA8;
                            break;
                        }
                    case BitmapPixelFormat.Rgba8:
                        {
                            pixelFormat = PixelFormat.RGBA8;
                            break;
                        }
                    case BitmapPixelFormat.Gray8:
                        {
                            pixelFormat = PixelFormat.L8;
                            break;
                        }
                    case BitmapPixelFormat.Gray16:
                        {
                            pixelFormat = PixelFormat.L16;
                            break;
                        }
                }

                return pixelFormat;
            }

            public static BitmapPixelFormat ConvertFormat(PixelFormat format)
            {
                BitmapPixelFormat bmpFormat = BitmapPixelFormat.Unknown;

                switch (format)
                {
                    case PixelFormat.NV12:
                        {
                            bmpFormat = BitmapPixelFormat.Nv12;
                            break;
                        }
                    case PixelFormat.YUY2:
                        {
                            bmpFormat = BitmapPixelFormat.Yuy2;
                            break;
                        }
                    case PixelFormat.BGRA8:
                        {
                            bmpFormat = BitmapPixelFormat.Bgra8;
                            break;
                        }
                    case PixelFormat.RGBA8:
                        {
                            bmpFormat = BitmapPixelFormat.Rgba8;
                            break;
                        }
                    case PixelFormat.L8:
                        {
                            bmpFormat = BitmapPixelFormat.Gray8;
                            break;
                        }
                    case PixelFormat.L16:
                        {
                            bmpFormat = BitmapPixelFormat.Gray16;
                            break;
                        }
                }

                return bmpFormat;
            }
        }

        private class CameraFrameInternal : CameraFrame
        {
            /// <summary>
            /// This is the buffer that the SoftwareBitmap copies through.
            /// </summary>
            public IBuffer PixelDataBuffer;

            /// <summary>
            /// The pool that owns this camera frame
            /// </summary>
            private CameraFramePool pool;

            /// <summary>
            /// Create a new camera frame and set the pool that owns this camera frame
            /// </summary>
            /// <param name="pool"></param>
            public CameraFrameInternal(CameraFramePool pool)
            {
                this.pool = pool;
            }

            /// <summary>
            /// Called when finished with the frame to release back to the pool.
            /// </summary>
            public override void Release()
            {
                base.Release();

                if (refCount <= 0 && pool != null)
                {
                    // return this to the pool. 
                    pool.ReleaseFrame(this);
                }
            }
        }

        private class CameraFramePool
        {
            public static CameraFramePool Instance { get; } = new CameraFramePool();

            /// <summary>
            /// List of camera frames that are not currently being used.
            /// </summary>
            private LinkedList<CameraFrameInternal> freeCameraFrames;

            /// <summary>
            /// List of camera frames that are currently in use.
            /// </summary>
            private LinkedList<CameraFrameInternal> usedCameraFrames;

            /// <summary>
            /// Lock to synchronize access to the frame pool.
            /// </summary>
            private readonly object frameLock = new object();

            public CameraFramePool()
            {
                freeCameraFrames = new LinkedList<CameraFrameInternal>();
                usedCameraFrames = new LinkedList<CameraFrameInternal>();
            }

            /// <summary>
            /// This acquires a new CameraFrame from the pool and copies the pixel data through the internal IBuffer.
            /// </summary>
            /// <param name="bmpFrame">The software bitmap from the camera</param>
            /// <returns>The camera frame (with the pixel data copied already)</returns>
            public CameraFrameInternal AcquireFrame(SoftwareBitmap bmpFrame, PixelFormat desiredPixelFormat)
            {
                lock (frameLock)
                {
                    CameraFrameInternal frame = null;

                    // convert the data format and get the data size - 
                    PixelFormat pixelFormat = PixelHelpers.ConvertFormat(bmpFrame.BitmapPixelFormat);
                    if (pixelFormat != desiredPixelFormat)
                    {
                        BitmapPixelFormat bitmapPixelFormat = PixelHelpers.ConvertFormat(desiredPixelFormat);
                        bmpFrame = SoftwareBitmap.Convert(bmpFrame, PixelHelpers.ConvertFormat(desiredPixelFormat));
                    }

                    int dataSize = PixelHelpers.GetDataSize(bmpFrame.PixelWidth, bmpFrame.PixelHeight, desiredPixelFormat);

                    if (freeCameraFrames.Count > 0)
                    {
                        // find a frame with the required size buffer
                        for (LinkedListNode<CameraFrameInternal> node = freeCameraFrames.First; node != null; node = node.Next)
                        {
                            // check if the pixel data array is the correct length
                            if (node.Value != null && node.Value?.PixelData?.Length == dataSize)
                            {
                                // remove the frame from the free list
                                freeCameraFrames.Remove(node);

                                // add the frame to the used list (need to create a new node)
                                frame = node.Value;
                                usedCameraFrames.AddFirst(frame);
                            }
                        }
                    }

                    // if there were no free frames, create a new one
                    if (frame == null)
                    {
                        frame = new CameraFrameInternal(this);
                        frame.PixelData = new byte[dataSize];
                        frame.PixelDataBuffer = frame.PixelData.AsBuffer();

                        // add the new frame to the used camera frames list
                        usedCameraFrames.AddFirst(frame);
                    }

                    // copy the pixel data to the frame through the internal IBuffer
                    bmpFrame.CopyToBuffer(frame.PixelDataBuffer);

                    return frame;
                }
            }

            public void ReleaseFrame(CameraFrameInternal frame)
            {
                lock (frameLock)
                {
                    // remove the frame from the used list and add to the free list
                    if (frame != null)
                    {
                        LinkedListNode<CameraFrameInternal> frameNode = usedCameraFrames.Find(frame);

                        // if the frame was found, add to the free pile and remove from the used list
                        if (frameNode != null)
                        {
                            // need to create a new node as nodes don't swap between linked lists
                            freeCameraFrames.AddFirst(frameNode.Value);

                            // remove from the current list
                            usedCameraFrames.Remove(frameNode);
                        }
                    }
                }
            }

            /// <summary>
            /// Clear all frames in the free list
            /// </summary>
            public void TrimFrames()
            {
                lock (frameLock)
                {
                    freeCameraFrames.Clear();
                }
            }
        }
#endif

        /// <summary>
        /// The capture mode of this camera
        /// </summary>
        public CaptureMode CaptureMode { get; protected set; }

        /// <summary>
        /// All native resolutions supported in the current camera mode
        /// </summary>
        public StreamSelector StreamSelector { get; private set; }

        /// <summary>
        /// Specifies whether to keep a reference to the camera frames SoftwareBitmap for future operations.
        /// </summary>
        public bool KeepSoftwareBitmap { get; set; }

        /// <summary>
        /// Current exposure duration in seconds. 
        /// </summary>
        public double Exposure
        {
            get
            {
                double exposure = 0.0;
#if CAN_USE_UWP_TYPES
                if (videoDeviceController != null && videoDeviceController.Exposure != null)
                {
                    double exponent = 0.0;
                    videoDeviceController.Exposure.TryGetValue(out exponent);

                    var a = videoDeviceController.AdvancedPhotoControl;

                    double bcv;
                    var bc = videoDeviceController.BacklightCompensation.TryGetValue(out bcv);

                    double bv;
                    var b = videoDeviceController.Brightness.TryGetValue(out bv);

                    double cv;
                    var c = videoDeviceController.Contrast.TryGetValue(out cv);

                    var dO = videoDeviceController.DesiredOptimization;

                    double fv;
                    var f = videoDeviceController.Focus.TryGetValue(out fv);

                    exposure = Math.Pow(2.0, exponent);
                }
#endif
                return exposure;
            }

            set
            {
#if CAN_USE_UWP_TYPES
                // if manual exposure is not supported, throw exception
                if (ManualExposureSupported)
                {
                    if (AutoExposure)
                    {
                        AutoExposure = false;
                    }

                    // get exposure properties and find closest value
                    double min = videoDeviceController.Exposure.Capabilities.Min;
                    double max = videoDeviceController.Exposure.Capabilities.Max;
                    double step = videoDeviceController.Exposure.Capabilities.Step;

                    // get exponent
                    double exponent = Math.Log(value, 2.0);

                    // clamp between min and max
                    double val = Clamp((float)exponent, (float)min, (float)max);

                    val = (double)((int)((val - min) / step) * step + min);

                    bool worked = videoDeviceController.Exposure.TrySetValue(val);
                    System.Diagnostics.Debug.WriteLine($"Setting exposure worked = {worked}");
                }
                else
                {
                    throw new NotSupportedException("Manual exposure is not available for this camera");
                }
#endif
            }
        }

        /// <summary>
        /// Maximum exposure value
        /// </summary>
        public double MinExposure
        {
            get
            {
#if CAN_USE_UWP_TYPES
                return Math.Pow(2.0, videoDeviceController.Exposure.Capabilities.Min);
#else
                return 0.0;
#endif
            }
        }

        /// <summary>
        /// Minimum Exposure value
        /// </summary>
        public double MaxExposure
        {
            get
            {
#if CAN_USE_UWP_TYPES
                return Math.Pow(2.0, videoDeviceController.Exposure.Capabilities.Max);
#else
                return 0.0;
#endif
            }
        }

        /// <summary>
        /// True if exposure is automatic, false if exposure is manually set through Exposure property.
        /// </summary>
        public bool AutoExposure
        {
            get
            {
                bool isAuto = true;
#if CAN_USE_UWP_TYPES
                if (videoDeviceController != null && videoDeviceController.Exposure != null)
                {
                    videoDeviceController.Exposure.TryGetAuto(out isAuto);
                }

                bool canfocus = videoDeviceController.Focus.Capabilities.Supported;
#endif
                return isAuto;
            }

            set
            {
#if CAN_USE_UWP_TYPES
                if (ManualExposureSupported)
                {
                    videoDeviceController.Exposure.TrySetAuto(value);
                }
#endif
            }
        }

        /// <summary>
        /// Relative brightness value of the camera image. Range is 0 to 1.
        /// </summary>
        public double Brightness
        {
            get
            {
                // remap the internal brightness from 0 to 1
                double brightness = 0.0;
#if CAN_USE_UWP_TYPES
                if (videoDeviceController.Brightness.TryGetValue(out brightness))
                {
                    double min = videoDeviceController.Brightness.Capabilities.Min;
                    double max = videoDeviceController.Brightness.Capabilities.Max;
                    brightness = (brightness - min) / (max - min);
                }
#endif
                return brightness;
            }

            set
            {
#if CAN_USE_UWP_TYPES
                double min = videoDeviceController.Brightness.Capabilities.Min;
                double max = videoDeviceController.Brightness.Capabilities.Max;

                double brightness = value * (max - min) + min;

                videoDeviceController.Brightness.TrySetValue(brightness);
#endif
            }
        }

        /// <summary>
        /// Relative contrast value of the camera image. Range is 0 to 1.
        /// </summary>
        public double Contrast
        {
            get
            {
                // remap the internal contrast from 0 to 1
                double contrast = 0.0;
#if CAN_USE_UWP_TYPES
                if (videoDeviceController.Contrast.TryGetValue(out contrast))
                {
                    double min = videoDeviceController.Contrast.Capabilities.Min;
                    double max = videoDeviceController.Contrast.Capabilities.Max;
                    contrast = (contrast - min) / (max - min);
                }
#endif

                return contrast;
            }

            set
            {
#if CAN_USE_UWP_TYPES
                double min = videoDeviceController.Contrast.Capabilities.Min;
                double max = videoDeviceController.Contrast.Capabilities.Max;

                double contrast = value * (max - min) + min;

                videoDeviceController.Contrast.TrySetValue(contrast);
#endif
            }
        }

        /// <summary>
        /// True if manual exposure is supported
        /// </summary>
        public bool ManualExposureSupported
        {
            get
            {
                bool manualAvailable = false;
#if CAN_USE_UWP_TYPES
                if (videoDeviceController != null && videoDeviceController.Exposure != null && videoDeviceController.Exposure.Capabilities != null)
                {
                    manualAvailable = videoDeviceController.Exposure.Capabilities.Supported;
                }
#endif
                return manualAvailable;
            }
        }

        /// <summary>
        /// Current camera gain setting. This is a 0 to 1 value that linearly maps to ISO values usable by the device. 
        /// Note: Setting the value is a non-awaited asynchronous operation, so a get immediately after a set may not 
        /// result in the same value.
        /// </summary>
        public float Gain
        {
            get
            {
                float gain = 0.0f;
#if CAN_USE_UWP_TYPES
                if (videoDeviceController != null && videoDeviceController.IsoSpeedControl != null && videoDeviceController.IsoSpeedControl.Supported)
                {
                    uint minIso = videoDeviceController.IsoSpeedControl.Min;
                    uint maxIso = videoDeviceController.IsoSpeedControl.Max;
                    uint iso = videoDeviceController.IsoSpeedControl.Value;

                    gain = (iso - minIso) / (maxIso - minIso);
                }
#endif
                return gain;
            }

            set
            {
#if CAN_USE_UWP_TYPES
                if (videoDeviceController != null && videoDeviceController.IsoSpeedControl != null && videoDeviceController.IsoSpeedControl.Supported)
                {
                    double gain = value;

                    uint minIso = videoDeviceController.IsoSpeedControl.Min;
                    uint maxIso = videoDeviceController.IsoSpeedControl.Max;

                    uint iso = (uint)(gain * (double)(maxIso - minIso) + minIso);
                    SetIsoSpeedControl(iso);
                }
#endif
            }
        }

#if CAN_USE_UWP_TYPES
        private async void SetIsoSpeedControl(uint iso)
        {
            await videoDeviceController.IsoSpeedControl.SetValueAsync(iso);
        }
#endif

        /// <summary>
        /// True if automatic gain adjustment, false if gain is manually set through Gain property.
        /// Note: Setting the value is a non-awaited asynchronous operation, so a get immediately after a set may not 
        /// result in the same value.
        /// </summary>
        public bool AutoGain
        {
            get
            {
                bool isAutoGain = true;
#if CAN_USE_UWP_TYPES
                if (ManualGainSupported)
                {
                    isAutoGain = videoDeviceController.IsoSpeedControl.Auto;
                }
#endif
                return isAutoGain;
            }

            set
            {
#if CAN_USE_UWP_TYPES
                if (ManualGainSupported)
                {
                    if (value)
                    {
                        // TODO: As this is async, there is a possibility to get out of sync with this.
                        SetAutoIsoSpeed();
                    }
                    else
                    {
                        // Set to manual by setting value to current value. 
                        // TODO: As this is async, there is  a possibility to get out of sync with this.
                        SetIsoSpeedControl(videoDeviceController.IsoSpeedControl.Value);
                    }
                }
#endif
            }
        }

#if CAN_USE_UWP_TYPES
        private async void SetAutoIsoSpeed()
        {
            await videoDeviceController.IsoSpeedControl.SetAutoAsync();
        }
#endif

        /// <summary>
        /// Returns whether the camera gain can be set manually or not.
        /// </summary>
        public bool ManualGainSupported
        {
            get
            {
                bool manualGainSupported = false;
#if CAN_USE_UWP_TYPES
                if (videoDeviceController != null && videoDeviceController.IsoSpeedControl != null)
                {
                    manualGainSupported = videoDeviceController.IsoSpeedControl.Supported;
                }
#endif
                return manualGainSupported;
            }
        }

        /// <summary>
        /// Current state of the camera
        /// </summary>
        public CameraState State { get; private set; }

        /// <summary>
        /// Type of the camera
        /// </summary>
        internal CameraType CameraType { get; private set; }

#if !(CAN_USE_UWP_TYPES)
// These events are unused when running in the editor
#pragma warning disable 0067
#endif
        /// <summary>
        /// Callback to register for frame captured events
        /// </summary>
        public event OnFrameCapturedHandler OnFrameCaptured;

        /// <summary>
        /// Callback to register for camera initialized events
        /// </summary>
        public event OnCameraInitializedHandler OnCameraInitialized;

        /// <summary>
        /// Callback to register for camera started events
        /// </summary>
        public event OnCameraStartedHandler OnCameraStarted;
#if !(CAN_USE_UWP_TYPES)
#pragma warning restore 0067
#endif

#if CAN_USE_UWP_TYPES
        private MediaCapture mediaCapture;
        private MediaFrameReader frameReader;
        private VideoDeviceController videoDeviceController;
        private SpatialCoordinateSystem rootCoordinateSystem;
        private CameraFramePool framePool = CameraFramePool.Instance;
        private CameraFrame latestFrame;
#endif
        private Object stateLock = new Object();
        private PixelFormat desiredPixelFormat;
        public CameraResolution Resolution { get; private set; }

        public HoloLensCamera(CaptureMode captureMode, PixelFormat pixelFormat = PixelFormat.BGRA8)
        {
            desiredPixelFormat = pixelFormat;
            CameraType = CameraType.Invalid;
            CaptureMode = captureMode;

#if CAN_USE_UNITY_TYPES && UNITY_WSA && CAN_USE_UWP_TYPES
            IntPtr coordinateSystemPtr;
            // this must be done from the main thread, so done in 
            coordinateSystemPtr = UnityEngine.XR.WSA.WorldManager.GetNativeISpatialCoordinateSystemPtr();
            if (coordinateSystemPtr != null)
            {
                rootCoordinateSystem = Marshal.GetObjectForIUnknown(coordinateSystemPtr) as SpatialCoordinateSystem;
            }
#endif
        }

        /// <summary>
        /// Start the video stream. This just prepares the stream for capture, and doesn't start collecting frames
        /// </summary>
        /// <param name="streamDesc">The description of the stream to start.</param>
        public async void Start(StreamDescription streamDesc)
        {
#if CAN_USE_UWP_TYPES
            lock (stateLock)
            {
                if (State != CameraState.Initialized)
                {
                    throw new InvalidOperationException("Start cannot be called until the camera is in the Initialized state");
                }

                State = CameraState.Starting;
            }

            Resolution = streamDesc.Resolution;
            CameraType = streamDesc.CameraType;

            StreamDescriptionInternal desc = streamDesc as StreamDescriptionInternal;

            MediaCaptureInitializationSettings initSettings = new MediaCaptureInitializationSettings()
            {
                SourceGroup = desc.FrameSourceGroup,
                SharingMode = MediaCaptureSharingMode.ExclusiveControl,
                MemoryPreference = MediaCaptureMemoryPreference.Cpu,
                StreamingCaptureMode = StreamingCaptureMode.Video
            };

            // initialize the media device
            mediaCapture = new MediaCapture();

            try
            {
                await mediaCapture.InitializeAsync(initSettings);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MediaCapture initialization failed: {ex.Message}");
                mediaCapture.Dispose();
                mediaCapture = null;
            }

            if (mediaCapture != null)
            {
                // get access to the video device controller for property settings
                videoDeviceController = mediaCapture.VideoDeviceController;

                // choose media source
                MediaFrameSource frameSource = mediaCapture.FrameSources[desc.FrameSourceInfo.Id];
                MediaFrameFormat preferredFormat = null;

                foreach (MediaFrameFormat format in frameSource.SupportedFormats)
                {
                    if (format.VideoFormat.Width == desc.Resolution.Width && format.VideoFormat.Height == desc.Resolution.Height && Math.Abs((double)format.FrameRate.Numerator / (double)format.FrameRate.Denominator - desc.Resolution.Framerate) < epsilon)
                    {
                        preferredFormat = format;
                        break;
                    }
                }

                if (preferredFormat != null && preferredFormat != frameSource.CurrentFormat)
                {
                    await frameSource.SetFormatAsync(preferredFormat);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"failed to set desired frame format");
                }

                // set up frame readercapture frame data
                frameReader = await mediaCapture.CreateFrameReaderAsync(frameSource);
                frameReader.FrameArrived += OnMediaFrameArrived;
                await frameReader.StartAsync();

                lock (stateLock)
                {
                    State = CameraState.Ready;
                    OnCameraStarted?.Invoke(this, true);
                }
            }
            else
            {
                lock (stateLock)
                {
                    // drop back to initialized when the camera doesn't initialize
                    State = CameraState.Initialized;
                    OnCameraStarted?.Invoke(this, false);
                }
            }
#else
            await Task.CompletedTask;
#endif
        }

        /// <summary>
        /// Stop the camera. This will release resources and 
        /// </summary>
        public async void Stop()
        {
#if CAN_USE_UWP_TYPES
            bool isStopping = false;

            lock (stateLock)
            {
                if (State == CameraState.CapturingContinuous ||
                    State == CameraState.CapturingSingle ||
                    State == CameraState.Ready ||
                    State == CameraState.Starting)
                {
                    // TODO: may have to do something about being in the starting state. i.e. wait until started, or perhaps cancel remaining starting operations.
                    State = CameraState.Stopping;
                    isStopping = true;
                }
            }

            if (isStopping)
            {
                frameReader.FrameArrived -= OnMediaFrameArrived;
                await frameReader.StopAsync();
            }
#else
            await Task.CompletedTask;
#endif
        }

        /// <summary>
        /// Take a single exposure. The camera must be in the ready state for this call to work. All types 
        /// of cameras can grab a single frame if not recording continuous
        /// </summary>
        /// <returns></returns>
        public bool TakeSingle()
        {
            bool success = false;
#if CAN_USE_UWP_TYPES
            lock (stateLock)
            {
                if (State == CameraState.Ready)
                {
                    State = CameraState.CapturingSingle;
                    success = true;
                }
            }
#endif
            return success;
        }

        /// <summary>
        /// Starts continuous capture
        /// </summary>
        /// <returns>Returns true if starting continuous capture succeeded</returns>
        public bool StartContinuousCapture()
        {
            bool success = false;
#if CAN_USE_UWP_TYPES
            // TODO: support continuous capture of all frames for all capture modes?
            if (CaptureMode == CaptureMode.Continuous)
            {
                lock (stateLock)
                {
                    // check current state supports going to continuous.
                    if (State == CameraState.Ready || State == CameraState.CapturingSingle)
                    {
                        State = CameraState.CapturingContinuous;
                        success = true;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Unable to set camera in continuous capture mode");
                    }
                }
            }
#endif
            return success;
        }

        /// <summary>
        /// Stops continuous capture
        /// </summary>
        public void StopContinuousCapture()
        {
#if CAN_USE_UWP_TYPES
            lock (stateLock)
            {
                if (CaptureMode == CaptureMode.Continuous && State == CameraState.CapturingContinuous)
                {
                    State = CameraState.Ready;
                }
            }
#endif
        }

        /// <summary>
        /// Initializes the camera
        /// </summary>
        /// <returns></returns>
        public async Task Initialize()
        {
            lock (stateLock)
            {
                State = CameraState.Initializing;
            }

#if CAN_USE_UWP_TYPES
            try
            {
                var frameSourceGroups = await MediaFrameSourceGroup.FindAllAsync();

                StreamSelector = new StreamSelector();

                foreach (var sourceGroup in frameSourceGroups)
                {
                    string name = sourceGroup.DisplayName;
                    string id = sourceGroup.Id;

                    foreach (var sourceInfo in sourceGroup.SourceInfos)
                    {
                        switch (CaptureMode)
                        {
                            case CaptureMode.Continuous:
                            case CaptureMode.SingleLowLatency:
                                {
                                    if ((sourceInfo.MediaStreamType == MediaStreamType.VideoRecord || sourceInfo.MediaStreamType == MediaStreamType.VideoPreview) && sourceInfo.SourceKind == MediaFrameSourceKind.Color)
                                    {
                                        foreach (var setting in sourceInfo.VideoProfileMediaDescription)
                                        {
                                            StreamDescriptionInternal desc = new StreamDescriptionInternal()
                                            {
                                                SourceName = sourceInfo.DeviceInformation.Name,
                                                SourceId = sourceInfo.Id,
                                                Resolution = new CameraResolution() { Width = setting.Width, Height = setting.Height, Framerate = setting.FrameRate },
                                                FrameSourceInfo = sourceInfo,
                                                FrameSourceGroup = sourceGroup,
                                                CameraType = GetCameraType(sourceInfo.SourceKind)
                                            };

                                            StreamSelector.AddStream(desc);
                                        }
                                    }

                                    break;
                                }
                            case CaptureMode.Single:
                                {
                                    if (sourceInfo.MediaStreamType == MediaStreamType.Photo && sourceInfo.SourceKind == MediaFrameSourceKind.Color)
                                    {
                                        foreach (var setting in sourceInfo.VideoProfileMediaDescription)
                                        {
                                            StreamDescriptionInternal desc = new StreamDescriptionInternal()
                                            {
                                                SourceName = sourceInfo.DeviceInformation.Name,
                                                SourceId = sourceInfo.Id,
                                                Resolution = new CameraResolution() { Width = setting.Width, Height = setting.Height, Framerate = setting.FrameRate },
                                                FrameSourceInfo = sourceInfo,
                                                FrameSourceGroup = sourceGroup,
                                                CameraType = GetCameraType(sourceInfo.SourceKind)
                                            };

                                            StreamSelector.AddStream(desc);
                                        }
                                    }

                                    break;
                                }
                        }
                    }
                }

                lock (stateLock)
                {
                    State = CameraState.Initialized;
                    OnCameraInitialized?.Invoke(this, true);
                }
            }
            catch
            {
                OnCameraInitialized?.Invoke(this, false);
            }
#else
            await Task.CompletedTask;
#endif
        }

#if CAN_USE_UWP_TYPES
        private CameraType GetCameraType(MediaFrameSourceKind sourceKind)
        {
            CameraType cameraType = CameraType.Infrared;

            switch (sourceKind)
            {
                case MediaFrameSourceKind.Image:
                case MediaFrameSourceKind.Color:
                    {
                        cameraType = CameraType.Color;
                        break;
                    }
                case MediaFrameSourceKind.Depth:
                    {
                        cameraType = CameraType.Depth;
                        break;
                    }
                case MediaFrameSourceKind.Infrared:
                    {
                        cameraType = CameraType.Infrared;
                        break;
                    }
            }

            return cameraType;
        }

        private CameraFrameInternal GetFrameFromMediaFrameReader(MediaFrameReader frameReader)
        {
            // get the latest frame
            MediaFrameReference frameReference = frameReader?.TryAcquireLatestFrame();
            VideoMediaFrame videoFrame = frameReference?.VideoMediaFrame;
            SoftwareBitmap frameBmp = videoFrame?.SoftwareBitmap;

            CameraFrameInternal frame = null;

            if (frameBmp != null)
            {
                // get a camera frame and populate with the correct data for this frame - acquire copies the bitmap to the frame
                frame = framePool.AcquireFrame(frameBmp, desiredPixelFormat);
                frame.PixelFormat = desiredPixelFormat;
                frame.Resolution = Resolution;
                frame.FrameTime = frameReference.SystemRelativeTime.HasValue ? frameReference.SystemRelativeTime.Value.TotalSeconds : 0.0;
                frame.Exposure = frameReference.Duration.TotalSeconds;
                frame.Gain = Gain;

                if (KeepSoftwareBitmap)
                {
                    frame.SoftwareBitmap = frameBmp;
                }
                else
                {
                    frameBmp.Dispose();
                }

                // extrinsics and intrinsics
                frame.Extrinsics = GetExtrinsics(frameReference.CoordinateSystem);
                frame.Intrinsics = ConvertIntrinsics(frameReference.VideoMediaFrame.CameraIntrinsics);
            }

            frameReference?.Dispose();

            return frame;
        }

        /// <summary>
        /// Sets the spatial coordinate system of the world root. All camera extrinsics are relative to this. 
        /// Unity applications will default to WorldManager.GetNativeISpatialCoordinateSystemPtr()
        /// </summary>
        /// <param name="rootCoordinateSystem">The SpatialCoordinateSystem to use as the world root</param>
        public void SetCoordinateSystem(SpatialCoordinateSystem rootCoordinateSystem)
        {
            this.rootCoordinateSystem = rootCoordinateSystem;
        }

        private static Vector3 WindowsVectorToUnityVector(WindowsVector3 v)
        {
            return new Vector3(v.X, v.Y, -v.Z);
        }

        private CameraExtrinsics GetExtrinsics(SpatialCoordinateSystem frameCoordinateSystem)
        {
            if (frameCoordinateSystem == null)
            {
                return null;
            }

            CameraExtrinsics extrinsics = null;

            if (rootCoordinateSystem == null)
            {
                return null;
            }

            System.Numerics.Matrix4x4? worldMatrix = frameCoordinateSystem.TryGetTransformTo(rootCoordinateSystem);

            if (worldMatrix.HasValue)
            {
                WindowsVector3 position;
                WindowsVector3 scale;
                WindowsQuaternion rotation;
                WindowsMatrix4x4.Decompose(worldMatrix.Value, out scale, out rotation, out position);

                WindowsVector3 forward = WindowsVector3.Transform(-WindowsVector3.UnitZ, rotation);
                WindowsVector3 up = WindowsVector3.Transform(WindowsVector3.UnitY, rotation);

                Matrix4x4 unityWorldMatrix = Matrix4x4.TRS(WindowsVectorToUnityVector(position), Quaternion.LookRotation(WindowsVectorToUnityVector(forward), WindowsVectorToUnityVector(up)), Vector3.one);

                extrinsics = new CameraExtrinsics()
                {
                    ViewFromWorld = unityWorldMatrix
                };
            }

            return extrinsics;
        }

        private void OnMediaFrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            bool getFrame = false;

            lock (stateLock)
            {
                getFrame = (State == CameraState.CapturingContinuous || State == CameraState.CapturingSingle);

                // quickly handle state changes here (only changes if we're capturing a single image)
                if (State == CameraState.CapturingSingle)
                {
                    State = CameraState.Ready;
                }
            }

            CameraFrameInternal frame = null;

            if (getFrame)
            {
                frame = GetFrameFromMediaFrameReader(sender);
            }

            if (frame != null)
            {
                // clean up state after frame
                if (CaptureMode == CaptureMode.SingleLowLatency)
                {
                    // release previous frame and addref current frame
                    if (latestFrame != null)
                    {
                        latestFrame.Release();
                    }

                    // add ref for the camera
                    frame.AddRef();
                    latestFrame = frame;
                }

                // callback any registered listeners
                OnFrameCaptured?.Invoke(this, frame);
            }
        }

        private CameraIntrinsics ConvertIntrinsics(Windows.Media.Devices.Core.CameraIntrinsics mediaFrameIntrinsics)
        {
            CameraIntrinsics intrinsics = null;

            if (mediaFrameIntrinsics != null)
            {
                Vector2 focalLength = new Vector2(mediaFrameIntrinsics.FocalLength.X, mediaFrameIntrinsics.FocalLength.Y);
                uint imageWidth = mediaFrameIntrinsics.ImageWidth;
                uint imageHeight = mediaFrameIntrinsics.ImageHeight;
                Vector2 principalPoint = new Vector2(mediaFrameIntrinsics.PrincipalPoint.X, mediaFrameIntrinsics.PrincipalPoint.Y);
                Vector3 radialDistortion = new Vector3(mediaFrameIntrinsics.RadialDistortion.X, mediaFrameIntrinsics.RadialDistortion.Y, mediaFrameIntrinsics.RadialDistortion.Z);
                Vector2 tangentialDistortion = new Vector2(mediaFrameIntrinsics.TangentialDistortion.X, mediaFrameIntrinsics.TangentialDistortion.Y);
                Matrix4x4 undistortedProjectionTransform = ConvertMatrix(mediaFrameIntrinsics.UndistortedProjectionTransform);

                intrinsics = new CameraIntrinsics(focalLength, imageWidth, imageHeight, principalPoint, radialDistortion, tangentialDistortion, undistortedProjectionTransform);
            }

            return intrinsics;
        }

        /// <summary>
        /// Convert a windows matrix to the default matrix type
        /// </summary>
        private Matrix4x4 ConvertMatrix(System.Numerics.Matrix4x4 windowsMat)
        {
            Vector4 col0 = new Vector4(windowsMat.M11, windowsMat.M21, windowsMat.M31, windowsMat.M41);
            Vector4 col1 = new Vector4(windowsMat.M12, windowsMat.M22, windowsMat.M32, windowsMat.M42);
            Vector4 col2 = new Vector4(windowsMat.M13, windowsMat.M23, windowsMat.M33, windowsMat.M43);
            Vector4 col3 = new Vector4(windowsMat.M14, windowsMat.M24, windowsMat.M34, windowsMat.M44);
            Matrix4x4 mat = new Matrix4x4(col0, col1, col2, col3);

            return mat;
        }
#endif

        private static float Clamp(float val, float min, float max)
        {
            if (val.CompareTo(min) < 0)
                return min;
            else if (val.CompareTo(max) > 0)
                return max;
            else
                return val;
        }

        /// <summary>
        /// Disposes the camera
        /// </summary>
        public void Dispose()
        {
#if CAN_USE_UWP_TYPES
            if (frameReader != null)
            {
                frameReader.FrameArrived -= OnMediaFrameArrived;
                frameReader.Dispose();
            }
            mediaCapture?.Dispose();
            mediaCapture = null;
            videoDeviceController = null;
#endif
        }
    }
}
