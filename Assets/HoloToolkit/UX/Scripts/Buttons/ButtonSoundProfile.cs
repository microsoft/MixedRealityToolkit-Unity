// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using UnityEngine;

namespace HoloToolkit.Unity.Buttons
{
    public class ButtonSoundProfile : ButtonProfile
    {
        // Direct interaction clips
        [HideInMRTKInspector]
        public AudioClip ButtonCanceled;
        [HideInMRTKInspector]
        public AudioClip ButtonHeld;
        [HideInMRTKInspector]
        public AudioClip ButtonPressed;
        [HideInMRTKInspector]
        public AudioClip ButtonReleased;

        // State change clips
        [HideInMRTKInspector]
        public AudioClip ButtonObservation;
        [HideInMRTKInspector]
        public AudioClip ButtonObservationTargeted;
        [HideInMRTKInspector]
        public AudioClip ButtonTargeted;

        // Volumes
        [HideInMRTKInspector]
        public float ButtonCanceledVolume = 1f;
        [HideInMRTKInspector]
        public float ButtonHeldVolume = 1f;
        [HideInMRTKInspector]
        public float ButtonPressedVolume = 1f;
        [HideInMRTKInspector]
        public float ButtonReleasedVolume = 1f;
        [HideInMRTKInspector]
        public float ButtonObservationVolume = 1f;
        [HideInMRTKInspector]
        public float ButtonObservationTargetedVolume = 1f;
        [HideInMRTKInspector]
        public float ButtonTargetedVolume = 1f;

#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(ButtonSoundProfile))]
        public class CustomEditor : ProfileInspector
        {
            protected override void DrawCustomFooter() {
                ButtonSoundProfile soundProfile = (ButtonSoundProfile)target;

                DrawClipEditor(ref soundProfile.ButtonPressed, ref soundProfile.ButtonPressedVolume, "Button Pressed");
                DrawClipEditor(ref soundProfile.ButtonTargeted, ref soundProfile.ButtonTargetedVolume, "Button Targeted");
                DrawClipEditor(ref soundProfile.ButtonHeld, ref soundProfile.ButtonHeldVolume, "Button Held");
                DrawClipEditor(ref soundProfile.ButtonReleased, ref soundProfile.ButtonReleasedVolume, "Button Released");
                DrawClipEditor(ref soundProfile.ButtonCanceled, ref soundProfile.ButtonCanceledVolume, "Button Canceled");
                DrawClipEditor(ref soundProfile.ButtonObservation, ref soundProfile.ButtonObservationVolume, "Button Observation");
                DrawClipEditor(ref soundProfile.ButtonObservationTargeted, ref soundProfile.ButtonObservationTargetedVolume, "Button Observation Targeted");
            }

            protected void DrawClipEditor(ref AudioClip clip, ref float volume, string label) {
                UnityEditor.EditorGUILayout.Space();
                UnityEditor.EditorGUILayout.LabelField(label, UnityEditor.EditorStyles.boldLabel);
                UnityEditor.EditorGUI.indentLevel++;
                UnityEditor.EditorGUILayout.BeginHorizontal();
                clip = (AudioClip)UnityEditor.EditorGUILayout.ObjectField(clip, typeof(UnityEngine.AudioClip), true);
                volume = UnityEditor.EditorGUILayout.Slider(volume, 0f, 1f);
                UnityEditor.EditorGUILayout.EndHorizontal();
                UnityEditor.EditorGUI.indentLevel--;
            }
        }
#endif
    }
}