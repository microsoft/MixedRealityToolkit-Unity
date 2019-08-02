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

    public class DwellHandler : MonoBehaviour, IMixedRealityFocusHandler
    {
        [SerializeField]
        protected DwellProfile dwellProfile = null;

        [SerializeField]
        private bool allowDwellResume = false;

        [SerializeField]
        private float timeToAllowDwellResume = 1;

        [SerializeField]
        private PointerUnityEvent DwellIntended = new PointerUnityEvent();

        [SerializeField]
        private PointerUnityEvent DwellStarted = new PointerUnityEvent();

        [SerializeField]
        private PointerUnityEvent DwellCompleted = new PointerUnityEvent();

        [SerializeField]
        private PointerUnityEvent DwellCanceled = new PointerUnityEvent();

        public UnityAction<bool> CancelDwellAction;

        public bool dwellInteractionEnabled;

        protected float DwellProgress;
        protected bool HasFocus { get; private set; }

        protected DwellState currentDwellState = DwellState.None;

        public Action OnDwellInterrupted;
        private IMixedRealityPointer pointerRef;
        private DateTime focusEnterTime = DateTime.MaxValue;
        private DateTime focusExitTime = DateTime.MaxValue;

        protected enum DwellState
        {
            FocusGained = 0,
            DwellIntended,
            DwellStarted,
            DwellCompleted,
            DwellCanceled,
            None
        }

        private void Awake()
        {
            if (dwellProfile == null)
            {
                dwellProfile = ScriptableObject.CreateInstance("DwellProfile") as DwellProfile;
            }
            Debug.Assert(dwellProfile != null, "DwellProfile is null");

            CancelDwellAction += OnDwellCancelAction;
        }

        private void OnDwellCancelAction(bool isPermanent = false)
        {
            if (!isPermanent)
            {
                DwellCanceled.Invoke(pointerRef);
                focusEnterTime = DateTime.MaxValue;
            }
        }

        public virtual float CalculateDwellProgress()
        {
            switch (currentDwellState)
            {
                case DwellState.None:
                    DwellProgress =  0;
                    break;
                case DwellState.FocusGained:
                    DwellProgress = 0;
                    break;
                case DwellState.DwellStarted:
                    DwellProgress = Mathf.Clamp((float)(DateTime.UtcNow - focusEnterTime.AddSeconds(dwellProfile.DwellIntentDelay + dwellProfile.DwellStartDelay)).TotalSeconds 
                        / dwellProfile.TimeToCompleteDwell,
                        0f, 1f);
                    break;
                case DwellState.DwellCompleted:
                    DwellProgress = 1;
                    break;
                case DwellState.DwellCanceled:
                default:
                    return DwellProgress;
            }

            return DwellProgress;
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

        public void OnFocusEnter(FocusEventData eventData)
        {
            if (eventData.NewFocusedObject == gameObject 
                && eventData.Pointer.InputSourceParent.SourceType == dwellProfile.DwellPointerType)
            {
                HasFocus = true;
                if (currentDwellState == DwellState.None || currentDwellState == DwellState.DwellIntended || !allowDwellResume || (DateTime.UtcNow - focusExitTime).TotalSeconds > timeToAllowDwellResume)
                {
                    focusEnterTime = DateTime.UtcNow;
                    currentDwellState = DwellState.FocusGained;
                    pointerRef = eventData.Pointer;
                }
                //potential intent to resume //todo: and pointer id is same
                else if (currentDwellState == DwellState.DwellCanceled && allowDwellResume  
                    && (DateTime.UtcNow - focusExitTime).TotalSeconds <= timeToAllowDwellResume)
                {
                    focusEnterTime.AddSeconds(timeToAllowDwellResume);
                    currentDwellState = DwellState.DwellStarted;
                    DwellStarted.Invoke(pointerRef);
                }
            }
        }

        public void OnFocusExit(FocusEventData eventData)
        {
            if (eventData.OldFocusedObject == gameObject 
                && eventData.Pointer.InputSourceParent.SourceType == dwellProfile.DwellPointerType)
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
    }
}
