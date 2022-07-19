// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.OpenXR;
using Microsoft.MixedReality.Toolkit.Subsystems;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Basic controller visualizer which draws a generic motion controller when one is detected
    /// </summary>
    [AddComponentMenu("MRTK/Input/Controller Visualizer")]
    public class ControllerVisualizer : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The XRNode on which this hand is located.")]
        private XRNode handNode = XRNode.LeftHand;

        /// <summary> The XRNode on which this hand is located. </summary>
        public XRNode HandNode { get => handNode; set => handNode = value; }

        // caching the controller we belong to
        private XRBaseController baseController;

        protected void OnEnable()
        {
            Debug.Assert(handNode == XRNode.LeftHand || handNode == XRNode.RightHand, $"HandVisualizer has an invalid XRNode ({handNode})!");

            ControllerLookup[] lookups = GameObject.FindObjectsOfType(typeof(ControllerLookup)) as ControllerLookup[];
            baseController = lookups[0].LeftHandController;

            InputDevices.deviceConnected += CheckToShowVisuals;
        }

        protected void OnDisable()
        {
            InputDevices.deviceConnected -= CheckToShowVisuals;
            baseController.hideControllerModel = true;
        }

        private const InputDeviceCharacteristics controllerCharacteristics = InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Left;

        private void CheckToShowVisuals(InputDevice inputDevice)
        {
            if (inputDevice.isValid && (inputDevice.characteristics & controllerCharacteristics) == controllerCharacteristics)
            {
                TryGenerateControllerModelFromPlatformSDK(ControllerModel.Left);
                baseController.hideControllerModel = false;
            }
            else
            {
                baseController.hideControllerModel = false;
            }
        }

        public async Task<GameObject> TryGenerateControllerModelFromPlatformSDK(ControllerModel controllerModelProvider)
        {
            GameObject gltfGameObject = null;

#if MROPENXR_PRESENT && (UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_ANDROID)
            if (!controllerModelProvider.TryGetControllerModelKey(out ulong modelKey))
            {
                Debug.LogError("Failed to obtain controller model key from platform.");
                return null;
            }

            if (ControllerModelDictionary.TryGetValue(modelKey, out gltfGameObject))
            {
                gltfGameObject.SetActive(true);
                return gltfGameObject;
            }

            byte[] modelStream = await controllerModelProvider.TryGetControllerModel(modelKey);

            if (modelStream == null || modelStream.Length == 0)
            {
                Debug.LogError("Failed to obtain controller model from platform.");
                return null;
            }

            Utilities.Gltf.Schema.GltfObject gltfObject = GltfUtility.GetGltfObjectFromGlb(modelStream);
            gltfGameObject = await gltfObject.ConstructAsync();

            if (gltfGameObject != null)
            {
                // After all the awaits, double check that another task didn't finish earlier
                if (ControllerModelDictionary.TryGetValue(modelKey, out GameObject existingGameObject))
                {
                    Object.Destroy(gltfGameObject);
                    return existingGameObject;
                }
                else
                {
                    ControllerModelDictionary.Add(modelKey, gltfGameObject);
                }
            }
#endif //MROPENXR_PRESENT && (UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_ANDROID)

            return gltfGameObject;
        }


        /// <inheritdoc />
        public virtual void OnSourceDetected(SourceStateEventData eventData) { }

        /// <inheritdoc />
        public virtual void OnSourceLost(SourceStateEventData eventData)
        {
            if (eventData.SourceId == Controller?.InputSource.SourceId &&
                eventData.Controller?.ControllerHandedness == Handedness)
            {
                poseActionDetected = false;
                TrackingState = TrackingState.NotTracked;

                if (DestroyOnSourceLost)
                {
                    GameObjectExtensions.DestroyGameObject(gameObject);
                }
            }
        }


        protected override GenericXRSDKController GetOrAddController(InputDevice inputDevice)
        {
            using (GetOrAddControllerPerfMarker.Auto())
            {
                InputDeviceCharacteristics inputDeviceCharacteristics = inputDevice.characteristics;

                // If this is a new input device, search if an existing input device has matching characteristics
                if (!ActiveControllers.ContainsKey(inputDevice))
                {
                    foreach (InputDevice device in ActiveControllers.Keys)
                    {
                        InputDeviceCharacteristics deviceCharacteristics = device.characteristics;

                        if (((deviceCharacteristics.IsMaskSet(InputDeviceCharacteristics.Controller) && inputDeviceCharacteristics.IsMaskSet(InputDeviceCharacteristics.Controller))
                            || (deviceCharacteristics.IsMaskSet(InputDeviceCharacteristics.HandTracking) && inputDeviceCharacteristics.IsMaskSet(InputDeviceCharacteristics.HandTracking)))
                            && ((deviceCharacteristics.IsMaskSet(InputDeviceCharacteristics.Left) && inputDeviceCharacteristics.IsMaskSet(InputDeviceCharacteristics.Left))
                            || (deviceCharacteristics.IsMaskSet(InputDeviceCharacteristics.Right) && inputDeviceCharacteristics.IsMaskSet(InputDeviceCharacteristics.Right))))
                        {
                            ActiveControllers.Add(inputDevice, ActiveControllers[device]);
                            break;
                        }
                    }
                }

                if (inputDeviceCharacteristics.IsMaskSet(InputDeviceCharacteristics.HandTracking)
                    && inputDevice.TryGetFeatureValue(CommonUsages.isTracked, out bool isTracked)
                    && !isTracked)
                {
                    // If this is an input device from the Microsoft Hand Interaction profile, it doesn't go invalid but instead goes untracked. Ignore it if untracked.
                    return null;
                }

                return base.GetOrAddController(inputDevice);
            }
        }
    }
}
