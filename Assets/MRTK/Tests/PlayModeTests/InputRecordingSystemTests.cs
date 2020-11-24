#if !WINDOWS_UWP

using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.TestTools;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Tests {
    public class InputRecordingSystemTests
    {
        private static readonly string profilePath = AssetDatabase.GUIDToAssetPath("d1a78f1a97d7be74fb6f2b34328a240f");
        
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
        public IEnumerator TestHandRecording()
        {
            var profile = AssetDatabase.LoadAssetAtPath(profilePath, typeof(MixedRealityToolkitConfigurationProfile)) as MixedRealityToolkitConfigurationProfile;

            Debug.Log(profilePath);
            MixedRealityToolkit.Instance.ActiveProfile = profile;
            
            yield return null;
            yield return null;

            var inputSystem = PlayModeTestUtilities.GetInputSystem();
            var recordingService = CoreServices.GetInputSystemDataProvider<IMixedRealityInputRecordingService>();
            
            recordingService.StartRecording();

            yield return MixedRealityToolkit.Instance.StartCoroutine(MoveAround());
            
            recordingService.StopRecording();
            
            string path = recordingService.SaveInputAnimation("TestHandRecording.bin", null);
            var playbackService = CoreServices.GetInputSystemDataProvider<IMixedRealityInputPlaybackService>();

            yield return new WaitForSeconds(1f);

            playbackService.LoadInputAnimation(path);
            playbackService.Play();
            
            yield return new WaitWhile(() => playbackService.IsPlaying);
            yield return new WaitForSeconds(1f);
            yield return null;
        }

        private static IEnumerator MoveAround()
        {
            var rightHand = new TestHand(Handedness.Right);
            
            yield return rightHand.Show(new Vector3(0, 0, 0.5f));
            yield return rightHand.MoveTo(new Vector3(0, 0, 2f), 40);
            yield return rightHand.MoveTo(new Vector3(0, 0, 0.5f), 40);
            yield return rightHand.SetRotation(Quaternion.AngleAxis(-90f, Vector3.forward), 40);
            yield return rightHand.SetRotation(Quaternion.AngleAxis(90f, Vector3.forward), 40);
            yield return rightHand.SetRotation(Quaternion.AngleAxis(0f, Vector3.forward), 20);
        }
    }
}

#endif