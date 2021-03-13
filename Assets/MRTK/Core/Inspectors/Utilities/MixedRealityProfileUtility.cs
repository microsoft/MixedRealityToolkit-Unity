// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// This class has utilities and functions for working with profiles in the Unity editor.
    /// </summary>
    public static class MixedRealityProfileUtility
    {
        /// <summary>
        /// Private class that listens for asset modifications and updates caches.
        /// </summary>
        private class AssetImportListener : UnityEditor.AssetPostprocessor
        {
            public static Action OnAssetsChanged { get; set; }

            public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
            {
                if (Application.isPlaying)
                {
                    return;
                }

                OnAssetsChanged?.Invoke();
            }
        }

        private static Dictionary<Type, ScriptableObject[]> profileCaches = new Dictionary<Type, ScriptableObject[]>();
        private static Dictionary<Type, GUIContent[]> profileContentCaches = new Dictionary<Type, GUIContent[]>();
        private static Dictionary<Type, Type[]> profileTypesForServiceCaches = new Dictionary<Type, Type[]>();

        /// <summary>
        /// Returns an array of profiles that match profile type.
        /// </summary>
        public static ScriptableObject[] GetProfilesOfType(Type profileType)
        {
            ScriptableObject[] profilesOfType = null;
            if (!profileCaches.TryGetValue(profileType, out profilesOfType))
            {
                profilesOfType = ScriptableObjectExtensions.GetAllInstances(profileType);
                profileCaches.Add(profileType, profilesOfType);
            }
            return profilesOfType;
        }

        /// <summary>
        /// Returns an array of GUIContent for use in a dropdown for a type of profile.
        /// Includes a (None) option at the start. This means that the array length will always be 1 greater than the available profiles.
        /// </summary>
        public static GUIContent[] GetProfilePopupOptionsByType(Type profileType)
        {
            GUIContent[] profileContent = null;
            if (!profileContentCaches.TryGetValue(profileType, out profileContent))
            {
                ScriptableObject[] profilesOfType = GetProfilesOfType(profileType);
                profileContent = new GUIContent[profilesOfType.Length];
                for (int i = 0; i < profilesOfType.Length; i++)
                {
                    profileContent[i] = new GUIContent(profilesOfType[i].name);
                }
                profileContentCaches.Add(profileType, profileContent);
            }
            return profileContent;
        }

        /// <summary>
        /// Returns true if the given profile type is designed to configure the given service.
        /// </summary>
        public static bool IsProfileForService(Type profileType, Type serviceType)
        {
            foreach (MixedRealityServiceProfileAttribute serviceProfileAttribute in profileType.GetCustomAttributes(typeof(MixedRealityServiceProfileAttribute), true))
            {
                bool requirementsMet = true;
                foreach (Type requiredType in serviceProfileAttribute.RequiredTypes)
                {
                    if (!requiredType.IsAssignableFrom(serviceType))
                    {
                        requirementsMet = false;
                        break;
                    }
                }

                if (requirementsMet)
                {
                    foreach (Type excludedType in serviceProfileAttribute.ExcludedTypes)
                    {
                        if (excludedType.IsAssignableFrom(serviceType))
                        {
                            requirementsMet = false;
                            break;
                        }

                    }
                }

                return requirementsMet;
            }
            return false;
        }

        /// <summary>
        /// Returns true if profile is NOT a BaseMixedRealityProfile class type.
        /// </summary>
        public static bool IsConcreteProfileType(Type profileType)
        {
            return profileType != typeof(BaseMixedRealityProfile);
        }

        /// <summary>
        /// Given a service type, finds all sub-classes of BaseMixedRealityProfile that are
        /// designed to configure that service.
        /// </summary>
        public static IReadOnlyCollection<Type> GetProfileTypesForService(Type serviceType)
        {
            if (serviceType == null)
            {
                return Array.Empty<Type>();
            }

            Type[] types;
            if (!profileTypesForServiceCaches.TryGetValue(serviceType, out types))
            {
                HashSet<Type> allTypes = new HashSet<Type>();

#if UNITY_2019_1_OR_NEWER
                foreach (var type in TypeCache.GetTypesDerivedFrom<BaseMixedRealityProfile>())
                {
                    if (IsProfileForService(type, serviceType))
                    {
                        allTypes.Add(type);
                    }
                }
#else
                ScriptableObject[] allProfiles = GetProfilesOfType(typeof(BaseMixedRealityProfile));
                for (int i = 0; i < allProfiles.Length; i++)
                {
                    ScriptableObject profile = allProfiles[i];
                    if (IsProfileForService(profile.GetType(), serviceType))
                    {
                        allTypes.Add(profile.GetType());
                    }
                }
#endif

                types = allTypes.ToArray();
                profileTypesForServiceCaches.Add(serviceType, types);
            }

            return types;
        }

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            AssetImportListener.OnAssetsChanged += AssetImportListener_OnAssetsChange;
        }

        private static void AssetImportListener_OnAssetsChange()
        {
            RefreshProfileCaches();
        }

        private static void RefreshProfileCaches()
        {
            List<Type> cachedTypes = new List<Type>(profileCaches.Keys);
            profileContentCaches.Clear();
            foreach (Type profileType in cachedTypes)
            {
                profileCaches[profileType] = ScriptableObjectExtensions.GetAllInstances(profileType);
            }
        }
    }
}
