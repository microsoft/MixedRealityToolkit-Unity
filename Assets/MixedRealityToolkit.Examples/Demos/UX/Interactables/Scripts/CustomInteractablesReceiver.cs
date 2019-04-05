// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Example of building a custom receiver that can be loaded as part of the events on the Interactable or
    /// in InteractableReceiverList or InteractableReceiver
    /// 
    /// Extend ReceiverBaseMonoBehavior to build external event components
    /// </summary>
    public class CustomInteractablesReceiver : ReceiverBase
    {
        private State lastState;
        private string statusString = "State: %state%";
        private string clickString = "Clicked!";
        private string voiceString = "VoiceCommand: %voiceCommand%";
        private string outputString;

        private string lastVoiceCommand = "";

        private float clickTime = 2;
        private Coroutine showClicked;
        private Coroutine showVoice;
        private int clickCount = 0;

        public CustomInteractablesReceiver(UnityEvent ev) : base(ev)
        {
            Name = "CustomEvent";
            HideUnityEvents = true; // hides Unity events in the receiver - meant to be code only
        }

        /// <summary>
        /// find a textMesh to output button status to
        /// </summary>
        private void SetOutput()
        {
            if (Host != null)
            {
                TextMesh mesh = Host.GetComponentInChildren<TextMesh>();

                if (mesh != null)
                {
                    outputString = statusString.Replace("%state%", lastState.Name);

                    if(showClicked != null)
                    {
                        outputString += "\n" + clickString + "(" + clickCount + ")";
                    }

                    if (showVoice != null)
                    {
                        outputString += "\n" + voiceString.Replace("%voiceCommand%", lastVoiceCommand);
                    }

                    mesh.text = outputString;
                }
            }
        }

        /// <summary>
        /// allow the info to remove click info if a click event has expired
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private IEnumerator ClickTimer(float time)
        {
            yield return new WaitForSeconds(time);
            showClicked = null;
        }

        /// <summary>
        /// allow the info to remove voice command info and it expires
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private IEnumerator VoiceTimer(float time)
        {
            yield return new WaitForSeconds(time);
            showVoice = null;
        }

        /// <summary>
        /// Called on update, check to see if the state has changed sense the last call
        /// </summary>
        /// <param name="state"></param>
        /// <param name="source"></param>
        public override void OnUpdate(InteractableStates state, Interactable source)
        {
            if (state.CurrentState() != lastState)
            {
                // the state has changed, do something new
                /*
                bool hasDown = state.GetState(InteractableStates.InteractableStateEnum.Pressed).Value > 0;

                bool focused = state.GetState(InteractableStates.InteractableStateEnum.Focus).Value > 0;

                bool isDisabled = state.GetState(InteractableStates.InteractableStateEnum.Disabled).Value > 0;

                bool hasInteractive = state.GetState(InteractableStates.InteractableStateEnum.Interactive).Value > 0;

                bool hasObservation = state.GetState(InteractableStates.InteractableStateEnum.Observation).Value > 0;

                bool hasObservationTargeted = state.GetState(InteractableStates.InteractableStateEnum.ObservationTargeted).Value > 0;

                bool isTargeted = state.GetState(InteractableStates.InteractableStateEnum.Targeted).Value > 0;

                bool isToggled = state.GetState(InteractableStates.InteractableStateEnum.Toggled).Value > 0;

                bool isVisited = state.GetState(InteractableStates.InteractableStateEnum.Visited).Value > 0;

                bool isDefault = state.GetState(InteractableStates.InteractableStateEnum.Default).Value > 0;

                bool hasGesture = state.GetState(InteractableStates.InteractableStateEnum.Gesture).Value > 0;

                bool hasGestureMax = state.GetState(InteractableStates.InteractableStateEnum.GestureMax).Value > 0;

                bool hasCollistion = state.GetState(InteractableStates.InteractableStateEnum.Collision).Value > 0;

                bool hasCustom = state.GetState(InteractableStates.InteractableStateEnum.Custom).Value > 0;
               
                or: 

                bool hasFocus = source.HasFocus;
                bool hasPress = source.HasPress;
                 */

                lastState = state.CurrentState();
                SetOutput();
            }
        }

        /// <summary>
        /// click happened
        /// </summary>
        /// <param name="state"></param>
        /// <param name="source"></param>
        /// <param name="pointer"></param>
        public override void OnClick(InteractableStates state, Interactable source, IMixedRealityPointer pointer = null)
        {
            base.OnClick(state, source);
            if (Host != null)
            {
                if(showClicked != null)
                {
                    Host.StopCoroutine(showClicked);
                    showClicked = null;
                }

                showClicked = Host.StartCoroutine(ClickTimer(clickTime));
            }

            clickCount++;
            SetOutput();
        }

        /// <summary>
        /// voice command called
        /// </summary>
        /// <param name="state"></param>
        /// <param name="source"></param>
        /// <param name="command"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        public override void OnVoiceCommand(InteractableStates state, Interactable source, string command, int index = 0, int length = 1)
        {
            base.OnVoiceCommand(state, source, command, index, length);
            lastVoiceCommand = command;
            
            if (Host != null)
            {
                if (showVoice != null)
                {
                    Host.StopCoroutine(showVoice);
                    showVoice = null;
                }

                showVoice = Host.StartCoroutine(VoiceTimer(clickTime));
            }

            SetOutput();
        }
    }
}
