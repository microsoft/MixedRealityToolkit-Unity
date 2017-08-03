// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace HoloToolkit.Examples.InteractiveElements
{
    /// <summary>
    /// updates slider UI based on gesture input
    /// </summary>
    public class SliderGestureControl : GestureInteractiveControl
    {
        [Tooltip("The main bar of the slider, used to get the actual width of the slider")]
        public GameObject SliderBar;
        [Tooltip("The visual marker of the slider value")]
        public GameObject Knob;
        [Tooltip("The fill that represents the volume of the shader value")]
        public GameObject SliderFill;
        [Tooltip("The text representation of the slider value")]
        public TextMesh Label;

        [Tooltip("Used for centered format only, will be turned off if LeftJustified")]
        public GameObject CenteredDot;

        [Tooltip("Sends slider event information on Update")]
        public UnityEvent OnUpdateEvent;

        /// <summary>
        /// The value of the slider
        /// </summary>
        public float SliderValue
        {
            private set
            {
                if (mSliderValue != value)
                {
                    mSliderValue = value;
                    OnUpdateEvent.Invoke();
                }
            }
            get
            {
                return mSliderValue;
            }
        }

        [Tooltip("Min numeric value to display in the slider label")]
        public float MinSliderValue = 0;

        [Tooltip("Max numeric value to display in the slider label")]
        public float MaxSliderValue = 1;

        [Tooltip("Switches between a left justified or centered slider")]
        public bool Centered = false;

        [Tooltip("Format the slider value and control decimal places if needed")]
        public string LabelFormat = "#.##";

        private float mSliderValue;

        // calculation variables
        private float mValueSpan;
        private float mCachedValue;
        private float mDeltaValue;
        private Vector3 mStartCenter = new Vector3();
        private float mSliderMagnitude;
        private Vector3 mStartSliderPosition;

        // cached UI values
        private Vector3 mKnobVector;
        private Vector3 mSliderFillScale;
        private float mSliderWidth;

        private float AutoSliderTime = 0.25f;
        private float AutoSliderTimerCounter = 0.5f;
        private float AutoSliderValue = 0;

        private Vector3 mSliderVector;
        private Quaternion mCachedRotation;

        protected override void Awake()
        {
            base.Awake();
            
            if (Knob != null)
            {
                mStartCenter.z = Knob.transform.localPosition.z;
            }

            mCachedRotation = SliderBar.transform.rotation;

            // with some better math below, I may be able to avoid rotating to get the proper size of the component

            SliderBar.transform.rotation = Quaternion.identity;

            // set the width of the slider 
            mSliderMagnitude = SliderBar.transform.InverseTransformVector(SliderBar.GetComponent<Renderer>().bounds.size).x;

            // set the center position
            mStartSliderPosition = mStartCenter + Vector3.left * mSliderMagnitude / 2;

            mValueSpan = MaxSliderValue - MinSliderValue;
            mSliderValue = Mathf.Clamp(SliderValue, MinSliderValue, MaxSliderValue);

            if (!Centered)
            {
                mDeltaValue = SliderValue / mValueSpan;
            }
            else
            {
                mValueSpan = (MaxSliderValue - MinSliderValue) / 2;
                mDeltaValue = (SliderValue + mValueSpan) / 2 / mValueSpan;
            }

            mSliderFillScale = new Vector3(1, 1, 1);
            mSliderWidth = mSliderMagnitude;
            if (SliderFill != null)
            {
                mSliderFillScale = SliderFill.transform.localScale;
                mSliderWidth = SliderFill.transform.InverseTransformVector(SliderFill.GetComponent<Renderer>().bounds.size).x;
            }

            if (CenteredDot != null && !Centered)
            {
                CenteredDot.SetActive(false);
            }

            SliderBar.transform.rotation = mCachedRotation;

            UpdateVisuals();
            mCachedValue = mDeltaValue;

            mSliderVector = SliderBar.transform.InverseTransformPoint(mStartCenter + SliderBar.transform.right * mSliderMagnitude / 2) - SliderBar.transform.InverseTransformPoint(mStartCenter - SliderBar.transform.right * mSliderMagnitude / 2);
            AlignmentVector = SliderBar.transform.right;
            AlignmentVector = mSliderVector;
        }

        public override void ManipulationUpdate(Vector3 startGesturePosition, Vector3 currentGesturePosition, Vector3 startHeadOrigin, Vector3 startHeadRay, GestureInteractive.GestureManipulationState gestureState)
        {
            if (AlignmentVector != SliderBar.transform.right)
            {
                mSliderVector = SliderBar.transform.InverseTransformPoint(mStartCenter + SliderBar.transform.right * mSliderMagnitude / 2) - SliderBar.transform.InverseTransformPoint(mStartCenter - SliderBar.transform.right * mSliderMagnitude / 2);
                AlignmentVector = SliderBar.transform.right;

                mCachedRotation = SliderBar.transform.rotation;
            }

            base.ManipulationUpdate(startGesturePosition, currentGesturePosition, startHeadOrigin, startHeadRay, gestureState);
            
            // get the current delta
            float delta =  (CurrentDistance > 0) ? CurrentPercentage : -CurrentPercentage;
            print(delta);
            
            // combine the delta with the current slider position so the slider does not start over every time
            mDeltaValue = Mathf.Clamp01(delta + mCachedValue);

            if (!Centered)
            {
                SliderValue = mDeltaValue * mValueSpan;
            }
            else
            {
                SliderValue = mDeltaValue * mValueSpan * 2 - mValueSpan;
            }
            
            UpdateVisuals();

            if (gestureState == GestureInteractive.GestureManipulationState.None)
            {
                // gesture ended - cache the current delta
                mCachedValue = mDeltaValue;
            }
        }

        /// <summary>
        /// allows the slider to be automated or triggered by a key word
        /// </summary>
        /// <param name="gestureValue"></param>
        public override void setGestureValue(int gestureValue)
        {
            //base.setGestureValue(gestureValue);

            if (GestureStarted)
            {
                return;
            }

            switch (gestureValue)
            {
                case 0:
                    AutoSliderValue = 0;
                    break;
                case 1:
                    AutoSliderValue = 0.5f;
                    break;
                case 2:
                    AutoSliderValue = 1;
                    break;
            }
            AutoSliderTimerCounter = 0;
        }
		
        /// <summary>
        /// set the distance of the slider
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
		public void SetSpan(float min, float max)
		{
			mValueSpan = max - min;
			MaxSliderValue = max;
			MinSliderValue = min;
		}

        /// <summary>
        /// override the slider value
        /// </summary>
        /// <param name="value"></param>
		public void SetSliderValue(float value)
		{
			if(GestureStarted)
			{
				return;
			}
			
			mSliderValue = Mathf.Clamp(value, MinSliderValue, MaxSliderValue);
			mDeltaValue = SliderValue / MaxSliderValue;
			UpdateVisuals();
            mCachedValue = mDeltaValue;

        }

        // update visuals
        private void UpdateVisuals()
        {
            // set the knob position
            mKnobVector = mStartSliderPosition + Vector3.right * mSliderMagnitude * mDeltaValue;
            mKnobVector.z = mStartCenter.z;

            // TODO: Add snapping!

            if (Knob != null)
            {
                Knob.transform.localPosition = mKnobVector;
            }

            // set the fill scale and position
            if (SliderFill != null)
            {
                Vector3 scale = mSliderFillScale;
                scale.x = mSliderFillScale.x * mDeltaValue;

                Vector3 position = Vector3.left * (mSliderWidth * 0.5f - mSliderWidth * mDeltaValue * 0.5f); // left justified;

                if (Centered)
                {
                    if (SliderValue < 0)
                    {
                        position = Vector3.left * (mSliderWidth * 0.5f - mSliderWidth * 0.5f * (mDeltaValue + 0.5f)); // pinned to center, going left
                        scale.x = mSliderFillScale.x * (1 - mDeltaValue / 0.5f) * 0.5f;
                    }
                    else
                    {
                        position = Vector3.right * ((mSliderWidth * 0.5f * (mDeltaValue - 0.5f))); // pinned to center, going right
                        scale.x = mSliderFillScale.x * ((mDeltaValue - 0.5f) / 0.5f) * 0.5f;
                    }
                }

                SliderFill.transform.localScale = scale;
                SliderFill.transform.localPosition = position;
            }

            // set the label
            if (Label != null)
            {
                float displayValue = SliderValue;
                if (Centered)
                {
                    displayValue = SliderValue * 2 - SliderValue;
                }

                if (LabelFormat.IndexOf('.') > -1)
                {
                    Label.text = displayValue.ToString(LabelFormat);

                }
                else
                {
                    Label.text = Mathf.Round(displayValue).ToString(LabelFormat);
                }
            }
        }

        /// <summary>
        /// Handle automation
        /// </summary>
        protected override void Update()
        {
            base.Update();

            if (AutoSliderTimerCounter < AutoSliderTime)
            {
                if (GestureStarted)
                {
                    AutoSliderTimerCounter = AutoSliderTime;
                    return;
                }

                AutoSliderTimerCounter += Time.deltaTime;
                if (AutoSliderTimerCounter >= AutoSliderTime)
                {
                    AutoSliderTimerCounter = AutoSliderTime;
                    mCachedValue = AutoSliderValue;
                }

                mDeltaValue = (AutoSliderValue - mCachedValue) * AutoSliderTimerCounter / AutoSliderTime + mCachedValue;
                
                if (!Centered)
                {
                    SliderValue = mDeltaValue * mValueSpan;
                }
                else
                {
                    SliderValue = mDeltaValue * mValueSpan * 2 - mValueSpan;
                }

                UpdateVisuals();

            }
        }
    }
}
