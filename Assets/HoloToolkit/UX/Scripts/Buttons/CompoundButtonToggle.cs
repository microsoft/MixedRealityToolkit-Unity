//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System;
using System.Reflection;
using UnityEngine;
using HoloToolkit.Unity;

namespace HoloToolkit.Unity.Buttons
{
    /// <summary>
    /// Class that can be used to toggle between to button profiles for any target component inheriting from ProfileButtonBase
    /// </summary>
    [RequireComponent(typeof(CompoundButton))]
    public class CompoundButtonToggle : MonoBehaviour
    {
        [Tooltip("Toggle behavior")]
        public ToggleBehaviorEnum Behavior = ToggleBehaviorEnum.OnTapped;

        [Tooltip("Profile to use when State is TRUE")]
        [HideInMRTKInspector]
        public ButtonProfile OnProfile;

        [Tooltip("Profile to use when State is FALSE")]
        [HideInMRTKInspector]
        public ButtonProfile OffProfile;

        [DropDownComponent(true)]
        [Tooltip("Component to target - must inherit from ProfileButtonBase")]
        public Component Target;

        /// <summary>
        /// Private reference of the compound button component
        /// </summary>
        private CompoundButton m_compButton;

        public bool State {
            get {
                return state;
            }
            set {
                SetState(value);
            }
        }

        [SerializeField]
        private bool state;

        /// <summary>
        /// On enable subscribe to button state change on the compound button component
        /// </summary>
        private void OnEnable() {
            m_compButton = GetComponent<CompoundButton>();

            // Force initial state setting
            SetState(state, true);

            if (m_compButton != null)
                m_compButton.StateChange += ButtonStateChange;
        }

        /// <summary>
        /// On disable unsubscribe to button state change on the compound button component
        /// </summary>
        private void OnDisable()
        {
            if (m_compButton != null)
                m_compButton.StateChange -= ButtonStateChange;
        }

        /// <summary>
        /// Handle button pressed callback from button
        /// </summary>
        /// <param name="buttonObj"></param>
        public void ButtonStateChange(ButtonStateEnum newState) {
            if(newState == ButtonStateEnum.Pressed)
            {
                switch (Behavior)
                {
                    default:
                        break;

                    case ToggleBehaviorEnum.OnTapped:
                        State = !State;
                        break;

                }
            }
            else if(newState == ButtonStateEnum.ObservationTargeted || newState == ButtonStateEnum.Targeted)
            {
                switch (Behavior)
                {
                    default:
                        break;

                    case ToggleBehaviorEnum.OnFocus:
                        State = !State;
                        break;
                }
            }
        }

        private void SetState (bool newState, bool force = false) {
            if ((!force || !Application.isPlaying) && state == newState)
                return;

            if (Target == null || OnProfile == null || OffProfile == null)
                return;

            state = newState;

            // Get the profile field of the target component and set it to the on profile
            // Store all icons in iconLookup via reflection
#if USE_WINRT
            FieldInfo fieldInfo = Target.GetType().GetTypeInfo().GetField("Profile");
#else
            FieldInfo fieldInfo = Target.GetType().GetField("Profile");
#endif
            if (fieldInfo == null) {
                Debug.LogError("Target component had no field type profile in CompoundButtonToggle");
                return;
            }

            fieldInfo.SetValue(Target, state ? OnProfile : OffProfile);

            if (Application.isPlaying) {
                // Disable, then re-enable the target
                // This will force the component to update itself
                ((MonoBehaviour)Target).enabled = false;
                ((MonoBehaviour)Target).enabled = true;
            }
        }

#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(CompoundButtonToggle))]
        public class CustomEditor : MRTKEditor
        {
            protected override void DrawCustomFooter() {
                CompoundButtonToggle toggle = (CompoundButtonToggle)target;

                FieldInfo fieldInfo = null;
                Type profileType = null;
                if (toggle.Target == null) {
                    DrawError("Target must be set.");
                    return;
                } else {

                    fieldInfo = toggle.Target.GetType().GetField("Profile");

                    if (fieldInfo == null) {
                        DrawError("Target component has no 'Profile' field - are you use this class inherits from ProfileButtonBase?");
                        return;
                    }

                    GUIStyle labelStyle = new GUIStyle(UnityEditor.EditorStyles.label);
                    labelStyle.fontSize = 18;
                    labelStyle.fontStyle = FontStyle.Bold;

                    profileType = fieldInfo.FieldType;
                    UnityEditor.EditorGUILayout.LabelField("Type: " + toggle.Target.GetType().Name + " / " + fieldInfo.FieldType.Name, labelStyle, GUILayout.MinHeight(24));

                }

                UnityEditor.EditorGUILayout.LabelField("Select on/off profiles of the type " + profileType.Name);
                if (toggle.OnProfile == null) {
                    toggle.OnProfile = (ButtonProfile)fieldInfo.GetValue(toggle.Target);
                }
                if (toggle.OffProfile == null) {
                    toggle.OffProfile = toggle.OnProfile;
                }
                ButtonProfile onProfile = (ButtonProfile)UnityEditor.EditorGUILayout.ObjectField("On Profile", toggle.OnProfile, typeof(ButtonProfile), false);
                ButtonProfile offProfile = (ButtonProfile)UnityEditor.EditorGUILayout.ObjectField("Off Profile", toggle.OffProfile, typeof(ButtonProfile), false);
                if (onProfile.GetType() == profileType) {
                    toggle.OnProfile = onProfile;
                }
                if (offProfile.GetType() == profileType) {
                    toggle.OffProfile = offProfile;
                }

                if (toggle.OnProfile.GetType() != profileType) {
                    DrawError("On profile object does not match type " + profileType.Name);
                }
                if (toggle.OffProfile.GetType() != profileType) {
                    DrawError("Off profile object does not match type " + profileType.Name);
                }

                if (onProfile == offProfile) {
                    DrawWarning("Profiles are the same - toggle will have no effect");
                }

                toggle.Behavior = (ToggleBehaviorEnum)UnityEditor.EditorGUILayout.EnumPopup("Toggle behavior", toggle.Behavior);
            }
        }
#endif
    }
}
