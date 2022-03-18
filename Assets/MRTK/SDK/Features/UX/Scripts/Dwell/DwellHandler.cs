// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Dwell
{
    /// <summary>
    /// Use this component to add a Dwell modality (https://docs.microsoft.com/windows/mixed-reality/gaze-and-dwell) to the UI target.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/DwellHandler")]
    public class DwellHandler : MonoBehaviour, IMixedRealityFocusChangedHandler
    {
        [Tooltip("The profile to use with this handler")]
        [Header("Dwell Settings")]
        [SerializeField]
        [FormerlySerializedAs("DwellProfile")]
        protected DwellProfile dwellProfile = null;

        [Tooltip("The event to trigger when being focused longer than the DwellIntentDelay")]
        [Header("Dwell Events")]
        [SerializeField]
        [FormerlySerializedAs("DwellIntended")]
        private DwellUnityEvent dwellIntended = new DwellUnityEvent();

        [Tooltip("The event to trigger when being focused longer than the DwellStartDelay after the DwellIntentDelay")]
        [SerializeField]
        [FormerlySerializedAs("DwellStarted")]
        private DwellUnityEvent dwellStarted = new DwellUnityEvent();

        [Tooltip("The event to trigger when being focused longer than the TimeToCompleteDwell after the DwellStartDelay")]
        [SerializeField]
        [FormerlySerializedAs("DwellCompleted")]
        private DwellUnityEvent dwellCompleted = new DwellUnityEvent();

        [Tooltip("The event to trigger when losing focus while being in the dwell started state")]
        [SerializeField]
        [FormerlySerializedAs("DwellCanceled")]
        private DwellUnityEvent dwellCanceled = new DwellUnityEvent();

        /// <summary>
        /// The profile to use with this handler
        /// </summary>
        public DwellProfile DwellProfile
        {
            get => dwellProfile;
            set => dwellProfile = value;
        }

        /// <summary>
        /// The event to trigger when being focused longer than the DwellIntentDelay
        /// </summary>
        public DwellUnityEvent DwellIntended
        {
            get => dwellIntended;
            set => dwellIntended = value;
        }

        /// <summary>
        /// The event to trigger when being focused longer than the DwellStartDelay after the DwellIntentDelay
        /// </summary>
        public DwellUnityEvent DwellStarted
        {
            get => dwellStarted;
            set => dwellStarted = value;
        }

        /// <summary>
        /// The event to trigger when being focused longer than the TimeToCompleteDwell after the DwellStartDelay
        /// </summary>
        public DwellUnityEvent DwellCompleted
        {
            get => dwellCompleted;
            set => dwellCompleted = value;
        }

        /// <summary>
        /// The event to trigger when losing focus while being in the dwell started state
        /// </summary>
        public DwellUnityEvent DwellCanceled
        {
            get => dwellCanceled;
            set => dwellCanceled = value;
        }

        /// <summary>
        /// Captures the dwell status 
        /// </summary>
        public DwellStateType CurrentDwellState { get; protected set; } = DwellStateType.None;

        /// <summary>
        /// Property exposing the computation for what percentage of dwell has progressed, ranging from 0 to 1.
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
                        if (dwellProfile.TimeToAllowDwellResume > 0)
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
        /// Exposes whether the target has focus from the pointer type defined in dwell profile settings
        /// </summary>
        protected bool HasFocus { get; private set; }

        /// <summary>
        /// Abstracted value for the how long the dwelled object still needs to be focused to complete the dwell action
        /// Value ranges from 0 to "TimeToCompleteDwell" setting in the dwellprofile. This picks up the same unit as TimeToCompleteDwell
        /// </summary>
        protected float FillTimer { get; set; } = 0;

        /// <summary>
        /// Cached pointer reference to track focus events maps to the same pointer id that initiated dwell
        /// </summary>
        private IMixedRealityPointer pointer;

        private int pointerCount = 0;

        private float focusEnterTime = float.MaxValue;
        private float focusExitTime = float.MaxValue;

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
        /// </summary>
        private void Update()
        {
            UpdateFillTimer();

            if (HasFocus && CurrentDwellState != DwellStateType.DwellCompleted)
            {
                float focusDuration = Time.time - focusEnterTime;

                if (CurrentDwellState == DwellStateType.FocusGained && focusDuration >= dwellProfile.DwellIntentDelay)
                {
                    CurrentDwellState = DwellStateType.DwellIntended;
                    dwellIntended.Invoke(pointer);
                }
                else if (CurrentDwellState == DwellStateType.DwellIntended && (focusDuration - dwellProfile.DwellIntentDelay) >= dwellProfile.DwellStartDelay)
                {
                    CurrentDwellState = DwellStateType.DwellStarted;
                    dwellStarted.Invoke(pointer);
                }
                else if (CurrentDwellState == DwellStateType.DwellStarted && FillTimer >= dwellProfile.TimeToCompleteDwell)
                {
                    CurrentDwellState = DwellStateType.DwellCompleted;
                    dwellCompleted.Invoke(pointer);
                }
            }
        }

        /// <summary>
        /// Get the current progess of dwell. Return value ranges from 0 to 1.
        /// </summary>
        protected float GetCurrentDwellProgress()
        {
            return Mathf.Clamp(FillTimer / dwellProfile.TimeToCompleteDwell, 0f, 1f);
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
                    bool dwellCompleted = false;
                    if (dwellProfile.DecayDwellOverTime)
                    {
                        FillTimer -= Time.deltaTime * dwellProfile.TimeToCompleteDwell / dwellProfile.TimeToAllowDwellResume;
                        dwellCompleted = FillTimer <= 0;
                    }
                    else
                    {
                        dwellCompleted = (Time.time - focusExitTime) > dwellProfile.TimeToAllowDwellResume;
                    }

                    if (FillTimer <= 0)
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

        /// <inheritdoc/>
        public void OnFocusChanged(FocusEventData eventData)
        {
            InputSourceType inputSourceType = eventData.Pointer is GGVPointer ? InputSourceType.Head : eventData.Pointer.InputSourceParent.SourceType;

            if (eventData.NewFocusedObject == gameObject
                && inputSourceType == dwellProfile.DwellPointerType)
            {
                if (!HasFocus)
                {
                    HasFocus = true;

                    // check intent to resume
                    if (CurrentDwellState == DwellStateType.DwellCanceled
                        && (Time.time - focusExitTime) <= dwellProfile.TimeToAllowDwellResume)
                    {
                        // Add the time duration focus was away since this is a dwell resume and we need to account for the time that focus was lost for the target.
                        // Assigning this the current time would restart computation for dwell progress.
                        focusEnterTime += Time.time - focusExitTime;
                        CurrentDwellState = DwellStateType.DwellStarted;
                        dwellStarted.Invoke(pointer);
                    }
                    // dwell state machine re-starts
                    else if (CurrentDwellState <= DwellStateType.DwellIntended)
                    {
                        focusEnterTime = Time.time;
                        CurrentDwellState = DwellStateType.FocusGained;
                        pointer = eventData.Pointer;
                        FillTimer = 0;
                    }
                }
                pointerCount++;

            }
            else if (eventData.OldFocusedObject == gameObject
                && inputSourceType == dwellProfile.DwellPointerType)
            {
                pointerCount--;
                if (pointerCount == 0)
                {
                    HasFocus = false;

                    if (CurrentDwellState == DwellStateType.DwellStarted)
                    {
                        dwellCanceled.Invoke(eventData.Pointer);
                        CurrentDwellState = DwellStateType.DwellCanceled;
                        focusExitTime = Time.time;
                    }
                    else
                    {
                        CurrentDwellState = DwellStateType.None;
                        focusExitTime = float.MaxValue;
                    }
                }
            }
        }

        /// <summary>
        /// Method that can be invoked if external factors (e.g. alternate input modality  preemptively invoked the target) force the dwell action to prematurely end
        /// </summary>
        public virtual void CancelDwell()
        {
            dwellCanceled.Invoke(pointer);
            focusEnterTime = float.MaxValue;
            CurrentDwellState = DwellStateType.None;
            focusExitTime = float.MaxValue;
            FillTimer = 0;
        }

        /// <inheritdoc/>
        public void OnBeforeFocusChange(FocusEventData eventData) { }

    }
}

