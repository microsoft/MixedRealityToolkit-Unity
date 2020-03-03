// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// New controller types can be registered by adding the MixedRealityControllerAttribute to
    /// the controller class.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Profiles/Mixed Reality Controller Mapping Profile", fileName = "MixedRealityControllerMappingProfile", order = (int)CreateProfileMenuItemIndices.ControllerMapping)]
    public class MixedRealityControllerMappingProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [Tooltip("The list of controller mappings your application can use.")]
        [FormerlySerializedAs("mixedRealityControllerMappingProfiles")]
        private MixedRealityControllerMapping[] mixedRealityControllerMappings = Array.Empty<MixedRealityControllerMapping>();

        /// <summary>
        /// The list of controller mappings your application can use.
        /// </summary>
        public MixedRealityControllerMapping[] MixedRealityControllerMappings => mixedRealityControllerMappings;

        [Obsolete("MixedRealityControllerMappingProfiles is obsolete. Please use MixedRealityControllerMappings.")]
        public MixedRealityControllerMapping[] MixedRealityControllerMappingProfiles => mixedRealityControllerMappings;

#if UNITY_EDITOR
        [MenuItem("Mixed Reality Toolkit/Utilities/Update/Controller Mapping Profiles")]
        private static void UpdateAllControllerMappingProfiles()
        {
            string[] guids = AssetDatabase.FindAssets("t:MixedRealityControllerMappingProfile");
            string[] assetPaths = new string[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string guid = guids[i];
                assetPaths[i] = AssetDatabase.GUIDToAssetPath(guid);

                MixedRealityControllerMappingProfile asset = AssetDatabase.LoadAssetAtPath(assetPaths[i], typeof(MixedRealityControllerMappingProfile)) as MixedRealityControllerMappingProfile;

                List<MixedRealityControllerMapping> updatedMappings = new List<MixedRealityControllerMapping>();

                foreach (MixedRealityControllerMapping mapping in asset.MixedRealityControllerMappings)
                {
                    if (mapping.ControllerType.Type == null)
                    {
                        continue;
                    }

                    if (!mapping.HasCustomInteractionMappings)
                    {
                        mapping.UpdateInteractionSettingsFromDefault();
                    }

                    updatedMappings.Add(mapping);
                }

                asset.mixedRealityControllerMappings = updatedMappings.ToArray();
            }
            AssetDatabase.ForceReserializeAssets(assetPaths);
        }

        private static Type[] controllerMappingTypes;

        public static Type[] ControllerMappingTypes { get { CollectControllerTypes(); return controllerMappingTypes; } }

        public static Type[] CustomControllerMappingTypes { get => (from type in ControllerMappingTypes where UsesCustomInteractionMapping(type) select type).ToArray(); }

        private static void CollectControllerTypes()
        {
            if (controllerMappingTypes == null)
            {
                List<Type> controllerTypes = new List<Type>();
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    IEnumerable<Type> types = null;
                    try
                    {
                        types = assembly.ExportedTypes;
                    }
                    catch (NotSupportedException)
                    {
                        // assembly.ExportedTypes may not be supported.
                    }
                    catch (ReflectionTypeLoadException e)
                    {
                        // Not all assemblies may load correctly, but even upon encountering error
                        // some subset may have loaded in.
                        if (e.Types != null)
                        {
                            List<Type> loadedTypes = new List<Type>();
                            foreach (Type type in e.Types)
                            {
                                // According to API docs, this array may contain null values
                                // so they must be filtered out here.
                                if (type != null)
                                {
                                    loadedTypes.Add(type);
                                }
                            }
                            types = loadedTypes;
                        }
                    }

                    if (types != null)
                    {
                        foreach (Type type in types)
                        {
                            if (type.IsSubclassOf(typeof(BaseController)) &&
                                MixedRealityControllerAttribute.Find(type) != null)
                            {
                                controllerTypes.Add(type);
                            }
                        }
                    }
                }

                controllerMappingTypes = controllerTypes.ToArray();
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
                    int idx = Array.FindIndex(MixedRealityControllerMappings, 0, MixedRealityControllerMappings.Length,
                        profile => profile.ControllerType.Type == controllerType && profile.Handedness == handedness);

                    if (idx < 0)
                    {
                        var newMapping = new MixedRealityControllerMapping(controllerType, handedness);
                        newMapping.SetDefaultInteractionMapping(overwrite: false);

                        // Re-use existing mapping with the same supported controller type.
                        foreach (var otherMapping in mixedRealityControllerMappings)
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

                        idx = mixedRealityControllerMappings.Length;
                        Array.Resize(ref mixedRealityControllerMappings, idx + 1);
                        mixedRealityControllerMappings[idx] = newMapping;
                    }
                    else
                    {
                        mixedRealityControllerMappings[idx].UpdateInteractionSettingsFromDefault();
                    }
                }
            }
        }

        private void SortMappings()
        {
            Array.Sort(mixedRealityControllerMappings, (profile1, profile2) =>
            {
                bool isOptional1 = (profile1.ControllerType.Type == null || profile1.HasCustomInteractionMappings);
                bool isOptional2 = (profile2.ControllerType.Type == null || profile2.HasCustomInteractionMappings);
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
            return attribute != null ? attribute.SupportedHandedness : Array.Empty<Handedness>();
        }
    }
}