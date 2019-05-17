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
    public class CalibrationTestWindow : CompositorWindowBase<CalibrationTestWindow>
    {
        private Vector2 scrollPosition;

        private static string holographicCameraIPAddressKey = $"{nameof(CalibrationTestWindow)}.{nameof(holographicCameraIPAddress)}";
        private const int startStopRecordingButtonWidth = 200;
        private const int startStopRecordingButtonHeight = 100;
        private const int startStopRecordingButtonRightMargin = 30;
        private const string indexFileName = "index.json";

        private bool isRecording;

        private string currentRecordingSubdirectoryName;
        private float recordingStartTime;
        private float nextRecordingFrameTime;

        private CalibrationRecording recording = new CalibrationRecording()
        {
            Poses = new List<CalibrationRecordingPose>()
        };

        [MenuItem("Spectator View/Calibration Test", false, 1)]
        public static void ShowCalibrationRecordingWindow()
        {
            ShowWindow();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            holographicCameraIPAddress = PlayerPrefs.GetString(holographicCameraIPAddressKey, "localhost");
            recording.FrameWidth = renderFrameWidth;
            recording.FrameHeight = renderFrameHeight;
        }

        private void OnDisable()
        {
            PlayerPrefs.SetString(holographicCameraIPAddressKey, holographicCameraIPAddress);
            PlayerPrefs.Save();
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.BeginVertical();
                    {
                        RenderTitle("Recording", Color.green);

                        EditorGUILayout.BeginVertical("Box");
                        {
                            HolographicCameraNetworkConnectionGUI();
                            RecordControllerGUI();
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();

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

                        GUILayout.Space(startStopRecordingButtonRightMargin);
                        GUILayout.Label($"Recorded {recording.Poses.Count} frames");
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

                        if (!string.IsNullOrEmpty(currentRecordingSubdirectoryName))
                        {
                            GUILayout.Space(startStopRecordingButtonRightMargin);
                            GUILayout.BeginVertical();
                            {
                                GUILayout.Label($"Recording complete: {currentRecordingSubdirectoryName}");
                            }
                            GUILayout.EndVertical();
                        }

                    }
                    GUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
        }

        private bool CanRecord
        {
            get
            {
                HolographicCameraNetworkManager cameraNetworkManager = GetHolographicCameraNetworkManager();
                return cameraNetworkManager != null && cameraNetworkManager.IsConnected;
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
                        recording.Poses.Clear();
                        currentRecordingSubdirectoryName = DateTime.Now.ToString("s").Replace(':', '.');
                        recordingStartTime = Time.time;
                        nextRecordingFrameTime = recordingStartTime;

                        compositionManager.TextureManager.TextureRenderCompleted += Instance_TextureRenderCompleted;
                    }
                    else
                    {
                        compositionManager.TextureManager.TextureRenderCompleted -= Instance_TextureRenderCompleted;

                        if (recording.Poses.Count > 0)
                        {
                            string rootFolder, targetFolder;
                            GetRecordingDirectories(out rootFolder, out targetFolder);
                            string indexPath = Path.Combine(targetFolder, indexFileName);

                            File.WriteAllText(indexPath, JsonUtility.ToJson(recording));
                        }
                    }
                }
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

                string fileName = Path.Combine(targetFolder, $"{recording.Poses.Count}.raw");

                UnityCompositorInterface.TakeRawPicture(fileName);

                recording.Poses.Add(new CalibrationRecordingPose
                {
                    FrameFileNumber = recording.Poses.Count,
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
    }
}