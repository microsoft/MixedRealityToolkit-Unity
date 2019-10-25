// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// TODO
    /// </summary>
    [RequireComponent(typeof(TransformScaleHandler))]
    public class PinchSliderBox : MonoBehaviour
    {
        #region Serialized Fields and Properties

        [SerializeField, Tooltip("TODO")]
        private bool showXAxisHandles = true;

        /// <summary>
        /// TODO
        /// </summary>
        public bool ShowXAxisHandles
        {
            get => showXAxisHandles;
            set
            {
                showXAxisHandles = value;
                CreateHandles();
            }
        }

        [SerializeField, Tooltip("TODO")]
        private bool showYAxisHandles = true;

        /// <summary>
        /// TODO
        /// </summary>
        public bool ShowYAxisHandles
        {
            get => showYAxisHandles;
            set
            {
                showYAxisHandles = value;
                CreateHandles();
            }
        }

        [SerializeField, Tooltip("TODO")]
        private bool showZAxisHandles = true;

        /// <summary>
        /// TODO
        /// </summary>
        public bool ShowZAxisHandles
        {
            get => showZAxisHandles;
            set
            {
                showZAxisHandles = value;
                CreateHandles();
            }
        }

        [SerializeField, Tooltip("TODO")]
        private GameObject handlePrefab = null;

        /// <summary>
        /// TODO
        /// </summary>
        public GameObject HandlePrefab
        {
            get => handlePrefab;
            set
            {
                handlePrefab = value;
                CreateHandles();
            }
        }

        #endregion

        #region Private Members

        private TransformScaleHandler transformScaleHandler = null;
        private Transform pivot = null;
        private Material defaultHandleMaterial = null;
        private bool quitting = false;

        private class SliderPair
        {
            public PinchSlider PositiveSlider = null;
            public PinchSlider NegativeSlider = null;

            public float Value
            {
                get
                {
                    return (PositiveSlider.SliderValue + NegativeSlider.SliderValue) * 0.5f;
                }
            }
        }

        private class SliderPlane
        {
            public PinchSlider.SliderAxis Axis = PinchSlider.SliderAxis.XAxis;
            public SliderPair PositiveSliderPair = null;
            public SliderPair NegativeSliderPair = null;

            public SliderPair GetSliderPair(PinchSlider slider)
            {
                if (slider == PositiveSliderPair.PositiveSlider ||
                    slider == PositiveSliderPair.NegativeSlider)
                {
                    return PositiveSliderPair;
                }
                else
                {
                    return NegativeSliderPair;
                }
            }
        }

        private List<PinchSlider> pinchSliders = new List<PinchSlider>();
        private Dictionary<PinchSlider, SliderPlane> sliderPlanes = new Dictionary<PinchSlider, SliderPlane>();

        #endregion

        #region MonoBehaviour Implementation

        private void Awake()
        {
            // Ensure a TransformScaleHandler exists and it is initialized.
            transformScaleHandler = gameObject.EnsureComponent<TransformScaleHandler>();
            transformScaleHandler.Start();

            if (handlePrefab == null)
            {
                defaultHandleMaterial = new Material(StandardShaderUtility.MrtkStandardShader);
            }
        }

        private void OnDestroy()
        {
            Destroy(defaultHandleMaterial);
        }

        private void OnEnable()
        {
            CreateHandles();
        }

        private void OnDisable()
        {
            DestroyHandles();
        }

        private void OnApplicationQuit()
        {
            quitting = true;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// TODO
        /// </summary>
        public void CreateHandles()
        {
            DestroyHandles();

            pivot = new GameObject("ClippingBoxControlPivot").transform;
            pivot.position = transformScaleHandler.TargetTransform.position;
            pivot.parent = transformScaleHandler.TargetTransform.parent;
            transformScaleHandler.TargetTransform.parent = pivot;

            if (showXAxisHandles)
            {
                AddSliderPlane(PinchSlider.SliderAxis.XAxis);
            }

            if (ShowYAxisHandles)
            {
                AddSliderPlane(PinchSlider.SliderAxis.YAxis);
            }

            if (ShowZAxisHandles)
            {
                AddSliderPlane(PinchSlider.SliderAxis.ZAxis);
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        public void DestroyHandles()
        {
            if (pivot != null)
            {
                // Restore the original parent. Unity will present a warning if parents are altered when quitting.
                if (!quitting)
                {
                    transformScaleHandler.TargetTransform.parent = pivot.transform.parent;
                }

                Destroy(pivot.gameObject);
            }

            foreach (var slider in pinchSliders)
            {
                Destroy(slider.gameObject);
            }

            pinchSliders.Clear();
            sliderPlanes.Clear();
        }

        #endregion

        #region Private Methods

        private void AddSliderPlane(PinchSlider.SliderAxis axis)
        {
            var sliders = new PinchSlider[4];
            var globalDirection = 1.0f;

            for (var i = 0; i < 2; ++i)
            {
                var localDirection = 1.0f;

                for (var j = 0; j < 2; ++j)
                {
                    sliders[i * 2 + j] = AddSlider(axis, globalDirection, localDirection);
                    localDirection *= -1.0f;
                }

                globalDirection *= -1.0f;
            }

            pinchSliders.AddRange(sliders);

            var sliderPlane = new SliderPlane()
            {
                Axis = axis,
                PositiveSliderPair = new SliderPair()
                {
                    PositiveSlider = sliders[0],
                    NegativeSlider = sliders[1]
                },
                NegativeSliderPair = new SliderPair()
                {
                    PositiveSlider = sliders[2],
                    NegativeSlider = sliders[3]
                },
            };

            foreach (var slider in sliders)
            {
                sliderPlanes.Add(slider, sliderPlane);
            }
        }

        private PinchSlider AddSlider(PinchSlider.SliderAxis axis, float globalDirection, float localDirection)
        {
            var axisIndex = (int)axis;
            var targetTransform = transformScaleHandler.TargetTransform;

            var slider = new GameObject($"Slider{axis}({globalDirection})({localDirection})").AddComponent<PinchSlider>();
            slider.transform.parent = transform;

            // Calculates a normal to the pinch slider axis to place the slider at.
            var axisNormal = PinchSlider.GetSliderAxis((PinchSlider.SliderAxis)((axisIndex + 1) % 3));
            var axisNormalHalfScale = Vector3.Dot(axisNormal, targetTransform.localScale) * 0.5f;
            slider.transform.position = targetTransform.position - ((axisNormal * axisNormalHalfScale) * globalDirection);
            slider.transform.rotation = targetTransform.rotation;

            slider.SliderAxisType = axis;
            slider.SliderStartDistance = transformScaleHandler.ScaleMinimumVector[axisIndex] * 0.5f * localDirection;
            slider.SliderEndDistance = transformScaleHandler.ScaleMaximumVector[axisIndex] * 0.5f * localDirection;

            GameObject thumb;

            if (handlePrefab != null)
            {
                thumb = Instantiate(handlePrefab);
                thumb.EnsureComponent<NearInteractionGrabbable>();

                if (thumb.GetComponentInChildren<Collider>() == null)
                {
                    Debug.LogWarning("The thumb prefab is missing a collider, adding a default one.");
                    var sphereCollider = thumb.AddComponent<SphereCollider>();
                    sphereCollider.radius = Mathf.Max(sphereCollider.radius, 0.05f);
                }
            }
            else
            {
                thumb = CreateDefaultThumb();
            }

            thumb.transform.parent = slider.transform;
            thumb.transform.localPosition = Vector3.zero;
            slider.ThumbRoot = thumb;

            var scaleRange = transformScaleHandler.ScaleMaximumVector[axisIndex] - transformScaleHandler.ScaleMinimumVector[axisIndex];
            slider.SliderValue = (targetTransform.localScale[axisIndex] - transformScaleHandler.ScaleMinimumVector[axisIndex]) / scaleRange;
            slider.OnValueUpdated.AddListener(OnSlideValueUpdated);

            return slider;
        }

        private GameObject CreateDefaultThumb()
        {
            var thumb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            thumb.AddComponent<NearInteractionGrabbable>();
            thumb.GetComponent<Renderer>().material = defaultHandleMaterial;
            thumb.GetComponent<SphereCollider>().radius *= 3.0f;
            thumb.transform.localScale = Vector3.one * 0.03f;

            return thumb;
        }

        private void OnSlideValueUpdated(SliderEventData data)
        {
            var sliderPlane = sliderPlanes[data.Slider];
            var sliderPair = sliderPlane.GetSliderPair(data.Slider);

            var axisIndex = (int)sliderPlane.Axis;
            var scaleMin = transformScaleHandler.ScaleMinimumVector[axisIndex];
            var scaleMax = transformScaleHandler.ScaleMaximumVector[axisIndex];
            var scaleRange = scaleMax - scaleMin;
            var targetTransform = transformScaleHandler.TargetTransform;

            // Update scale.
            var scale = targetTransform.localScale;
            var axisScale = (sliderPair.Value * scaleRange + scaleMin);
            scale[axisIndex] = axisScale;
            targetTransform.localScale = scale;

            // Update position.
            var position = targetTransform.localPosition;
            position[axisIndex] = ((sliderPair.PositiveSlider.SliderValue * scaleRange + scaleMin) * 0.25f) -
                                  ((sliderPair.NegativeSlider.SliderValue * scaleRange + scaleMin) * 0.25f);
            targetTransform.localPosition = position;

            // Update the opposite slider pair.
            var oppositeSliderPair = (sliderPair == sliderPlane.PositiveSliderPair) ? sliderPlane.NegativeSliderPair : sliderPlane.PositiveSliderPair;

            if (oppositeSliderPair.PositiveSlider.SliderValue != sliderPair.PositiveSlider.SliderValue)
            {
                oppositeSliderPair.PositiveSlider.SliderValue = sliderPair.PositiveSlider.SliderValue;
            }

            if (oppositeSliderPair.NegativeSlider.SliderValue != sliderPair.NegativeSlider.SliderValue)
            {
                oppositeSliderPair.NegativeSlider.SliderValue = sliderPair.NegativeSlider.SliderValue;
            }
        }

        #endregion
    }
}
