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
            MRTKProjectValidation.AddTargetIndependentRules(new List<BuildValidationRule>() { GenerateSkinWeightsRule(), GenerateGLTFastRule() });

            // Only generate the KTX rule for platforms related to Meta
            MRTKProjectValidation.AddTargetDependentRules(new List<BuildValidationRule>() { GenerateKTXRule() }, BuildTargetGroup.Android);
            MRTKProjectValidation.AddTargetDependentRules(new List<BuildValidationRule>() { GenerateKTXRule() }, BuildTargetGroup.Standalone);
        }

        private static BuildValidationRule GenerateSpeechInteractorRule(BuildTargetGroup buildTargetGroup)
        {
            return new BuildValidationRule()
            {
                IsRuleEnabled = () => (MRTKProjectValidation.GetLoadedSubsystemsForBuildTarget(buildTargetGroup)?.Any(s => typeof(KeywordRecognitionSubsystem).IsAssignableFrom(s.Type))).GetValueOrDefault()
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
                Message = "The skin weights setting under quality settings needs to be set to TwoBones or higher for optimal rigged hand mesh visualizations.",
                CheckPredicate = () => QualitySettings.skinWeights > SkinWeights.OneBone,
                FixIt = () => QualitySettings.skinWeights = SkinWeights.TwoBones,
                FixItMessage = "Set the skin weights to TwoBones",
                FixItAutomatic = true,
                Error = false
            };
        }

        private static BuildValidationRule GenerateGLTFastRule()
        {
            return new BuildValidationRule()
            {
                Category = "MRTK3",
                Message = "For controller models to show up in scenes, you need a glTF importer. We recommend the open source glTFast importer. " +
                "Please ignore this message if you already have another glTF importer in the project.",
                CheckPredicate = () =>
                {
#if GLTFAST_PRESENT
                    return true;
#else
                    return false;
#endif
                },
                FixIt = () => Application.OpenURL("https://github.com/atteneder/glTFast"),
                FixItMessage = "Open the GitHub repo for glTFast. Please refer to the installing section for instructions.",
                FixItAutomatic = false,
                Error = false
            };
        }

        private static BuildValidationRule GenerateKTXRule()
        {
            return new BuildValidationRule()
            {
                Category = "MRTK3",
                Message = "For Meta's controller models to show up in scenes, you need KTX support for your glTF importer. " +
                "If you are using our recommended importer glTFast, you need to also include the open source KTX Unity package in the project. " +
                "Please ignore this message if you already have another glTF importer with KTX support in the project.",
                CheckPredicate = () =>
                {
#if KTX_PRESENT
                    return true;
#else
                    return false;
#endif
                },
                FixIt = () => Application.OpenURL("https://github.com/atteneder/KtxUnity"),
                FixItMessage = "Open the GitHub repo for KTX Unity. Please refer to the installing section for instructions.",
                FixItAutomatic = false,
                Error = false
            };
        }
    }
}
