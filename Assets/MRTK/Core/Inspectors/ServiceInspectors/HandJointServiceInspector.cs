// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [MixedRealityServiceInspector(typeof(IMixedRealityHandJointService))]
    public class HandJointServiceInspector : BaseMixedRealityServiceInspector
    {
        private const string ShowHandPreviewInSceneViewKey = "MRTK_HandJointServiceInspector_ShowHandPreviewInSceneViewKey";
        private const string ShowHandJointFoldoutKey = "MRTK_HandJointServiceInspector_ShowHandJointFoldoutKey";
        private const string ShowHandJointKeyPrefix = "MRTK_HandJointServiceInspector_ShowHandJointKeyPrefixKey_";
        private static bool ShowHandPreviewInSceneView = false;
        private static bool ShowHandJointFoldout = false;

        private const float previewJointSize = 0.02f;
        private static readonly Color previewJointColor = new Color(0.5f, 0.1f, 0.6f, 0.75f);
        private static readonly Color enabledColor = GUI.backgroundColor;
        private static readonly Color disabledColor = Color.Lerp(enabledColor, Color.clear, 0.5f);
        private static Dictionary<TrackedHandJoint, bool> showHandJointSettings;
        private static Dictionary<TrackedHandJoint, string> showHandJointSettingKeys;

        // We want hand preview to always be visible
        public override bool AlwaysDrawSceneGUI { get { return true; } }

        public override void DrawInspectorGUI(object target)
        {
            IMixedRealityHandJointService handJointService = (IMixedRealityHandJointService)target;

            EditorGUILayout.LabelField("Tracking State", EditorStyles.boldLabel);

            if (!Application.isPlaying)
            {
                GUI.color = disabledColor;
                EditorGUILayout.Toggle("Left Hand Tracked", false);
                EditorGUILayout.Toggle("Right Hand Tracked", false);
            }
            else
            {
                GUI.color = enabledColor;
                EditorGUILayout.Toggle("Left Hand Tracked", handJointService.IsHandTracked(Handedness.Left));
                EditorGUILayout.Toggle("Right Hand Tracked", handJointService.IsHandTracked(Handedness.Right));
            }

            GenerateHandJointLookup();

            GUI.color = enabledColor;
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Editor Settings", EditorStyles.boldLabel);
            ShowHandPreviewInSceneView = SessionState.GetBool(ShowHandPreviewInSceneViewKey, false);
            bool showHandPreviewInSceneView = EditorGUILayout.Toggle("Show Preview in Scene View", ShowHandPreviewInSceneView);
            if (ShowHandPreviewInSceneView != showHandPreviewInSceneView)
            {
                SessionState.SetBool(ShowHandPreviewInSceneViewKey, showHandPreviewInSceneView);
            }

            ShowHandJointFoldout = SessionState.GetBool(ShowHandJointFoldoutKey, false);
            ShowHandJointFoldout = EditorGUILayout.Foldout(ShowHandJointFoldout, "Visible Hand Joints", true);
            SessionState.SetBool(ShowHandJointFoldoutKey, ShowHandJointFoldout);

            if (ShowHandJointFoldout)
            {
                #region setting buttons

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("All"))
                {
                    foreach (TrackedHandJoint joint in Enum.GetValues(typeof(TrackedHandJoint)))
                    {
                        if (joint == TrackedHandJoint.None)
                        {
                            continue;
                        }

                        SessionState.SetBool(showHandJointSettingKeys[joint], true);
                        showHandJointSettings[joint] = true;
                    }
                }

                if (GUILayout.Button("Fingers"))
                {
                    foreach (TrackedHandJoint joint in Enum.GetValues(typeof(TrackedHandJoint)))
                    {
                        bool setting = false;
                        switch (joint)
                        {
                            case TrackedHandJoint.IndexTip:
                            case TrackedHandJoint.IndexDistalJoint:
                            case TrackedHandJoint.IndexKnuckle:
                            case TrackedHandJoint.IndexMetacarpal:
                            case TrackedHandJoint.IndexMiddleJoint:

                            case TrackedHandJoint.MiddleTip:
                            case TrackedHandJoint.MiddleDistalJoint:
                            case TrackedHandJoint.MiddleKnuckle:
                            case TrackedHandJoint.MiddleMetacarpal:
                            case TrackedHandJoint.MiddleMiddleJoint:

                            case TrackedHandJoint.PinkyTip:
                            case TrackedHandJoint.PinkyDistalJoint:
                            case TrackedHandJoint.PinkyKnuckle:
                            case TrackedHandJoint.PinkyMetacarpal:
                            case TrackedHandJoint.PinkyMiddleJoint:

                            case TrackedHandJoint.RingTip:
                            case TrackedHandJoint.RingDistalJoint:
                            case TrackedHandJoint.RingKnuckle:
                            case TrackedHandJoint.RingMetacarpal:
                            case TrackedHandJoint.RingMiddleJoint:

                            case TrackedHandJoint.ThumbTip:
                            case TrackedHandJoint.ThumbDistalJoint:
                            case TrackedHandJoint.ThumbMetacarpalJoint:
                            case TrackedHandJoint.ThumbProximalJoint:
                                setting = true;
                                break;

                            default:
                                break;

                            case TrackedHandJoint.None:
                                continue;
                        }

                        SessionState.SetBool(showHandJointSettingKeys[joint], setting);
                        showHandJointSettings[joint] = setting;
                    }
                }

                if (GUILayout.Button("Fingertips"))
                {
                    foreach (TrackedHandJoint joint in Enum.GetValues(typeof(TrackedHandJoint)))
                    {
                        bool setting = false;
                        switch (joint)
                        {
                            case TrackedHandJoint.IndexTip:
                            case TrackedHandJoint.MiddleTip:
                            case TrackedHandJoint.PinkyTip:
                            case TrackedHandJoint.RingTip:
                            case TrackedHandJoint.ThumbTip:
                                setting = true;
                                break;

                            default:
                                break;

                            case TrackedHandJoint.None:
                                continue;
                        }

                        SessionState.SetBool(showHandJointSettingKeys[joint], setting);
                        showHandJointSettings[joint] = setting;
                    }
                }

                if (GUILayout.Button("None"))
                {
                    foreach (TrackedHandJoint joint in Enum.GetValues(typeof(TrackedHandJoint)))
                    {
                        if (joint == TrackedHandJoint.None)
                        {
                            continue;
                        }

                        SessionState.SetBool(showHandJointSettingKeys[joint], false);
                        showHandJointSettings[joint] = false;
                    }
                }

                EditorGUILayout.EndHorizontal();

                #endregion

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                foreach (TrackedHandJoint joint in Enum.GetValues(typeof(TrackedHandJoint)))
                {
                    if (joint == TrackedHandJoint.None)
                    {
                        continue;
                    }

                    bool prevSetting = showHandJointSettings[joint];
                    bool newSetting = EditorGUILayout.Toggle(joint.ToString(), prevSetting);
                    if (newSetting != prevSetting)
                    {
                        SessionState.SetBool(showHandJointSettingKeys[joint], newSetting);
                        showHandJointSettings[joint] = newSetting;
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }

        public override void DrawSceneGUI(object target, SceneView sceneView)
        {
            if (!Application.isPlaying || !ShowHandPreviewInSceneView)
            {
                return;
            }

            IMixedRealityHandJointService handJointService = (IMixedRealityHandJointService)target;

            DrawHandPreview(handJointService, Handedness.Left);
            DrawHandPreview(handJointService, Handedness.Right);
        }

        public override void DrawGizmos(object target) { }

        public static void DrawHandPreview(IMixedRealityHandJointService handJointService, Handedness handedness)
        {
            if (!handJointService.IsHandTracked(handedness))
            {
                return;
            }

            GenerateHandJointLookup();

            Handles.color = previewJointColor;

            foreach (KeyValuePair<TrackedHandJoint, bool> setting in showHandJointSettings)
            {
                if (!setting.Value)
                {
                    continue;
                }

                Transform jointTransform = handJointService.RequestJointTransform(setting.Key, handedness);

                if (jointTransform == null)
                {
                    continue;
                }

                Handles.SphereHandleCap(0, jointTransform.position, jointTransform.rotation, previewJointSize, EventType.Repaint);
            }
        }

        private static void GenerateHandJointLookup()
        {
            if (showHandJointSettings != null)
            {
                return;
            }

            showHandJointSettingKeys = new Dictionary<TrackedHandJoint, string>();
            showHandJointSettings = new Dictionary<TrackedHandJoint, bool>();

            foreach (TrackedHandJoint joint in Enum.GetValues(typeof(TrackedHandJoint)))
            {
                if (joint == TrackedHandJoint.None)
                {
                    continue;
                }

                string key = ShowHandJointKeyPrefix + joint;
                showHandJointSettingKeys.Add(joint, key);

                bool showHandJoint = SessionState.GetBool(key, true);
                showHandJointSettings.Add(joint, showHandJoint);
            }
        }
    }
}