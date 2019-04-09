// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Input.Utilities
{
    [CustomEditor(typeof(InputAnimationAsset))]
    public class InputAnimationAssetEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            bool isGUIEnabled = GUI.enabled;
            var asset = target as InputAnimationAsset;

            GUI.enabled = isGUIEnabled && Application.isPlaying;

            bool wasRecording = asset.IsRecording;
            bool record;
            record = GUILayout.Toggle(wasRecording, wasRecording ? "Stop Recording" : "Start Recording", "Button");

            if (record != wasRecording)
            {
                if (record)
                {
                    asset.StartRecording();
                }
                else
                {
                    asset.StopRecording();
                    EditorUtility.SetDirty(asset);
                }
            }

            if (asset.IsRecording)
            {
                Rect rect = GUILayoutUtility.GetRect(new GUIContent(""), "Button");
                DrawKeyframes(asset.RecordingInputAnimation, rect, Color.red, Color.white);

                Repaint();
            }
            else if (asset.InputAnimation != null)
            {
                Rect rect = GUILayoutUtility.GetRect(new GUIContent(""), "Button");
                DrawKeyframes(asset.InputAnimation, rect, Color.gray, Color.yellow);
            }

            GUI.enabled = isGUIEnabled;
        }

        /// <summary>
        /// Draws a timeline of which keyframes are defined in the input animation.
        /// </summary>
        private void DrawKeyframes(InputAnimation anim, Rect rect, Color backgroundColor, Color keyframeColor)
        {
            if (rect.width <= 0.0f)
            {
                return;
            }

            EditorGUI.DrawRect(rect, backgroundColor);

            int numKeyframes = anim.KeyframeCount;
            double duration = anim.Duration;
            if (numKeyframes == 0 || duration <= 0.0)
            {
                return;
            }

            float pixelsPerSecond = rect.width / (float)duration;

            float keyframePos = (float)anim.GetTime(0) * pixelsPerSecond;
            float blockStart = Mathf.Floor(keyframePos);
            float blockEnd = blockStart + 1.0f;
            for (int i = 1; i < numKeyframes; ++i)
            {
                keyframePos = (float)anim.GetTime(i) * pixelsPerSecond;
                if (keyframePos > rect.width)
                {
                    blockEnd = rect.width;
                    break;
                }

                if (keyframePos > blockEnd + 1.0f)
                {
                    // Next keyframe is more than one pixel away, draw the current block and start the next
                    var blockRect = Rect.MinMaxRect(rect.xMin + blockStart, rect.yMin, rect.xMin + blockEnd, rect.yMax);
                    EditorGUI.DrawRect(blockRect, keyframeColor);

                    blockStart = Mathf.Floor(keyframePos);
                    blockEnd = blockStart + 1.0f;
                }
                else if (keyframePos > blockEnd)
                {
                    // Next keyframe is inside the next pixel, extend the current block
                    blockEnd += pixelsPerSecond;
                }
                else
                {
                    // Next keyframe is in the same pixel, block size unchanged
                }
            }
            // Draw final block
            {
                var blockRect = Rect.MinMaxRect(rect.xMin + blockStart, rect.yMin, rect.xMin + blockEnd, rect.yMax);
                EditorGUI.DrawRect(blockRect, keyframeColor);
            }
        }
    }
}