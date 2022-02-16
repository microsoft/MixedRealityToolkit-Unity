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

using Microsoft.MixedReality.Toolkit.Experimental.Joystick;
using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class JoystickTests : BasePlayModeTests
    {
        #region Tests

        [UnityTest]
        public IEnumerator TestJoystickTranslation()
        {
            InstantiateJoystick(out JoystickController joystick, out Transform grabber);
            Assert.IsNotNull(joystick);
            Assert.IsNotNull(grabber);

            yield return null;

            // Switch the joystick mode to 'Move'.
            joystick.Mode = JoystickMode.Move;

            // Instantiate large object and set as target.
            var targetObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            targetObject.transform.localScale = 7.0f * Vector3.one;
            joystick.TargetObject = targetObject;

            Vector3 startPosition, endPosition;

            // Capture the starting rotation of the target object.
            startPosition = targetObject.transform.position;

            // Tilt the joystick to the right.
            joystick.StartDrag();
            grabber.localPosition += 2.5f * Vector3.right;

            // Wait a short while, then capture once again the rotation of the target object.
            yield return new WaitForSecondsRealtime(1.5f);
            endPosition = targetObject.transform.position;

            // Untilt the joystick.
            grabber.localPosition += 2.5f * Vector3.left;
            joystick.StopDrag();

            Assert.IsTrue(startPosition != endPosition);
        }

        [UnityTest]
        public IEnumerator TestJoystickRotation()
        {
            InstantiateJoystick(out JoystickController joystick, out Transform grabber);
            Assert.IsNotNull(joystick);
            Assert.IsNotNull(grabber);

            yield return null;

            // Switch the joystick mode to 'rotate'.
            joystick.Mode = JoystickMode.Rotate;

            // Instantiate large object and set as target.
            var targetObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            targetObject.transform.localScale = 7.0f * Vector3.one;
            joystick.TargetObject = targetObject;

            Quaternion startRotation, endRotation;

            // Capture the starting rotation of the target object.
            startRotation = targetObject.transform.rotation;

            // Tilt the joystick to the right.
            joystick.StartDrag();
            grabber.localPosition += 2.5f * Vector3.right;

            // Wait a short while, then capture once again the rotation of the target object.
            yield return new WaitForSecondsRealtime(1.5f);
            endRotation = targetObject.transform.rotation;

            // Untilt the joystick.
            grabber.localPosition += 2.5f * Vector3.left;
            joystick.StopDrag();

            Assert.IsTrue(Quaternion.Angle(startRotation, endRotation) > 0.0f);
        }

        [UnityTest]
        public IEnumerator TestJoystickScale()
        {
            InstantiateJoystick(out JoystickController joystick, out Transform grabber);
            Assert.IsNotNull(joystick);
            Assert.IsNotNull(grabber);

            yield return null;

            // Switch the joystick mode to 'Scale'.
            joystick.Mode = JoystickMode.Scale;

            // Instantiate large object and set as target.
            var targetObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            targetObject.transform.localScale = 7.0f * Vector3.one;
            joystick.TargetObject = targetObject;

            Vector3 startScale, endScale;

            // Capture the starting rotation of the target object.
            startScale = targetObject.transform.localScale;

            // Tilt the joystick to the right.
            joystick.StartDrag();
            grabber.localPosition += 2.5f * Vector3.right;

            // Wait a short while, then capture once again the rotation of the target object.
            yield return new WaitForSecondsRealtime(1.5f);
            endScale = targetObject.transform.position;

            // Untilt the joystick.
            grabber.localPosition += 2.5f * Vector3.left;
            joystick.StopDrag();

            Assert.IsTrue(startScale != endScale);
        }

        #endregion Tests

        #region Utilities

        public readonly string JoystickPrefabAssetPath = AssetDatabase.GUIDToAssetPath(JoystickPrefabGuid);

        // Examples/Experimental/Joystick/JoystickPrefab.prefab
        private const string JoystickPrefabGuid = "8bdd451919f46a94ba6d151b6d0cdffd";

        private const string TargetGrabberPath = "Grabber";

        private void InstantiateJoystick(out JoystickController joystick, out Transform grabber)
        {
            InstantiateJoystickPrefab(JoystickPrefabAssetPath, TargetGrabberPath, out joystick, out grabber);
        }

        private void InstantiateJoystickPrefab(string prefabPath, string grabberPath, out JoystickController joystick, out Transform grabber)
        {
            // Load joystick prefab.
            var joystickObject = InstantiatePrefabFromPath(prefabPath);
            joystick = joystickObject.GetComponent<JoystickController>();
            Assert.IsNotNull(joystick);

            // Find grabber object.
            grabber = joystickObject.transform.Find(grabberPath);
            Assert.IsNotNull(grabber);
        }

        private GameObject InstantiatePrefabFromPath(string path)
        {
            // Load joystick prefab.
            Object joystickPrefab = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            GameObject joystick = Object.Instantiate(joystickPrefab) as GameObject;
            Assert.IsNotNull(joystick);
            return joystick;
        }

        #endregion Utilities
    }
}
#endif