// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if !WINDOWS_UWP

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.ComponentModel.Design;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class InputRecordingSystemTests
    {
        private static readonly string profilePath = AssetDatabase.GUIDToAssetPath("d1a78f1a97d7be74fb6f2b34328a240f");
        private static readonly string testAllPath = AssetDatabase.GUIDToAssetPath("04200fe2a80bc2242917926e80d14d65");
        private static readonly string testHandsPath = AssetDatabase.GUIDToAssetPath("28169421bf13afb4fbf236978fe48c7f");
        private static readonly string testCameraPath = AssetDatabase.GUIDToAssetPath("7a36c67a5ac9477439c07d1c13b65da4");
        private static readonly string testGazePath = AssetDatabase.GUIDToAssetPath("826224f1a0b054b488ff1f960d02a9f7");

        [UnitySetUp]
        public IEnumerator Init()
        {
            var profile = AssetDatabase.LoadAssetAtPath(profilePath, typeof(MixedRealityToolkitConfigurationProfile)) as MixedRealityToolkitConfigurationProfile;
            TestUtilities.InitializeMixedRealityToolkit(profile);

            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            PlayModeTestUtilities.TearDown();

            yield return null;
        }

        [UnityTest]
        public IEnumerator TestHandRecording() => TestRecording(testHandsPath);

        [UnityTest]
        public IEnumerator TestCameraRecording() => TestRecording(testCameraPath);

        [UnityTest]
        public IEnumerator TestGazeRecording() => TestRecording(testGazePath);

        [UnityTest]
        public IEnumerator TestAllRecording() => TestRecording(testAllPath);

        private static IEnumerator TestRecording(string recordingProfilePath)
        {
            var inputSystem = PlayModeTestUtilities.GetInputSystem();
            var recordingService = CoreServices.GetInputSystemDataProvider<InputRecordingService>();
            var recordingProfile = AssetDatabase.LoadAssetAtPath(recordingProfilePath, typeof(MixedRealityInputRecordingProfile)) as MixedRealityInputRecordingProfile;

            Debug.Log($"Record hand data: {recordingProfile.RecordHandData}");
            Debug.Log($"Record camera data: {recordingProfile.RecordCameraPose}");
            Debug.Log($"Record gaze data: {recordingProfile.RecordEyeGaze}");

            recordingService.InputRecordingProfile = recordingProfile;

            if (recordingProfile.RecordEyeGaze)
            {
                CoreServices.InputSystem.EyeGazeProvider.Enabled = true;
                CoreServices.InputSystem.EyeGazeProvider.UpdateEyeTrackingStatus(null, true);
            }

            recordingService.StartRecording();

            yield return MoveAround(recordingProfile.RecordHandData, recordingProfile.RecordCameraPose, recordingProfile.RecordEyeGaze);

            recordingService.StopRecording();

            string path = recordingService.SaveInputAnimation("TestRecording.bin", null);
            var playbackService = CoreServices.GetInputSystemDataProvider<IMixedRealityInputPlaybackService>();

            yield return null;

            playbackService.LoadInputAnimation(path);

            var animation = playbackService.Animation;

            Assert.True(recordingProfile.RecordHandData == animation.HasHandData && recordingProfile.RecordCameraPose == animation.HasCameraPose && recordingProfile.RecordEyeGaze == animation.HasEyeGaze);
            playbackService.Play();

            yield return new WaitWhile(() => playbackService.IsPlaying);
            yield return null;
        }

        private static IEnumerator MoveAround(bool moveHands, bool moveCamera, bool moveGaze)
        {
            if (moveHands)
            {
                var rightHand = new TestHand(Handedness.Right);

                yield return rightHand.Show(new Vector3(0, 0, 0.5f));
                yield return rightHand.SetRotation(Quaternion.identity, 0);
                yield return rightHand.MoveTo(new Vector3(0, 0, 2f), 40);
                yield return rightHand.MoveTo(new Vector3(0, 0, 0.5f), 40);
                yield return rightHand.SetRotation(Quaternion.AngleAxis(-90f, Vector3.forward), 40);
                yield return rightHand.SetRotation(Quaternion.AngleAxis(90f, Vector3.forward), 40);
                yield return rightHand.SetRotation(Quaternion.identity, 20);
            }

            if (moveCamera)
            {
                var camera = CameraCache.Main.transform;

                camera.rotation = Quaternion.identity;

                yield return InterpRotation(camera, 0.5f, Quaternion.AngleAxis(-90f, Vector3.up));
                yield return InterpRotation(camera, 0.5f, Quaternion.AngleAxis(90f, Vector3.up));
                yield return InterpRotation(camera, 0.5f, Quaternion.identity);
            }

            if (moveGaze)
            {
                var gazeProvider = CoreServices.InputSystem.EyeGazeProvider;

                gazeProvider.UpdateEyeTrackingStatus(null, true);
                gazeProvider.UpdateEyeGaze(null, new Ray(Vector3.zero, Vector3.forward), DateTime.UtcNow);

                yield return InterpGaze(gazeProvider, 0.5f, new Vector3(-1f, 0f, 1f).normalized);
                yield return InterpGaze(gazeProvider, 0.5f, new Vector3(1f, 0f, 1f).normalized);
                yield return InterpGaze(gazeProvider, 0.5f, Vector3.forward);
            }
        }

        private static IEnumerator InterpRotation(Transform transform, float duration, Quaternion to)
        {
            var start = transform.rotation;

            return Interp(duration, t => transform.rotation = Quaternion.Slerp(start, to, t));
        }

        private static IEnumerator InterpGaze(IMixedRealityEyeGazeProvider gazeProvider, float duration, Vector3 to)
        {
            var start = gazeProvider.LatestEyeGaze;

            return Interp(duration, t =>
            {
                var direction = Vector3.Slerp(start.direction, to, t);

                gazeProvider.UpdateEyeTrackingStatus(null, true);
                gazeProvider.UpdateEyeGaze(null, new Ray(start.origin, direction), DateTime.UtcNow);
            });
        }

        private static IEnumerator Interp(float duration, Action<float> action)
        {
            float startTime = Time.time;
            float endTime = startTime + duration;

            while (Time.time < endTime)
            {
                action((Time.time - startTime) / duration);

                yield return null;
            }

            action(1f);
        }
    }
}

#endif