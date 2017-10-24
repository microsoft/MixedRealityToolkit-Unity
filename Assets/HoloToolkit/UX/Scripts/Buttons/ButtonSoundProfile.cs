//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using MRDL;
using UnityEngine;

namespace HoloToolkit.Unity.Buttons
{
    public class ButtonSoundProfile : ButtonProfile
    {
        // Direct interaction clips
        [HideInMRDLInspector]
        public AudioClip ButtonCanceled;
        [HideInMRDLInspector]
        public AudioClip ButtonHeld;
        [HideInMRDLInspector]
        public AudioClip ButtonPressed;
        [HideInMRDLInspector]
        public AudioClip ButtonReleased;

        // State change clips
        [HideInMRDLInspector]
        public AudioClip ButtonObservation;
        [HideInMRDLInspector]
        public AudioClip ButtonObservationTargeted;
        [HideInMRDLInspector]
        public AudioClip ButtonTargeted;

        // Volumes
        [HideInMRDLInspector]
        public float ButtonCanceledVolume = 1f;
        [HideInMRDLInspector]
        public float ButtonHeldVolume = 1f;
        [HideInMRDLInspector]
        public float ButtonPressedVolume = 1f;
        [HideInMRDLInspector]
        public float ButtonReleasedVolume = 1f;
        [HideInMRDLInspector]
        public float ButtonObservationVolume = 1f;
        [HideInMRDLInspector]
        public float ButtonObservationTargetedVolume = 1f;
        [HideInMRDLInspector]
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