//------------------------------------------------------------------------------ -
//MRTK - Quest
//https ://github.com/provencher/MRTK-Quest
//------------------------------------------------------------------------------ -
//
//MIT License
//
//Copyright(c) 2020 Eric Provencher
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files(the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions :
//
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
//------------------------------------------------------------------------------ -

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using System.Linq;
using prvncher.MixedReality.Toolkit.Config;
using prvncher.MixedReality.Toolkit.Input.Teleport;
using UnityEngine;

namespace prvncher.MixedReality.Toolkit.OculusQuestInput
{
    /// <summary>
    /// Manages Oculus Quest Hand Inputs
    /// </summary>
    [MixedRealityDataProvider(typeof(IMixedRealityInputSystem), SupportedPlatforms.Android | SupportedPlatforms.WindowsStandalone, "Oculus Quest Input Manager")]
    public class OculusQuestInputManager : BaseInputDeviceManager, IMixedRealityCapabilityCheck
    {
        private Dictionary<Handedness, OculusQuestHand> trackedHands = new Dictionary<Handedness, OculusQuestHand>();
        private Dictionary<Handedness, OculusQuestController> trackedControllers = new Dictionary<Handedness, OculusQuestController>();

        private Dictionary<Handedness, OculusQuestHand> inactiveHandCache = new Dictionary<Handedness, OculusQuestHand>();
        private Dictionary<Handedness, OculusQuestController> inactiveControllerCache = new Dictionary<Handedness, OculusQuestController>();
        private Dictionary<Handedness, CustomTeleportPointer> teleportPointers = new Dictionary<Handedness, CustomTeleportPointer>();

#if OCULUSINTEGRATION_PRESENT
        private OVRCameraRig cameraRig;

        private OVRHand rightHand;
        private OVRMeshRenderer righMeshRenderer;
        private OVRSkeleton rightSkeleton;

        private OVRHand leftHand;
        private OVRMeshRenderer leftMeshRenderer;
        private OVRSkeleton leftSkeleton;
#endif

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the data provider.</param>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public OculusQuestInputManager(
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(inputSystem, name, priority, profile)
        {
        }
        
        /// <inheritdoc />
        public bool CheckCapability(MixedRealityCapability capability)
        {
            return (capability == MixedRealityCapability.ArticulatedHand);
        }
#if OCULUSINTEGRATION_PRESENT
        public override void Enable()
        {
            base.Enable();
            SetupInput();
            ConfigurePerformancePreferences();
            MRTKOculusConfig.OnCustomHandMaterialUpdate += UpdateHandMaterial;
        }

        private void SetupInput()
        {
            cameraRig = GameObject.FindObjectOfType<OVRCameraRig>();
            if (cameraRig == null)
            {
                var mainCamera = Camera.main;

                // Instantiate camera rig as a child of the MixedRealityPlayspace
                cameraRig = GameObject.Instantiate(MRTKOculusConfig.Instance.OVRCameraRigPrefab);

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

            if (useAvatarHands && !MRTKOculusConfig.Instance.AllowDevToManageAvatarPrefab)
            {
                // Initialize the local avatar controller
                GameObject.Instantiate(MRTKOculusConfig.Instance.LocalAvatarPrefab, cameraRig.trackingSpace);
            }

            var ovrHands = cameraRig.GetComponentsInChildren<OVRHand>();

            foreach (var ovrHand in ovrHands)
            {
                // Manage Hand skeleton data
                var skeltonDataProvider = ovrHand as OVRSkeleton.IOVRSkeletonDataProvider;
                var skeltonType = skeltonDataProvider.GetSkeletonType();
                var meshRenderer = ovrHand.GetComponent<OVRMeshRenderer>();

                var ovrSkelton = ovrHand.GetComponent<OVRSkeleton>();
                if (ovrSkelton == null)
                {
                    continue;
                }

                switch (skeltonType)
                {
                    case OVRSkeleton.SkeletonType.HandLeft:
                        leftHand = ovrHand;
                        leftSkeleton = ovrSkelton;
                        leftMeshRenderer = meshRenderer;
                        break;
                    case OVRSkeleton.SkeletonType.HandRight:
                        rightHand = ovrHand;
                        rightSkeleton = ovrSkelton;
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
            RemoveAllControllerDevices();
            RemoveAllHandDevices();
        }

        public override IMixedRealityController[] GetActiveControllers()
        {
            if (trackedHands.Values.Count > 0)
            {
                return trackedHands.Values.ToArray<IMixedRealityController>();
            }
            else if (trackedControllers.Values.Count > 0)
            {
                return trackedControllers.Values.ToArray<IMixedRealityController>();
            }
            return Enumerable.Empty<IMixedRealityController>().ToArray();
        }

        public override void Update()
        {
            base.Update();
            if (OVRPlugin.GetHandTrackingEnabled())
            {
                RemoveAllControllerDevices();
                UpdateHands();
            }
            else
            {
                RemoveAllHandDevices();
                UpdateControllers();
            }
        }

        #region Controller Management
        protected void UpdateControllers()
        {
            UpdateController(OVRInput.Controller.LTouch, Handedness.Left);
            UpdateController(OVRInput.Controller.RTouch, Handedness.Right);
        }

        protected void UpdateController(OVRInput.Controller controller, Handedness handedness)
        {
            if (OVRInput.IsControllerConnected(controller) && OVRInput.GetControllerPositionTracked(controller))
            {
                var touchController = GetOrAddController(handedness);
                touchController.UpdateController(controller, cameraRig.trackingSpace);
            }
            else
            {
                RemoveControllerDevice(handedness);
            }
        }

        private OculusQuestController GetOrAddController(Handedness handedness)
        {
            if (trackedControllers.ContainsKey(handedness))
            {
                return trackedControllers[handedness];
            }

            var pointers = RequestPointers(SupportedControllerType.ArticulatedHand, handedness);
            var inputSourceType = InputSourceType.Hand;

            IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;
            var inputSource = inputSystem?.RequestNewGenericInputSource($"Oculus Quest {handedness} Controller", pointers, inputSourceType);

            if (!inactiveControllerCache.TryGetValue(handedness, out var controller))
            {
                controller = new OculusQuestController(TrackingState.Tracked, handedness, inputSource);
                controller.UpdateAvatarMaterial(MRTKOculusConfig.Instance.CustomHandMaterial);
            }
            inactiveHandCache.Remove(handedness);
            controller.ApplyHandMaterial();

            for (int i = 0; i < controller.InputSource?.Pointers?.Length; i++)
            {
                controller.InputSource.Pointers[i].Controller = controller;
            }

            if (MRTKOculusConfig.Instance.ActiveTeleportPointerMode == MRTKOculusConfig.TeleportPointerMode.Custom && MixedRealityToolkit.IsTeleportSystemEnabled)
            {
                if (!teleportPointers.TryGetValue(handedness, out CustomTeleportPointer pointer))
                {
                    pointer = GameObject.Instantiate(MRTKOculusConfig.Instance.CustomTeleportPrefab).GetComponent<CustomTeleportPointer>();
                    pointer.gameObject.SetActive(false);
                    teleportPointers.Add(handedness, pointer);
                }
                pointer.Controller = controller;
                controller.TeleportPointer = pointer;
            }

            inputSystem?.RaiseSourceDetected(controller.InputSource, controller);

            trackedControllers.Add(handedness, controller);

            return controller;
        }

        private void RemoveControllerDevice(Handedness handedness)
        {
            if (trackedControllers.TryGetValue(handedness, out OculusQuestController controller))
            {
                RemoveControllerDevice(controller);
            }
        }

        private void RemoveAllControllerDevices()
        {
            if (trackedControllers.Count == 0) return;

            // Create a new list to avoid causing an error removing items from a list currently being iterated on.
            foreach (var controller in new List<OculusQuestController>(trackedControllers.Values))
            {
                RemoveControllerDevice(controller);
            }
            trackedControllers.Clear();
        }

        private void RemoveControllerDevice(OculusQuestController controller)
        {
            if (controller == null) return;
            CoreServices.InputSystem?.RaiseSourceLost(controller.InputSource, controller);
            trackedControllers.Remove(controller.ControllerHandedness);

            if (teleportPointers.TryGetValue(controller.ControllerHandedness, out CustomTeleportPointer pointer))
            {
                if (pointer == null)
                {
                    teleportPointers.Remove(controller.ControllerHandedness);
                }
                else
                {
                    pointer.Reset();
                }
                controller.TeleportPointer = null;
            }

            RecyclePointers(controller.InputSource);
        }
        #endregion
      
        #region Hand Management
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
            foreach (var hand in inactiveHandCache.Values)
            {
                hand.UpdateHandMaterial(MRTKOculusConfig.Instance.CustomHandMaterial);
            }

            foreach (var controller in trackedControllers.Values)
            {
                controller.UpdateAvatarMaterial(MRTKOculusConfig.Instance.CustomHandMaterial);
            }
            foreach (var controller in inactiveControllerCache.Values)
            {
                controller.UpdateAvatarMaterial(MRTKOculusConfig.Instance.CustomHandMaterial);
            }
        }

        private OculusQuestHand GetOrAddHand(Handedness handedness, OVRHand ovrHand)
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

            if (!inactiveHandCache.TryGetValue(handedness, out var handController))
            {
                handController = new OculusQuestHand(TrackingState.Tracked, handedness, inputSource);
                handController.InitializeHand(ovrHand, MRTKOculusConfig.Instance.CustomHandMaterial);
            }
            inactiveHandCache.Remove(handedness);

            for (int i = 0; i < handController.InputSource?.Pointers?.Length; i++)
            {
                handController.InputSource.Pointers[i].Controller = handController;
            }

            if (MRTKOculusConfig.Instance.ActiveTeleportPointerMode == MRTKOculusConfig.TeleportPointerMode.Custom &&  MixedRealityToolkit.IsTeleportSystemEnabled)
            {
                if (!teleportPointers.TryGetValue(handedness, out CustomTeleportPointer pointer))
                {
                    pointer = GameObject.Instantiate(MRTKOculusConfig.Instance.CustomTeleportPrefab).GetComponent<CustomTeleportPointer>();
                    pointer.gameObject.SetActive(false);
                    teleportPointers.Add(handedness, pointer);
                }
                pointer.Controller = handController;
                handController.TeleportPointer = pointer;
            }

            inputSystem?.RaiseSourceDetected(handController.InputSource, handController);

            trackedHands.Add(handedness, handController);

            return handController;
        }

        private void RemoveHandDevice(Handedness handedness)
        {
            if (trackedHands.TryGetValue(handedness, out OculusQuestHand hand))
            {
                RemoveHandDevice(hand);
            }
        }

        private void RemoveAllHandDevices()
        {
            if (trackedHands.Count == 0) return;

            // Create a new list to avoid causing an error removing items from a list currently being iterated on.
            foreach (var hand in new List<OculusQuestHand>(trackedHands.Values))
            {
                RemoveHandDevice(hand);
            }
            trackedHands.Clear();
        }

        private void RemoveHandDevice(OculusQuestHand hand)
        {
            if (hand == null) return;

            if (teleportPointers.TryGetValue(hand.ControllerHandedness, out CustomTeleportPointer pointer))
            {
                if (pointer == null)
                {
                    teleportPointers.Remove(hand.ControllerHandedness);
                }
                else
                {
                    pointer.Reset();
                }
                hand.TeleportPointer = null;
            }

            hand.CleanupHand();
            inactiveHandCache.Add(hand.ControllerHandedness, hand);

            CoreServices.InputSystem?.RaiseSourceLost(hand.InputSource, hand);
            trackedHands.Remove(hand.ControllerHandedness);

            RecyclePointers(hand.InputSource);
        }
        #endregion
#endif
    }
}
