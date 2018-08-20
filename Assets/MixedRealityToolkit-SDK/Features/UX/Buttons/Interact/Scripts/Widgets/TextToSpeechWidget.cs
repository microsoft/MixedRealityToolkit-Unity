// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interact.Widgets
{
    /// <summary>
    /// Speaks the contents of a TextMesh's text property.
    /// There must be a TextToSpeechManager in the scene.
    /// </summary>
    public class TextToSpeechWidget : InteractiveWidget
    {
        [Tooltip("TextMesh for getting the text to speak")]
        public TextMesh Label;

        [Tooltip("A value to override the Label text")]
        public string ToolTip = "";

        [Tooltip("Should the speech occur even if the Sound Source is in use")]
        public bool Interrupt = true;

        [Tooltip("Do not append additional words, like in the case of a toggle button stating on or off")]
        public bool SpeakLabelOnly = false;

        [Tooltip("Word spoken in the default toggle state")]
        public string DefaultStateWord = "Off";
        [Tooltip("Word spoken in the selected toggle state")]
        public string SelectedStateWord = "On";

        private Object mSpeechManager;// TEMP
        private bool mButtonLostFocus = true;
        private Interactive.ButtonStateEnum LastFocusedState;

        /// <summary>
        /// Get the TextMesh
        /// </summary>
        private void Awake()
        {
            if (Label == null)
            {
                Label = GetComponent<TextMesh>();
            }
        }

        /// <summary>
        /// speak the text value
        /// </summary>
        /// <param name="state"></param>
        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);
            
            bool toggledRadial = TestToggleRadial(state);

            // speak on the focus state only - or if the label changes during a selection - but only if the tooltip is not available.
            if ((state == Interactive.ButtonStateEnum.FocusSelected || state == Interactive.ButtonStateEnum.Focus || toggledRadial) && (mButtonLostFocus || (LastFocusedState != state && ToolTip == "")))
            {
                // get the TextToSpeechManager in the parent
                // TEMP mSpeechManager = GetComponentInParent<TextToSpeech>();

                if (mSpeechManager == null)
                {
                    // find the TextToSpeechManager somewhere
                    // TEMP mSpeechManager = FindObjectOfType<TextToSpeech>();
                }

                if (mSpeechManager != null)
                {
                    bool speak = false;// TEMP = (!mSpeechManager.IsSpeaking() || Interrupt) && !mSpeechManager.AudioSource.mute;

                    // mSpeechManager.SpeechTextInQueue()
                    if (speak)
                    {
                        // TEMPmSpeechManager.StartSpeaking(GetSelectedString());
                    }
                }

                LastFocusedState = state;
            }

            // the button has lost focus - make sure the button does not repeat the lable after a down
            mButtonLostFocus = state == Interactive.ButtonStateEnum.Default || state == Interactive.ButtonStateEnum.Selected;
        }

        /// <summary>
        /// see if we are using a radial behavior
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected virtual bool TestToggleRadial(Interactive.ButtonStateEnum state)
        {
            InteractiveToggle toggle = InteractiveHost.gameObject.GetComponent<InteractiveToggle>();
            if (toggle != null)
            {
                if(!toggle.AllowDeselect && toggle.AllowSelection && state == Interactive.ButtonStateEnum.Selected && LastFocusedState == Interactive.ButtonStateEnum.Focus)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Find the correct strings that should be used for the selected and default states
        /// </summary>
        /// <returns></returns>
        protected virtual string GetSelectedString()
        {
            if (ToolTip != "")
            {
                return ToolTip;
            }

            if (Label == null)
            {
                Debug.LogError("Textmesh:Label and Tool Tip is not set in TextToSpeechWidget!");
            }

            InteractiveToggle toggle = InteractiveHost.gameObject.GetComponent<InteractiveToggle>();

            if (toggle != null && !SpeakLabelOnly)
            {
                if (State == Interactive.ButtonStateEnum.Focus)
                {
                    return Label.text + " " + DefaultStateWord;
                }
                else
                {
                    return Label.text + " " + SelectedStateWord;
                }
            }

            return Label.text;

        }
    }
}
