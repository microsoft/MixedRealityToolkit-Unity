// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// A slider that can be moved by grabbing / pinching a slider thumb
    /// </summary>
    [HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/ux-building-blocks/sliders")]
    [AddComponentMenu("Scripts/MRTK/SDK/PinchSlider")]
    public class PinchSlider : MonoBehaviour, IMixedRealityPointerHandler, IMixedRealityFocusHandler, IMixedRealityTouchHandler
    {
        #region Serialized Fields and Public Properties
        [Tooltip("The gameObject that contains the slider thumb.")]
        [SerializeField]
        private GameObject thumbRoot = null;
        public GameObject ThumbRoot
        {
            get
            {
                return thumbRoot;
            }
            set
            {
                thumbRoot = value;
                InitializeSliderThumb();
            }
        }


        [SerializeField]
        [Tooltip("Whether or not this slider is controllable via touch events")]
        private bool isTouchable;

        /// <summary>
        /// Property accessor of isTouchable. Determines whether or not this slider is controllable via touch events
        /// </summary>
        public bool IsTouchable
        {
            get { return isTouchable; }
            set { isTouchable = value; }
        }

        [SerializeField]
        [Tooltip("Whether or not this slider snaps to the designated position on the slider")]
        private bool snapToPosition;

        /// <summary>
        /// Property accessor of snapToPosition. Determines whether or not this slider snaps to the designated position on the slider
        /// </summary>
        public bool SnapToPosition
        {
            get { return snapToPosition; }
            set { snapToPosition = value; touchCollider.enabled = value; thumbCollider.enabled = !value; }
        }

        [SerializeField]
        /// <summary>
        /// Used to control the slider on the track when snapToPosition is false
        /// </summary>
        private Collider thumbCollider = null;

        /// <summary>
        /// Property accessor of thumbCollider. Used to control the slider on the track when snapToPosition is false
        /// </summary>
        public Collider ThumbCollider
        {
            get { return thumbCollider; }
            set { thumbCollider = value; }
        }

        [SerializeField]
        /// <summary>
        /// Used to determine the position we snap the slider do when snapToPosition is true
        /// </summary>
        private Collider touchCollider = null;

        /// <summary>
        /// Property accessor of touchCollider. Used to determine the position we snap the slider do when snapToPosition is true
        /// </summary>
        public Collider TouchCollider
        {
            get { return touchCollider; }
            set { touchCollider = value; }
        }

        [Range(minVal, maxVal)]
        [SerializeField]
        private float sliderValue = 0.5f;
        public float SliderValue
        {
            get { return sliderValue; }
            set
            {
                var oldSliderValue = sliderValue;
                sliderValue = value;
                UpdateUI();
                OnValueUpdated.Invoke(new SliderEventData(oldSliderValue, value, ActivePointer, this));
            }
        }

        [SerializeField]
        [Tooltip("Controls whether this slider is increments in steps or continuously")]
        private bool useSliderStepDivisions;

        /// <summary>
        /// Property accessor of useSliderStepDivisions, it determines whether the slider steps according to subdivisions
        /// </summary>
        public bool UseSliderStepDivisions
        {
            get { return useSliderStepDivisions; }
            set { useSliderStepDivisions = value; }
        }

        [SerializeField]
        [Min(1)]
        [Tooltip("Number of subdivisions the slider is split into.")]
        private int sliderStepDivisions = 1;

        /// <summary>
        /// Property accessor of sliderStepDivisions, it holds the number of subdivisions the slider is split into.
        /// </summary>
        public int SliderStepDivisions
        {
            get { return sliderStepDivisions; }
            set { sliderStepDivisions = value; }
        }

        [Header("Slider Axis Visuals")]

        [Tooltip("The gameObject that contains the trackVisuals. This will get rotated to match the slider axis")]
        [SerializeField]
        private GameObject trackVisuals = null;
        /// <summary>
        /// Property accessor of trackVisuals, it contains the desired track Visuals. This will get rotated to match the slider axis.
        /// </summary>
        public GameObject TrackVisuals
        {
            get
            {
                return trackVisuals;
            }
            set
            {
                if (trackVisuals != value)
                {
                    trackVisuals = value;
                    UpdateTrackVisuals();
                }
            }
        }

        [Tooltip("The gameObject that contains the tickMarks.  This will get rotated to match the slider axis")]
        [SerializeField]
        private GameObject tickMarks = null;
        /// <summary>
        /// Property accessor of tickMarks, it contains the desired tick Marks.  This will get rotated to match the slider axis.
        /// </summary>
        public GameObject TickMarks
        {
            get
            {
                return tickMarks;
            }
            set
            {
                if (tickMarks != value)
                {
                    tickMarks = value;
                    UpdateTickMarks();
                }
            }
        }

        [Tooltip("The gameObject that contains the thumb visuals.  This will get rotated to match the slider axis.")]
        [SerializeField]
        private GameObject thumbVisuals = null;
        /// <summary>
        /// Property accessor of thumbVisuals, it contains the desired tick marks.  This will get rotated to match the slider axis.
        /// </summary>
        public GameObject ThumbVisuals
        {
            get
            {
                return thumbVisuals;
            }
            set
            {
                if (thumbVisuals != value)
                {
                    thumbVisuals = value;
                    UpdateThumbVisuals();
                }
            }
        }


        [Header("Slider Track")]

        [Tooltip("The axis the slider moves along")]
        [SerializeField]
        private SliderAxis sliderAxis = SliderAxis.XAxis;
        /// <summary>
        /// Property accessor of sliderAxis. The axis the slider moves along.
        /// </summary>
        public SliderAxis CurrentSliderAxis
        {
            get { return sliderAxis; }
            set
            {
                sliderAxis = value;
                UpdateVisualsOrientation();
            }
        }

        /// <summary>
        /// Previous value of slider axis, is used in order to detect change in current slider axis value
        /// </summary>
        private SliderAxis? previousSliderAxis = null;
        /// <summary>
        /// Property accessor for previousSliderAxis that is used also to initialize the property with the current value in case of null value.
        /// </summary>
        private SliderAxis PreviousSliderAxis
        {
            get
            {
                if (previousSliderAxis == null)
                {
                    previousSliderAxis = CurrentSliderAxis;
                }
                return previousSliderAxis.Value;
            }
            set
            {
                previousSliderAxis = value;
            }
        }

        [SerializeField]
        [Tooltip("Where the slider track starts, as distance from center along slider axis, in local space units.")]
        private float sliderStartDistance = -.5f;
        public float SliderStartDistance
        {
            get { return sliderStartDistance; }
            set { sliderStartDistance = value; }
        }

        [SerializeField]
        [Tooltip("Where the slider track ends, as distance from center along slider axis, in local space units.")]
        private float sliderEndDistance = .5f;
        public float SliderEndDistance
        {
            get { return sliderEndDistance; }
            set { sliderEndDistance = value; }
        }

        /// <summary>
        /// Gets the start position of the slider, in world space, or zero if invalid.
        /// Sets the start position of the slider, in world space, projected to the slider's axis.
        /// </summary>
        public Vector3 SliderStartPosition
        {
            get { return transform.TransformPoint(GetSliderAxis() * sliderStartDistance); }
            set { sliderStartDistance = Vector3.Dot(transform.InverseTransformPoint(value), GetSliderAxis()); }
        }

        /// <summary>
        /// Gets the end position of the slider, in world space, or zero if invalid.
        /// Sets the end position of the slider, in world space, projected to the slider's axis.
        /// </summary>
        public Vector3 SliderEndPosition
        {
            get { return transform.TransformPoint(GetSliderAxis() * sliderEndDistance); }
            set { sliderEndDistance = Vector3.Dot(transform.InverseTransformPoint(value), GetSliderAxis()); }
        }

        /// <summary>
        /// Returns the vector from the slider start to end positions
        /// </summary>
        public Vector3 SliderTrackDirection
        {
            get { return SliderEndPosition - SliderStartPosition; }
        }

        #endregion

        #region Event Handlers
        [Header("Events")]
        public SliderEvent OnValueUpdated = new SliderEvent();
        public SliderEvent OnInteractionStarted = new SliderEvent();
        public SliderEvent OnInteractionEnded = new SliderEvent();
        public SliderEvent OnHoverEntered = new SliderEvent();
        public SliderEvent OnHoverExited = new SliderEvent();
        #endregion

        #region Private Fields

        /// <summary>
        /// Position offset for slider handle in world space.
        /// </summary>
        private Vector3 sliderThumbOffset = Vector3.zero;


        /// <summary>
        /// Private member used to adjust slider values
        /// </summary>
        private float sliderStepVal => (maxVal - minVal) / sliderStepDivisions;

        #endregion

        #region Protected Properties

        /// <summary>
        /// Float value that holds the starting value of the slider.
        /// </summary>
        protected float StartSliderValue { get; private set; }

        /// <summary>
        /// Starting position of mixed reality pointer in world space
        /// Used to track pointer movement
        /// </summary>
        protected Vector3 StartPointerPosition { get; private set; }

        /// <summary>
        /// Interface for handling pointer being used in UX interaction.
        /// </summary>
        protected IMixedRealityPointer ActivePointer { get; private set; }

        #endregion

        #region Constants
        /// <summary>
        /// Minimum distance between start and end of slider, in world space
        /// </summary>
        private const float MinSliderLength = 0.001f;

        /// <summary>
        /// The minimum value that the slider can take on
        /// </summary>
        private const float minVal = 0.0f;

        /// <summary>
        /// The maximum value that the slider can take on
        /// </summary>
        private const float maxVal = 1.0f;

        #endregion  

        #region Unity methods
        protected virtual void Start()
        {
            if (useSliderStepDivisions)
            {
                InitializeStepDivisions();
            }

            if (thumbRoot == null)
            {
                throw new Exception($"Slider thumb on gameObject {gameObject.name} is not specified. Did you forget to set it?");
            }

            SnapToPosition = snapToPosition;
            InitializeSliderThumb();
            OnValueUpdated.Invoke(new SliderEventData(sliderValue, sliderValue, null, this));
        }

        private void OnDisable()
        {
            if (ActivePointer != null)
            {
                EndInteraction();
            }
        }

        private void OnValidate()
        {
            CurrentSliderAxis = sliderAxis;
        }

        #endregion

        #region Private Methods
        private void InitializeSliderThumb()
        {
            var startToThumb = thumbRoot.transform.position - SliderStartPosition;
            var thumbProjectedOnTrack = SliderStartPosition + Vector3.Project(startToThumb, SliderTrackDirection);
            sliderThumbOffset = thumbRoot.transform.position - thumbProjectedOnTrack;

            UpdateUI();
        }

        /// <summary> 
        /// Private method used to adjust initial slider value to stepwise values
        /// </summary>
        private void InitializeStepDivisions()
        {
            SliderValue = SnapSliderToStepPositions(SliderValue);
        }

        /// <summary>
        /// Update orientation of track visuals based on slider axis orientation
        /// </summary>
        private void UpdateTrackVisuals()
        {
            if (TrackVisuals)
            {
                TrackVisuals.transform.localPosition = Vector3.zero;

                switch (sliderAxis)
                {
                    case SliderAxis.XAxis:
                        TrackVisuals.transform.localRotation = Quaternion.identity;
                        break;
                    case SliderAxis.YAxis:
                        TrackVisuals.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
                        break;
                    case SliderAxis.ZAxis:
                        TrackVisuals.transform.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
                        break;
                }
            }
        }

        /// <summary>
        /// Update orientation of tick marks based on slider axis orientation
        /// </summary>
        private void UpdateTickMarks()
        {
            if (TickMarks)
            {
                TickMarks.transform.localPosition = Vector3.zero;
                TickMarks.transform.localRotation = Quaternion.identity;

                var grid = TickMarks.GetComponent<Utilities.GridObjectCollection>();
                if (grid)
                {
                    // Update cellwidth or cellheight depending on what was the previous axis set to
                    var previousAxis = grid.Layout;
                    if (previousAxis == Utilities.LayoutOrder.Vertical)
                    {
                        grid.CellWidth = grid.CellHeight;
                    }
                    else
                    {
                        grid.CellHeight = grid.CellWidth;
                    }

                    grid.Layout = (sliderAxis == SliderAxis.YAxis) ? Utilities.LayoutOrder.Vertical : Utilities.LayoutOrder.Horizontal;
                    grid.UpdateCollection();
                }

                if (sliderAxis == SliderAxis.ZAxis)
                {
                    TickMarks.transform.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
                }
            }
        }

        /// <summary>
        /// Update orientation of thumb visuals based on slider axis orientation
        /// </summary>
        private void UpdateThumbVisuals()
        {
            if (ThumbVisuals)
            {
                ThumbVisuals.transform.localPosition = Vector3.zero;

                switch (sliderAxis)
                {
                    case SliderAxis.XAxis:
                        ThumbVisuals.transform.localRotation = Quaternion.identity;
                        break;
                    case SliderAxis.YAxis:
                        ThumbVisuals.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
                        break;
                    case SliderAxis.ZAxis:
                        ThumbVisuals.transform.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
                        break;
                }
            }
        }

        /// <summary>
        /// Update orientation of the visual components of pinch slider
        /// </summary>
        private void UpdateVisualsOrientation()
        {
            if (PreviousSliderAxis != sliderAxis)
            {
                UpdateThumbVisuals();
                UpdateTrackVisuals();
                UpdateTickMarks();
                PreviousSliderAxis = sliderAxis;
            }
        }

        private Vector3 GetSliderAxis()
        {
            switch (sliderAxis)
            {
                case SliderAxis.XAxis:
                    return Vector3.right;
                case SliderAxis.YAxis:
                    return Vector3.up;
                case SliderAxis.ZAxis:
                    return Vector3.forward;
                default:
                    throw new ArgumentOutOfRangeException("Invalid slider axis");
            }
        }

        private void UpdateUI()
        {
            var newSliderPos = SliderStartPosition + sliderThumbOffset + SliderTrackDirection * sliderValue;
            thumbRoot.transform.position = newSliderPos;
        }

        private void EndInteraction()
        {
            if (OnInteractionEnded != null)
            {
                OnInteractionEnded.Invoke(new SliderEventData(sliderValue, sliderValue, ActivePointer, this));
            }
            ActivePointer = null;
        }


        private float SnapSliderToStepPositions(float value)
        {
            var stepCount = value / sliderStepVal;
            var snappedValue = sliderStepVal * Mathf.RoundToInt(stepCount);
            Mathf.Clamp(snappedValue, minVal, maxVal);
            return snappedValue;
        }

        private void CalculateSliderValueBasedOnTouchPoint(Vector3 touchPoint)
        {
            var sliderTouchPoint = touchPoint - SliderStartPosition;
            var sliderVector = SliderEndPosition - SliderStartPosition;

            // If our touch point goes off the start side of the slider, set it's value to minVal and return immediately
            // Explanation of the math here: https://www.quora.com/Can-scalar-projection-be-negative
            if (Vector3.Dot(sliderTouchPoint, sliderVector) < 0)
            {
                SliderValue = minVal;
                return;
            }

            float sliderProgress = Vector3.Project(sliderTouchPoint, sliderVector).magnitude;
            float result = sliderProgress / sliderVector.magnitude;
            float clampedResult = result;
            if (UseSliderStepDivisions)
            {
                clampedResult = SnapSliderToStepPositions(result);
            }
            clampedResult = Mathf.Clamp(clampedResult, minVal, maxVal);

            SliderValue = clampedResult;
        }


        #endregion

        #region IMixedRealityFocusHandler
        public void OnFocusEnter(FocusEventData eventData)
        {
            OnHoverEntered.Invoke(new SliderEventData(sliderValue, sliderValue, eventData.Pointer, this));
        }

        public void OnFocusExit(FocusEventData eventData)
        {
            OnHoverExited.Invoke(new SliderEventData(sliderValue, sliderValue, eventData.Pointer, this));
        }
        #endregion

        #region IMixedRealityPointerHandler

        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            if (eventData.Pointer == ActivePointer && !eventData.used)
            {
                EndInteraction();

                // Mark the pointer data as used to prevent other behaviors from handling input events
                eventData.Use();
            }
        }

        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            if (ActivePointer == null && !eventData.used)
            {
                ActivePointer = eventData.Pointer;
                StartPointerPosition = ActivePointer.Position;

                if (SnapToPosition)
                {
                    CalculateSliderValueBasedOnTouchPoint(ActivePointer.Result.Details.Point);
                }

                if (OnInteractionStarted != null)
                {
                    OnInteractionStarted.Invoke(new SliderEventData(sliderValue, sliderValue, ActivePointer, this));
                }

                StartSliderValue = sliderValue;

                // Mark the pointer data as used to prevent other behaviors from handling input events
                eventData.Use();
            }
        }

        public virtual void OnPointerDragged(MixedRealityPointerEventData eventData)
        {
            if (eventData.Pointer == ActivePointer && !eventData.used)
            {
                var delta = ActivePointer.Position - StartPointerPosition;
                var handDelta = Vector3.Dot(SliderTrackDirection.normalized, delta);

                if (useSliderStepDivisions)
                {
                    var stepVal = (handDelta / SliderTrackDirection.magnitude > 0) ? sliderStepVal : (sliderStepVal * -1);
                    var stepMag = Mathf.Floor(Mathf.Abs(handDelta / SliderTrackDirection.magnitude) / sliderStepVal);
                    SliderValue = Mathf.Clamp(StartSliderValue + (stepVal * stepMag), 0, 1);
                }
                else
                {
                    SliderValue = Mathf.Clamp(StartSliderValue + handDelta / SliderTrackDirection.magnitude, 0, 1);
                }

                // Mark the pointer data as used to prevent other behaviors from handling input events
                eventData.Use();
            }
        }

        public void OnPointerClicked(MixedRealityPointerEventData eventData) { }

        #endregion


        #region IMixedRealityTouchHandler
        public void OnTouchStarted(HandTrackingInputEventData eventData)
        {
            if (isTouchable)
            {
                if (OnInteractionStarted != null)
                {
                    OnInteractionStarted.Invoke(new SliderEventData(sliderValue, sliderValue, ActivePointer, this));
                }
                eventData.Use();
            }
        }


        public void OnTouchCompleted(HandTrackingInputEventData eventData)
        {
            if (isTouchable)
            {
                if (!eventData.used)
                {
                    EndInteraction();

                    // Mark the pointer data as used to prevent other behaviors from handling input events
                    eventData.Use();
                }
            }
        }

        /// <summary>b  
        /// When the collider is touched, use the touch point to Calculate the Slider value
        /// </summary>
        public void OnTouchUpdated(HandTrackingInputEventData eventData)
        {
            if(isTouchable)
            {
                CalculateSliderValueBasedOnTouchPoint(eventData.InputData);
            }
        }

        #endregion IMixedRealityTouchHandler
    }
}
