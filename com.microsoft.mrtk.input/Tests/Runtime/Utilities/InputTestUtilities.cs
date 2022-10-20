// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input.Simulation;
using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using HandshapeId = Microsoft.MixedReality.Toolkit.Input.HandshapeTypes.HandshapeId;

namespace Microsoft.MixedReality.Toolkit.Input.Tests
{
    public class InputTestUtilities
    {
        private const string MRTKRigPrefabGuid = "4d7e2f87fefe0ba468719b15288b46e7";
        private static readonly string MRTKRigPrefabPath = AssetDatabase.GUIDToAssetPath(MRTKRigPrefabGuid);

        private static GameObject rigReference;

        /// <summary>
        /// The default number of frames that elapse for each test controller movement.
        /// Intentionally a smaller number to keep tests fast.
        /// </summary>
        private const int ControllerMoveStepsDefault = 5;

        /// <summary>
        /// The default number of frames that elapse when in slow test controller mode.
        /// See UseSlowTestController for more information.
        /// </summary>
        private const int ControllerMoveStepsSlow = 60;

        /// <summary>
        /// If true, the controller movement test steps will take a longer number of frames. This is especially
        /// useful for seeing motion in play mode tests (where the default smaller number of frames tends
        /// to make tests too fast to be understandable to the human eye). This is false by default
        /// to ensure that tests will run quickly in general, and can be set to true manually in specific
        /// test cases using the example below.
        /// </summary>
        /// <example> 
        /// <code>
        /// [UnityTest]
        /// public IEnumerator YourTestCase()
        /// {
        ///     RuntimeInputTestUtils.UseSlowTestController = true;
        ///     ...
        ///     RuntimeInputTestUtils.UseSlowTestController = false;
        /// }
        /// </code>
        /// </example>
        /// <remarks>
        /// <para>Note that this value is reset to false after each play mode test that uses
        /// PlayModeTestUtilities.Setup() - this is to reduce the chance that a forgotten
        /// UseSlowTestController = true ends up slowing all subsequent tests.</para>
        /// </remarks>
        public static bool UseSlowTestController { get; set; } = false;

        /// <summary>
        /// The number of frames that elapse for each test controller movement, taking into account if
        /// slow test controller mode has been engaged.
        /// </summary>
        public static int ControllerMoveSteps => UseSlowTestController ? ControllerMoveStepsSlow : ControllerMoveStepsDefault;

        /// <summary>
        /// A sentinel value used by controller test utilities to indicate that the default number of move
        /// steps should be used or not.
        /// </summary>
        /// <remarks>
        /// <para>This is primarily something that exists to get around the limitation of default parameter
        /// values requiring compile-time constants.</para>
        /// </remarks>
        public const int ControllerMoveStepsSentinelValue = -1;

        #region Simulated Devices

        private static SimulatedController leftController;
        private static ControllerSimulationSettings leftControllerSettings;
        private static ControllerControls leftControls;

        private static SimulatedController rightController;
        private static ControllerSimulationSettings rightControllerSettings;
        private static ControllerControls rightControls;

        private static SimulatedHMD hmd;

        #endregion Simulated Devices

        /// <summary>
        /// Creates and returns the MRTK rig.
        /// </summary>
        public static GameObject InstantiateRig()
        {
            Object rigPrefab = AssetDatabase.LoadAssetAtPath(MRTKRigPrefabPath, typeof(Object));
            rigReference = Object.Instantiate(rigPrefab) as GameObject;
            return rigReference;
        }

        /// <summary>
        /// Forces the playspace camera to origin facing forward along +Z.
        /// </summary>
        public static void InitializeCameraToOriginAndForward()
        {
            // Move the camera to origin looking at +z to more easily see the target at 0,0,+Z
            hmd.ResetToOrigin();
        }

        /// <summary>
        /// Forces the playspace camera to origin facing forward along +Z.
        /// </summary>
        public static void RotateCamera(Vector3 rotationDelta)
        {
            // Move the camera to origin looking at +z to more easily see the target at 0,0,+Z
            hmd.Update(Vector3.zero, rotationDelta);
        }

        /// <summary>
        /// Returns a position placed in front of the user's head, offset forward by the given distance.
        /// </summary>
        public static Vector3 InFrontOfUser(float distanceFromHead = 0.4f)
        {
            return Camera.main.transform.position + Camera.main.transform.forward * distanceFromHead;
        }

        /// <summary>
        /// Returns a position placed in front of the user's head, offset by the given vector, which
        /// is transformed by the camera's reference frame.
        /// </summary>
        public static Vector3 InFrontOfUser(Vector3 offset)
        {
            return Camera.main.transform.position + Camera.main.transform.TransformVector(offset);
        }

        /// <summary>
        /// Destroys the MRTK rig object.
        /// </summary>
        public static void TeardownRig()
        {
            if (Application.isPlaying)
            {
                UnityEngine.Object.Destroy(rigReference);
            }
        }

        /// <summary>
        /// Creates the simulated devices (two <see cref="SimulatedController"/>s and a <see cref="SimulatedHMD"/>)
        /// as well as the associated <see cref="ControllerControls"/> for each.
        /// </summary>
        /// <param name="rayHalfLife">
        /// Optional value for ray smoothing halflife, handy for suppressing smoothing during automated tests.
        /// </param>
        public static void SetupSimulation(float rayHalfLife = 0.01f)
        {
            // See comments for UseSlowTestController for why this is reset to false on each test case.
            UseSlowTestController = false;

            leftControllerSettings = new ControllerSimulationSettings();
            rightControllerSettings = new ControllerSimulationSettings();

            leftController = new SimulatedController(
                Handedness.Left, leftControllerSettings, Vector3.zero, rayHalfLife);
            rightController = new SimulatedController(
                Handedness.Right, rightControllerSettings, Vector3.zero, rayHalfLife);

            leftControls = new ControllerControls();
            rightControls = new ControllerControls();

            // Most tests use the ArticulatedHand simulation mode
            SetHandSimulationMode(Handedness.Left, ControllerSimulationMode.ArticulatedHand);
            SetHandSimulationMode(Handedness.Right, ControllerSimulationMode.ArticulatedHand);

            // Most tests rely on the anchor point being centered around the index finger
            SetHandAnchorPoint(Handedness.Left, ControllerAnchorPoint.IndexFinger);
            SetHandAnchorPoint(Handedness.Right, ControllerAnchorPoint.IndexFinger);

            hmd = new SimulatedHMD();
        }

        /// <summary>
        /// Disposes of simulated input devices.
        /// </summary>
        public static void TeardownSimulation()
        {
            leftController.Dispose();
            rightController.Dispose();
            hmd.Dispose();
        }

        /// <summary>
        /// Sets the tracking state of the simulated device as specified by the Handedness parameter.
        /// </summary>
        public static IEnumerator SetHandTrackingState(Handedness handedness, bool isTracked)
        {
            SimulatedController controller = handedness == Handedness.Right ? rightController : leftController;
            ControllerControls controls = handedness == Handedness.Right ? rightControls : leftControls;
            controls.IsTracked = isTracked;
            controls.TrackingState = controls.IsTracked ?
                (UnityEngine.XR.InputTrackingState.Position | UnityEngine.XR.InputTrackingState.Rotation) : UnityEngine.XR.InputTrackingState.None;
            controller.UpdateControls(controls);
            yield return null;
        }

        /// <summary>
        /// Moves the hand from startPos to endPos, rotates the hand from startRot to endRot, and lerps the pinchedness
        /// based on the provided handshapeId.
        /// </summary>
        /// <remarks>
        /// <para>Note that numSteps defaults to a value of -1, which is a sentinel value to indicate that the
        /// default number of steps should be used (i.e. ControllerMoveSteps). ControllerMoveSteps is not a compile
        /// time constant, which is a requirement for default parameter values.</para>
        /// </remarks>
        public static IEnumerator UpdateHand(
            Vector3 startPos, Vector3 endPos,
            Quaternion startRot, Quaternion endRot,
            HandshapeId handshapeId, Handedness handedness,
            int numSteps = ControllerMoveStepsSentinelValue)
        {
            Debug.Assert(handedness == Handedness.Right || handedness == Handedness.Left, "handedness must be either right or left");
            bool isPinching = handshapeId == HandshapeId.Grab || handshapeId == HandshapeId.Pinch || handshapeId == HandshapeId.PinchSteadyWrist;

            numSteps = CalculateNumSteps(numSteps);

            SimulatedController controller = handedness == Handedness.Right ? rightController : leftController;
            ControllerControls controls = handedness == Handedness.Right ? rightControls : leftControls;
            ControllerAnchorPoint anchorPoint = handedness == Handedness.Right ? rightControllerSettings.AnchorPoint : leftControllerSettings.AnchorPoint;

            float startPinch = controls.TriggerAxis;

            for (int i = 1; i <= numSteps; i++)
            {
                float t = i / (float)numSteps;

                Pose handPose = new Pose(
                    Vector3.Lerp(startPos, endPos, t),
                    Quaternion.Lerp(startRot, endRot, t)
                );
                float pinchAmount = Mathf.Lerp(startPinch, isPinching ? 1 : 0, t);

                controls.TriggerAxis = pinchAmount;
                switch (anchorPoint)
                {
                    // We always pass in useRayVector = false during unit tests, because we always want the pointerPosition
                    // to match the devicePosition so that we can aim the "hand" wherever we'd like. Otherwise, we'd
                    // be using the generated hand-joint-based ray vector which is unreliable to aim from automated tests.
                    case ControllerAnchorPoint.Device:
                        controller.UpdateAbsolute(handPose, controls, ControllerRotationMode.UserControl, false);
                        break;
                    case ControllerAnchorPoint.IndexFinger:
                        controller.UpdateAbsoluteWithPokeAnchor(handPose, controls, ControllerRotationMode.UserControl, false);
                        break;
                    case ControllerAnchorPoint.Grab:
                        controller.UpdateAbsoluteWithGrabAnchor(handPose, controls, ControllerRotationMode.UserControl, false);
                        break;
                }

                yield return null;
            }
        }

        /// <summary>
        /// Moves the hand from startPos to endPos and lerps the pinchedness based on the provided handshapeId.
        /// </summary>
        /// <remarks>
        /// <para>Note that numSteps defaults to a value of -1, which is a sentinel value to indicate that the
        /// default number of steps should be used (i.e. ControllerMoveSteps). ControllerMoveSteps is not a compile
        /// time constant, which is a requirement for default parameter values.</para>
        /// </remarks>
        public static IEnumerator MoveHand(
            Vector3 startPos, Vector3 endPos,
            HandshapeId handshapeId, Handedness handedness,
            int numSteps = ControllerMoveStepsSentinelValue)
        {
            SimulatedController controller = handedness == Handedness.Right ? rightController : leftController;
            Quaternion rot = controller.WorldRotation;
            return UpdateHand(startPos, endPos, rot, rot, handshapeId, handedness, numSteps);
        }

        /// <summary>
        /// Moves the hand to newPos from its current position and lerps the pinchedness based on the provided handshapeId.
        /// </summary>
        /// <remarks>
        /// <para>Note that numSteps defaults to a value of -1, which is a sentinel value to indicate that the
        /// default number of steps should be used (i.e. ControllerMoveSteps). ControllerMoveSteps is not a compile
        /// time constant, which is a requirement for default parameter values.</para>
        /// </remarks>
        public static IEnumerator MoveHandTo(
            Vector3 newPos, HandshapeId handshapeId, Handedness handedness,
            int numSteps = ControllerMoveStepsSentinelValue)
        {
            Vector3 pos = GetHandPose(handedness).position;

            return MoveHand(pos, newPos, handshapeId, handedness, numSteps);
        }

        /// <summary>
        /// Rotates the hand from startRot to endRot and lerps the pinchedness based on the provided handshapeId.
        /// </summary>
        /// <remarks>
        /// <para>Note that numSteps defaults to a value of -1, which is a sentinel value to indicate that the
        /// default number of steps should be used (i.e. ControllerMoveSteps). ControllerMoveSteps is not a compile
        /// time constant, which is a requirement for default parameter values.</para>
        /// </remarks>
        public static IEnumerator RotateHand(
            Quaternion startRot, Quaternion endRot,
            HandshapeId handshapeId, Handedness handedness,
            int numSteps = ControllerMoveStepsSentinelValue)
        {
            SimulatedController controller = handedness == Handedness.Right ? rightController : leftController;
            Vector3 pos = GetHandPose(handedness).position;
            return UpdateHand(pos, pos, startRot, endRot, handshapeId, handedness, numSteps);
        }

        /// <summary>
        /// Rotates the hand to newRot from its current rotation and lerps the pinchedness based on the
        /// provided handshapeId.
        /// </summary>
        /// <remarks>
        /// <para>Note that numSteps defaults to a value of -1, which is a sentinel value to indicate that the
        /// default number of steps should be used (i.e. ControllerMoveSteps). ControllerMoveSteps is not a compile
        /// time constant, which is a requirement for default parameter values.</para>
        /// </remarks>
        public static IEnumerator RotateHandTo(
            Quaternion newRot, HandshapeId handshapeId, Handedness handedness,
            int numSteps = ControllerMoveStepsSentinelValue)
        {
            SimulatedController controller = handedness == Handedness.Right ? rightController : leftController;
            Quaternion rot = controller.WorldRotation;
            return RotateHand(rot, newRot, handshapeId, handedness, numSteps);
        }

        /// <summary>
        /// The hand so it's pointing towards the specified point in space
        /// </summary>
        public static IEnumerator PointHandToTarget(Vector3 target, HandshapeId handshapeId, Handedness handedness, int numSteps = ControllerMoveStepsSentinelValue)
        {
            SimulatedController controller = handedness == Handedness.Right ? rightController : leftController;

            // find the vector pointing from our position to the target
            Vector3 pos = GetHandPose(handedness).position;
            Vector3 direction = (target - pos).normalized;


            // create the rotation we need to be in to look at the target
            Quaternion lookRotation = Quaternion.LookRotation(direction);


            return RotateHandTo(lookRotation, handshapeId, handedness, numSteps);
        }

        public static IEnumerator SetHandshape(HandshapeId handshapeId, Handedness handedness, int numSteps = ControllerMoveStepsSentinelValue)
        {
            SimulatedController controller = handedness == Handedness.Right ? rightController : leftController;
            Vector3 pos = GetHandPose(handedness).position;
            Quaternion rot = controller.WorldRotation;
            return UpdateHand(pos, pos, rot, rot, handshapeId, handedness, numSteps);
        }

        /// <summary>
        /// Switches the simulation mode for the controller of the given handedness
        /// Required to test variations in behavior.
        /// </summary>
        public static void SetHandSimulationMode(Handedness hand, ControllerSimulationMode mode)
        {
            switch (hand)
            {
                case Handedness.Left:
                    leftControllerSettings.SimulationMode = mode;
                    break;
                case Handedness.Right:
                    rightControllerSettings.SimulationMode = mode;
                    break;
                case Handedness.None:
                    Debug.LogError("Handedness not supported");
                    break;
            }
        }


        /// <summary>
        /// Switches the anchor point mode for the controller of the given handedness
        /// Required to test variations in behavior.
        /// </summary>
        public static void SetHandAnchorPoint(Handedness hand, ControllerAnchorPoint anchorPoint)
        {
            switch (hand)
            {
                case Handedness.Left:
                    leftControllerSettings.AnchorPoint = anchorPoint;
                    break;
                case Handedness.Right:
                    rightControllerSettings.AnchorPoint = anchorPoint;
                    break;
                case Handedness.None:
                    Debug.LogError("Handedness not supported");
                    break;
            }
        }


        /// <summary>
        /// Returns the pose of the hmd
        /// </summary>
        public static Pose GetCameraPose(Handedness handedness)
        {
            return new Pose(hmd.Position, hmd.Rotation);
        }


        /// <summary>
        /// Returns the pose of the hand, rooted on the poke position.
        /// </summary>
        public static Pose GetHandPose(Handedness handedness)
        {
            SimulatedController controller = handedness == Handedness.Right ? rightController : leftController;
            ControllerAnchorPoint anchorPoint = handedness == Handedness.Right ? rightControllerSettings.AnchorPoint : leftControllerSettings.AnchorPoint;

            Vector3 anchorPosition = controller.WorldPosition;
            switch (anchorPoint)
            {
                case ControllerAnchorPoint.Device:
                    anchorPosition = controller.WorldPosition;
                    break;
                case ControllerAnchorPoint.IndexFinger:
                    anchorPosition = controller.PokePosition;
                    break;
                case ControllerAnchorPoint.Grab:
                    anchorPosition = controller.GrabPosition;
                    break;
            }

            return new Pose(anchorPosition, controller.WorldRotation);
        }

        /// <summary>
        /// Given a numSteps value, determines if the value is a 'sentinel' value of
        /// ControllerMoveStepsSentinelValue, which should be converted to the current
        /// default value of ControllerMoveSteps. If it's not the sentinel value,
        /// this returns numSteps unchanged.
        /// </summary>
        public static int CalculateNumSteps(int numSteps)
        {
            return numSteps == ControllerMoveStepsSentinelValue ? ControllerMoveSteps : numSteps;
        }

        /// <summary>
        /// Disables gaze interactions.
        /// </summary>
        /// <remarks>This is currently done by disabling the Fuzzy Gaze Interactor GameObject. Ideally, we'd want to do this via a more system level approach</remarks>
        public static void DisableGaze()
        {
            GameObject.FindObjectOfType<GazeInteractor>().gameObject.SetActive(false);
        }

        /// <summary>
        /// Enables gaze interactions.
        /// </summary>
        /// <remarks>This is currently done by enabling the Fuzzy Gaze Interactor GameObject.  Ideally, we'd want to do this via a more system level approach</remarks>
        public static void EnableGaze()
        {
            GameObject.FindObjectOfType<GazeInteractor>().gameObject.SetActive(true);
        }
    }
}
