// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.Events;
using MixedRealityToolkit.InputModule.EventData;

#if UNITY_WSA || UNITY_STANDALONE_WIN
using UnityEngine.Windows.Speech;
#endif

namespace MixedRealityToolkit.Examples.UX
{
    /// <summary>
    /// InteractiveToggle expands Interactive to expose selection or toggle states.
    /// 
    /// Beyond the basic button functionality, Interactive also maintains the notion of selection and enabled, which allow for more robust UI features.
    /// InteractiveEffects are behaviors that listen for updates from Interactive, which allows for visual feedback to be customized and placed on
    /// individual elements of the Interactive GameObject
    /// </summary>
    public class InteractiveToggle : Interactive
    {

        /// <summary>
        /// Sets the button to act like a navigation button or toggle type button
        /// </summary>
        public bool AllowSelection = true;

        public bool Selection
        {
            get { return AllowSelection; }
            set
            {
                AllowSelection = value;
                if (AllowSelection == false)
                {
                    HasGaze = false;
                }
            }
        }

        /// <summary>
        /// Similar to a Tab button or radial checkbox, once selected, you much select another item to change state
        /// </summary>
        public bool AllowDeselect = true;

        /// <summary>
        /// Current selected state, can be set from the Unity Editor for default behavior
        /// </summary>
        public bool HasSelection = false;
        public void SetSelection(bool selection)
        {
            HasSelection = selection;
        }

        /// <summary>
        /// A Read-only button or visual item. Passive mode ignores input, but updates the visuals as if it were enabled.
        /// Good for things like dashboard lights and data visualization
        /// </summary>
        public bool PassiveMode = false;

        /// <summary>
        /// Exposed Unity Events
        /// </summary>
        
        public UnityEvent OnSelection;
        public UnityEvent OnDeselection;
        
        /// <summary>
        /// Set default visual states on Start
        /// </summary>
        protected override void Start()
        {
            IsSelected = HasSelection;

            base.Start();
        }

        public void SetAllowSelect(bool allowSelect)
        {
            AllowSelection = allowSelect;
        }

        public override void OnInputClicked(InputClickedEventData eventData)
        {
            if (PassiveMode || !IsEnabled)
            {
                return;
            }

            base.OnInputClicked(eventData);

            ToggleLogic();
        }

        public virtual void ToggleLogic()
        {
            if (AllowSelection)
            {
                if (AllowDeselect && IsSelected)
                {
                    IsSelected = false;
                    if (!PassiveMode)
                    {
                        OnDeselection.Invoke();
                    }
                    
                }
                else if (!IsSelected)
                {
                    IsSelected = true;
                    if (!PassiveMode)
                    {
                        OnSelection.Invoke();
                    }

                    if (!AllowDeselect)
                    {
                        HasGaze = false;
                    }
                }
            }

            HasSelection = IsSelected;
        }

        public void SetState(bool isSelected)
        {
            IsSelected = !isSelected;
            ToggleLogic();
        }

        public override void OnFocusEnter()
        {
            if (((AllowDeselect && IsSelected) || !IsSelected ) &&!PassiveMode)
            {
                base.OnFocusEnter();
            }
        }

        public override void OnFocusExit()
        {
            if (((AllowDeselect && IsSelected) || !IsSelected) && !PassiveMode)
            {
                base.OnFocusExit();
            }
        }

        public override void OnHold()
        {
            if (((AllowDeselect && IsSelected) || !IsSelected) && !PassiveMode)
            {
                base.OnHold();
            }
        }

        public override void OnInputDown(InputEventData eventData)
        {
            if (((AllowDeselect && IsSelected) || !IsSelected) && !PassiveMode)
            {
                base.OnInputDown(eventData);
            }
        }

        public override void OnInputUp(InputEventData eventData)
        {

            if (((AllowDeselect && IsSelected) || !IsSelected) && !PassiveMode)
            {
                base.OnInputUp(eventData);
            }
            else
            {
                HasGaze = false;
                HasDown = false;
                UpdateEffects();
            }
        }

        /// <summary>
        /// Run timers and check for updates
        /// </summary>
        protected override void Update()
        {
            if (!UserInitiatedEvent && IsSelected != HasSelection)
            {
                 IsSelected = HasSelection;
            }

            base.Update();
            
        }

#if UNITY_WSA || UNITY_STANDALONE_WIN
        protected override void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            base.KeywordRecognizer_OnPhraseRecognized(args);
            
            // Check to make sure the recognized keyword matches, then invoke the corresponding method.
            if ((!KeywordRequiresGaze || HasGaze) && mKeywordDictionary != null)
            {
                int index;

                if (mKeywordDictionary.TryGetValue(args.text, out index))
                {
                    HasSelection = index == 1;
                }
            }
        }
#endif
    }
}
