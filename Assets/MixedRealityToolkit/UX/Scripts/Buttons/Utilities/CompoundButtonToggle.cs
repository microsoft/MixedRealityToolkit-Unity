// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Utilities.Attributes;
using MixedRealityToolkit.UX.Buttons.Enums;
using MixedRealityToolkit.UX.Buttons.Profiles;
using System.Reflection;
using UnityEngine;

namespace MixedRealityToolkit.UX.Buttons.Utilities
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
            FieldInfo fieldInfo = Target.GetType().GetField("Profile");

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
    }
}
