// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;

#if UNITY_WSA || UNITY_STANDALONE_WIN
using UnityEngine.Windows.Speech;
#endif

namespace HoloToolkit.Examples.InteractiveElements
{
    /// <summary>
    /// Interactive exposes basic button type events to the Unity Editor and receives messages from the GestureManager and GazeManager.
    /// 
    /// Beyond the basic button functionality, Interactive also maintains the notion of selection and enabled, which allow for more robust UI features.
    /// InteractiveEffects are behaviors that listen for updates from Interactive, which allows for visual feedback to be customized and placed on
    /// individual elements of the Interactive GameObject
    /// </summary>
    public class Interactive : MonoBehaviour, IInputClickHandler, IFocusable, IInputHandler
    {

        public GameObject ParentObject;

        /// <summary>
        /// Should the button listen to input?
        /// </summary>
        public bool IsEnabled = true;

        /// <summary>
        /// Does the GameObject currently have focus?
        /// </summary>
        public bool HasGaze { get; protected set; }

        /// <summary>
        /// Is the Tap currently in the down state?
        /// </summary>
        public bool HasDown { get; protected set; }

        /// <summary>
        /// Should the button care about click and hold?
        /// </summary>
        public bool DetectHold = false;

        /// <summary>
        /// Configure the amount to time for the hold event to fire
        /// </summary>
        public float HoldTime = 0.5f;

        /// <summary>
        /// Configure the amount of time a roll off update should incur. When building more advanced UI,
        /// we may need to evaluate what the next gazed item is before updating.
        /// </summary>
        public float RollOffTime = 0.02f;

        /// <summary>
        /// Current selected state, can be set from the Unity Editor for default behavior
        /// </summary>
        public bool IsSelected { get; protected set; }

        [Tooltip("Set a keyword to invoke the OnSelect event")]
        public string Keyword = "";

        [Tooltip("Gaze is required for the keyword to trigger this Interactive.")]
        public bool KeywordRequiresGaze = true;

        /// <summary>
        /// Exposed Unity Events
        /// </summary>
        public UnityEvent OnSelectEvents;
        public UnityEvent OnDownEvent;
        public UnityEvent OnHoldEvent;

        /// <summary>
        /// A button typically has 8 potential states.
        /// We can update visual feedback based on state change, all the logic is already done, making InteractiveEffects behaviors less complex then comparing selected + Disabled.
        /// </summary>
        public enum ButtonStateEnum { Default, Focus, Press, Selected, FocusSelected, PressSelected, Disabled, DisabledSelected }
        protected ButtonStateEnum mState = ButtonStateEnum.Default;

        /// <summary>
        /// Timers
        /// </summary>
        protected float mRollOffTimer = 0;
        protected float mHoldTimer = 0;
        protected bool mCheckRollOff = false;
        protected bool mCheckHold = false;

#if UNITY_WSA || UNITY_STANDALONE_WIN
        protected KeywordRecognizer mKeywordRecognizer;
#endif
        protected Dictionary<string, int> mKeywordDictionary;
        protected string[] mKeywordArray;

        /// <summary>
        /// Internal comparison variables to allow for live state updates no matter the input method
        /// </summary>
        protected bool mIgnoreSelect = false;
        protected bool mCheckEnabled = false;
        protected bool mCheckSelected = false;
        protected bool UserInitiatedEvent = false;
        protected bool mAllowSelection = false;

        protected List<InteractiveWidget> mInteractiveWidgets = new List<InteractiveWidget>();

        protected virtual void Awake()
        {
            if (ParentObject == null)
            {
                ParentObject = this.gameObject;
            }

            CollectWidgets(forceCollection: true);
        }

        /// <summary>
        /// Set default visual states on Start
        /// </summary>
        protected virtual void Start()
        {
            if (Keyword != "")
            {
                mKeywordArray = new string[1] { Keyword };
                if (Keyword.IndexOf(',') > -1)
                {
                    mKeywordArray = Keyword.Split(',');

                    mKeywordDictionary = new Dictionary<string, int>();
                    for (int i = 0; i < mKeywordArray.Length; ++i)
                    {
                        mKeywordDictionary.Add(mKeywordArray[i], i);
                    }
                }

#if UNITY_WSA || UNITY_STANDALONE_WIN
                if (!KeywordRequiresGaze)
                {
                    mKeywordRecognizer = new KeywordRecognizer(mKeywordArray);
                    mKeywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
                    mKeywordRecognizer.Start();
                }
#endif

            }

            mCheckEnabled = IsEnabled;
            mCheckSelected = IsSelected;

            UpdateEffects();
        }

        /// <summary>
        /// An OnTap event occurred
        /// </summary>
        public virtual void OnInputClicked(InputClickedEventData eventData)
        {
            if (!IsEnabled)
            {
                return;
            }

            UserInitiatedEvent = true;

            if (mIgnoreSelect)
            {
                mIgnoreSelect = false;
                return;
            }

            UpdateEffects();

            OnSelectEvents.Invoke();
        }

        /// <summary>
        /// The gameObject received gaze
        /// </summary>
        public virtual void OnFocusEnter()
        {
            if (!IsEnabled)
            {
                return;
            }

            HasGaze = true;

            SetKeywordListener(true);

            UpdateEffects();
        }

        /// <summary>
        /// The gameObject no longer has gaze
        /// </summary>
        public virtual void OnFocusExit()
        {
            HasGaze = false;
            EndHoldDetection();
            mRollOffTimer = 0;
            mCheckRollOff = true;
            SetKeywordListener(false);
            UpdateEffects();
        }

        private void SetKeywordListener(bool listen)
        {
#if UNITY_WSA || UNITY_STANDALONE_WIN
            if (listen)
            {
                if (KeywordRequiresGaze && mKeywordArray != null)
                {
                    if (mKeywordRecognizer == null)
                    {
                        mKeywordRecognizer = new KeywordRecognizer(mKeywordArray);
                        mKeywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
                        mKeywordRecognizer.Start();
                    }
                    else
                    {
                        if (!mKeywordRecognizer.IsRunning)
                        {
                            mKeywordRecognizer.Start();
                        }
                    }
                }
            }
            else
            {
                if (mKeywordRecognizer != null && KeywordRequiresGaze)
                {
                    if (mKeywordRecognizer.IsRunning)
                    {
                        mKeywordRecognizer.Stop();
                        mKeywordRecognizer.OnPhraseRecognized -= KeywordRecognizer_OnPhraseRecognized;
                        mKeywordRecognizer.Dispose();
                        mKeywordRecognizer = null;
                    }
                }
            }
#endif
        }

        /// <summary>
        /// The user is initiating a tap or hold
        /// </summary>
        public virtual void OnInputDown(InputEventData eventData)
        {
            if (!HasGaze)
            {
                return;
            }

            HasDown = true;
            mCheckRollOff = false;

            if (DetectHold)
            {
                mHoldTimer = 0;
                mCheckHold = true;
            }
            UpdateEffects();

            OnDownEvent.Invoke();
        }

        /// <summary>
        /// All tab, hold, and gesture events are completed
        /// </summary>
        public virtual void OnInputUp(InputEventData eventData)
        {
            mCheckHold = false;
            HasDown = false;
            mIgnoreSelect = false;
            EndHoldDetection();
            mCheckRollOff = false;

            UpdateEffects();
        }

        /// <summary>
        /// The hold timer has finished
        /// </summary>
        public virtual void OnHold()
        {
            mIgnoreSelect = true;
            EndHoldDetection();

            UpdateEffects();

            OnHoldEvent.Invoke();
        }

        /// <summary>
        /// The percentage of hold time completed
        /// </summary>
        /// <returns>percentage 0 - 1</returns>
        public float GetHoldPercentage()
        {
            return mHoldTimer / HoldTime;
        }

        protected void EndHoldDetection()
        {
            mHoldTimer = 0;
            mCheckHold = false;
        }

        private void CollectWidgets(bool forceCollection = false)
        {
            if (mInteractiveWidgets.Count == 0 || forceCollection)
            {
                if (ParentObject != null)
                {
                    ParentObject.GetComponentsInChildren(mInteractiveWidgets);
                }
                for (int i = 0; i < mInteractiveWidgets.Count; ++i)
                {
                    if (mInteractiveWidgets[i].InteractiveHost == null)
                    {
                        mInteractiveWidgets[i].InteractiveHost = this;
                    }
                    else
                    {
                        mInteractiveWidgets.RemoveAt(i);
                        --i;
                    }
                }
            }
        }

        /// <summary>
        /// Loop through all InteractiveEffects on child elements and update their states
        /// </summary>
        protected void UpdateEffects()
        {
            CollectWidgets();

            CompareStates();

            int interactiveCount = mInteractiveWidgets.Count;
            for (int i = 0; i < interactiveCount; ++i)
            {
                InteractiveWidget widget = mInteractiveWidgets[i];
                widget.SetState(mState);

                int currentCount = mInteractiveWidgets.Count;
                if (currentCount < interactiveCount)
                {
                    Debug.LogWarningFormat("Call to {0}'s SetState removed other interactive widgets. GameObject name: {1}.", widget.GetType(), widget.name);
                    interactiveCount = currentCount;
                }
            }
        }

        public void RegisterWidget(InteractiveWidget widget)
        {
            CollectWidgets();
            if (mInteractiveWidgets.Contains(widget))
            {
                return;
            }

            mInteractiveWidgets.Add(widget);
            widget.SetState(mState);
        }

        public void UnregisterWidget(InteractiveWidget widget)
        {
            if (mInteractiveWidgets != null)
            {
                mInteractiveWidgets.Remove(widget);
            }
        }

#if UNITY_WSA || UNITY_STANDALONE_WIN
        protected virtual void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {

            // Check to make sure the recognized keyword matches, then invoke the corresponding method.
            if (args.text == Keyword && (!KeywordRequiresGaze || HasGaze) && IsEnabled)
            {
                if (mKeywordDictionary == null)
                {
                    OnInputClicked(null);
                }
            }
        }
#endif

        /// <summary>
        /// Check if any state changes have occurred, from alternate input sources
        /// </summary>
        protected void CompareStates()
        {
            if (IsEnabled)
            {
                // all states
                if (IsSelected)
                {
                    if (HasGaze)
                    {
                        if (HasDown)
                        {
                            mState = ButtonStateEnum.PressSelected;
                        }
                        else
                        {
                            mState = ButtonStateEnum.FocusSelected;
                        }
                    }
                    else
                    {
                        if (HasDown)
                        {
                            mState = ButtonStateEnum.PressSelected;
                        }
                        else
                        {
                            mState = ButtonStateEnum.Selected;
                        }
                    }
                }
                else
                {
                    if (HasGaze)
                    {
                        if (HasDown)
                        {
                            mState = ButtonStateEnum.Press;
                        }
                        else
                        {
                            mState = ButtonStateEnum.Focus;
                        }
                    }
                    else
                    {
                        if (HasDown)
                        {
                            mState = ButtonStateEnum.Press;
                        }
                        else
                        {
                            mState = ButtonStateEnum.Default;
                        }
                    }
                }

            }
            else
            {
                if (IsSelected)
                {
                    mState = ButtonStateEnum.DisabledSelected;
                }
                else
                {
                    mState = ButtonStateEnum.Disabled;
                }
            }
            mCheckSelected = IsSelected;
            mCheckEnabled = IsEnabled;
        }

        /// <summary>
        /// Run timers and check for updates
        /// </summary>
        protected virtual void Update()
        {

            if (mCheckRollOff && HasDown)
            {
                if (mRollOffTimer < RollOffTime)
                {
                    mRollOffTimer += Time.deltaTime;
                }
                else
                {
                    mCheckRollOff = false;
                    OnInputUp(null);
                }
            }
            if (mCheckHold)
            {
                if (mHoldTimer < HoldTime)
                {
                    mHoldTimer += Time.deltaTime;
                }
                else
                {
                    mCheckHold = false;
                    OnHold();
                }
            }

            if (!UserInitiatedEvent && (mCheckEnabled != IsEnabled || mCheckSelected != IsSelected))
            {
                UpdateEffects();
            }

            UserInitiatedEvent = false;
        }

        protected virtual void OnDestroy()
        {
            SetKeywordListener(false);
        }

        protected virtual void OnEnable()
        {
#if UNITY_WSA || UNITY_STANDALONE_WIN
            if (mKeywordRecognizer != null && !KeywordRequiresGaze)
            {
                SetKeywordListener(true);
            }
#endif
        }

        protected virtual void OnDisable()
        {
            OnFocusExit();
        }
    }
}
