// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Compositor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Editor
{
    [Serializable]
    public class CalibrationRecording
    {
        public int FrameWidth;
        public int FrameHeight;
        public List<CalibrationRecordingPose> Poses;
    }

    [Serializable]
    public class CalibrationRecordingPose
    {
        public int FrameFileNumber;
        public float FrameTime;
        public Vector3 CameraPosition;
        public Vector3 CameraRotationEuler;
    }

    [Description("Calibration Test")]
    internal class CalibrationTestWindow : CompositorWindowBase<CalibrationTestWindow>
    {
        private Vector2 scrollPosition;

        private static readonly string holographicCameraIPAddressKey = $"{nameof(CalibrationTestWindow)}.{nameof(holographicCameraIPAddress)}";
        private static readonly string calibrationPlaybackIndexFilePathPreferenceKey = $"{nameof(CalibrationTestWindow)}.{nameof(indexFilePath)}";
        private static readonly string calibrationPlaybackCalibrationFilePathPreferenceKey = $"{nameof(CalibrationTestWindow)}.{nameof(calibrationFilePath)}";

        private const int startStopRecordingButtonWidth = 200;
        private const int startStopRecordingButtonHeight = 100;
        private const string indexFileName = "index.json";
        private const float scrollBarWidth = 30.0f;

        private bool isRecording;

        private string currentRecordingSubdirectoryName;
        private float recordingStartTime;
        private float nextRecordingFrameTime;

        private bool isPlaying;
        private float currentTime;
        private int currentFrameIndex;
        private Texture2D imageTexture;
        private GameObject testCube;
        private ICalibrationData previousCalibrationData;
        private ICalibrationData calibrationDataForPlayback;

        private string indexFilePath;
        private string calibrationFilePath;

        private bool isIndexFileParsed;
        private bool isCalibrationDataParsed;

        private CalibrationRecording recordingForRecording = new CalibrationRecording()
        {
            Poses = new List<CalibrationRecordingPose>()
        };

        private CalibrationRecording recordingForPlayback;

        [MenuItem("Spectator View/Calibration Test", false, 1)]
        public static void ShowCalibrationRecordingWindow()
        {
            ShowWindow();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            IsRecording = false;
            IsPlaying = false;

            holographicCameraIPAddress = PlayerPrefs.GetString(holographicCameraIPAddressKey, "localhost");
            indexFilePath = PlayerPrefs.GetString(calibrationPlaybackIndexFilePathPreferenceKey, string.Empty);
            calibrationFilePath = PlayerPrefs.GetString(calibrationPlaybackCalibrationFilePathPreferenceKey, string.Empty);

            recordingForRecording.FrameWidth = renderFrameWidth;
            recordingForRecording.FrameHeight = renderFrameHeight;

            UpdateIndexFileForPlayback();
            UpdateCalibrationFileForPlayback();
        }

        private void OnDisable()
        {
            IsRecording = false;
            IsPlaying = false;

            PlayerPrefs.SetString(holographicCameraIPAddressKey, holographicCameraIPAddress);
            PlayerPrefs.SetString(calibrationPlaybackIndexFilePathPreferenceKey, indexFilePath);
            PlayerPrefs.SetString(calibrationPlaybackCalibrationFilePathPreferenceKey, calibrationFilePath);
            PlayerPrefs.Save();
        }

        protected override void Update()
        {
            base.Update();

            CompositionManager compositionManager = GetCompositionManager();
            if (compositionManager == null || compositionManager.TextureManager == null)
            {
                IsPlaying = false;
            }

            if (IsPlaying)
            {
                currentTime += Time.deltaTime;
                if (currentFrameIndex == recordingForPlayback.Poses.Count - 1)
                {
                    currentFrameIndex = 0;
                    currentTime = 0;
                    ApplyFrame(currentFrameIndex);
                }
                else
                {
                    for (int i = currentFrameIndex + 1; i < recordingForPlayback.Poses.Count; i++)
                    {
                        if (recordingForPlayback.Poses[i].FrameTime - recordingForPlayback.Poses[0].FrameTime <= currentTime)
                        {
                            currentFrameIndex = i;
                            ApplyFrame(currentFrameIndex);
                        }
                    }
                }
            }
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.BeginVertical(GUILayout.Width((position.width - scrollBarWidth) / 2));
                    {
                        RenderTitle("Recording", Color.green);

                        EditorGUILayout.BeginVertical("Box", GUILayout.MinHeight(250.0f));
                        {
                            var cameraDevice = GetHolographicCameraDevice();
                            HolographicCameraNetworkConnectionGUI(HolographicCameraDeviceTypeLabel, cameraDevice, GetSpatialCoordinateSystemParticipant(cameraDevice), showCalibrationStatus: true, ref holographicCameraIPAddress);

                            GUILayout.FlexibleSpace();

                            RecordControllerGUI();
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical(GUILayout.Width((position.width - scrollBarWidth) / 2));
                    {
                        RenderTitle("Playback", Color.green);

                        EditorGUILayout.BeginVertical("Box", GUILayout.MinHeight(250.0f));
                        {
                            PlaybackControllerGUI();
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();

                if (IsPlaying)
                {
                    RenderTitle($"Playback [{currentFrameIndex} / {recordingForPlayback.Poses.Count}]", Color.green);
                }
                else if (IsRecording)
                {
                    RenderTitle($"Recording [{recordingForRecording.Poses.Count} frames]", Color.green);
                }
                else
                {
                    RenderTitle("Preview", Color.yellow);
                }
                CompositeTextureGUI(textureRenderModeComposite);
            }
            EditorGUILayout.EndScrollView();
        }

        private void RecordControllerGUI()
        {
            if (!CanRecord)
            {
                IsRecording = false;
            }

            EditorGUILayout.BeginVertical("Box");
            {
                if (IsRecording)
                {
                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Stop Recording", GUILayout.Width(startStopRecordingButtonWidth), GUILayout.Height(startStopRecordingButtonHeight)))
                        {
                            IsRecording = false;
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUI.enabled = CanRecord;

                        if (GUILayout.Button("Start Recording", GUILayout.Width(startStopRecordingButtonWidth), GUILayout.Height(startStopRecordingButtonHeight)))
                        {
                            IsRecording = true;
                        }

                        GUI.enabled = true;
                    }
                    GUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void PlaybackControllerGUI()
        {
            GUILayout.BeginVertical("Box");
            {
                RenderTitle("Playback Parameters", Color.green);

                GUILayout.Label("Recording index file location");
                IndexFilePath = EditorGUILayout.TextField(IndexFilePath);

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                GUILayout.Label("Calibration file");
                CalibrationFilePath = EditorGUILayout.TextField(CalibrationFilePath);
            }
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical("Box");
            {
                GUILayout.BeginHorizontal();
                {
                    CompositionManager compositionManager = GetCompositionManager();
                    GUI.enabled = CanPlay;
                    if (IsPlaying)
                    {
                        if (GUILayout.Button("Stop", GUILayout.Width(startStopRecordingButtonWidth), GUILayout.Height(startStopRecordingButtonHeight)))
                        {
                            IsPlaying = false;
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Play", GUILayout.Width(startStopRecordingButtonWidth), GUILayout.Height(startStopRecordingButtonHeight)))
                        {
                            IsPlaying = true;
                        }
                    }
                    GUI.enabled = true;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        private bool CanRecord
        {
            get
            {
                HolographicCameraObserver cameraNetworkManager = GetHolographicCameraObserver();
                return cameraNetworkManager != null && cameraNetworkManager.IsConnected && !IsPlaying;
            }
        }

        private bool CanPlay
        {
            get
            {
                CompositionManager compositionManager = GetCompositionManager();
                return compositionManager != null &&
                       compositionManager.TextureManager != null &&
                       !IsRecording &&
                       isCalibrationDataParsed &&
                       isIndexFileParsed;
            }
        }

        private bool IsRecording
        {
            get { return isRecording; }
            set
            {
                if (isRecording != value)
                {
                    isRecording = value;

                    CompositionManager compositionManager = GetCompositionManager();

                    if (isRecording)
                    {
                        recordingForRecording.Poses.Clear();
                        currentRecordingSubdirectoryName = DateTime.Now.ToString("s").Replace(':', '.');
                        recordingStartTime = Time.time;
                        nextRecordingFrameTime = recordingStartTime;

                        compositionManager.TextureManager.TextureRenderCompleted += Instance_TextureRenderCompleted;
                    }
                    else
                    {
                        if (compositionManager != null && compositionManager.TextureManager != null)
                        {
                            compositionManager.TextureManager.TextureRenderCompleted -= Instance_TextureRenderCompleted;
                        }

                        if (recordingForRecording.Poses.Count > 0)
                        {
                            string rootFolder, targetFolder;
                            GetRecordingDirectories(out rootFolder, out targetFolder);
                            string indexPath = Path.Combine(targetFolder, indexFileName);

                            File.WriteAllText(indexPath, JsonUtility.ToJson(recordingForRecording));

                            IndexFilePath = indexPath;
                        }
                    }
                }
            }
        }

        private bool IsPlaying
        {
            get { return isPlaying; }
            set
            {
                if (isPlaying != value)
                {
                    isPlaying = value;

                    if (isPlaying)
                    {
                        StartPlayback();
                    }
                    else
                    {
                        StopPlayback();
                    }
                }
            }
        }

        private string IndexFilePath
        {
            get { return indexFilePath; }
            set
            {
                if (indexFilePath != value)
                {
                    indexFilePath = value;
                    UpdateIndexFileForPlayback();
                }
            }
        }

        private void UpdateIndexFileForPlayback()
        {
            isIndexFileParsed = false;

            try
            {
                if (File.Exists(indexFilePath))
                {
                    recordingForPlayback = JsonUtility.FromJson<CalibrationRecording>(File.ReadAllText(indexFilePath));
                    isIndexFileParsed = true;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Unexpected exception parsing file {indexFilePath}: {ex.ToString()}");
            }
        }

        private string CalibrationFilePath
        {
            get { return calibrationFilePath; }
            set
            {
                if (calibrationFilePath != value)
                {
                    calibrationFilePath = value;
                    UpdateCalibrationFileForPlayback();
                }
            }
        }

        private void UpdateCalibrationFileForPlayback()
        {
            isCalibrationDataParsed = false;
            try
            {
                if (File.Exists(calibrationFilePath))
                {
                    byte[] calibrationDataPayload = File.ReadAllBytes(calibrationFilePath);

                    CalculatedCameraCalibration calibration;
                    if (CalculatedCameraCalibration.TryDeserialize(calibrationDataPayload, out calibration))
                    {
                        calibrationDataForPlayback = new CalibrationData(calibration.Intrinsics, calibration.Extrinsics);

                        isCalibrationDataParsed = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Unexpected exception parsing file {indexFilePath}: {ex.ToString()}");
            }
        }

        private void Instance_TextureRenderCompleted()
        {
            if (Time.time > nextRecordingFrameTime)
            {
                CompositionManager compositionManager = GetCompositionManager();

                string rootFolder, targetFolder;
                GetRecordingDirectories(out rootFolder, out targetFolder);
                if (!Directory.Exists(rootFolder))
                {
                    Directory.CreateDirectory(rootFolder);
                }
                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }

                string fileName = Path.Combine(targetFolder, $"{recordingForRecording.Poses.Count}.raw");

                UnityCompositorInterface.TakeRawPicture(fileName);

                recordingForRecording.Poses.Add(new CalibrationRecordingPose
                {
                    FrameFileNumber = recordingForRecording.Poses.Count,
                    FrameTime = Time.time,
                    CameraPosition = compositionManager.transform.parent.localPosition,
                    CameraRotationEuler = compositionManager.transform.parent.localRotation.eulerAngles,
                });

                nextRecordingFrameTime += 1.0f / 10.0f;
            }
        }

        private void GetRecordingDirectories(out string rootFolder, out string targetFolder)
        {
            rootFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CalibrationVideos");
            targetFolder = Path.Combine(rootFolder, currentRecordingSubdirectoryName);
        }

        private void StartPlayback()
        {
            currentTime = 0;
            currentFrameIndex = 0;
            recordingForPlayback = JsonUtility.FromJson<CalibrationRecording>(File.ReadAllText(indexFilePath));
            ApplyFrame(currentFrameIndex);

            CompositionManager compositionManager = GetCompositionManager();
            HolographicCameraObserver networkManager = GetHolographicCameraObserver();

            previousCalibrationData = compositionManager.CalibrationData;
            compositionManager.EnableHolographicCamera(networkManager.transform, calibrationDataForPlayback);

            testCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testCube.transform.localScale = Vector3.one * DeviceInfoObserver.arUcoMarkerSizeInMeters;
            testCube.transform.localPosition = new Vector3(0.0f, 0.0f, 0.05f);
        }

        private void StopPlayback()
        {
            CompositionManager compositionManager = GetCompositionManager();
            HolographicCameraObserver networkManager = GetHolographicCameraObserver();

            if (compositionManager != null && compositionManager.TextureManager != null)
            {
                compositionManager.TextureManager.SetOverrideColorTexture(null);
                compositionManager.ClearOverridePose();

                if (previousCalibrationData != null)
                {
                    compositionManager.EnableHolographicCamera(networkManager.transform, previousCalibrationData);
                }
                Destroy(testCube);
            }

            testCube = null;
        }

        private void ApplyFrame(int frameIndex)
        {
            CompositionManager compositionManager = GetCompositionManager();

            string directory = Path.GetDirectoryName(indexFilePath);
            string frameFile = Path.Combine(directory, $"{recordingForPlayback.Poses[frameIndex].FrameFileNumber}.raw");
            if (imageTexture == null)
            {
                imageTexture = new Texture2D(recordingForPlayback.FrameWidth, recordingForPlayback.FrameHeight, TextureFormat.BGRA32, false);
            }
            imageTexture.LoadRawTextureData(File.ReadAllBytes(frameFile));
            imageTexture.Apply();
            aspect = ((float)imageTexture.width) / imageTexture.height;
            compositionManager.TextureManager.SetOverrideColorTexture(imageTexture);
            compositionManager.SetOverridePose(recordingForPlayback.Poses[frameIndex].CameraPosition, Quaternion.Euler(recordingForPlayback.Poses[frameIndex].CameraRotationEuler));
        }
    }
}