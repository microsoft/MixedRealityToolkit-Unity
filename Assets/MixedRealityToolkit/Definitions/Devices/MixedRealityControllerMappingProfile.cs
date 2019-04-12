// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// New controller types can be registered by adding the MixedRealityControllerAttribute to
    /// the controller class.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Controller Mapping Profile", fileName = "MixedRealityControllerMappingProfile", order = (int)CreateProfileMenuItemIndices.ControllerMapping)]
    public class MixedRealityControllerMappingProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [Tooltip("The list of controller templates your application can use.")]
        private MixedRealityControllerMapping[] mixedRealityControllerMappingProfiles = new MixedRealityControllerMapping[0];

        public MixedRealityControllerMapping[] MixedRealityControllerMappingProfiles => mixedRealityControllerMappingProfiles;

#if UNITY_EDITOR

        private static Type[] controllerMappingTypes;

        public static Type[] ControllerMappingTypes { get { CollectControllerTypes(); return controllerMappingTypes; } }

        public static Type[] CustomControllerMappingTypes { get => (from type in ControllerMappingTypes where UsesCustomInteractionMapping(type) select type).ToArray(); }

        private static void CollectControllerTypes()
        {
            if (controllerMappingTypes == null)
            {
                List<Type> tmp = new List<Type>();
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        foreach (Type type in assembly.ExportedTypes)
                        {
                            if (type.IsSubclassOf(typeof(BaseController)) &&
                                MixedRealityControllerAttribute.Find(type) != null)
                            {
                                tmp.Add(type);
                            }
                        }
                    }
                    catch (NotSupportedException) // assembly.ExportedTypes may not be supported.
                    { }
                }

                controllerMappingTypes = tmp.ToArray();
            }
        }

        public void Awake()
        {
            AddMappings();
            SortMappings();
        }

        private void AddMappings()
        {
            foreach (var controllerType in ControllerMappingTypes)
            {
                // Don't auto-add custom mappings when migrating, these can be removed by the user in the inspector.
                if (UsesCustomInteractionMapping(controllerType))
                {
                    continue;
                }

                foreach (Handedness handedness in GetSupportedHandedness(controllerType))
                {
                    // Try to find index of mapping in asset.
                    int idx = Array.FindIndex(MixedRealityControllerMappingProfiles, 0, MixedRealityControllerMappingProfiles.Length,
                        profile => profile.ControllerType.Type == controllerType && profile.Handedness == handedness);

                    if (idx < 0)
                    {
                        var newMapping = new MixedRealityControllerMapping(controllerType, handedness);
                        newMapping.SetDefaultInteractionMapping(overwrite: false);

                        // Re-use existing mapping with the same supported controller type.
                        foreach (var otherMapping in mixedRealityControllerMappingProfiles)
                        {
                            if (otherMapping.SupportedControllerType == newMapping.SupportedControllerType &&
                                otherMapping.Handedness == newMapping.Handedness)
                            {
                                try
                                {
                                    newMapping.SynchronizeInputActions(otherMapping.Interactions);
                                }
                                catch (ArgumentException e)
                                {
                                    Debug.LogError($"Controller mappings between {newMapping.Description} and {otherMapping.Description} do not match. Error message: {e.Message}");
                                }
                                break;
                            }
                        }

                        idx = mixedRealityControllerMappingProfiles.Length;
                        Array.Resize(ref mixedRealityControllerMappingProfiles, idx + 1);
                        mixedRealityControllerMappingProfiles[idx] = newMapping;
                    }
                }
            }
        }

        private void SortMappings()
        {
            Array.Sort(mixedRealityControllerMappingProfiles, (profile1, profile2) => 
            {
                bool isOptional1 = (profile1.ControllerType.Type == null || UsesCustomInteractionMapping(profile1.ControllerType.Type));
                bool isOptional2 = (profile2.ControllerType.Type == null || UsesCustomInteractionMapping(profile2.ControllerType.Type));
                if (!isOptional1 && !isOptional2)
                {
                    int idx1 = Array.FindIndex(ControllerMappingTypes, type => type == profile1.ControllerType.Type);
                    int idx2 = Array.FindIndex(ControllerMappingTypes, type => type == profile2.ControllerType.Type);

                    if (idx1 == idx2)
                    {
                        idx1 = (int)profile1.Handedness;
                        idx2 = (int)profile2.Handedness;
                    }

                    return Math.Sign(idx1 - idx2);
                }

                if (isOptional1 && isOptional2)
                {
                    return 0;
                }

                return isOptional1 ? 1 : -1; // Put custom mappings at the end. These can be added / removed in the inspector.
            });
        }

#endif // UNITY_EDITOR

        private static bool UsesCustomInteractionMapping(Type controllerType)
        {
            var attribute = MixedRealityControllerAttribute.Find(controllerType);
            return attribute != null ? attribute.Flags.HasFlag(MixedRealityControllerConfigurationFlags.UseCustomInteractionMappings) : false;
        }

        private static Handedness[] GetSupportedHandedness(Type controllerType)
        {
            var attribute = MixedRealityControllerAttribute.Find(controllerType);
            return attribute != null ? attribute.SupportedHandedness : new Handedness[0];
        }
    }
}