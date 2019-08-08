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
        protected enum DwellState
        {
            None = 0,
            FocusGained,
            DwellIntended,
            DwellStarted,
            DwellCompleted,
            DwellCanceled,
            Invalid
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
        public UnityAction<bool> CancelDwellAction;

        protected bool HasFocus { get; private set; }

        /// <summary>
        /// Cached pointer reference to track focus events maps to teh same pointer that initiated dwell
        /// </summary>
        private IMixedRealityPointer pointerRef;

        protected DateTime focusEnterTime = DateTime.MaxValue;
        protected DateTime focusExitTime = DateTime.MaxValue;
        protected DwellState currentDwellState = DwellState.None;

        private void Awake()
        {
            Debug.Assert(dwellProfile != null, "DwellProfile is null, creating default profile.");

            if (dwellProfile == null)
            {
                dwellProfile = ScriptableObject.CreateInstance("DwellProfile") as DwellProfile;
            }

            CancelDwellAction += OnDwellCancelAction;
        }

        private void Update()
        {
            if (HasFocus && currentDwellState != DwellState.DwellCompleted)
            {
                double focusTime = (DateTime.UtcNow - this.focusEnterTime).TotalSeconds;

                if (focusTime >= dwellProfile.DwellIntentDelay && currentDwellState == DwellState.FocusGained)
                {
                    DwellIntended.Invoke(pointerRef);
                    currentDwellState = DwellState.DwellIntended;
                }
                else if ((focusTime - dwellProfile.DwellIntentDelay) >= dwellProfile.DwellStartDelay && currentDwellState == DwellState.DwellIntended)
                {
                    DwellStarted.Invoke(pointerRef);
                    currentDwellState = DwellState.DwellStarted;
                }
                else if ((focusTime - dwellProfile.DwellIntentDelay - dwellProfile.DwellStartDelay) >= dwellProfile.TimeToCompleteDwell && currentDwellState == DwellState.DwellStarted)
                {
                    DwellCompleted.Invoke(pointerRef);
                    currentDwellState = DwellState.DwellCompleted;
                }
            }
        }

        public virtual float CalculateDwellProgress(float lastProgressValue)
        {
            float dwellProgress = 0;

            switch (currentDwellState)
            {
                case DwellState.None:
                    dwellProgress =  0;
                    break;
                case DwellState.FocusGained:
                    dwellProgress = 0;
                    break;
                case DwellState.DwellStarted:
                    dwellProgress = Mathf.Clamp((float)(DateTime.UtcNow - focusEnterTime.AddSeconds(dwellProfile.DwellIntentDelay + dwellProfile.DwellStartDelay)).TotalSeconds 
                        / dwellProfile.TimeToCompleteDwell,
                        0f, 1f);
                    break;
                case DwellState.DwellCompleted:
                    dwellProgress = 1;
                    break;
                case DwellState.DwellCanceled:
                case DwellState.Invalid:
                default:
                    return dwellProgress;
            }

            return dwellProgress;
        }

        public void OnFocusEnter(FocusEventData eventData)
        {
            if (eventData.NewFocusedObject == gameObject 
                && eventData.Pointer.InputSourceParent.SourceType == dwellProfile.DwellPointerType)
            {
                HasFocus = true;

                // dwell state machine re-starts
                if (currentDwellState <= DwellState.DwellIntended
                    || !dwellProfile.AllowDwellResume || (DateTime.UtcNow - focusExitTime).TotalSeconds > dwellProfile.TimeToAllowDwellResume)
                {
                    focusEnterTime = DateTime.UtcNow;
                    currentDwellState = DwellState.FocusGained;
                    pointerRef = eventData.Pointer;
                }
                //intent to resume 
                else if (currentDwellState == DwellState.DwellCanceled && dwellProfile.AllowDwellResume 
                    && pointerRef.InputSourceParent.SourceId == eventData.Pointer.InputSourceParent.SourceId //make sure the returning pointer id is the same
                    && (DateTime.UtcNow - focusExitTime).TotalSeconds <= dwellProfile.TimeToAllowDwellResume)
                {
                    focusEnterTime = focusEnterTime.AddSeconds(dwellProfile.TimeToAllowDwellResume); //todo: is this correct ?
                    currentDwellState = DwellState.DwellStarted;
                    DwellStarted.Invoke(pointerRef);
                }
            }
        }

        public void OnFocusExit(FocusEventData eventData)
        {
            if (eventData.OldFocusedObject == gameObject 
                && eventData.Pointer.InputSourceParent.SourceType == dwellProfile.DwellPointerType
                && pointerRef.InputSourceParent.SourceId == eventData.Pointer.InputSourceParent.SourceId)
            {
                HasFocus = false;

                if (currentDwellState == DwellState.DwellStarted)
                {
                    DwellCanceled.Invoke(eventData.Pointer);
                    currentDwellState = DwellState.DwellCanceled;
                    focusExitTime = DateTime.UtcNow;
                }
                else 
                {
                    currentDwellState = DwellState.None;
                    focusExitTime = DateTime.MinValue;
                }
            }
        }

        private void OnDwellCancelAction(bool isPermanent = false)
        {
            if (!isPermanent)
            {
                DwellCanceled.Invoke(pointerRef);
                focusEnterTime = DateTime.MaxValue;
                currentDwellState = DwellState.None;
                focusExitTime = DateTime.MinValue;
            }
        }
    }
}
