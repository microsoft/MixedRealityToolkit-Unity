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
using prvncher.MixedReality.Toolkit.Config;
using prvncher.MixedReality.Toolkit.Input.Teleport;
using UnityEngine;
using TeleportPointer = Microsoft.MixedReality.Toolkit.Teleport.TeleportPointer;

namespace prvncher.MixedReality.Toolkit.OculusQuestInput
{
    [MixedRealityController(SupportedControllerType.ArticulatedHand, new[] { Handedness.Left, Handedness.Right })]
    public class OculusQuestController : BaseController, IMixedRealityHand
    {
        private MixedRealityPose currentPointerPose = MixedRealityPose.ZeroIdentity;

        private MixedRealityPose currentIndexPose = MixedRealityPose.ZeroIdentity;
        private MixedRealityPose currentGripPose = MixedRealityPose.ZeroIdentity;

        /// <summary>
        /// Teleport pointer reference. Needs custom pointer because MRTK does not support teleporting with articulated hands.
        /// </summary>
        public CustomTeleportPointer TeleportPointer { get; set; }

        private List<Renderer> handRenderers = new List<Renderer>();
        private Material handMaterial = null;

        private bool isInPointingPose = true;
        public override bool IsInPointingPose => isInPointingPose;

        #region AvatarHandReferences
        private GameObject handRoot = null;
        private GameObject handGrip = null;

        private GameObject handIndex1 = null;
        private GameObject handIndex2 = null;
        private GameObject handIndex3 = null;
        private GameObject handIndexTip = null;

        private GameObject handMiddle1 = null;
        private GameObject handMiddle2 = null;
        private GameObject handMiddle3 = null;
        private GameObject handMiddleTip = null;

        private GameObject handRing1 = null;
        private GameObject handRing2 = null;
        private GameObject handRing3 = null;
        private GameObject handRingTip = null;

        private GameObject handPinky0 = null;
        private GameObject handPinky1 = null;
        private GameObject handPinky2 = null;
        private GameObject handPinky3 = null;
        private GameObject handPinkyTip = null;

        private GameObject handThumb1 = null;
        private GameObject handThumb2 = null;
        private GameObject handThumb3 = null;
        private GameObject handThumbTip = null;
        #endregion

        protected readonly Dictionary<TrackedHandJoint, MixedRealityPose> jointPoses = new Dictionary<TrackedHandJoint, MixedRealityPose>();

        private const float cTriggerDeadZone = 0.1f;
        private int pinchStrengthProp;


        public OculusQuestController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
            pinchStrengthProp = Shader.PropertyToID(MRTKOculusConfig.Instance.PinchStrengthMaterialProperty);
        }

        /// <summary>
        /// The Windows Mixed Reality Controller default interactions.
        /// </summary>
        /// <remarks>A single interaction mapping works for both left and right controllers.</remarks>
        public override MixedRealityInteractionMapping[] DefaultInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, new MixedRealityInputAction(4, "Pointer Pose", AxisType.SixDof)),
            new MixedRealityInteractionMapping(1, "Spatial Grip", AxisType.SixDof, DeviceInputType.SpatialGrip, new MixedRealityInputAction(3, "Grip Pose", AxisType.SixDof)),
            new MixedRealityInteractionMapping(2, "Select", AxisType.Digital, DeviceInputType.Select, new MixedRealityInputAction(1, "Select", AxisType.Digital)),
            new MixedRealityInteractionMapping(3, "Grab", AxisType.SingleAxis, DeviceInputType.TriggerPress, new MixedRealityInputAction(7, "Grip Press", AxisType.SingleAxis)),
            new MixedRealityInteractionMapping(4, "Index Finger Pose", AxisType.SixDof, DeviceInputType.IndexFinger,  new MixedRealityInputAction(13, "Index Finger Pose", AxisType.SixDof)),
        };

        public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => DefaultInteractions;

        public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions => DefaultInteractions;

        public override void SetupDefaultInteractions()
        {
            AssignControllerMappings(DefaultInteractions);
        }

#if OCULUSINTEGRATION_PRESENT
        /// <summary>
        /// Update the controller data from the provided platform state
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        public void UpdateController(OVRInput.Controller ovrController, Transform trackingRoot)
        {
            if (!Enabled || trackingRoot == null)
            {
                return;
            }

            IsPositionAvailable = OVRInput.GetControllerPositionValid(ovrController);
            IsRotationAvailable = OVRInput.GetControllerOrientationValid(ovrController);

            // Update transform
            Vector3 localPosition = OVRInput.GetLocalControllerPosition(ovrController);
            Vector3 worldPosition = trackingRoot.TransformPoint(localPosition);
            // Debug.Log("Controller " + Handedness + " - local: " + localPosition + " - world: " + worldPosition);

            Quaternion localRotation = OVRInput.GetLocalControllerRotation(ovrController);
            Quaternion worldRotation = trackingRoot.rotation * localRotation;

            // Update velocity
            Vector3 localVelocity = OVRInput.GetLocalControllerVelocity(ovrController);
            Velocity = trackingRoot.TransformDirection(localVelocity);

            Vector3 localAngularVelocity = OVRInput.GetLocalControllerAngularVelocity(ovrController);
            AngularVelocity = trackingRoot.TransformDirection(localAngularVelocity);

            UpdateJointPoses();

            // If not rendering avatar hands, pointer pose is not available, so we approximate it
            if (IsPositionAvailable)
            {
                currentPointerPose.Position = currentGripPose.Position = worldPosition;
            }

            if (IsRotationAvailable)
            {
                currentPointerPose.Rotation = currentGripPose.Rotation = worldRotation;
            }

            // Todo: Complete touch controller mapping
            bool isTriggerPressed;
            float triggerValue;

            bool isGripPressed;
            float gripValue;

            Vector2 stickInput;

            if (ControllerHandedness == Handedness.Left)
            {
                triggerValue = OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger);
                isTriggerPressed = triggerValue > cTriggerDeadZone;

                gripValue = OVRInput.Get(OVRInput.RawAxis1D.LHandTrigger);
                isGripPressed = gripValue > cTriggerDeadZone;

                stickInput = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick);
            }
            else
            {
                triggerValue = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);
                isTriggerPressed = triggerValue > cTriggerDeadZone;

                gripValue = OVRInput.Get(OVRInput.RawAxis1D.RHandTrigger);
                isGripPressed = gripValue > cTriggerDeadZone;

                stickInput = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);
            }

            UpdateHandPinchStrength(Mathf.Max(triggerValue, gripValue));

            bool isSelecting = isTriggerPressed || isGripPressed;

            UpdateTeleport(stickInput);

            for (int i = 0; i < Interactions?.Length; i++)
            {
                switch (Interactions[i].InputType)
                {
                    case DeviceInputType.SpatialPointer:
                        Interactions[i].PoseData = currentPointerPose;
                        if (Interactions[i].Changed)
                        {
                            CoreServices.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, currentPointerPose);
                        }
                        break;
                    case DeviceInputType.SpatialGrip:
                        Interactions[i].PoseData = currentGripPose;
                        if (Interactions[i].Changed)
                        {
                            CoreServices.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, currentGripPose);
                        }
                        break;
                    case DeviceInputType.Select:
                        Interactions[i].BoolData = isSelecting;

                        if (Interactions[i].Changed)
                        {
                            if (Interactions[i].BoolData)
                            {
                                CoreServices.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
                            }
                            else
                            {
                                CoreServices.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
                            }
                        }
                        break;
                    case DeviceInputType.TriggerPress:
                        Interactions[i].BoolData = isSelecting;

                        if (Interactions[i].Changed)
                        {
                            if (Interactions[i].BoolData)
                            {
                                CoreServices.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
                            }
                            else
                            {
                                CoreServices.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
                            }
                        }
                        break;
                    case DeviceInputType.IndexFinger:
                        UpdateIndexFingerData(Interactions[i]);
                        break;
                }
            }
        }

        private void UpdateTeleport(Vector2 stickInput)
        {
            if (MRTKOculusConfig.Instance.ActiveTeleportPointerMode == MRTKOculusConfig.TeleportPointerMode.None) return;

            MixedRealityInputAction teleportAction = MixedRealityInputAction.None;
            IMixedRealityTeleportPointer teleportPointer = TeleportPointer;

            // Check if we're focus locked or near something interactive to avoid teleporting unintentionally.
            bool anyPointersLockedWithHand = false;
            for (int i = 0; i < InputSource?.Pointers?.Length; i++)
            {
                if (InputSource.Pointers[i] == null) continue;
                if (InputSource.Pointers[i] is IMixedRealityNearPointer)
                {
                    var nearPointer = (IMixedRealityNearPointer)InputSource.Pointers[i];
                    anyPointersLockedWithHand |= nearPointer.IsNearObject;
                }
                anyPointersLockedWithHand |= InputSource.Pointers[i].IsFocusLocked;

                // If official teleport mode and we have a teleport pointer registered, we get the input action to trigger it.
                if (MRTKOculusConfig.Instance.ActiveTeleportPointerMode == MRTKOculusConfig.TeleportPointerMode.Official
                    && InputSource.Pointers[i] is IMixedRealityTeleportPointer)
                {
                    teleportPointer = (TeleportPointer)InputSource.Pointers[i];
                    teleportAction = ((TeleportPointer)teleportPointer).TeleportInputAction;
                }
            }

            bool isReadyForTeleport = !anyPointersLockedWithHand && stickInput != Vector2.zero;

            // If not ready for teleport, we raise a cancellation event to prevent accidental teleportation.
            if (!isReadyForTeleport && teleportPointer != null)
            {
                CoreServices.TeleportSystem?.RaiseTeleportCanceled(teleportPointer, null);
            }

            isInPointingPose = !isReadyForTeleport;

            RaiseTeleportInput(isInPointingPose ? Vector2.zero : stickInput, teleportAction, isReadyForTeleport);
        }

        private void RaiseTeleportInput(Vector2 teleportInput, MixedRealityInputAction teleportAction, bool isReadyForTeleport)
        {
            switch (MRTKOculusConfig.Instance.ActiveTeleportPointerMode)
            {
                case MRTKOculusConfig.TeleportPointerMode.Custom:
                    if (TeleportPointer == null) return;
                    TeleportPointer.gameObject.SetActive(IsPositionAvailable);
                    TeleportPointer.transform.position = currentPointerPose.Position;
                    TeleportPointer.transform.rotation = currentPointerPose.Rotation;
                    TeleportPointer.UpdatePointer(isReadyForTeleport, teleportInput);
                    break;
                case MRTKOculusConfig.TeleportPointerMode.Official:
                    if (teleportAction.Equals(MixedRealityInputAction.None)) return;
                    CoreServices.InputSystem?.RaisePositionInputChanged(InputSource, ControllerHandedness, teleportAction, teleportInput);
                    break;
                default:
                    return;
            }
        }

        /// <summary>
        /// Updates material instance used for avatar hands.
        /// </summary>
        /// <param name="newMaterial">Material to use for hands.</param>
        public void UpdateAvatarMaterial(Material newMaterial)
        {
            if (newMaterial == null || !MRTKOculusConfig.Instance.UseCustomHandMaterial) return;
            if (handMaterial != null)
            {
                Object.Destroy(handMaterial);
            }
            handMaterial = new Material(newMaterial);

            ApplyHandMaterial();
        }
#endif
        /// <summary>
        /// Updates hand material set on hand renderers with member variable stored on controller.
        /// </summary>
        public void ApplyHandMaterial()
        {
            foreach (var handRenderer in handRenderers)
            {
                handRenderer.sharedMaterial = handMaterial;
            }
        }

        private void UpdateHandRenderers(GameObject handRoot)
        {
            if (handRoot == null) return;
            handRenderers = new List<Renderer>(handRoot.GetComponentsInChildren<Renderer>());
            ApplyHandMaterial();
        }

        private void UpdateHandPinchStrength(float pinchStrength)
        {
            if (!MRTKOculusConfig.Instance.UpdateMaterialPinchStrengthValue || handMaterial == null) return;
            foreach (var handRenderer in handRenderers)
            {
                handRenderer.sharedMaterial.SetFloat(pinchStrengthProp, pinchStrength);
            }
        }

        private bool InitializeAvatarHandReferences()
        {
            if (!MRTKOculusConfig.Instance.RenderAvatarHandsInsteadOfController) return false;

            // If we already have a valid hand root, proceed
            if (handRoot != null) return true;

            string handSignififer = ControllerHandedness == Handedness.Left ? "l" : "r";
            string handStructure = "hands:b_" + handSignififer;

            string handRootString = handStructure + "_hand";

            handRoot = GameObject.Find(handRootString);

            // With no root, no use in looking up other joints
            if (handRoot == null) return false;

            string handMeshRoot = "hand_" + (ControllerHandedness == Handedness.Left ? "left" : "right");
            UpdateHandRenderers(GameObject.Find(handMeshRoot));

            // If we have a hand root match, we look up all other hand joints

            // Wrist
            string gripString = handStructure + "_grip";
            handGrip = GameObject.Find(gripString);

            // Index
            string indexString = handStructure + "_index";

            string indexString1 = indexString + "1";
            handIndex1 = GameObject.Find(indexString1);

            string indexString2 = indexString + "2";
            handIndex2 = GameObject.Find(indexString2);

            string indexString3 = indexString + "3";
            handIndex3 = GameObject.Find(indexString3);

            string indexStringTip = indexString + "_ignore";
            handIndexTip = GameObject.Find(indexStringTip);

            // Middle
            string middleString = handStructure + "_middle";

            string middleString1 = middleString + "1";
            handMiddle1 = GameObject.Find(middleString1);

            string middleString2 = middleString + "2";
            handMiddle2 = GameObject.Find(middleString2);

            string middleString3 = middleString + "3";
            handMiddle3 = GameObject.Find(middleString3);

            string middleStringTip = middleString + "_ignore";
            handMiddleTip = GameObject.Find(middleStringTip);

            // Pinky
            string pinkyString = handStructure + "_pinky";

            string pinkyString0 = pinkyString + "0";
            handPinky0 = GameObject.Find(pinkyString0);

            string pinkyString1 = pinkyString + "1";
            handPinky1 = GameObject.Find(pinkyString1);

            string pinkyString2 = pinkyString + "2";
            handPinky2 = GameObject.Find(pinkyString2);

            string pinkyString3 = pinkyString + "3";
            handPinky3 = GameObject.Find(pinkyString3);

            string pinkyStringTip = pinkyString + "_ignore";
            handPinkyTip = GameObject.Find(pinkyStringTip);

            // Ring
            string ringString = handStructure + "_ring";

            string ringString1 = ringString + "1";
            handRing1 = GameObject.Find(ringString1);

            string ringString2 = ringString + "2";
            handRing2 = GameObject.Find(ringString2);

            string ringString3 = ringString + "3";
            handRing3 = GameObject.Find(ringString3);

            string ringStringTip = ringString + "_ignore";
            handRingTip = GameObject.Find(ringStringTip);

            // Thumb
            string thumbString = handStructure + "_thumb";

            string thumbString1 = thumbString + "1";
            handThumb1 = GameObject.Find(thumbString1);

            string thumbString2 = thumbString + "2";
            handThumb2 = GameObject.Find(thumbString2);

            string thumbString3 = thumbString + "3";
            handThumb3 = GameObject.Find(thumbString3);

            string thumStringbTip = thumbString + "_ignore";
            handThumbTip = GameObject.Find(thumStringbTip);

            return true;
        }

        private void UpdateJointPoses()
        {
            if (InitializeAvatarHandReferences())
            {
                UpdateAvatarJointPoses();
            }
            else
            {
                UpdateFakeHandJointPoses();
            }
        }

        private void UpdateAvatarJointPoses()
        {
            // Reference
            /*
            { BoneId.Hand_Thumb1, TrackedHandJoint.ThumbMetacarpalJoint },
            { BoneId.Hand_Thumb2, TrackedHandJoint.ThumbProximalJoint },
            { BoneId.Hand_Thumb3, TrackedHandJoint.ThumbDistalJoint },
            { BoneId.Hand_ThumbTip, TrackedHandJoint.ThumbTip },
            { BoneId.Hand_Index1, TrackedHandJoint.IndexKnuckle },
            { BoneId.Hand_Index2, TrackedHandJoint.IndexMiddleJoint },
            { BoneId.Hand_Index3, TrackedHandJoint.IndexDistalJoint },
            { BoneId.Hand_IndexTip, TrackedHandJoint.IndexTip },
            { BoneId.Hand_Middle1, TrackedHandJoint.MiddleKnuckle },
            { BoneId.Hand_Middle2, TrackedHandJoint.MiddleMiddleJoint },
            { BoneId.Hand_Middle3, TrackedHandJoint.MiddleDistalJoint },
            { BoneId.Hand_MiddleTip, TrackedHandJoint.MiddleTip },
            { BoneId.Hand_Ring1, TrackedHandJoint.RingKnuckle },
            { BoneId.Hand_Ring2, TrackedHandJoint.RingMiddleJoint },
            { BoneId.Hand_Ring3, TrackedHandJoint.RingDistalJoint },
            { BoneId.Hand_RingTip, TrackedHandJoint.RingTip },
            { BoneId.Hand_Pinky1, TrackedHandJoint.PinkyKnuckle },
            { BoneId.Hand_Pinky2, TrackedHandJoint.PinkyMiddleJoint },
            { BoneId.Hand_Pinky3, TrackedHandJoint.PinkyDistalJoint },
            { BoneId.Hand_PinkyTip, TrackedHandJoint.PinkyTip },
            { BoneId.Hand_WristRoot, TrackedHandJoint.Wrist },
            */

            // Thumb
            UpdateAvatarJointPose(TrackedHandJoint.ThumbMetacarpalJoint, handThumb1.transform.position, handThumb1.transform.rotation);
            UpdateAvatarJointPose(TrackedHandJoint.ThumbProximalJoint, handThumb2.transform.position, handThumb2.transform.rotation);
            UpdateAvatarJointPose(TrackedHandJoint.ThumbDistalJoint, handThumb3.transform.position, handThumb3.transform.rotation);
            UpdateAvatarJointPose(TrackedHandJoint.ThumbTip, handThumbTip.transform.position, handThumbTip.transform.rotation);

            // Index
            UpdateAvatarJointPose(TrackedHandJoint.IndexKnuckle, handIndex1.transform.position, handIndex1.transform.rotation);
            UpdateAvatarJointPose(TrackedHandJoint.IndexMiddleJoint, handIndex2.transform.position, handIndex2.transform.rotation);
            UpdateAvatarJointPose(TrackedHandJoint.IndexDistalJoint, handIndex3.transform.position, handIndex3.transform.rotation);
            UpdateAvatarJointPose(TrackedHandJoint.IndexTip, handIndexTip.transform.position, handIndexTip.transform.rotation);

            // Middle
            UpdateAvatarJointPose(TrackedHandJoint.MiddleKnuckle, handMiddle1.transform.position, handMiddle1.transform.rotation);
            UpdateAvatarJointPose(TrackedHandJoint.MiddleMiddleJoint, handMiddle2.transform.position, handMiddle2.transform.rotation);
            UpdateAvatarJointPose(TrackedHandJoint.MiddleDistalJoint, handMiddle3.transform.position, handMiddle3.transform.rotation);
            UpdateAvatarJointPose(TrackedHandJoint.MiddleTip, handMiddleTip.transform.position, handMiddleTip.transform.rotation);

            // Ring
            UpdateAvatarJointPose(TrackedHandJoint.RingKnuckle, handRing1.transform.position, handRing1.transform.rotation);
            UpdateAvatarJointPose(TrackedHandJoint.RingMiddleJoint, handRing2.transform.position, handRing2.transform.rotation);
            UpdateAvatarJointPose(TrackedHandJoint.RingDistalJoint, handRing3.transform.position, handRing3.transform.rotation);
            UpdateAvatarJointPose(TrackedHandJoint.RingTip, handRingTip.transform.position, handRingTip.transform.rotation);

            // Pinky
            UpdateAvatarJointPose(TrackedHandJoint.PinkyKnuckle, handPinky1.transform.position, handPinky1.transform.rotation);
            UpdateAvatarJointPose(TrackedHandJoint.PinkyMiddleJoint, handPinky2.transform.position, handPinky2.transform.rotation);
            UpdateAvatarJointPose(TrackedHandJoint.PinkyDistalJoint, handPinky3.transform.position, handPinky3.transform.rotation);
            UpdateAvatarJointPose(TrackedHandJoint.PinkyTip, handPinkyTip.transform.position, handPinkyTip.transform.rotation);

            // Wrist
            // Wrist rotation works very differently from the other joints, so we correct it separately
            UpdateJointPose(TrackedHandJoint.Wrist, handPinky0.transform.position, handGrip.transform.rotation);

            UpdatePalm();
        }

        protected void UpdatePalm()
        {
            bool hasMiddleKnuckle = TryGetJoint(TrackedHandJoint.MiddleKnuckle, out var middleKnucklePose);
            bool hasWrist = TryGetJoint(TrackedHandJoint.Wrist, out var wristPose);

            if (hasMiddleKnuckle && hasWrist)
            {
                Vector3 wristRootPosition = wristPose.Position;
                Vector3 middle3Position = middleKnucklePose.Position;

                Vector3 palmPosition = Vector3.Lerp(wristRootPosition, middle3Position, 0.5f);
                Quaternion palmRotation = CorrectPalmRotation(wristPose.Rotation);

                UpdateJointPose(TrackedHandJoint.Palm, palmPosition, palmRotation);
            }
        }

        private Quaternion CorrectPalmRotation(Quaternion palmRotation)
        {
            Quaternion correctedPalmRotation = palmRotation;

            // WARNING THIS CODE IS SUBJECT TO CHANGE WITH THE OCULUS SDK - This fix is a hack to fix broken and inconsistent rotations for hands
            if (ControllerHandedness == Handedness.Right)
            {
                correctedPalmRotation *= Quaternion.Euler(90f, 90f, 0f);
            }
            else
            {
                correctedPalmRotation *= Quaternion.Euler(90f, 0, 90f);
            }
            return correctedPalmRotation;
        }

        private void UpdateFakeHandJointPoses()
        {
            // While we can get pretty much everything done with just the grip pose, we simulate hand sizes for bounds calculations

            // Index
            Vector3 fingerTipPos = currentGripPose.Position + currentGripPose.Rotation * Vector3.forward * 0.1f;
            UpdateJointPose(TrackedHandJoint.IndexTip, fingerTipPos, currentGripPose.Rotation);

            // Handed directional offsets
            Vector3 inWardVector;
            if (ControllerHandedness == Handedness.Left)
            {
                inWardVector = currentGripPose.Rotation * Vector3.right;
            }
            else
            {
                inWardVector = currentGripPose.Rotation * -Vector3.right;
            }

            // Thumb
            Vector3 thumbPose = currentGripPose.Position + inWardVector * 0.04f;
            UpdateJointPose(TrackedHandJoint.ThumbTip, thumbPose, currentGripPose.Rotation);
            UpdateJointPose(TrackedHandJoint.ThumbMetacarpalJoint, thumbPose, currentGripPose.Rotation);
            UpdateJointPose(TrackedHandJoint.ThumbDistalJoint, thumbPose, currentGripPose.Rotation);

            // Pinky
            Vector3 pinkyPose = currentGripPose.Position - inWardVector * 0.03f;
            UpdateJointPose(TrackedHandJoint.PinkyKnuckle, pinkyPose, currentGripPose.Rotation);

            // Palm
            UpdateJointPose(TrackedHandJoint.Palm, currentGripPose.Position, currentGripPose.Rotation);

            // Wrist
            Vector3 wristPose = currentGripPose.Position - currentGripPose.Rotation * Vector3.forward * 0.05f;
            UpdateJointPose(TrackedHandJoint.Palm, wristPose, currentGripPose.Rotation);
        }

        protected void UpdateAvatarJointPose(TrackedHandJoint joint, Vector3 position, Quaternion rotation)
        {
            Quaternion correctedRotation = rotation;

            // WARNING THIS CODE IS SUBJECT TO CHANGE WITH THE OCULUS SDK - This fix is a hack to fix broken and inconsistent rotations for hands
            if (ControllerHandedness == Handedness.Left)
            {
                // Rotate palm 180 on X to flip up
                // Rotate palm 90 degrees on y to align x with right
                correctedRotation *= Quaternion.Euler(180f, 90f, 0f);
            }
            else
            {
                // Rotate palm 90 degrees on y to align x with right
                correctedRotation *= Quaternion.Euler(0f, 90f, 0f);
            }

            UpdateJointPose(joint, position, correctedRotation);
        }

        protected void UpdateJointPose(TrackedHandJoint joint, Vector3 position, Quaternion rotation)
        {
            MixedRealityPose pose = new MixedRealityPose(position, rotation);
            if (!jointPoses.ContainsKey(joint))
            {
                jointPoses.Add(joint, pose);
            }
            else
            {
                jointPoses[joint] = pose;
            }

            CoreServices.InputSystem?.RaiseHandJointsUpdated(InputSource, ControllerHandedness, jointPoses);
        }

        private void UpdateIndexFingerData(MixedRealityInteractionMapping interactionMapping)
        {
            if (jointPoses.TryGetValue(TrackedHandJoint.IndexTip, out var pose))
            {
                currentIndexPose.Rotation = pose.Rotation;
                currentIndexPose.Position = pose.Position;
            }

            interactionMapping.PoseData = currentIndexPose;

            // If our value changed raise it.
            if (interactionMapping.Changed)
            {
                // Raise input system Event if it enabled
                CoreServices.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, currentIndexPose);
            }
        }

        public bool TryGetJoint(TrackedHandJoint joint, out MixedRealityPose pose)
        {
            if (jointPoses.TryGetValue(joint, out pose))
            {
                return true;
            }
            pose = currentGripPose;
            return true;
        }
    }
}
