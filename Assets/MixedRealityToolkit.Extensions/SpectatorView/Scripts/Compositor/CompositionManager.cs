// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Compositor
{
    /// <summary>
    /// Manages compositing real-world video and holograms together and creating an output
    /// video texture with recorded audio.
    /// </summary>
    public class CompositionManager : MonoBehaviour
    {
        private const int DSPBufferSize = 1024;
        private const AudioSpeakerMode SpeakerMode = AudioSpeakerMode.Stereo;

        public enum FrameProviderDeviceType { BlackMagic, Elgato };
        public enum Depth { None, Sixteen = 16, TwentyFour = 24 }
        public enum AntiAliasingSamples { One = 1, Two = 2, Four = 4, Eight = 8 };

        /// <summary>
        /// Gets whether or not the holographic camera rig is connected and sending poses
        /// for the camera to the compositor.
        /// </summary>
        public bool IsHolographicCameraConnected { get; set; }

        /// <summary>
        /// Gets or sets the texture depth used for the RenderTextures used during compositing.
        /// </summary>
        [Header("Hologram Settings")]
        [Tooltip("Texture depth for the RenderTexture used by the compositor")]
        public Depth TextureDepth = Depth.TwentyFour;

        /// <summary>
        /// Gets or sets the sampling level for antialiasing when supersampling is used.
        /// </summary>
        [Tooltip("Anti-aliasing sampling level for downsampling the supersample textures to the target video resolution.")]
        public AntiAliasingSamples AntiAliasing = AntiAliasingSamples.Eight;

        /// <summary>
        /// Gets or sets the filter mode for downsampling when supersampling is used.
        /// </summary>
        [Tooltip("Filtering mode used for downsampling the supersample textures to the target video resolution.")]
        public FilterMode Filter = FilterMode.Trilinear;

        /// <summary>
        /// Gets or sets the number of additional buffers to use for supersampling.
        /// Each additional buffer doubles the size of the rendered holograms before they're
        /// downsampled to the video resolution.
        /// </summary>
        [Range(0, 2)]
        [Tooltip("Number of additional buffers used to render holograms at a higher resolution. Each additional level doubles the size of the rendered hologram before it is downsampled to the video resolution.")]
        public int SuperSampleLevel = 0;

        /// <summary>
        /// Gets or sets the alpha value used for rendering holograms on top of video.
        /// </summary>
        [Tooltip("Default alpha for the holograms in the composite video.")]
        public float DefaultAlpha = 0.9f;

        /// <summary>
        /// Gets or sets whether microphone audio should be recorded into the output video.
        /// </summary>
        [Tooltip("Enables or disables recording microphone audio when recording videos.")]
        public bool EnableMicrophoneAudio = true;


        private float videoTimestampToHolographicTimestampOffset = -10.0f;
        private int captureDeviceIndex = -1;
        private TextureManager textureManager;
        private MicrophoneInput microphoneInput;
        private ICalibrationData calibrationData;

        private bool frameProviderInitialized = false;
        private SpectatorViewPoseCache poseCache = new SpectatorViewPoseCache();
        private SpectatorViewTimeSynchronizer timeSynchronizer = new SpectatorViewTimeSynchronizer();

        private Camera spectatorCamera;

        /// <summary>
        /// Gets the index of the video frame currently being composited.
        /// </summary>
        public int CurrentCompositeFrame { get; private set; }

#if UNITY_EDITOR
        private bool overrideCameraPose;
        private Vector3 overrideCameraPosition;
        private Quaternion overrideCameraRotation;
        private MemoryStream audioMemoryStream = null;

        /// <summary>
        /// Clears the usage of an overridden camera pose and returns to normal pose calculations.
        /// </summary>
        public void ClearOverridePose()
        {
            overrideCameraPose = false;
        }

        /// <summary>
        /// Stops computing the position and rotation of the holographic camera from external sources
        /// and instead fixes the position and rotation as specified.
        /// </summary>
        /// <param name="position">The local position of the holographic camera.</param>
        /// <param name="rotation">The local rotation of the holographic camera.</param>
        public void SetOverridePose(Vector3 position, Quaternion rotation)
        {
            overrideCameraPose = true;
            overrideCameraPosition = position;
            overrideCameraRotation = rotation;
        }
#endif

        /// <summary>
        /// Gets or sets the additional time offset in seconds to adjust holographic timestamps
        /// from the HoloLens to video timestamps from the compositor.
        /// </summary>
        public float VideoTimestampToHolographicTimestampOffset
        {
            get
            {
                if (videoTimestampToHolographicTimestampOffset < -1.0f)
                {
                    videoTimestampToHolographicTimestampOffset = PlayerPrefs.GetFloat(nameof(VideoTimestampToHolographicTimestampOffset));
                }
                return videoTimestampToHolographicTimestampOffset;
            }
            set
            {
                if (videoTimestampToHolographicTimestampOffset != value)
                {
                    videoTimestampToHolographicTimestampOffset = value;
                    PlayerPrefs.SetFloat(nameof(VideoTimestampToHolographicTimestampOffset), videoTimestampToHolographicTimestampOffset);
                    PlayerPrefs.Save();
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of capture device to read video content from.
        /// </summary>
        public FrameProviderDeviceType CaptureDevice
        {
            get
            {
                if (captureDeviceIndex == -1)
                {
                    captureDeviceIndex = PlayerPrefs.GetInt("CaptureDevice", (int)FrameProviderDeviceType.BlackMagic);
                }
                return (FrameProviderDeviceType)captureDeviceIndex;
            }
            set
            {
                if (captureDeviceIndex != (int)value)
                {
                    captureDeviceIndex = (int)value;
                    PlayerPrefs.SetInt("CaptureDevice", captureDeviceIndex);
                    PlayerPrefs.Save();
                }
            }
        }

        #region AudioData
        private BinaryWriter audioStreamWriter;
        private double audioStartTime;
        private int numCachedAudioFrames;
        private const int MAX_NUM_CACHED_AUDIO_FRAMES = 5;
        #endregion

        const int statCapacity = 60;
        Queue<float> PCFpsStats = new Queue<float>(statCapacity);
        Queue<float> StepStats = new Queue<float>(statCapacity);

        void UpdateStatsElement(Queue<float> statElements, float newVal)
        {
            if (statElements.Count == statCapacity)
                statElements.Dequeue();
            statElements.Enqueue(newVal);
        }

        string GetStatsString(string title, Queue<float> statElements, out float average)
        {
            float min = float.MaxValue;
            float max = float.MinValue;
            float total = 0.0f;
            foreach (var v in statElements)
            {
                min = Mathf.Min(v, min);
                max = Mathf.Max(v, max);
                total += v;
            }
            average = total / statElements.Count;
            return string.Format("{0}:{1} Min:{2} Max:{3} Avg:{4:N1}", title, (int)statElements.Peek(), (int)min, (int)max, average);
        }

        public string GetFPSStatsString(out float average)
        {
            return GetStatsString("PC FPS", PCFpsStats, out average);
        }
        public string GetStepStatsString(out float average)
        {
            return GetStatsString("STEPS", StepStats, out average);
        }

        private void Start()
        {
            IsHolographicCameraConnected = false;
            spectatorCamera = GetComponent<Camera>();

#if !UNITY_EDITOR
            Camera[] cameras = gameObject.GetComponentsInChildren<Camera>();
            for (int i = 0; i < cameras.Length; i++)
            {
                cameras[i].enabled = false;
            }
#endif
            Initialize();
        }

        private void Initialize()
        {
#if UNITY_EDITOR
            // Ensure that runInBackground is set to true so that the compositor can run even when not focused
            Application.runInBackground = true;

            textureManager = gameObject.AddComponent<TextureManager>();
            microphoneInput = GetComponentInChildren<MicrophoneInput>();
            textureManager.Compositor = this;

            // Change audio listener to the holographic camera.
            AudioListener listener = null;
            var mainCamera = Camera.main;
            if (mainCamera)
            {
                listener = mainCamera.GetComponent<AudioListener>();
                if (listener != null)
                {
                    GameObject.DestroyImmediate(listener);
                }
            }

            listener = GetComponent<AudioListener>();
            if (listener == null)
            {
                gameObject.AddComponent<AudioListener>();
            }

            // Disable vsync in editor to ensure that the game runs at the maximum possible framerate
            QualitySettings.vSyncCount = 0;

            AudioConfiguration currentConfiguration = AudioSettings.GetConfiguration();
            currentConfiguration.dspBufferSize = DSPBufferSize;
            currentConfiguration.speakerMode = SpeakerMode;
            AudioSettings.Reset(currentConfiguration);

            // resetting the audio settings kills the mic so don't init until here
            if (EnableMicrophoneAudio && microphoneInput != null)
            {
                microphoneInput.StartMicrophone();
            }
#endif
        }


#if UNITY_EDITOR
        /// <summary>
        /// Gets the time duration of a single video frame.
        /// </summary>
        /// <returns>The time duration of a single video frame, in seconds.</returns>
        private float GetVideoFrameDuration()
        {
            return (0.0001f * (UnityCompositorInterface.GetColorDuration() / 1000));
        }

        /// <summary>
        /// Gets the time for a video frame relative to the start of video capture.
        /// </summary>
        /// <param name="frame">The index of the video frame.</param>
        /// <returns>The time of the video frame relative to the start of the video capture, in seconds.</returns>
        private float GetTimeFromFrame(int frame)
        {
            return GetVideoFrameDuration() * frame;
        }

#endif

        /// <summary>
        /// Gets the timestamp of the hologram that will be composited for the current frame of the compositor.
        /// </summary>
        /// <returns>The hologram timestamp corresponding to the current video frame, in Unity's timeline.</returns>
        public float GetHologramTime()
        {
            float time = Time.time;

#if UNITY_EDITOR
            if (frameProviderInitialized)
            {
                if (poseCache.poses.Count > 0)
                {
                    time = timeSynchronizer.GetUnityTimeFromCameraTime(GetTimeFromFrame(CurrentCompositeFrame));
                }
                else
                {
                    //Clamp time to video dt
                    float videoDeltaTime = GetVideoFrameDuration();
                    int frame = (int)(time / videoDeltaTime);
                    //Subtract the queued frames
                    frame -= UnityCompositorInterface.GetCaptureFrameIndex() - CurrentCompositeFrame;
                    time = videoDeltaTime * frame;
                }
            }
#endif

            return time;
        }

        private void Update()
        {
#if UNITY_EDITOR

            UpdateStatsElement(PCFpsStats, 1.0f / Time.deltaTime);

            UnityCompositorInterface.UpdateSpectatorView();

            int captureFrameIndex = UnityCompositorInterface.GetCaptureFrameIndex();

            int prevCompositeFrame = CurrentCompositeFrame;

            //Set our current frame towards the latest captured frame. Dont get too close to it, and dont fall too far behind 
            int step = (captureFrameIndex - CurrentCompositeFrame);
            if (step < 8)
            {
                step = 0;
            }
            else if (step > 16)
            {
                step -= 16;
            }
            else
            {
                step = 1;
            }
            CurrentCompositeFrame += step;

            UpdateStatsElement(StepStats, step);

            UnityCompositorInterface.SetCompositeFrameIndex(CurrentCompositeFrame);

            #region Spectator View Transform
            if (IsHolographicCameraConnected && transform.parent != null)
            {
                //Update time syncronizer
                {
                    float captureTime = GetTimeFromFrame(captureFrameIndex);

                    SpectatorViewPoseCache.PoseData poseData = poseCache.GetLatestPose();
                    if (poseData != null)
                    {
                        timeSynchronizer.Update(UnityCompositorInterface.GetCaptureFrameIndex(), captureTime, poseData.Index, poseData.TimeStamp);
                    }
                }

                if (overrideCameraPose)
                {
                    transform.parent.localPosition = overrideCameraPosition;
                    transform.parent.localRotation = overrideCameraRotation;
                }
                else
                {
                    //Set camera transform for the currently composited frame
                    float cameraTime = GetTimeFromFrame(prevCompositeFrame);
                    float poseTime = timeSynchronizer.GetPoseTimeFromCameraTime(cameraTime);

                    Quaternion currRot;
                    Vector3 currPos;
                    poseTime += VideoTimestampToHolographicTimestampOffset;
                    if (captureFrameIndex <= 0) //No frames captured yet, lets use the very latest camera transform
                    {
                        poseTime = float.MaxValue;
                    }
                    poseCache.GetPose(poseTime, out currPos, out currRot);

                    transform.parent.localPosition = currPos;
                    transform.parent.localRotation = currRot;
                }
            }

            #endregion

            if (!frameProviderInitialized)
            {
                frameProviderInitialized = UnityCompositorInterface.InitializeFrameProviderOnDevice((int)CaptureDevice);
                if (frameProviderInitialized)
                {
                    CurrentCompositeFrame = 0;
                    timeSynchronizer.Reset();
                    poseCache.Reset();
                }
            }

            UnityCompositorInterface.UpdateCompositor();
#endif
        }

        private void UpdateCullingMask()
        {
            // Copy the culling mask from the main camera to the spectator camera
            Camera cam = Camera.main;
            if (cam)
            {
                spectatorCamera.cullingMask = cam.cullingMask;
            }
        }

        protected void OnPreCull()
        {
            UpdateCullingMask();
        }

#if UNITY_EDITOR
        /// <summary>
        /// Enables the holographic camera rig for compositing. The hologram camera will be adjusted to match
        /// calibration data (its position and rotation will track the external camera, and its projection matrix
        /// will match the calibration information for the video camera used for compositing).
        /// </summary>
        /// <param name="parent">The parent transform that the holographic camera rig should be attached to.</param>
        /// <param name="calibrationData">The calibration data used to set up the position, rotation, and
        /// projection matrix for the holographic camera.</param>
        public void EnableHolographicCamera(Transform parent, ICalibrationData calibrationData)
        {
            this.calibrationData = calibrationData;
            GameObject container = GameObject.Find("SpectatorView");
            if (container == null)
            {
                container = GameObject.CreatePrimitive(PrimitiveType.Cube);
                container.GetComponent<MeshRenderer>().enabled = false;
                GameObject.DestroyImmediate(container.GetComponent<Collider>());
                container.name = "SpectatorView";
            }

            container.transform.SetParent(parent);
            container.transform.localPosition = Vector3.zero;
            container.transform.localRotation = Quaternion.identity;

            gameObject.transform.parent = container.transform;

            calibrationData.SetUnityCameraExtrinstics(transform);
            calibrationData.SetUnityCameraIntrinsics(GetComponent<Camera>());

            IsHolographicCameraConnected = true;
        }

        private void OnEnable()
        {
            frameProviderInitialized = false;
        }

        private void OnDestroy()
        {
            ResetCompositor();
        }

        private void ResetCompositor()
        {
            Debug.Log("Stopping the video composition system.");
            UnityCompositorInterface.Reset();

            UnityCompositorInterface.StopFrameProvider();
            if (UnityCompositorInterface.IsRecording())
            {
                UnityCompositorInterface.StopRecording();
            }
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            if (!UnityCompositorInterface.IsRecording())
            {
                return;
            }

            //Create new stream
            if (audioMemoryStream == null)
            {
                audioMemoryStream = new MemoryStream();
                audioStreamWriter = new BinaryWriter(audioMemoryStream);
                audioStartTime = AudioSettings.dspTime;
                numCachedAudioFrames = 0;
            }

            //Put data into stream
            for (int i = 0; i < data.Length; i++)
            {
                // Rescale float to short range for encoding.
                short audioEntry = (short)(data[i] * short.MaxValue);
                audioStreamWriter.Write(audioEntry);
            }

            numCachedAudioFrames++;

            //Send to compositor (buffer a few calls to reduce potential timing errors between packages)
            if (numCachedAudioFrames >= MAX_NUM_CACHED_AUDIO_FRAMES)
            {
                audioStreamWriter.Flush();
                byte[] outBytes = audioMemoryStream.ToArray();
                audioMemoryStream = null;
                UnityCompositorInterface.SetAudioData(outBytes, outBytes.Length, audioStartTime);
            }
        }

        /// <summary>
        /// Stops audio recording by ensuring the audio stream is fully written immediately.
        /// </summary>
        public void StopRecordingAudio()
        {
            //Send any left over stream
            if (audioMemoryStream != null)
            {
                audioStreamWriter.Flush();
                byte[] outBytes = audioMemoryStream.ToArray();
                UnityCompositorInterface.SetAudioData(outBytes, outBytes.Length, audioStartTime);
                audioMemoryStream = null;
            }
        }
#endif
    }
}