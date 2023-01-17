// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Unity.XR.CoreUtils.Editor;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Speech.Windows.Editor
{
    /// <summary>
    /// A class adding Windows Speech related rule(s) to the validator
    /// </summary>
    internal static class WindowsSpeechValidation
    {
        [InitializeOnLoadMethod]
        private static void AddWindowsSpeechValidationRule()
        {
            List<BuildValidationRule> rules = new List<BuildValidationRule>(){ GenerateMicrophoneCapabilityRule() };
            MRTKProjectValidation.AddTargetDependentRules(rules, BuildTargetGroup.WSA);
        }

        private static BuildValidationRule GenerateMicrophoneCapabilityRule()
        {
            return new BuildValidationRule()
            {
                IsRuleEnabled = () => (MRTKProjectValidation.GetLoadedSubsystemsForBuildTarget(BuildTargetGroup.WSA)?.Contains(typeof(WindowsKeywordRecognitionSubsystem))).GetValueOrDefault() ||
                (MRTKProjectValidation.GetLoadedSubsystemsForBuildTarget(BuildTargetGroup.WSA)?.Contains(typeof(WindowsDictationSubsystem))).GetValueOrDefault(),
                Category = "MRTK3",
                Message = "WindowsKeywordRecognition/DictationSubsystem requires the WSA Microphone capability to be set to true.",
                CheckPredicate = () => PlayerSettings.WSA.GetCapability(PlayerSettings.WSACapability.Microphone),
                FixIt = () => PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.Microphone, true),
                FixItMessage = "Set the WSA Microphone capability to true",
                FixItAutomatic = true,
                Error = true
            };
        }
    }
}
