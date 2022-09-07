// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input.Editor
{
    /// <summary>
    /// A class adding input related rule(s) to the validator
    /// </summary>
    internal static class InputValidation
    {
        [InitializeOnLoadMethod]
        private static void AddInputValidationRule()
        {
            foreach (var buildTargetGroup in MRTKProjectValidation.BuildTargetGroups)
            {
                MRTKProjectValidation.AddTargetDependentRules(new List<BuildValidationRule>() { GenerateSpeechInteractorRule(buildTargetGroup) }, buildTargetGroup);
            }
            MRTKProjectValidation.AddTargetIndependentRules(new List<BuildValidationRule>() { GenerateSkinWeightsRule() });
        }

        private static BuildValidationRule GenerateSpeechInteractorRule(BuildTargetGroup buildTargetGroup)
        {
            return new BuildValidationRule()
            {
                IsRuleEnabled = () => (MRTKProjectValidation.GetLoadedSubsystemsForBuildTarget(buildTargetGroup)?.Any(s => typeof(PhraseRecognitionSubsystem).IsAssignableFrom(s.Type))).GetValueOrDefault()
                    && Object.FindObjectOfType<SpeechInteractor>(true),
                Category = "MRTK3",
                Message = "The speech interactor needs to be active and enabled in the scene to allow for speech interactions with interactables (e.g. buttons).",
                CheckPredicate = () => Object.FindObjectOfType<SpeechInteractor>(true).isActiveAndEnabled,
                FixIt = () => EditorGUIUtility.PingObject(Object.FindObjectOfType<SpeechInteractor>(true)),
                FixItMessage = "Make sure the speech interactor component is enabled and in active in the hierarchy",
                FixItAutomatic = false,
                Error = false
            };
        }

        private static BuildValidationRule GenerateSkinWeightsRule()
        {
            return new BuildValidationRule()
            {
                Category = "MRTK3",
                Message = "The skin weights setting under quality settings needs to be set to TwoBones or higher for optimal rigged hand mesh visaulizations.",
                CheckPredicate = () => QualitySettings.skinWeights > SkinWeights.OneBone,
                FixIt = () => QualitySettings.skinWeights = SkinWeights.TwoBones,
                FixItMessage = "Set the skin weights to TwoBones",
                FixItAutomatic = true,
                Error = false
            };
        }
    }
}
