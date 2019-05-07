// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Compositor
{
    public class CompositionManager : MonoBehaviour
    {
        private const int DSPBufferSize = 1024;
        private const AudioSpeakerMode SpeakerMode = AudioSpeakerMode.Stereo;

        public enum FrameProviderDeviceType { BlackMagic, Elgato };
        public bool IsCurrentlyActive { get; set; }

        [Header("Hologram Settings")]
        public Depth TextureDepth = Depth.TwentyFour;
        public AntiAliasingSamples AntiAliasing = AntiAliasingSamples.Eight;
        public FilterMode Filter = FilterMode.Trilinear;
        [Range(0, 2)]
        public int SuperSampleLevel = 0;

        [Tooltip("Default alpha for the holograms in the composite video.")]
        public float DefaultAlpha = 0.9f;

        private float frameOffset = -10.0f;
        private TextureManager textureManager;
        private MicrophoneInput microphoneInput;
        private Calibration calibration;

#if UNITY_EDITOR
        private bool overrideCameraPose;
        private Vector3 overrideCameraPosition;
        private Quaternion overrideCameraRotation;

        public void ClearOverridePose()
        {
            overrideCameraPose = false;
        }

        public void SetOverridePose(Vector3 position, Quaternion rotation)
        {
            overrideCameraPose = true;
            overrideCameraPosition = position;
            overrideCameraRotation = rotation;
        }
#endif

        public float FrameOffset
        {
            get
            {
                if (frameOffset < -1.0f)
                {
                    frameOffset = PlayerPrefs.GetFloat("FrameOffset");
                }
                return frameOffset;
            }
            set
            {
                if (frameOffset != value)
                {
                    Debug.LogFormat("FrameOffset set:{0}", value);
                    frameOffset = value;
                    PlayerPrefs.SetFloat("FrameOffset", frameOffset);
                    PlayerPrefs.Save();
                }
            }
        }

        private int captureDeviceIndex = -1;
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

        public bool EnableMicrophoneAudio = true;

#if UNITY_EDITOR
        private MemoryStream audioMemoryStream = null;
#endif

        public enum Depth { None, Sixteen = 16, TwentyFour = 24 }
        public enum AntiAliasingSamples { One = 1, Two = 2, Four = 4, Eight = 8 };

        private bool frameProviderInitialized = false;

        private SpectatorViewPoseCache poseCache = new SpectatorViewPoseCache();

        private SpectatorViewTimeSynchronizer timeSynchronizer = new SpectatorViewTimeSynchronizer();
        
        public int CurrentCompositeFrame { get; private set; }

        private Camera spectatorCamera;

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
            //UnityEngine.XR.WSA.HolographicSettings.ActivateLatentFramePresentation(true);

            IsCurrentlyActive = false;
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

            calibration = gameObject.AddComponent<Calibration>();
            textureManager = gameObject.AddComponent<TextureManager>();
            microphoneInput = GetComponentInChildren<MicrophoneInput>();
            textureManager.Compositor = this;

            // Change audio listener to the holographic camera.
            AudioListener listener = null;
            var cam = Camera.main;
            if (cam)
            {
                listener = cam.GetComponent<AudioListener>();
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

        float GetVideoDT()
        {
            return (0.0001f * (UnityCompositorInterface.GetColorDuration() / 1000));
        }

        private float GetTimeFromFrame(int frame)
        {
            return GetVideoDT() * frame;
        }

#endif

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
                    float videoDt = GetVideoDT();
                    int frame = (int)(time / videoDt);
                    //Subtract the queued frames
                    frame -= UnityCompositorInterface.GetCaptureFrameIndex() - CurrentCompositeFrame;
                    time = videoDt * frame;
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
            if (IsCurrentlyActive && transform.parent != null)
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
                    poseTime += FrameOffset;
                    if (captureFrameIndex <= 0) //No frames captured yet, lets use the very latest camera transform
                    {
                        poseTime = float.MaxValue;
                    }
                    poseCache.GetPose(out currPos, out currRot, poseTime);

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
        public void EnableHolographicCamera(Transform parent)
        {
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

            calibration.SetUnityCameraExtrinstics(transform);
            calibration.SetUnityCameraIntrinsics(GetComponent<Camera>());

            IsCurrentlyActive = true;
        }

        void OnEnable()
        {
            frameProviderInitialized = false;
        }

        private void OnDestroy()
        {
            ResetCompositor();
        }

        public void ResetCompositor()
        {
            Debug.Log("Disposing DLL Resources.");
            UnityCompositorInterface.Reset();

            UnityCompositorInterface.StopFrameProvider();
            if (UnityCompositorInterface.IsRecording())
            {
                UnityCompositorInterface.StopRecording();
            }
        }

        // Send audio data to Compositor.
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