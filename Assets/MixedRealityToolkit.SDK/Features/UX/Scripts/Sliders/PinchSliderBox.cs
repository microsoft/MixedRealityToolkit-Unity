// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;
using static Microsoft.MixedReality.Toolkit.UI.PinchSlider;

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

        private const int PositiveIndex = 0;
        private const int NegativeIndex = 1;

        private class SliderPair
        {
            public PinchSlider[] Sliders = new PinchSlider[2];

            public float Value
            {
                get
                {
                    return (Sliders[PositiveIndex].SliderValue + 
                            Sliders[NegativeIndex].SliderValue) * 0.5f;
                }
            }
        }

        private class SliderPlane
        {
            public SliderAxis Axis = SliderAxis.XAxis;
            public SliderPair[] SliderPairs = new SliderPair[2];

            public SliderPair GetSliderPair(PinchSlider slider)
            {
                if (slider == SliderPairs[PositiveIndex].Sliders[PositiveIndex] ||
                    slider == SliderPairs[PositiveIndex].Sliders[NegativeIndex])
                {
                    return SliderPairs[PositiveIndex];
                }
                else
                {
                    return SliderPairs[NegativeIndex];
                }
            }
        }

        private const int SliderPlaneCount = 3;
        private SliderPlane[] sliderPlanes = new SliderPlane[SliderPlaneCount];
        private Dictionary<PinchSlider, SliderPlane> sliderToPlane = new Dictionary<PinchSlider, SliderPlane>();

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

            // Create a pivot object to contain the sliders and aide in non-uniform scaling. 
            pivot = new GameObject("ClippingBoxControlPivot").transform;
            pivot.position = transformScaleHandler.TargetTransform.position;
            pivot.rotation = transformScaleHandler.TargetTransform.rotation;
            pivot.parent = transformScaleHandler.TargetTransform.parent;
            transformScaleHandler.TargetTransform.parent = pivot;

            if (showXAxisHandles)
            {
                sliderPlanes[(int)SliderAxis.XAxis] = AddSliderPlane(SliderAxis.XAxis);
            }

            if (ShowYAxisHandles)
            {
                sliderPlanes[(int)SliderAxis.YAxis] = AddSliderPlane(SliderAxis.YAxis);
            }

            if (ShowZAxisHandles)
            {
                sliderPlanes[(int)SliderAxis.ZAxis] = AddSliderPlane(SliderAxis.ZAxis);
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
                pivot = null;
            }

            for (var i = 0; i < SliderPlaneCount; ++i)
            {
                sliderPlanes[i] = null;
            }

            sliderToPlane.Clear();
        }

        #endregion

        #region Private Methods

        private SliderPlane AddSliderPlane(SliderAxis axis)
        {
            var sliders = new PinchSlider[4];
            var globalDirection = 1.0f;

            for (var i = 0; i < 2; ++i)
            {
                var localDirection = 1.0f;

                for (var j = 0; j < 2; ++j)
                {
                    sliders[i * 2 + j] = AddSlider(axis, globalDirection, localDirection);
                    localDirection = -localDirection;
                }

                globalDirection = -globalDirection;
            }

            var sliderPlane = new SliderPlane()
            {
                Axis = axis,
                SliderPairs = new SliderPair[]
                {
                    new SliderPair() { Sliders = new PinchSlider[] { sliders[0], sliders[1] } },
                    new SliderPair() { Sliders = new PinchSlider[] { sliders[2], sliders[3] } }
                }
            };

            foreach (var slider in sliders)
            {
                sliderToPlane.Add(slider, sliderPlane);
            }

            return sliderPlane;
        }

        private PinchSlider AddSlider(SliderAxis axis, float globalDirection, float localDirection)
        {
            var slider = new GameObject($"Slider {axis} {globalDirection} {localDirection}").AddComponent<PinchSlider>();
            slider.transform.parent = pivot;

            // Calculates a normal to the pinch slider axis to place the slider at.
            var targetTransform = transformScaleHandler.TargetTransform;
            var axisNormal = GetSliderAxis(GetNormalAxis(axis));
            var axisNormalHalfScale = Vector3.Dot(axisNormal, targetTransform.localScale) * 0.5f;
            slider.transform.position = targetTransform.position + (((targetTransform.rotation * axisNormal) * axisNormalHalfScale) * globalDirection);
            slider.transform.rotation = targetTransform.rotation;

            slider.SliderAxisType = axis;
            var axisIndex = (int)axis;
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

        private SliderAxis GetNormalAxis(SliderAxis axis)
        {
            return (SliderAxis)(((int)axis + 1) % SliderPlaneCount);
        }

        private GameObject CreateDefaultThumb()
        {
            var thumb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            thumb.name = "Thumb";
            thumb.AddComponent<NearInteractionGrabbable>();
            thumb.GetComponent<Renderer>().material = defaultHandleMaterial;
            thumb.GetComponent<SphereCollider>().radius *= 3.0f;
            thumb.transform.localScale = Vector3.one * 0.03f;

            return thumb;
        }

        private void OnSlideValueUpdated(SliderEventData data)
        {
            var sliderPlane = sliderToPlane[data.Slider];
            var sliderPair = sliderPlane.GetSliderPair(data.Slider);

            var axisIndex = (int)sliderPlane.Axis;
            var scaleMin = transformScaleHandler.ScaleMinimumVector[axisIndex];
            var scaleMax = transformScaleHandler.ScaleMaximumVector[axisIndex];
            var scaleRange = scaleMax - scaleMin;
            var targetTransform = transformScaleHandler.TargetTransform;

            // Update scale.
            var scale = targetTransform.localScale;
            scale[axisIndex] = (sliderPair.Value * scaleRange + scaleMin);
            targetTransform.localScale = scale;

            // Update position.
            var position = targetTransform.localPosition;
            position[axisIndex] = ((sliderPair.Sliders[PositiveIndex].SliderValue * scaleRange + scaleMin) * 0.25f) -
                                  ((sliderPair.Sliders[NegativeIndex].SliderValue * scaleRange + scaleMin) * 0.25f);
            targetTransform.localPosition = position;

            // Update the opposite slider pair.
            var oppositeSliderPair = (sliderPair == sliderPlane.SliderPairs[PositiveIndex]) ? sliderPlane.SliderPairs[NegativeIndex] : 
                                                                                              sliderPlane.SliderPairs[PositiveIndex];
            for (var i = 0; i < 2; ++i)
            {
                if (oppositeSliderPair.Sliders[i].SliderValue != sliderPair.Sliders[i].SliderValue)
                {
                    oppositeSliderPair.Sliders[i].SliderValue = sliderPair.Sliders[i].SliderValue;
                }
            }

            // Update the position of sliders on the modified plane.
            var copanarSliderPlane = sliderPlanes[(axisIndex + 2) % SliderPlaneCount];

            if (copanarSliderPlane != null)
            {
                var axisNormal = GetSliderAxis(GetNormalAxis(copanarSliderPlane.Axis));
                var axisNormalInverse = axisNormal;

                for (var i = 0; i < 3; ++i)
                {
                    axisNormalInverse[i] = axisNormalInverse[i] == 1.0f ? 0.0f : 1.0f;
                }

                var axisNormalHalfScale = Vector3.Dot(axisNormal, targetTransform.localScale) * 0.5f;
                var globalDirection = 1.0f;

                for (var i = 0; i < 2; ++i)
                {
                    for (var j = 0; j < 2; ++j)
                    {
                        var slider = copanarSliderPlane.SliderPairs[i].Sliders[j];
                        slider.transform.position = targetTransform.position + (((targetTransform.rotation * axisNormal) * axisNormalHalfScale) * globalDirection);

                        // Remove any translation due to scale.
                        slider.transform.localPosition -= Vector3.Scale(slider.transform.localPosition, axisNormalInverse);
                    }

                    globalDirection = -globalDirection;
                }
            }
        }

        #endregion
    }
}
