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
    public class ClippingBoxControl : MonoBehaviour
    {
        #region Serialized Fields and Properties

        [SerializeField, Tooltip("TODO")]
        private GameObject handlePrefab = null;

        /// <summary>
        /// TODO
        /// </summary>
        public GameObject HandlePrefab
        {
            get => handlePrefab;
            set => handlePrefab = value;
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
            public int Axis = 0;
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

        public void CreateHandles()
        {
            DestroyHandles();

            pivot = new GameObject("ClippingBoxControlPivot").transform;
            pivot.position = transformScaleHandler.TargetTransform.position;
            pivot.parent = transformScaleHandler.TargetTransform.parent;
            transformScaleHandler.TargetTransform.parent = pivot;

            // TODO, make configurable.
            // XZ plane
            var sliders = new List<PinchSlider>();
            var positiveSliderPair = new SliderPair()
            {
                PositiveSlider = AddSlider(0, 1.0f, 1.0f),
                NegativeSlider = AddSlider(0, 1.0f, -1.0f)
            };

            sliders.Add(positiveSliderPair.PositiveSlider);
            sliders.Add(positiveSliderPair.NegativeSlider);

            var negativeSliderPair = new SliderPair()
            {
                PositiveSlider = AddSlider(0, -1.0f, 1.0f),
                NegativeSlider = AddSlider(0, -1.0f, -1.0f)
            };

            sliders.Add(negativeSliderPair.PositiveSlider);
            sliders.Add(negativeSliderPair.NegativeSlider);

            var sliderPlane = new SliderPlane()
            {
                Axis = 0, 
                PositiveSliderPair = positiveSliderPair,
                NegativeSliderPair = negativeSliderPair
            };

            pinchSliders.AddRange(sliders);

            foreach (var slider in sliders)
            {
                sliderPlanes.Add(slider, sliderPlane);
            }
        }

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

        private PinchSlider AddSlider(int axis, float axisSign, float axisDirection)
        {
            var targetTransform = transformScaleHandler.TargetTransform;
            var currentScale = targetTransform.localScale;

            var slider = new GameObject($"Slider{axis}{axisSign}{axisDirection}").AddComponent<PinchSlider>();
            slider.transform.parent = transform;

            // TODO, make generic.
            slider.transform.position = targetTransform.position - (targetTransform.forward * (currentScale.z * 0.5f) * axisSign);
            slider.transform.rotation = targetTransform.rotation;
            slider.SliderStartDistance = transformScaleHandler.ScaleMinimumVector[axis] * 0.5f * axisDirection;
            slider.SliderEndDistance = transformScaleHandler.ScaleMaximumVector[axis] * 0.5f * axisDirection;

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

            var scaleRange = transformScaleHandler.ScaleMaximumVector[axis] - transformScaleHandler.ScaleMinimumVector[axis];
            slider.SliderValue = (currentScale[axis] - transformScaleHandler.ScaleMinimumVector[axis]) / scaleRange;
            slider.OnValueUpdated.AddListener(OnSlideValueUpdated);

            return slider;
        }

        private GameObject CreateDefaultThumb()
        {
            var thumb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            thumb.AddComponent<NearInteractionGrabbable>();
            thumb.GetComponent<Renderer>().material = defaultHandleMaterial;
            thumb.GetComponent<SphereCollider>().radius *= 2.0f;
            thumb.transform.localScale = Vector3.one * 0.02f;

            return thumb;
        }

        private void OnSlideValueUpdated(SliderEventData data)
        {
            var sliderPlane = sliderPlanes[data.Slider];
            var sliderPair = sliderPlane.GetSliderPair(data.Slider);

            var axis = sliderPlane.Axis;
            var scaleMin = transformScaleHandler.ScaleMinimumVector[axis];
            var scaleMax = transformScaleHandler.ScaleMaximumVector[axis];
            var scaleRange = scaleMax - scaleMin;
            var targetTransform = transformScaleHandler.TargetTransform;

            // Update scale.
            var scale = targetTransform.localScale;
            var axisScale = (sliderPair.Value * scaleRange + scaleMin);
            scale[axis] = axisScale;
            targetTransform.localScale = scale;

            // Update position.
            var position = targetTransform.localPosition;
            position[axis] = ((sliderPair.PositiveSlider.SliderValue * scaleRange + scaleMin) * 0.25f) -
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
