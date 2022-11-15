// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX
{
    [RequireComponent(typeof(Slider))]
    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways]
    /// <summary>    
    /// A canvas-based visuals script to provide a visual layer on top of
    /// <see cref="Slider"/>.
    /// </summary>
    [AddComponentMenu("MRTK/UX/Canvas Slider Visuals")]
    public class CanvasSliderVisuals : MonoBehaviour
    {
        /// <summary>
        /// Setting that indicates one of four directions.
        /// </summary>
        public enum Direction
        {
            /// <summary>
            /// From the left to the right
            /// </summary>
            LeftToRight,

            /// <summary>
            /// From the right to the left
            /// </summary>
            RightToLeft,

            /// <summary>
            /// From the bottom to the top.
            /// </summary>
            BottomToTop,

            /// <summary>
            /// From the top to the bottom.
            /// </summary>
            TopToBottom,
        }

        [SerializeField]
        [Tooltip("The transform of the handle.")]
        private RectTransform handle;

        /// <summary>
        /// The transform of the handle.
        /// </summary>
        public RectTransform Handle
        {
            get => handle;
            set => handle = value;
        }

        [SerializeField]
        [Tooltip("The transform of the overall slider track area.")]
        private RectTransform trackArea;

        /// <summary>
        /// The transform of the overall slider track area.
        /// </summary>
        /// <remarks>
        /// This is snapped between the start/end transforms defined in the <see cref="Slider"/> script.
        /// </remarks>
        public RectTransform TrackArea
        {
            get => trackArea;
            set => trackArea = value;
        }

        [SerializeField]
        [Tooltip("The transform of the area representing the 'progress' of the slider.")]
        private RectTransform fillVisual;

        /// <summary>
        /// The transform of the area representing the 'progress' of the slider.
        /// </summary>
        public RectTransform FillVisual
        {
            get => fillVisual;
            set => fillVisual = value;
        }

        private Slider sliderState;

        /// <summary>
        /// Reference to the <see cref="Slider"/> component.
        /// </summary>
        protected Slider SliderState
        {
            get
            {
                if (sliderState == null)
                {
                    sliderState = GetComponent<Slider>();
                }

                return sliderState;
            }
        }

        [SerializeField]
        [Tooltip("The direction of the slider. Setting this will flip/reorganize the slider.")]
        private Direction sliderDirection;

        /// <summary>
        /// The direction of the slider. Setting this will flip/reorganize the slider.
        /// </summary>
        public Direction SliderDirection
        {
            get => sliderDirection;
            set
            {
                sliderDirection = value;
                SetLayout(sliderDirection);
            }
        }

        private RectTransform sliderStart;

        private RectTransform sliderEnd;

        private RectTransformColliderFitter touchableFitter;

        private UGUIInputAdapter uguiInputAdapter;

        void OnEnable()
        {
            sliderStart = SliderState.SliderStart.EnsureComponent<RectTransform>();
            sliderEnd = SliderState.SliderEnd.EnsureComponent<RectTransform>();
            touchableFitter = TrackArea.GetComponent<RectTransformColliderFitter>();
            uguiInputAdapter = GetComponent<UGUIInputAdapter>();

            if (sliderStart == null || sliderEnd == null)
            {
                Debug.LogError("Slider is missing start/end transforms.");
            }

            SliderState.OnValueUpdated.AddListener(UpdateHandle);

            // Initial update. We may miss the first OnValueUpdated, depending on execution order.
            UpdateHandle(SliderState.NormalizedValue);
            SetLayout(sliderDirection);
        }

        void OnDisable()
        {
            SliderState.OnValueUpdated.RemoveListener(UpdateHandle);
        }

#if UNITY_EDITOR

        // Keep track of the last direction, so we can re-layout if needed.
        private Direction prevDirection;

        void Update()
        {
            // Only do this in edit mode, for performance.
            if (Application.isPlaying)
            {
                return;
            }

            // Ensure sliderStart and sliderEnd are rooted at 0, we change the positions of these points via their anchors
            sliderStart.anchoredPosition = Vector3.zero;
            sliderEnd.anchoredPosition = Vector3.zero;

            // Ensure track is centered.
            trackArea.anchoredPosition = Vector3.zero;

            // Helper to apply layout if set from inspector.
            if (SliderDirection != prevDirection)
            {
                SetLayout(SliderDirection);
                prevDirection = SliderDirection;
            }

            // Update handle in editor view.
            UpdateHandle(SliderState.NormalizedValue);
        }
#endif // UNITY_EDITOR

        // Update the things that depend on the slider value.
        void UpdateHandle(SliderEventData data)
        {
            UpdateHandle(data.NewValue);
        }

        // Update the things that depend on the slider value.
        void UpdateHandle(float value)
        {
            handle.position = SliderState.SliderStart.position + (value * SliderState.SliderTrackDirection);

            switch (SliderDirection)
            {
                case Direction.LeftToRight:
                    fillVisual.anchorMax = new Vector2(value, 1.0f);
                    break;
                case Direction.RightToLeft:
                    fillVisual.anchorMin = new Vector2(1.0f - value, 0.0f);
                    break;
                case Direction.BottomToTop:
                    fillVisual.anchorMax = new Vector2(1.0f, value);
                    break;
                case Direction.TopToBottom:
                    fillVisual.anchorMin = new Vector2(0.0f, 1.0f - value);
                    break;
            }
        }

        void SetLayout(Direction direction)
        {
            float trackWidth = Mathf.Max(trackArea.sizeDelta.x, trackArea.sizeDelta.y);
            switch (direction)
            {
                case Direction.LeftToRight:

                    // Make sure UGUI understands which axis we can
                    // slide on. (Affects dpad/gamepad/etc)
                    uguiInputAdapter.MovableAxes = AxisFlags.XAxis;

                    sliderStart.anchorMin = new Vector2(0.0f, 0.5f);
                    sliderStart.anchorMax = new Vector2(0.0f, 0.5f);
                    sliderEnd.anchorMin = new Vector2(1.0f, 0.5f);
                    sliderEnd.anchorMax = new Vector2(1.0f, 0.5f);
                    handle.localRotation = Quaternion.Euler(0, 0, 0);

                    trackArea.anchorMin = new Vector2(0.0f, 0.5f);
                    trackArea.anchorMax = new Vector2(1.0f, 0.5f);
                    trackArea.sizeDelta = new Vector2(0.0f, trackWidth);
                    trackArea.anchoredPosition = Vector2.zero;

                    fillVisual.anchorMin = new Vector2(0.0f, 0.0f);
                    break;

                case Direction.RightToLeft:

                    // Make sure UGUI understands which axis we can
                    // slide on. (Affects dpad/gamepad/etc)
                    uguiInputAdapter.MovableAxes = AxisFlags.XAxis;

                    sliderStart.anchorMin = new Vector2(1.0f, 0.5f);
                    sliderStart.anchorMax = new Vector2(1.0f, 0.5f);
                    sliderEnd.anchorMin = new Vector2(0.0f, 0.5f);
                    sliderEnd.anchorMax = new Vector2(0.0f, 0.5f);
                    handle.localRotation = Quaternion.Euler(0, 0, 180);

                    trackArea.anchorMin = new Vector2(0.0f, 0.5f);
                    trackArea.anchorMax = new Vector2(1.0f, 0.5f);
                    trackArea.sizeDelta = new Vector2(0.0f, trackWidth);
                    trackArea.anchoredPosition = Vector2.zero;

                    fillVisual.anchorMax = new Vector2(1.0f, 1.0f);
                    break;

                case Direction.BottomToTop:

                    // Make sure UGUI understands which axis we can
                    // slide on. (Affects dpad/gamepad/etc)
                    uguiInputAdapter.MovableAxes = AxisFlags.YAxis;

                    sliderStart.anchorMin = new Vector2(0.5f, 0.0f);
                    sliderStart.anchorMax = new Vector2(0.5f, 0.0f);
                    sliderEnd.anchorMin = new Vector2(0.5f, 1.0f);
                    sliderEnd.anchorMax = new Vector2(0.5f, 1.0f);
                    handle.localRotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);

                    trackArea.anchorMin = new Vector2(0.5f, 0.0f);
                    trackArea.anchorMax = new Vector2(0.5f, 1.0f);
                    trackArea.sizeDelta = new Vector2(trackWidth, 0.0f);
                    trackArea.anchoredPosition = Vector2.zero;

                    fillVisual.anchorMin = new Vector2(0.0f, 0.0f);
                    break;

                case Direction.TopToBottom:

                    // Make sure UGUI understands which axis we can
                    // slide on. (Affects dpad/gamepad/etc)
                    uguiInputAdapter.MovableAxes = AxisFlags.YAxis;

                    sliderStart.anchorMin = new Vector2(0.5f, 1);
                    sliderStart.anchorMax = new Vector2(0.5f, 1);
                    sliderEnd.anchorMin = new Vector2(0.5f, 0);
                    sliderEnd.anchorMax = new Vector2(0.5f, 0);
                    handle.localRotation = Quaternion.Euler(0, 0, 270);

                    trackArea.anchorMin = new Vector2(0.5f, 0);
                    trackArea.anchorMax = new Vector2(0.5f, 1);
                    trackArea.sizeDelta = new Vector2(trackWidth, 0);
                    trackArea.anchoredPosition = Vector2.zero;

                    fillVisual.anchorMax = new Vector2(1.0f, 1.0f);
                    break;
            }

            if (touchableFitter != null)
            {
                float primaryPadding = Mathf.Max(touchableFitter.Padding.x, touchableFitter.Padding.y);
                float secondaryPadding = Mathf.Min(touchableFitter.Padding.x, touchableFitter.Padding.y);

                switch (direction)
                {
                    case Direction.LeftToRight:
                    case Direction.RightToLeft:
                        touchableFitter.Padding = new Vector2(secondaryPadding, primaryPadding);
                        break;
                    case Direction.BottomToTop:
                    case Direction.TopToBottom:
                        touchableFitter.Padding = new Vector2(primaryPadding, secondaryPadding);
                        break;
                }
            }

            UpdateHandle(SliderState.NormalizedValue);
        }
    }
}
