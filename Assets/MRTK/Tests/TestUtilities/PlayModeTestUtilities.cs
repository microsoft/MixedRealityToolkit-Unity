// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// The assembly that this file is part of is still built for the player,
// even though the assembly itself is marked as a test assembly (this is not
// expected because test assemblies should not be included in player builds).
// Because the .NET backend is deprecated in 2018 and removed in 2019 and this
// issue will likely persist for 2018, this issue is worked around by wrapping all
// play mode tests in this check.

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using System.Collections;
using System.IO;
using Microsoft.MixedReality.Toolkit.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class PlayModeTestUtilities
    {
        // Unity's default scene name for a recently created scene
        const string playModeTestSceneName = "MixedRealityToolkit.PlayModeTestScene";

        private static Stack<MixedRealityInputSimulationProfile> inputSimulationProfiles = new Stack<MixedRealityInputSimulationProfile>();

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
        ///     PlayModeTestUtilities.UseSlowTestController = true;
        ///     ...
        ///     PlayModeTestUtilities.UseSlowTestController = false;
        /// }
        /// </code>
        /// </example>
        /// <remarks>
        /// <para>Note that this value is reset to false after each play mode test that uses
        /// PlayModeTestUtilities.Setup() - this is to reduce the chance that a forgotten
        /// UseSlowTestController = true ends up slowing all subsequent tests.</para>
        /// </remarks>
        public static bool UseSlowTestController = false;

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
        internal const int ControllerMoveStepsSentinelValue = -1;

        /// <summary>
        /// Creates a play mode test scene, creates an MRTK instance, initializes playspace.
        /// </summary>
        /// <remarks>
        /// Takes an optional MixedRealityToolkitConfigurationProfile used to initialize the MRTK.
        /// </remarks>
        public static void Setup(MixedRealityToolkitConfigurationProfile profile = null)
        {
            Debug.Assert(Application.isPlaying, "This setup method should only be used during play mode tests. Use TestUtilities.");

            // See comments for UseSlowTestController for why this is reset to false on each test case.
            UseSlowTestController = false;

            bool sceneExists = false;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene playModeTestScene = SceneManager.GetSceneAt(i);
                if (playModeTestScene.name == playModeTestSceneName && playModeTestScene.isLoaded)
                {
                    SceneManager.SetActiveScene(playModeTestScene);
                    sceneExists = true;
                }
            }

            if (!sceneExists)
            {
                Scene playModeTestScene = SceneManager.CreateScene(playModeTestSceneName);
                SceneManager.SetActiveScene(playModeTestScene);
            }

            // Create an MRTK instance and set up playspace
            if (profile == null)
            {
                TestUtilities.InitializeMixedRealityToolkit(true);
            }
            else
            {
                TestUtilities.InitializeMixedRealityToolkit(profile);
            }
            TestUtilities.InitializePlayspace();

            // Ensure user input is disabled during the tests
            InputSimulationService inputSimulationService = GetInputSimulationService();
            inputSimulationService.UserInputEnabled = false;
        }

        /// <summary>
        /// Destroys all objects in the play mode test scene, if it has been loaded, and shuts down MRTK instance.
        /// </summary>
        public static void TearDown()
        {
            Scene playModeTestScene = SceneManager.GetSceneByName(playModeTestSceneName);
            if (playModeTestScene.isLoaded)
            {
                foreach (GameObject gameObject in playModeTestScene.GetRootGameObjects())
                {
                    // Delete the MixedRealityToolkit and MixedRealityPlayspace gameobjects are managed by the ShutdownMixedRealityToolkit() function
                    if (gameObject != MixedRealityToolkit.Instance && gameObject.transform != MixedRealityPlayspace.Transform)
                        GameObject.Destroy(gameObject);
                }
            }
            // Delete the MixedRealityToolkit and MixedRealityPlayspace after other objects to allow handlers to get cleaned up
            TestUtilities.ShutdownMixedRealityToolkit();

            // If we created a temporary untitled scene in edit mode to get us started, unload that now
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene editorScene = SceneManager.GetSceneAt(i);
                if (string.IsNullOrEmpty(editorScene.name))
                {   // We've found our editor scene. Unload it.
                    SceneManager.UnloadSceneAsync(editorScene);
                }
            }
        }

        public static SimulatedHandData.HandJointDataGenerator GenerateHandPose(ArticulatedHandPose.GestureId gesture, Handedness handedness, Vector3 worldPosition, Quaternion rotation)
        {
            return (jointsOut) =>
            {
                ArticulatedHandPose gesturePose = SimulatedArticulatedHandPoses.GetGesturePose(gesture);
                Quaternion worldRotation = rotation * CameraCache.Main.transform.rotation;
                gesturePose.ComputeJointPoses(handedness, worldRotation, worldPosition, jointsOut);
            };
        }

        public static SimulatedMotionControllerData.MotionControllerPoseUpdater UpdateMotionControllerPose(Handedness handedness, Vector3 worldPosition, Quaternion rotation)
        {
            return () =>
            {
                Quaternion worldRotation = rotation * CameraCache.Main.transform.rotation;
                Vector3 eulerAngles = worldRotation.eulerAngles;

                // Create an offset to rotation to align with the behavior of simulated hand
                float rotationXOffset = -15f;
                float rotationYOffset = -10f;
                int yOffsetSign = handedness == Handedness.Left ? -1 : 1;
                Quaternion modifiedRotation = Quaternion.Euler(eulerAngles.x + rotationXOffset, eulerAngles.y + rotationYOffset * yOffsetSign, eulerAngles.z);
                return new MixedRealityPose(worldPosition, modifiedRotation);
            };
        }

        public static IMixedRealityInputSystem GetInputSystem()
        {
            Debug.Assert((CoreServices.InputSystem != null), "MixedRealityInputSystem is null!");
            return CoreServices.InputSystem;
        }

        /// <summary>
        /// Utility function to simplify code for getting access to the running InputSimulationService
        /// </summary>
        /// <returns>Returns InputSimulationService registered for playmode test scene</returns>
        public static InputSimulationService GetInputSimulationService()
        {
            InputSimulationService inputSimulationService = CoreServices.GetInputSystemDataProvider<InputSimulationService>();
            Debug.Assert((inputSimulationService != null), "InputSimulationService is null!");
            return inputSimulationService;
        }

        /// <summary>
        /// Make sure there is a MixedRealityInputModule on the main camera, which is needed for using Unity UI with MRTK.
        /// </summary>
        /// <remarks>
        /// Workaround for #5061
        /// </remarks>
        public static void EnsureInputModule()
        {
            if (CameraCache.Main)
            {
                var inputModule = CameraCache.Main.gameObject.GetComponent<MixedRealityInputModule>();
                if (inputModule == null)
                {
                    CameraCache.Main.gameObject.AddComponent<MixedRealityInputModule>();
                }
                inputModule.forceModuleActive = true;
            }
        }

        /// <summary>
        /// Destroy the input module to ensure it gets initialized cleanly for the next test.
        /// </summary>
        /// <remarks>
        /// Workaround for #5116
        /// </remarks>
        public static void TeardownInputModule()
        {
            if (CameraCache.Main)
            {
                var inputModule = CameraCache.Main.gameObject.GetComponent<MixedRealityInputModule>();
                if (inputModule)
                {
                    UnityEngine.Object.DestroyImmediate(inputModule);
                }
            }
        }

        /// <summary>
        /// Initializes the MRTK such that there are no other input system listeners
        /// (global or per-interface).
        /// </summary>
        public static IEnumerator SetupMrtkWithoutGlobalInputHandlers()
        {
            if (!MixedRealityToolkit.IsInitialized)
            {
                Debug.LogError("MixedRealityToolkit must be initialized before it can be configured.");
                yield break;
            }

            Debug.Assert((CoreServices.InputSystem != null), "Input system must be initialized");

            // Let input system to register all cursors and managers.
            yield return null;

            // Switch off / Destroy all input components, which listen to global events
            UnityEngine.Object.Destroy(CoreServices.InputSystem.GazeProvider.GazeCursor as Behaviour);
            CoreServices.InputSystem.GazeProvider.Enabled = false;

            var diagnosticsVoiceControls = UnityEngine.Object.FindObjectsOfType<DiagnosticsSystemVoiceControls>();
            foreach (var diagnosticsComponent in diagnosticsVoiceControls)
            {
                diagnosticsComponent.enabled = false;
            }

            // Let objects be destroyed
            yield return null;

            // Forcibly unregister all other input event listeners.
            BaseEventSystem baseEventSystem = CoreServices.InputSystem as BaseEventSystem;
            MethodInfo unregisterHandler = baseEventSystem.GetType().GetMethod("UnregisterHandler");

            // Since we are iterating over and removing these values, we need to snapshot them
            // before calling UnregisterHandler on each handler.
            var eventHandlersByType = new Dictionary<System.Type, List<BaseEventSystem.EventHandlerEntry>>(((BaseEventSystem)CoreServices.InputSystem).EventHandlersByType);
            foreach (var typeToEventHandlers in eventHandlersByType)
            {
                var handlerEntries = new List<BaseEventSystem.EventHandlerEntry>(typeToEventHandlers.Value);
                foreach (var handlerEntry in handlerEntries)
                {
                    unregisterHandler.MakeGenericMethod(typeToEventHandlers.Key)
                        .Invoke(baseEventSystem,
                                new object[] { handlerEntry.handler });
                }
            }

            // Check that input system is clean
            Debug.Assert(((BaseEventSystem)CoreServices.InputSystem).EventListeners.Count == 0, "Input event system handler registry is not empty in the beginning of the test.");
            Debug.Assert(((BaseEventSystem)CoreServices.InputSystem).EventHandlersByType.Count == 0, "Input event system handler registry is not empty in the beginning of the test.");

            yield return null;
        }

        public static void PushControllerSimulationProfile()
        {
            var iss = GetInputSimulationService();
            inputSimulationProfiles.Push(iss.InputSimulationProfile);
        }

        public static void PopControllerSimulationProfile()
        {
            var iss = GetInputSimulationService();
            iss.InputSimulationProfile = inputSimulationProfiles.Pop();
        }

        public static void SetControllerSimulationMode(ControllerSimulationMode mode)
        {
            var iss = GetInputSimulationService();
            iss.ControllerSimulationMode = mode;
        }

        public static IEnumerator SetHandState(Vector3 handPos, ArticulatedHandPose.GestureId gestureId, Handedness handedness, InputSimulationService inputSimulationService)
        {
            yield return MoveHand(handPos, handPos, ArticulatedHandPose.GestureId.Pinch, handedness, inputSimulationService, 2);
        }

        public static IEnumerator SetMotionControllerState(Vector3 motionControllerPos, SimulatedMotionControllerButtonState buttonState, Handedness handedness, InputSimulationService inputSimulationService)
        {
            yield return MoveMotionController(motionControllerPos, motionControllerPos, buttonState, handedness, inputSimulationService, 2);
        }

        public static T GetPointer<T>(Handedness handedness) where T : class, IMixedRealityPointer
        {
            InputSimulationService simulationService = GetInputSimulationService();
            var controller = simulationService.GetControllerDevice(handedness);
            if (controller != null && controller.InputSource != null)
            {
                foreach (var pointer in controller.InputSource.Pointers)
                {
                    if (pointer is T result)
                    {
                        return result;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Moves the hand from startPos to endPos.
        /// </summary>
        /// <remarks>
        /// <para>Note that numSteps defaults to a value of -1, which is a sentinel value to indicate that the
        /// default number of steps should be used (i.e. ControllerMoveSteps). ControllerMoveSteps is not a compile
        /// time constant, which is a requirement for default parameter values.</para>
        /// </remarks>
        public static IEnumerator MoveHand(
            Vector3 startPos, Vector3 endPos,
            ArticulatedHandPose.GestureId gestureId, Handedness handedness, InputSimulationService inputSimulationService,
            int numSteps = ControllerMoveStepsSentinelValue)
        {
            Debug.Assert(handedness == Handedness.Right || handedness == Handedness.Left, "handedness must be either right or left");
            bool isPinching = gestureId == ArticulatedHandPose.GestureId.Grab || gestureId == ArticulatedHandPose.GestureId.Pinch || gestureId == ArticulatedHandPose.GestureId.PinchSteadyWrist;
            numSteps = CalculateNumSteps(numSteps);

            for (int i = 1; i <= numSteps; i++)
            {
                float t = i / (float)numSteps;
                Vector3 handPos = Vector3.Lerp(startPos, endPos, t);
                var handDataGenerator = GenerateHandPose(
                        gestureId,
                        handedness,
                        handPos,
                        Quaternion.identity);
                SimulatedHandData handData = handedness == Handedness.Right ? inputSimulationService.HandDataRight : inputSimulationService.HandDataLeft;
                handData.Update(true, isPinching, handDataGenerator);
                yield return null;
            }
        }

        /// <summary>
        /// Moves the motion controller from startPos to endPos.
        /// </summary>
        /// <remarks>
        /// <para>Note that numSteps defaults to a value of -1, which is a sentinel value to indicate that the
        /// default number of steps should be used (i.e. ControllerMoveSteps). ControllerMoveSteps is not a compile
        /// time constant, which is a requirement for default parameter values.</para>
        /// </remarks>
        public static IEnumerator MoveMotionController(
            Vector3 startPos, Vector3 endPos, SimulatedMotionControllerButtonState buttonState,
            Handedness handedness, InputSimulationService inputSimulationService,
            int numSteps = ControllerMoveStepsSentinelValue)
        {
            Debug.Assert(handedness == Handedness.Right || handedness == Handedness.Left, "handedness must be either right or left");
            numSteps = CalculateNumSteps(numSteps);

            for (int i = 1; i <= numSteps; i++)
            {
                float t = i / (float)numSteps;
                Vector3 motionControllerPos = Vector3.Lerp(startPos, endPos, t);
                var motionControllerDataUpdater = UpdateMotionControllerPose(
                        handedness,
                        motionControllerPos,
                        Quaternion.identity);
                SimulatedMotionControllerData motionControllerData = handedness == Handedness.Right ? inputSimulationService.MotionControllerDataRight : inputSimulationService.MotionControllerDataLeft;
                motionControllerData.Update(true, buttonState, motionControllerDataUpdater);
                yield return null;
            }
        }

        public static IEnumerator SetHandRotation(Quaternion fromRotation, Quaternion toRotation, Vector3 handPos, ArticulatedHandPose.GestureId gestureId,
            Handedness handedness, int numSteps, InputSimulationService inputSimulationService)
        {
            Debug.Assert(handedness == Handedness.Right || handedness == Handedness.Left, "handedness must be either right or left");
            bool isPinching = gestureId == ArticulatedHandPose.GestureId.Grab || gestureId == ArticulatedHandPose.GestureId.Pinch || gestureId == ArticulatedHandPose.GestureId.PinchSteadyWrist;

            for (int i = 1; i <= numSteps; i++)
            {
                float t = i / (float)numSteps;
                Quaternion handRotation = Quaternion.Lerp(fromRotation, toRotation, t);
                var handDataGenerator = GenerateHandPose(
                        gestureId,
                        handedness,
                        handPos,
                        handRotation);
                SimulatedHandData handData = handedness == Handedness.Right ? inputSimulationService.HandDataRight : inputSimulationService.HandDataLeft;
                handData.Update(true, isPinching, handDataGenerator);
                yield return null;
            }
        }

        public static IEnumerator SetMotionControllerRotation(Quaternion fromRotation, Quaternion toRotation, Vector3 motionControllerPos, SimulatedMotionControllerButtonState buttonState,
            Handedness handedness, int numSteps, InputSimulationService inputSimulationService)
        {
            Debug.Assert(handedness == Handedness.Right || handedness == Handedness.Left, "handedness must be either right or left");

            for (int i = 1; i <= numSteps; i++)
            {
                float t = i / (float)numSteps;
                Quaternion motionControllerRotation = Quaternion.Lerp(fromRotation, toRotation, t);
                var motionControllerDataUpdater = UpdateMotionControllerPose(
                        handedness,
                        motionControllerPos,
                        motionControllerRotation);
                SimulatedMotionControllerData motionControllerData = handedness == Handedness.Right ? inputSimulationService.MotionControllerDataRight : inputSimulationService.MotionControllerDataLeft;
                motionControllerData.Update(true, buttonState, motionControllerDataUpdater);
                yield return null;
            }
        }

        public static IEnumerator HideHand(Handedness handedness, InputSimulationService inputSimulationService)
        {
            yield return null;

            SimulatedHandData handData = handedness == Handedness.Right ? inputSimulationService.HandDataRight : inputSimulationService.HandDataLeft;
            handData.Update(false, false, GenerateHandPose(ArticulatedHandPose.GestureId.Open, handedness, Vector3.zero, Quaternion.identity));

            // Wait one frame for the hand to actually disappear
            yield return null;
        }

        public static IEnumerator HideController(Handedness handedness, InputSimulationService inputSimulationService)
        {
            yield return null;

            SimulatedMotionControllerData motionControllerData = handedness == Handedness.Right ? inputSimulationService.MotionControllerDataRight : inputSimulationService.MotionControllerDataLeft;
            SimulatedMotionControllerButtonState defaultButtonState = new SimulatedMotionControllerButtonState();
            motionControllerData.Update(false, defaultButtonState, UpdateMotionControllerPose(handedness, Vector3.zero, Quaternion.identity));

            // Wait one frame for the motion controller to actually disappear
            yield return null;
        }

        /// <summary>
        /// Shows the hand in the open state, at the origin
        /// </summary>
        public static IEnumerator ShowHand(Handedness handedness, InputSimulationService inputSimulationService)
        {
            yield return ShowHand(handedness, inputSimulationService, ArticulatedHandPose.GestureId.Open, Vector3.zero);
        }

        public static IEnumerator ShowHand(Handedness handedness, InputSimulationService inputSimulationService, ArticulatedHandPose.GestureId handPose, Vector3 handLocation)
        {
            yield return null;

            Debug.Assert(
                ((inputSimulationService.ControllerSimulationMode == ControllerSimulationMode.HandGestures) ||
                (inputSimulationService.ControllerSimulationMode == ControllerSimulationMode.ArticulatedHand)),
                "The current ControllerSimulationMode must be HandGestures or ArticulatedHand!");
            SimulatedHandData handData = handedness == Handedness.Right ? inputSimulationService.HandDataRight : inputSimulationService.HandDataLeft;
            handData.Update(true, false, GenerateHandPose(handPose, handedness, handLocation, Quaternion.identity));

            // Wait one frame for the hand to actually appear
            yield return null;
        }

        /// <summary>
        /// Shows the motion controller in the default state, at the origin
        /// </summary>
        public static IEnumerator ShowMontionController(Handedness handedness, InputSimulationService inputSimulationService)
        {
            SimulatedMotionControllerButtonState defaultButtonState = new SimulatedMotionControllerButtonState();
            yield return ShowMontionController(handedness, inputSimulationService, defaultButtonState, Vector3.zero);
        }

        public static IEnumerator ShowMontionController(Handedness handedness, InputSimulationService inputSimulationService, SimulatedMotionControllerButtonState buttonState, Vector3 motionControllerLocation)
        {
            yield return null;

            Debug.Assert((ControllerSimulationMode.MotionController == inputSimulationService.ControllerSimulationMode), "The current ControllerSimulationMode must be MotionController!");
            SimulatedMotionControllerData motionControllerData = handedness == Handedness.Right ? inputSimulationService.MotionControllerDataRight : inputSimulationService.MotionControllerDataLeft;
            motionControllerData.Update(true, buttonState, UpdateMotionControllerPose(handedness, motionControllerLocation, Quaternion.identity));

            // Wait one frame for the hand to actually appear
            yield return null;
        }

        public static void InstallTextMeshProEssentials()
        {
#if UNITY_EDITOR
            // Import the TMP Essential Resources package
            string packageFullPath = Path.GetFullPath("Packages/com.unity.textmeshpro");
            if (Directory.Exists(packageFullPath))
            {
                AssetDatabase.ImportPackage(packageFullPath + "/Package Resources/TMP Essential Resources.unitypackage", false);
            }
            else
            {
                Debug.LogError("Unable to locate the Text Mesh Pro package.");
            }
#endif
        }

        /// <summary>
        /// Waits for the user to press the enter key before a test continues.
        /// Not actually used by any test, but it is useful when debugging since you can
        /// pause the state of the test and inspect the scene.
        /// </summary>
        public static IEnumerator WaitForEnterKey()
        {
            Debug.Log(Time.time + "Press Enter...");
            while (!UnityEngine.Input.GetKeyDown(KeyCode.Return))
            {
                yield return null;
            }
        }

        /// <summary>
        /// Sometimes it take a few frames for inputs raised via InputSystem.OnInput*
        /// to actually get sent to input handlers. This method waits for enough frames
        /// to pass so that any events raised actually have time to send to handlers.
        /// We set it fairly conservatively to ensure that after waiting
        /// all input events have been sent.
        /// </summary>
        public static IEnumerator WaitForInputSystemUpdate()
        {
            const int inputSystemUpdateFrames = 10;
            for (int i = 0; i < inputSystemUpdateFrames; i++)
            {
                yield return null;
            }
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

        #region Obsolete Fields, Functions, and Properties
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
        ///     PlayModeTestUtilities.UseSlowTestController = true;
        ///     ...
        ///     PlayModeTestUtilities.UseSlowTestController = false;
        /// }
        /// </code>
        /// </example>
        /// <remarks>
        /// <para>Note that this value is reset to false after each play mode test that uses
        /// PlayModeTestUtilities.Setup() - this is to reduce the chance that a forgotten
        /// UseSlowTestController = true ends up slowing all subsequent tests.</para>
        /// </remarks>
        [Obsolete("Use UseSlowTestController instead.")]
        public static bool UseSlowTestHand
        {
            get => UseSlowTestController;
            set { UseSlowTestController = value; }
        }

        /// <summary>
        /// The number of frames that elapse for each test controller movement, taking into account if
        /// slow test controller mode has been engaged.
        /// </summary>
        [Obsolete("Use ControllerMoveSteps instead.")]
        public static int HandMoveSteps => ControllerMoveSteps;

        /// <summary>
        /// A sentinel value used by controller test utilities to indicate that the default number of move
        /// steps should be used or not.
        /// </summary>
        /// <remarks>
        /// <para>This is primarily something that exists to get around the limitation of default parameter
        /// values requiring compile-time constants.</para>
        /// </remarks>
        [Obsolete("Use ControllerMoveStepsSentinelValue instead.")]
        internal const int HandMoveStepsSentinelValue = ControllerMoveStepsSentinelValue;

        [Obsolete("Use SetControllerSimulationMode instead.")]
        public static void SetHandSimulationMode(ControllerSimulationMode mode)
        {
            SetControllerSimulationMode(mode);
        }

        [Obsolete("Use PushControllerSimulationProfile instead.")]
        public static void PushHandSimulationProfile()
        {
            PushControllerSimulationProfile();
        }

        [Obsolete("Use PopControllerSimulationProfile instead.")]
        public static void PopHandSimulationProfile()
        {
            PopControllerSimulationProfile();
        }
        #endregion
    }
}
#endif
