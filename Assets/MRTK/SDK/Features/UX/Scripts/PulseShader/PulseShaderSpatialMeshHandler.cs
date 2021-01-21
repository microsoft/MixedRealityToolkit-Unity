// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.PulseShader
{
    /// <summary>
    /// Script for triggering the pulse shader effect on the spatial mesh.
    /// </summary>
    public class PulseShaderSpatialMeshHandler : PulseShaderHandler, IMixedRealityPointerHandler
    {
        [Space]
        [SerializeField]
        [Tooltip("Trigger the pulse shader animation on select (air-tap/pinch) of the spatial mesh. ")]
        private bool pulseOnSelect = true;

        /// <summary>
        /// Trigger the pulse shader animation on select (air-tap/pinch) of the spatial mesh. 
        /// </summary>
        public bool PulseOnSelect
        {
            get { return pulseOnSelect; }
            set
            {
                if (pulseOnSelect != value)
                {
                    pulseOnSelect = value;
                }
            }
        }

        [SerializeField]
        [Tooltip("Automatically set pulse origin to the main camera location.")]
        private bool originFollowCamera = false;

        /// <summary>
        /// Automatically set pulse origin to the main camera location.
        /// </summary>
        public bool OriginFollowCamera
        {
            get { return originFollowCamera; }
            set
            {
                if (originFollowCamera != value)
                {
                    originFollowCamera = value;
                }
            }
        }

        protected override void Start()
        {
            base.Start();

            if (PulseOnSelect)
            {
                // Add PointerHandler script to the parent of dynamically generated spatial mesh on the device
                CoreServices.SpatialAwarenessSystem.SpatialAwarenessObjectParent.AddComponent<PointerHandler>();
                CoreServices.SpatialAwarenessSystem.SpatialAwarenessObjectParent.GetComponent<PointerHandler>().OnPointerClicked.AddListener(this.OnPointerClicked);
            }
        }

        protected override void Update()
        {
            if (OriginFollowCamera)
            {
                SetLocalOrigin(CameraCache.Main.transform.position);
            }
        }
        protected void TriggerAnimationOnSpatialMesh(Vector3 pulseAnimationOrigin)
        {
            SetLocalOrigin(pulseAnimationOrigin);
            PulseOnce();
        }

        #region Pointer Handler

        public void OnPointerDown(MixedRealityPointerEventData eventData) { }
        public void OnPointerDragged(MixedRealityPointerEventData eventData) { }
        public void OnPointerUp(MixedRealityPointerEventData eventData) { }

        public void OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            Vector3 pulseAnimationOrigin = eventData.Pointer.Result.Details.Point;
            TriggerAnimationOnSpatialMesh(pulseAnimationOrigin);
        }

        #endregion
    }
}
