// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils.Editor;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// A class helping users with validating the configuration of the project for use with MRTK3
    /// </summary>
    public static class MRTKProjectValidation
    {
        private const string XRProjectValidationSettingsPath = "Project/XR Plug-in Management/Project Validation";
        private const string DefaultMRTKProfileGuid = "c677e5c4eb85b7849a8da406775c299d";
        private static readonly BuildTargetGroup[] buildTargetGroups = ((BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup))).Distinct().ToArray();
        private static readonly Dictionary<BuildTargetGroup, List<BuildValidationRule>> validationRulesDictionary = new Dictionary<BuildTargetGroup, List<BuildValidationRule>>();

        [MenuItem("Mixed Reality/MRTK3/Utilities/Project Validation", priority = 0)]
        private static void MenuItem()
        {
            SettingsService.OpenProjectSettings(XRProjectValidationSettingsPath);
        }

        [InitializeOnLoadMethod]
        private static void MRTKProjectValidationCheck()
        {
            AddMRTKCoreValidationRules();
            // Add a delay to wait for all rules to be added (potentially from other MRTK scripts registered with InitializeOnLoadMethod)
            EditorApplication.delayCall += AddMRTKValidationRules;
        }

        private static void AddMRTKValidationRules()
        {
            foreach (var buildTargetGroup in validationRulesDictionary.Keys)
            {
                BuildValidator.AddRules(buildTargetGroup, validationRulesDictionary[buildTargetGroup]);
            }
        }

        private static void AddMRTKCoreValidationRules()
        {
            List<BuildValidationRule> mrtkCoreTargetIndependentRules = new List<BuildValidationRule>();
            // Always ensure the standalone target has a profile assigned no matter what target is being targeted
            mrtkCoreTargetIndependentRules.Add(GenerateProfileRule(BuildTargetGroup.Standalone));
            AddTargetIndependentRules(mrtkCoreTargetIndependentRules);

            foreach (var buildTargetGroup in buildTargetGroups)
            {
                // Skip the standalone target as the profile rule for it is already present for all build targets
                if (buildTargetGroup != BuildTargetGroup.Standalone)
                {
                    AddTargetDependentRules(new List<BuildValidationRule>() { GenerateProfileRule(buildTargetGroup) }, buildTargetGroup);
                }
            }
        }

        /// <summary>
        /// Add a build target independent rule for project configuration validation
        /// </summary>
        public static void AddTargetIndependentRules(List<BuildValidationRule> rules)
        {
            foreach (BuildTargetGroup buildTargetGroup in buildTargetGroups)
            {
                AddTargetDependentRules(rules, buildTargetGroup);
            }
        }

        /// <summary>
        /// Add a build target dependent rule (e.g. specific to Standalone) for project configuration validation
        /// </summary>
        public static void AddTargetDependentRules(List<BuildValidationRule> rules, BuildTargetGroup buildTargetGroup)
        {
            if (validationRulesDictionary.TryGetValue(buildTargetGroup, out List<BuildValidationRule> rulesList))
            {
                rulesList.AddRange(rules);
            }
            else
            {
                validationRulesDictionary.Add(buildTargetGroup, new List<BuildValidationRule>(rules));
            }
        }

        private static BuildValidationRule GenerateProfileRule(BuildTargetGroup buildTargetGroup)
        {
            return new BuildValidationRule()
            {
                Category = "MRTK3",
                Message = $"MRTK3 profile may need to be assigned for the {buildTargetGroup} build target.",
                CheckPredicate = () => MRTKSettings.ProfileForBuildTarget(buildTargetGroup) != null,
                FixIt = () => MRTKSettings.GetOrCreateSettings().SetProfileForBuildTarget(buildTargetGroup,
                AssetDatabase.LoadAssetAtPath<MRTKProfile>(AssetDatabase.GUIDToAssetPath(DefaultMRTKProfileGuid))),
                FixItMessage = $"Assign the default MRTK3 profile for the {buildTargetGroup} build target",
                Error = false,
                HelpLink = "https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/mrtk3-overview/setup#4-configure-mrtk-profile-after-import"
            };
        }
    }
}
