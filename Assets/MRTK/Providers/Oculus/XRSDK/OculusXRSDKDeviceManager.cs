// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.XRSDK.Input;
using System;
using UnityEngine;
using UnityEngine.XR;

#if OCULUS_ENABLED
using Unity.XR.Oculus;
#endif // OCULUS_ENABLED

#if OCULUSINTEGRATION_PRESENT
using System.Collections.Generic;
#endif // OCULUSINTEGRATION_PRESENT

namespace Microsoft.MixedReality.Toolkit.XRSDK.Oculus.Input
{
    /// <summary>
    /// Manages XR SDK devices on the Oculus platform.
    /// </summary>
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        SupportedPlatforms.WindowsStandalone | SupportedPlatforms.Android,
        "XR SDK Oculus Device Manager",
        "Oculus/XRSDK/Profiles/DefaultOculusXRSDKDeviceManagerProfile.asset",
        "MixedRealityToolkit.Providers",
        true,
        SupportedUnityXRPipelines.XRSDK)]
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

#if !OCULUSINTEGRATION_PRESENT && UNITY_EDITOR && UNITY_ANDROID
        public override void Initialize()
        {
            base.Initialize();
#if !UNITY_2020_1_OR_NEWER
            UnityEngine.Debug.Log(@"Detected a potential deployment issue for the Oculus Quest. In order to use hand tracking with the Oculus Quest, download the Oculus Integration Package from the Unity Asset Store and run the Integration tool before deploying.
The tool can be found under <i>Mixed Reality > Toolkit > Utilities > Oculus > Integrate Oculus Integration Unity Modules</i>");
#endif
        }
#endif

#if OCULUSINTEGRATION_PRESENT
        private readonly Dictionary<Handedness, OculusHand> trackedHands = new Dictionary<Handedness, OculusHand>();

        private OVRCameraRig cameraRig;
        private OVRControllerHelper leftControllerHelper;
        private OVRControllerHelper rightControllerHelper;

        private OVRHand rightHand;
        private OVRSkeleton rightSkeleton;

        private OVRHand leftHand;
        private OVRSkeleton leftSkeleton;

        /// <summary>
        /// The profile that contains settings for the Oculus XRSDK Device Manager input data provider.  This profile is nested under 
        /// Input > Input Data Providers > Oculus XRSDK Device Manager in the MixedRealityToolkit object in the hierarchy.
        /// </summary>
        private OculusXRSDKDeviceManagerProfile SettingsProfile => ConfigurationProfile as OculusXRSDKDeviceManagerProfile;
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

#if OCULUSINTEGRATION_PRESENT
        /// <inheritdoc />
        protected override GenericXRSDKController GetOrAddController(InputDevice inputDevice)
        {
            GenericXRSDKController controller = base.GetOrAddController(inputDevice);

            if (!cameraRig.IsNull() && controller is OculusXRSDKTouchController oculusTouchController && oculusTouchController.OculusControllerVisualization == null)
            {
                GameObject platformVisualization = null; 
                if (oculusTouchController.ControllerHandedness == Handedness.Left)
                {
                    platformVisualization = leftControllerHelper.gameObject;
                }
                if (oculusTouchController.ControllerHandedness == Handedness.Right)
                {
                    platformVisualization = rightControllerHelper.gameObject;
                }

                if(platformVisualization != null)
                {
                    oculusTouchController.RegisterControllerVisualization(platformVisualization);
                }
            }

            return controller;
        }
#endif

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
            if (inputDevice.characteristics.IsMaskSet(InputDeviceCharacteristics.HandTracking))
            {
                if (inputDevice.characteristics.IsMaskSet(InputDeviceCharacteristics.Left) ||
                    inputDevice.characteristics.IsMaskSet(InputDeviceCharacteristics.Right))
                {
                    // If it's a hand with a reported handedness, assume articulated hand
                    return SupportedControllerType.ArticulatedHand;
                }
            }

            if (inputDevice.characteristics.IsMaskSet(InputDeviceCharacteristics.Controller))
            {
                return SupportedControllerType.OculusTouch;
            }

            return base.GetCurrentControllerType(inputDevice);
        }

        #endregion Controller Utilities

        private bool? IsActiveLoader =>
#if OCULUS_ENABLED
            LoaderHelpers.IsLoaderActive<OculusLoader>();
#else
            false;
#endif // OCULUS_ENABLED

        /// <inheritdoc/>
        public override void Enable()
        {
            if (!IsActiveLoader.HasValue)
            {
                IsEnabled = false;
                EnableIfLoaderBecomesActive();
                return;
            }
            else if (!IsActiveLoader.Value)
            {
                IsEnabled = false;
                return;
            }

#if OCULUSINTEGRATION_PRESENT
            SetupInput();
            ConfigurePerformancePreferences();
#endif // OCULUSINTEGRATION_PRESENT

            base.Enable();
        }

        private async void EnableIfLoaderBecomesActive()
        {
            await new WaitUntil(() => IsActiveLoader.HasValue);
            if (IsActiveLoader.Value)
            {
                Enable();
            }
        }

#if OCULUSINTEGRATION_PRESENT
        /// <inheritdoc/>
        public override void Update()
        {
            if (!IsEnabled)
            {
                return;
            }

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

                var cameraRigObject = GameObject.Instantiate(SettingsProfile.OVRCameraRigPrefab);
                cameraRig = cameraRigObject.GetComponent<OVRCameraRig>();

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

            bool useAvatarHands = SettingsProfile.RenderAvatarHandsWithControllers;
            // If using Avatar hands, initialize the local avatar controller
            if (useAvatarHands)
            {
                GameObject.Instantiate(SettingsProfile.LocalAvatarPrefab, cameraRig.trackingSpace);
            }


            var ovrControllerHelpers = cameraRig.GetComponentsInChildren<OVRControllerHelper>();
            foreach (var ovrControllerHelper in ovrControllerHelpers)
            {
                switch (ovrControllerHelper.m_controller)
                {
                    case OVRInput.Controller.LTouch:
                        leftControllerHelper = ovrControllerHelper;
                        break;
                    case OVRInput.Controller.RTouch:
                        rightControllerHelper = ovrControllerHelper;
                        break;
                    default:
                        break;
                }
            }

            var ovrHands = cameraRig.GetComponentsInChildren<OVRHand>();
            foreach (var ovrHand in ovrHands)
            {
                // Manage Hand skeleton data
                var skeletonDataProvider = ovrHand as OVRSkeleton.IOVRSkeletonDataProvider;
                var skeletonType = skeletonDataProvider.GetSkeletonType();

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
                        break;
                    case OVRSkeleton.SkeletonType.HandRight:
                        rightHand = ovrHand;
                        rightSkeleton = ovrSkeleton;
                        break;
                }
            }
        }

        private void ConfigurePerformancePreferences()
        {
            SettingsProfile.ApplyConfiguredPerformanceSettings();
        }

        #region Hand Utilities

        protected void UpdateHands()
        {
            UpdateHand(rightHand, rightSkeleton, Handedness.Right);
            UpdateHand(leftHand, leftSkeleton, Handedness.Left);
        }

        protected void UpdateHand(OVRHand ovrHand, OVRSkeleton ovrSkeleton, Handedness handedness)
        {
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

        private OculusHand GetOrAddHand(Handedness handedness, OVRHand ovrHand)
        {
            if (trackedHands.ContainsKey(handedness))
            {
                return trackedHands[handedness];
            }

            // Add new hand
            var pointers = RequestPointers(SupportedControllerType.ArticulatedHand, handedness);
            var inputSourceType = InputSourceType.Hand;

            var inputSource = Service?.RequestNewGenericInputSource($"Oculus Quest {handedness} Hand", pointers, inputSourceType);


            OculusHand handDevice = new OculusHand(TrackingState.Tracked, handedness, inputSource);
            handDevice.InitializeHand(ovrHand, SettingsProfile);

            for (int i = 0; i < handDevice.InputSource?.Pointers?.Length; i++)
            {
                handDevice.InputSource.Pointers[i].Controller = handDevice;
            }

            Service?.RaiseSourceDetected(handDevice.InputSource, handDevice);

            trackedHands.Add(handedness, handDevice);

            return handDevice;
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

        private void RemoveHandDevice(OculusHand handDevice)
        {
            if (handDevice == null) return;

            CoreServices.InputSystem?.RaiseSourceLost(handDevice.InputSource, handDevice);
            trackedHands.Remove(handDevice.ControllerHandedness);

            RecyclePointers(handDevice.InputSource);
        }

        #endregion
#endif // OCULUSINTEGRATION_PRESENT
    }
}
