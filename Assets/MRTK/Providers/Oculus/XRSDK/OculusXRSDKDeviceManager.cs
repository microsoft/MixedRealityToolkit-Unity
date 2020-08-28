// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.XRSDK.Input;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.XRSDK.Oculus
{
    /// <summary>
    /// Manages XR SDK devices on the Oculus platform.
    /// </summary>
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        SupportedPlatforms.WindowsStandalone | SupportedPlatforms.Android,
        "XRSDK Oculus Device Manager")]
    public class OculusXRSDKDeviceManager : XRSDKDeviceManager
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public OculusXRSDKDeviceManager(
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(inputSystem, name, priority, profile) { }

        private Dictionary<Handedness, OculusHand> trackedHands = new Dictionary<Handedness, OculusHand>();

#if OCULUSINTEGRATION_PRESENT
        private OVRCameraRig cameraRig;

        private OVRHand rightHand;
        private OVRMeshRenderer righMeshRenderer;
        private OVRSkeleton rightSkeleton;

        private OVRHand leftHand;
        private OVRMeshRenderer leftMeshRenderer;
        private OVRSkeleton leftSkeleton;
#endif

        #region IMixedRealityCapabilityCheck Implementation

        /// <inheritdoc />
        public override bool CheckCapability(MixedRealityCapability capability)
        {
#if OCULUSINTEGRATION_PRESENT
            if (capability == MixedRealityCapability.ArticulatedHand)
            {
                return true;
            }
#endif
            return capability == MixedRealityCapability.MotionController;
        }

        #endregion IMixedRealityCapabilityCheck Implementation
        
        #region Controller Utilities
        /// <inheritdoc />
        protected override Type GetControllerType(SupportedControllerType supportedControllerType)
        {
            switch (supportedControllerType)
            {
                case SupportedControllerType.ArticulatedHand:
                    return typeof(OculusHand);
                case SupportedControllerType.OculusTouch:
                    return typeof(OculusXRSDKTouchController);
                default:
                    return base.GetControllerType(supportedControllerType);
            }
        }

        /// <inheritdoc />
        protected override InputSourceType GetInputSourceType(SupportedControllerType supportedControllerType)
        {
            switch (supportedControllerType)
            {
                case SupportedControllerType.OculusTouch:
                    return InputSourceType.Controller;
                case SupportedControllerType.ArticulatedHand:
                    return InputSourceType.Hand;
                default:
                    return base.GetInputSourceType(supportedControllerType);
            }
        }

        /// <inheritdoc />
        protected override SupportedControllerType GetCurrentControllerType(InputDevice inputDevice)
        {
            if (inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.HandTracking))
            {
                if (inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.Left) ||
                    inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.Right))
                {
                    // If it's a hand with a reported handedness, assume articulated hand
                    return SupportedControllerType.ArticulatedHand;
                }
            }

            if (inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.Controller))
            {
                return SupportedControllerType.OculusTouch;
            }

            return base.GetCurrentControllerType(inputDevice);
        }

        #endregion Controller Utilities


#if OCULUSINTEGRATION_PRESENT
        /// <inheritdoc/>
        public override void Enable()
        {
            base.Enable();
            SetupInput();
            ConfigurePerformancePreferences();
            MRTKOculusConfig.OnCustomHandMaterialUpdate += UpdateHandMaterial;
        }


        /// <inheritdoc/>
        public override void Update()
        {
            base.Update();
            if (OVRPlugin.GetHandTrackingEnabled())
            {
                UpdateHands();
            }
            else
            {
                RemoveAllHandDevices();
            }
        }


        private void SetupInput()
        {
            cameraRig = GameObject.FindObjectOfType<OVRCameraRig>();
            if (cameraRig == null)
            {
                var mainCamera = CameraCache.Main;

                // Instantiate camera rig as a child of the MixedRealityPlayspace
                cameraRig = GameObject.Instantiate(MRTKOculusConfig.Instance.OVRCameraRigPrefab).GetComponent<OVRCameraRig>();

                // Ensure all related game objects are configured
                cameraRig.EnsureGameObjectIntegrity();

                if (mainCamera != null)
                {
                    // We already had a main camera MRTK probably started using, let's replace the CenterEyeAnchor MainCamera with it
                    GameObject prefabMainCamera = cameraRig.trackingSpace.Find("CenterEyeAnchor").gameObject;
                    prefabMainCamera.SetActive(false);
                    mainCamera.transform.SetParent(cameraRig.trackingSpace.transform);
                    mainCamera.name = prefabMainCamera.name;
                    GameObject.Destroy(prefabMainCamera);
                }
                cameraRig.transform.SetParent(MixedRealityPlayspace.Transform);
            }
            else
            {
                // Ensure all related game objects are configured
                cameraRig.EnsureGameObjectIntegrity();
            }

            bool useAvatarHands = MRTKOculusConfig.Instance.RenderAvatarHandsInsteadOfController;
            // If using Avatar hands, de-activate ovr controller rendering
            foreach (var controllerHelper in cameraRig.gameObject.GetComponentsInChildren<OVRControllerHelper>())
            {
                controllerHelper.gameObject.SetActive(!useAvatarHands);
            }

            if (useAvatarHands)
            {
                // Initialize the local avatar controller
                GameObject.Instantiate(MRTKOculusConfig.Instance.LocalAvatarPrefab, cameraRig.trackingSpace);
            }

            var ovrHands = cameraRig.GetComponentsInChildren<OVRHand>();

            foreach (var ovrHand in ovrHands)
            {
                // Manage Hand skeleton data
                var skeletonDataProvider = ovrHand as OVRSkeleton.IOVRSkeletonDataProvider;
                var skeletonType = skeletonDataProvider.GetSkeletonType();
                var meshRenderer = ovrHand.GetComponent<OVRMeshRenderer>();

                var ovrSkeleton = ovrHand.GetComponent<OVRSkeleton>();
                if (ovrSkeleton == null)
                {
                    continue;
                }

                switch (skeletonType)
                {
                    case OVRSkeleton.SkeletonType.HandLeft:
                        leftHand = ovrHand;
                        leftSkeleton = ovrSkeleton;
                        leftMeshRenderer = meshRenderer;
                        break;
                    case OVRSkeleton.SkeletonType.HandRight:
                        rightHand = ovrHand;
                        rightSkeleton = ovrSkeleton;
                        righMeshRenderer = meshRenderer;
                        break;
                }
            }
        }

        private void ConfigurePerformancePreferences()
        {
            MRTKOculusConfig.Instance.ApplyConfiguredPerformanceSettings();
        }

        public override void Disable()
        {
            base.Disable();

            MRTKOculusConfig.OnCustomHandMaterialUpdate -= UpdateHandMaterial;
        }

        #region Hand Utilities
        protected void UpdateHands()
        {
            UpdateHand(rightHand, rightSkeleton, righMeshRenderer, Handedness.Right);
            UpdateHand(leftHand, leftSkeleton, righMeshRenderer, Handedness.Left);
        }

        protected void UpdateHand(OVRHand ovrHand, OVRSkeleton ovrSkeleton, OVRMeshRenderer ovrMeshRenderer, Handedness handedness)
        {
            // Until the ovrMeshRenderer is initialized we do nothing with the hand
            // This is a bit of a hack because the Oculus Integration fails if we touch the renderer before it has initialized itself
            if (ovrMeshRenderer == null || !ovrMeshRenderer.IsInitialized) return;

            if (ovrHand.IsTracked)
            {
                var hand = GetOrAddHand(handedness, ovrHand);
                hand.UpdateController(ovrHand, ovrSkeleton, cameraRig.trackingSpace);
            }
            else
            {
                RemoveHandDevice(handedness);
            }
        }

        private void UpdateHandMaterial()
        {
            foreach (var hand in trackedHands.Values)
            {
                hand.UpdateHandMaterial(MRTKOculusConfig.Instance.CustomHandMaterial);
            }
        }

        private OculusHand GetOrAddHand(Handedness handedness, OVRHand ovrHand)
        {
            if (trackedHands.ContainsKey(handedness))
            {
                return trackedHands[handedness];
            }

            // Add new hand
            var pointers = RequestPointers(SupportedControllerType.ArticulatedHand, handedness);
            var inputSourceType = InputSourceType.Hand;

            IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;
            var inputSource = inputSystem?.RequestNewGenericInputSource($"Oculus Quest {handedness} Hand", pointers, inputSourceType);


            OculusHand handController = new OculusHand(TrackingState.Tracked, handedness, inputSource);
            handController.InitializeHand(ovrHand, MRTKOculusConfig.Instance.CustomHandMaterial);

            for (int i = 0; i < handController.InputSource?.Pointers?.Length; i++)
            {
                handController.InputSource.Pointers[i].Controller = handController;
                handController.UpdateHandMaterial(MRTKOculusConfig.Instance.CustomHandMaterial);
            }

            inputSystem?.RaiseSourceDetected(handController.InputSource, handController);

            trackedHands.Add(handedness, handController);

            return handController;
        }

        private void RemoveHandDevice(Handedness handedness)
        {
            if (trackedHands.TryGetValue(handedness, out OculusHand hand))
            {
                RemoveHandDevice(hand);
            }
        }

        private void RemoveAllHandDevices()
        {
            if (trackedHands.Count == 0) return;

            // Create a new list to avoid causing an error removing items from a list currently being iterated on.
            foreach (var hand in new List<OculusHand>(trackedHands.Values))
            {
                RemoveHandDevice(hand);
            }
            trackedHands.Clear();
        }

        private void RemoveHandDevice(OculusHand hand)
        {
            if (hand == null) return;

            hand.CleanupHand();

            CoreServices.InputSystem?.RaiseSourceLost(hand.InputSource, hand);
            trackedHands.Remove(hand.ControllerHandedness);

            RecyclePointers(hand.InputSource);
        }
        #endregion
        
#endif
    }
}

