// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Dwell
{
    /// <summary>
    /// Use this component to add a Dwell modality (https://docs.microsoft.com/windows/mixed-reality/gaze-and-dwell) to the UI target.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/DwellHandler")]
    public class DwellHandler : MonoBehaviour, IMixedRealityFocusChangedHandler
    {
        /// <summary>
        /// None = 0 - Default state
        /// FocusGained - State reached when Focus enters target
        /// DwellIntended - State reached when Focus stays on target for dwellIntentDelay seconds. Signifies user's intent to interact with the target.
        /// DwellStarted - State reached when Focus stays on target for dwellIntentDelay + dwellStartDelay seconds. Typically tied to invoke feedback for dwell.
        /// DwellCompleted - State reached when Focus stays on target for dwellIntentDelay + dwellStartDelay + timeToCompleteDwell seconds. Typically invokes the button clicked event.
        /// DwellCanceled - State reached when DwellStarted state is reached but focus exits the target before timeToCompleteDwell.
        /// </summary>
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
        private DwellUnityEvent DwellIntended = new DwellUnityEvent();

        [SerializeField]
        private DwellUnityEvent DwellStarted = new DwellUnityEvent();

        [SerializeField]
        private DwellUnityEvent DwellCompleted = new DwellUnityEvent();

        [SerializeField]
        private DwellUnityEvent DwellCanceled = new DwellUnityEvent();

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
                        return GetCurrentDwellProgress();
                    case DwellStateType.DwellCompleted:
                        return 1;
                    case DwellStateType.DwellCanceled:
                        if (dwellProfile.TimeToAllowDwellResume > TimeSpan.Zero)
                        {
                            return GetCurrentDwellProgress();
                        }
                        break;
                    default:
                        return 0;
                }

                return 0;
            }
        }

        /// <summary>
        /// Cached pointer reference to track focus events maps to the same pointer id that initiated dwell
        /// </summary>
        private IMixedRealityPointer pointer;

        private int pointerCount = 0;

        private DateTime focusEnterTime = DateTime.MaxValue;
        private DateTime focusExitTime = DateTime.MaxValue;

        /// <summary>
        /// Exposes whether the target has focus from the pointer type defined in dwell profile settings
        /// </summary>
        protected bool HasFocus { get; private set; }

        /// <summary>
        /// Captures the dwell status 
        /// </summary>
        protected DwellStateType CurrentDwellState = DwellStateType.None;

        /// <summary>
        /// Abstracted value for the how long the dwelled object still needs to be focused to complete the dwell action
        /// Value ranges from 0 to "TimeToCompleteDwell" setting in the dwellprofile. This picks up the same unit as TimeToCompleteDwell
        /// </summary>
        protected float FillTimer = 0;

        private void Awake()
        {
            Debug.Assert(dwellProfile != null, "DwellProfile is null, creating default profile.");

            if (dwellProfile == null)
            {
                dwellProfile = ScriptableObject.CreateInstance<DwellProfile>();
            }
        }

        /// <summary>
        /// Valid state transitions for default implementation
        /// Current State | Valid Transitions | Condition (if any)
        /// None | FocusGained
        /// FocusGained | None
        /// FocusGained | DwellIntended
        /// DwellIntended | DwellStarted
        /// DwellIntended | None
        /// DwellCanceled | None
        /// DwellCanceled | DwellStarted | dwellProfile.TimeToAllowDwellResume > 0
        /// DwellStarted | DwellCompleted
        /// DwellStarted | DwellCanceled
        /// </summary>
        private void Update()
        {
            UpdateFillTimer();

            if (HasFocus && CurrentDwellState != DwellStateType.DwellCompleted)
            {
                TimeSpan focusDuration = (DateTime.UtcNow - this.focusEnterTime);

                if (CurrentDwellState == DwellStateType.FocusGained && focusDuration >= dwellProfile.DwellIntentDelay)
                {
                    CurrentDwellState = DwellStateType.DwellIntended;
                    DwellIntended.Invoke(pointer);
                }
                else if (CurrentDwellState == DwellStateType.DwellIntended && (focusDuration - dwellProfile.DwellIntentDelay) >= dwellProfile.DwellStartDelay)
                {
                    CurrentDwellState = DwellStateType.DwellStarted;
                    DwellStarted.Invoke(pointer);
                }
                else if (CurrentDwellState == DwellStateType.DwellStarted && FillTimer >= dwellProfile.TimeToCompleteDwell.TotalSeconds)
                {
                    CurrentDwellState = DwellStateType.DwellCompleted;
                    DwellCompleted.Invoke(pointer);
                }
            }
        }

        private float GetCurrentDwellProgress()
        {
            return Mathf.Clamp(FillTimer / (float)dwellProfile.TimeToCompleteDwell.TotalSeconds, 0f, 1f);
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
                    FillTimer += Time.deltaTime;
                    break;
                case DwellStateType.DwellCompleted:
                    break;
                case DwellStateType.DwellCanceled:
                    // this is a conditional state transition and can be overridden by the deriving class as per profile settings.
                    if ((DateTime.UtcNow - focusExitTime) > dwellProfile.TimeToAllowDwellResume)
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

        public void OnFocusChanged(FocusEventData eventData)
        {
            if (eventData.NewFocusedObject == gameObject
                && eventData.Pointer.InputSourceParent.SourceType == dwellProfile.DwellPointerType)
            {
                if (!HasFocus)
                {
                    HasFocus = true;
                    
                    // check intent to resume
                    if (CurrentDwellState == DwellStateType.DwellCanceled
                        && (DateTime.UtcNow - focusExitTime) <= dwellProfile.TimeToAllowDwellResume)
                    {
                        // Add the time duration focus was away since this is a dwell resume and we need to account for the time that focus was lost for the target.
                        // Assigning this the current time would restart computation for dwell progress.
                        focusEnterTime = focusEnterTime.AddSeconds((DateTime.UtcNow - focusExitTime).TotalSeconds);
                        CurrentDwellState = DwellStateType.DwellStarted;
                        DwellStarted.Invoke(pointer);
                    }
                    // dwell state machine re-starts
                    else if (CurrentDwellState <= DwellStateType.DwellIntended)
                    {
                        focusEnterTime = DateTime.UtcNow;
                        CurrentDwellState = DwellStateType.FocusGained;
                        pointer = eventData.Pointer;
                        FillTimer = 0;
                    }
                }
                pointerCount++;
                
            }
            else if (eventData.OldFocusedObject == gameObject
                && eventData.Pointer.InputSourceParent.SourceType == dwellProfile.DwellPointerType)
            {
                pointerCount--;
                if (pointerCount == 0)
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
                        focusExitTime = DateTime.MaxValue;
                    }
                }
            }
        }

        /// <summary>
        /// Method that can be invoked if external factors (e.g. alternate input modality  preemptively invoked the target) force the dwell action to prematurely end
        /// </summary>
        public virtual void CancelDwell()
        {
            DwellCanceled.Invoke(pointer);
            focusEnterTime = DateTime.MaxValue;
            CurrentDwellState = DwellStateType.None;
            focusExitTime = DateTime.MaxValue;
            FillTimer = 0;
        }

        public void OnBeforeFocusChange(FocusEventData eventData) { }

    }
}

