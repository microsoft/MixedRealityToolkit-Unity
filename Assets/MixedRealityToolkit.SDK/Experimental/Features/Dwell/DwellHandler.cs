// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Experimental.Dwell
{
    [Serializable]
    public class PointerUnityEvent : UnityEvent<IMixedRealityPointer>
    {
    }

    /// <summary>
    /// Use this component to add a Dwell modality (https://docs.microsoft.com/en-us/windows/mixed-reality/gaze-and-dwell) to the UI target.
    /// </summary>
    public class DwellHandler : MonoBehaviour, IMixedRealityFocusHandler
    {
        protected enum DwellStateType
        {
            None = 0,
            FocusGained,
            DwellIntended,
            DwellStarted,
            DwellCompleted,
            DwellCanceled,
        }

        [Header("Dwell Settings")]
        [SerializeField]
        protected DwellProfile dwellProfile = null;

        [Header("Dwell Events")]
        [SerializeField]
        private PointerUnityEvent DwellIntended = new PointerUnityEvent();

        [SerializeField]
        private PointerUnityEvent DwellStarted = new PointerUnityEvent();

        [SerializeField]
        private PointerUnityEvent DwellCompleted = new PointerUnityEvent();

        [SerializeField]
        private PointerUnityEvent DwellCanceled = new PointerUnityEvent();

        /// <summary>
        /// Invoke this event if the dwell is interrupted with alternate modality
        /// </summary>
        public UnityAction CancelDwellAction;

        /// <summary>
        /// Property exposing the computation for what percentage of dwell has progressed.
        /// </summary>
        public virtual float DwellProgress
        {
            get
            {
                switch (CurrentDwellState)
                {
                    case DwellStateType.None:
                    case DwellStateType.FocusGained:
                        return 0;
                    case DwellStateType.DwellStarted:
                        return Mathf.Clamp((float)(dwellProfile.TimeToCompleteDwell - FillTimer) / dwellProfile.TimeToCompleteDwell, 0f, 1f);
                    case DwellStateType.DwellCompleted:
                        return 1;
                    case DwellStateType.DwellCanceled:
                        if ((DateTime.UtcNow - focusExitTime).TotalSeconds > dwellProfile.TimeToAllowDwellResume)
                        {
                            CurrentDwellState = DwellStateType.None;
                            return 0;
                        }
                        else if (dwellProfile.TimeToAllowDwellResume > 0)
                        {
                            return Mathf.Clamp((float)(dwellProfile.TimeToCompleteDwell - FillTimer) / dwellProfile.TimeToCompleteDwell, 0f, 1f);
                        }
                        break;
                    default:
                        return 0;
                }

                return 0;
            }
        }

        /// <summary>
        /// Cached pointer reference to track focus events maps to teh same pointer that initiated dwell
        /// </summary>
        private IMixedRealityPointer pointer;

        private DateTime focusEnterTime = DateTime.MaxValue;
        private DateTime focusExitTime = DateTime.MaxValue;

        /// <summary>
        /// Exposes whether the target has focus from the pointer defined in dwell profile settings
        /// </summary>
        protected bool HasFocus { get; private set; }

        /// <summary>
        /// Captures the dwell status 
        /// </summary>
        protected DwellStateType CurrentDwellState = DwellStateType.None;

        /// <summary>
        /// Abstracted value for the how long the dwelled object still needs to be focused to comlete the dwell action
        /// </summary>
        protected float FillTimer = 0;

        private void Awake()
        {
            Debug.Assert(dwellProfile != null, "DwellProfile is null, creating default profile.");

            if (dwellProfile == null)
            {
                dwellProfile = ScriptableObject.CreateInstance<DwellProfile>();
            }

            CancelDwellAction += OnDwellCancelAction;
        }

        private void Update()
        {
            UpdateFillTimer();

            if (HasFocus && CurrentDwellState != DwellStateType.DwellCompleted)
            {
                double focusDuration = (DateTime.UtcNow - this.focusEnterTime).TotalSeconds;

                if (CurrentDwellState == DwellStateType.FocusGained && focusDuration >= dwellProfile.DwellIntentDelay)
                {
                    DwellIntended.Invoke(pointer);
                    CurrentDwellState = DwellStateType.DwellIntended;
                }
                else if (CurrentDwellState == DwellStateType.DwellIntended && (focusDuration - dwellProfile.DwellIntentDelay) >= dwellProfile.DwellStartDelay)
                {
                    DwellStarted.Invoke(pointer);
                    CurrentDwellState = DwellStateType.DwellStarted;
                    FillTimer = dwellProfile.TimeToCompleteDwell;
                }
                else if (CurrentDwellState == DwellStateType.DwellStarted && (focusDuration - dwellProfile.DwellIntentDelay - dwellProfile.DwellStartDelay) >= dwellProfile.TimeToCompleteDwell)
                {
                    DwellCompleted.Invoke(pointer);
                    CurrentDwellState = DwellStateType.DwellCompleted;
                }
            }
        }

        /// <summary>
        /// Default FillTimer computation based on profile settings
        /// </summary>
        protected virtual void UpdateFillTimer()
        {
            switch (CurrentDwellState)
            {
                case DwellStateType.None:
                case DwellStateType.FocusGained:
                case DwellStateType.DwellIntended:
                    FillTimer = 0;
                    break;
                case DwellStateType.DwellStarted:
                    FillTimer -= Time.deltaTime;
                    FillTimer = FillTimer < 0 ? 0 : FillTimer;
                    break;
                case DwellStateType.DwellCompleted:
                    Debug.Assert(FillTimer < float.Epsilon, $"Fill timer was not 0. It was {FillTimer}.");
                    break;
                case DwellStateType.DwellCanceled:
                    if ((DateTime.UtcNow - focusExitTime).TotalSeconds > dwellProfile.TimeToAllowDwellResume)
                    {
                        FillTimer = 0;
                        CurrentDwellState = DwellStateType.None;
                    }
                    break;
                default:
                    FillTimer = 0;
                    break;
            }
        }

        public void OnFocusEnter(FocusEventData eventData)
        {
            if (eventData.NewFocusedObject == gameObject
                && eventData.Pointer.InputSourceParent.SourceType == dwellProfile.DwellPointerType)
            {
                HasFocus = true;

                // check intent to resume
                if (CurrentDwellState == DwellStateType.DwellCanceled
                    && pointer.InputSourceParent.SourceId == eventData.Pointer.InputSourceParent.SourceId //make sure the returning pointer id is the same
                    && (DateTime.UtcNow - focusExitTime).TotalSeconds <= dwellProfile.TimeToAllowDwellResume)
                {
                    focusEnterTime = focusEnterTime.AddSeconds(dwellProfile.TimeToAllowDwellResume);
                    CurrentDwellState = DwellStateType.DwellStarted;
                    DwellStarted.Invoke(pointer);
                }
                // dwell state machine re-starts
                if (CurrentDwellState <= DwellStateType.DwellIntended)
                {
                    focusEnterTime = DateTime.UtcNow;
                    CurrentDwellState = DwellStateType.FocusGained;
                    pointer = eventData.Pointer;
                    FillTimer = 0;
                }
            }
        }

        public void OnFocusExit(FocusEventData eventData)
        {
            if (eventData.OldFocusedObject == gameObject
                && eventData.Pointer.InputSourceParent.SourceType == dwellProfile.DwellPointerType
                && pointer.InputSourceParent.SourceId == eventData.Pointer.InputSourceParent.SourceId)
            {
                HasFocus = false;

                if (CurrentDwellState == DwellStateType.DwellStarted)
                {
                    DwellCanceled.Invoke(eventData.Pointer);
                    CurrentDwellState = DwellStateType.DwellCanceled;
                    focusExitTime = DateTime.UtcNow;
                }
                else
                {
                    CurrentDwellState = DwellStateType.None;
                    focusExitTime = DateTime.MinValue;
                }
            }
        }

        /// <summary>
        /// Delegate that can be invoked if external factors (eg. alternate input modality  pre-emptively invoked the target) force the dwell action to prematuturely end
        /// </summary>
        private void OnDwellCancelAction()
        {
            DwellCanceled.Invoke(pointer);
            focusEnterTime = DateTime.MaxValue;
            CurrentDwellState = DwellStateType.None;
            focusExitTime = DateTime.MinValue;
        }
    }
}

