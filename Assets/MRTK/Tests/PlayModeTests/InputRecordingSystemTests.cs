// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if !WINDOWS_UWP

using System;
using System.Collections;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.TestTools;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Tests {
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
            TestUtilities.InitializeMixedRealityToolkit(true);

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
            var profile = AssetDatabase.LoadAssetAtPath(profilePath, typeof(MixedRealityToolkitConfigurationProfile)) as MixedRealityToolkitConfigurationProfile;

            Debug.Log(profilePath);
            MixedRealityToolkit.Instance.ActiveProfile = profile;

            yield return null;
            yield return null;

            var inputSystem = PlayModeTestUtilities.GetInputSystem();
            var recordingService = CoreServices.GetInputSystemDataProvider<InputRecordingService>();
            var recordingProfile = AssetDatabase.LoadAssetAtPath(recordingProfilePath, typeof(MixedRealityInputRecordingProfile)) as MixedRealityInputRecordingProfile;

            Debug.Log($"Record hand data: {recordingProfile.RecordHandData}");
            Debug.Log($"Record camera data: {recordingProfile.RecordCameraPose}");
            Debug.Log($"Record gaze data: {recordingProfile.RecordEyeGaze}");
            
            recordingService.InputRecordingProfile = recordingProfile;
            recordingService.StartRecording();

            yield return MoveAround(recordingProfile.RecordHandData, recordingProfile.RecordCameraPose, recordingProfile.RecordEyeGaze);
            
            recordingService.StopRecording();
            
            string path = recordingService.SaveInputAnimation("TestRecording.bin", null);
            var playbackService = CoreServices.GetInputSystemDataProvider<IMixedRealityInputPlaybackService>();

            yield return null;

            playbackService.LoadInputAnimation(path);
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
                yield return rightHand.MoveTo(new Vector3(0, 0, 2f), 40);
                yield return rightHand.MoveTo(new Vector3(0, 0, 0.5f), 40);
                yield return rightHand.SetRotation(Quaternion.AngleAxis(-90f, Vector3.forward), 40);
                yield return rightHand.SetRotation(Quaternion.AngleAxis(90f, Vector3.forward), 40);
                yield return rightHand.SetRotation(Quaternion.AngleAxis(0f, Vector3.forward), 20);
            }

            if (moveCamera)
            {
                var camera = CameraCache.Main.transform;

                yield return InterpRotation(camera, 0.5f, Quaternion.AngleAxis(-90f, Vector3.up));
                yield return InterpRotation(camera, 0.5f, Quaternion.AngleAxis(90f, Vector3.up));
                yield return InterpRotation(camera, 0.5f, Quaternion.AngleAxis(0f, Vector3.up));
            }

            if (moveGaze)
            {
                
            }
        }

        private static IEnumerator InterpRotation(Transform transform, float duration, Quaternion to)
        {
            var start = transform.rotation;
            
            return Interp(duration, t => transform.rotation = Quaternion.Slerp(start, to, t));
        }

        private static IEnumerator InterpGaze(IMixedRealityEyeGazeDataProvider dataProvider, IMixedRealityEyeGazeProvider gazeProvider, float duration, Vector3 to)
        {
            var start = gazeProvider.LatestEyeGaze;

            return Interp(duration, t =>
            {
                gazeProvider.UpdateEyeTrackingStatus(dataProvider, true);
                gazeProvider.UpdateEyeGaze(dataProvider, new Ray(start.origin, Vector3.Slerp(start.direction, to, t)), DateTime.UtcNow);
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