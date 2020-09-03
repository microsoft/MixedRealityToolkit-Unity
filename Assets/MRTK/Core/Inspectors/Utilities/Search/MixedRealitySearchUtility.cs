// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.SceneSystem;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor.Search
{
    /// <summary>
    /// Utility for retrieving a Unity object's serialized fields with a configurable search.
    /// </summary>
    public static class MixedRealitySearchUtility
    {
        /// <summary>
        /// True if a search is being executed. This must be false before calling StartProfileSearch.
        /// </summary>
        public static bool Searching { get { return activeTask != null && !activeTask.IsCompleted; } }

        private const int maxChildSearchDepth = 5;
        private const int minSearchStringLength = 3;
        private static Task activeTask;

        /// <summary>
        /// Field names that shouldn't be displayed in a profile field search.
        /// </summary>
        private static readonly HashSet<string> serializedPropertiesToIgnore = new HashSet<string>()
        {
            // Unity base class fields
            "m_Name",
            "m_Script",
            "m_Enabled",
            "m_GameObject",
            "m_ObjectHideFlags",
            "m_CorrespondingSourceObject",
            "m_PrefabInstance",
            "m_PrefabAsset",
            "m_EditorHideFlags",
            "m_EditorClassIdentifier",
            // Profile base class fields
            "isCustomProfile",
        };

        /// <summary>
        /// Field types that don't need their child properties displayed.
        /// </summary>
        private static readonly HashSet<string> serializedPropertyTypesToFlatten = new HashSet<string>()
        {
            typeof(SystemType).Name,
            typeof(SceneInfo).Name,
        };

        /// <summary>
        /// Starts a profile search. 'Searching' must be false or an exception will be thrown.
        /// </summary>
        /// <param name="profile">Profile object to search.</param>
        /// <param name="config">Configuration for the search.</param>
        /// <param name="onSearchComplete">Action to invoke once search is complete - delivers final results.</param>
        public static async void StartProfileSearch(UnityEngine.Object profile, SearchConfig config, Action<bool, UnityEngine.Object, IReadOnlyCollection<ProfileSearchResult>> onSearchComplete)
        {
            if (activeTask != null && !activeTask.IsCompleted)
            {
                throw new Exception("Can't start a new search until the old one has completed.");
            }

            List<ProfileSearchResult> searchResults = new List<ProfileSearchResult>();

            // Validate search configuration
            if (string.IsNullOrEmpty(config.SearchFieldString))
            {   // If the config is empty, bail early
                onSearchComplete?.Invoke(true, profile, searchResults);
                return;
            }

            // Generate keywords if we haven't yet
            if (config.Keywords == null)
            {
                config.Keywords = new HashSet<string>(config.SearchFieldString.Split(new string[] { " ", "," }, StringSplitOptions.RemoveEmptyEntries));
                config.Keywords.RemoveWhere(s => s.Length < minSearchStringLength);
            }

            if (config.Keywords.Count == 0)
            {   // If there are no useful keywords, bail early
                onSearchComplete?.Invoke(true, profile, searchResults);
                return;
            }

            // Launch the search task
            bool cancelled = false;
            try
            {
                activeTask = SearchProfileField(profile, config, searchResults);
                await activeTask;
            }
            catch (Exception e)
            {
                // Profile was probably deleted in the middle of searching.
                Debug.LogException(e);
                cancelled = true;
            }
            finally
            {
                searchResults.Sort(delegate (ProfileSearchResult r1, ProfileSearchResult r2)
                {
                    if (r1.ProfileMatchStrength != r2.ProfileMatchStrength)
                    {
                        return r2.ProfileMatchStrength.CompareTo(r1.ProfileMatchStrength);
                    }
                    else
                    {
                        return r2.Profile.name.CompareTo(r1.Profile.name);
                    }
                });

                searchResults.RemoveAll(r => r.Fields.Count <= 0);

                onSearchComplete?.Invoke(cancelled, profile, searchResults);
            }
        }

        private static async Task SearchProfileField(UnityEngine.Object profile, SearchConfig config, List<ProfileSearchResult> searchResults)
        {
            await Task.Yield();

            // The result that we will return, if not empty
            ProfileSearchResult result = new ProfileSearchResult();
            result.Profile = profile;
            BaseMixedRealityProfile baseProfile = (profile as BaseMixedRealityProfile);
            result.IsCustomProfile = (baseProfile != null) ? baseProfile.IsCustomProfile : false;
            searchResults.Add(result);

            // Go through the profile's serialized fields
            foreach (SerializedProperty property in GatherProperties(profile))
            {
                if (CheckFieldForProfile(property))
                {
                    await SearchProfileField(property.objectReferenceValue, config, searchResults);
                }
                else
                {
                    CheckFieldForKeywords(property, config, result);
                }
            }

            if (result.Fields.Count > 0)
            {
                result.Fields.Sort(delegate (FieldSearchResult r1, FieldSearchResult r2)
                {
                    if (r1.MatchStrength != r2.MatchStrength)
                    {
                        return r2.MatchStrength.CompareTo(r1.MatchStrength);
                    }
                    return r2.Property.name.CompareTo(r1.Property.name);
                });
            }
        }

        private static bool CheckFieldForProfile(SerializedProperty property)
        {
            bool isProfileField = false;
            if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue != null)
            {
                Type referenceType = property.objectReferenceValue.GetType();
                isProfileField = (typeof(BaseMixedRealityProfile).IsAssignableFrom(referenceType));
            }
            return isProfileField;
        }

        private static IEnumerable<SerializedProperty> GatherProperties(UnityEngine.Object profile)
        {
            List<SerializedProperty> properties = new List<SerializedProperty>();
            SerializedProperty iterator = new SerializedObject(profile).GetIterator();
            bool hasNextProperty = iterator.Next(true);
            while (hasNextProperty)
            {
                if (!serializedPropertiesToIgnore.Contains(iterator.name) && iterator.depth < maxChildSearchDepth)
                {
                    properties.Add(iterator.Copy());
                }

                if (serializedPropertyTypesToFlatten.Contains(iterator.type))
                {
                    hasNextProperty = iterator.Next(false);
                }
                else
                {
                    hasNextProperty = iterator.Next(true);
                }
            }
            return properties;
        }

        private static void CheckFieldForKeywords(SerializedProperty property, SearchConfig config, ProfileSearchResult result)
        {
            int numMatchedKeywords = 0;
            int numExactMatches = 0;
            int numFieldMatches = 0;
            int numTooltipMatches = 0;
            int numContentMatches = 0;
            string propertyName = property.name.ToLower();
            string toolTip = property.tooltip.ToLower();

            foreach (string keyword in config.Keywords)
            {
                bool keywordMatch = false;

                if (propertyName.Contains(keyword))
                {
                    keywordMatch = true;
                    numFieldMatches++;
                    if (propertyName == keyword)
                    {
                        numExactMatches++;
                    }
                }

                if (config.SearchTooltips)
                {
                    if (toolTip.Contains(keyword))
                    {
                        keywordMatch = true;
                        numTooltipMatches++;
                    }
                }

                if (config.SearchFieldContent)
                {
                    switch (property.propertyType)
                    {
                        case SerializedPropertyType.ObjectReference:
                            if (property.objectReferenceValue != null && property.objectReferenceValue.name.ToLower().Contains(keyword))
                            {
                                keywordMatch = true;
                                numContentMatches++;
                            }
                            break;

                        case SerializedPropertyType.String:
                            if (!string.IsNullOrEmpty(property.stringValue) && property.stringValue.ToLower().Contains(keyword))
                            {
                                keywordMatch = true;
                                numContentMatches++;
                            }
                            break;
                    }
                }

                if (keywordMatch)
                {
                    numMatchedKeywords++;
                }
            }

            bool requirementsMet = numMatchedKeywords > 0;
            if (config.RequireAllKeywords && config.Keywords.Count > 1)
            {
                requirementsMet &= numMatchedKeywords >= config.Keywords.Count;
            }

            if (requirementsMet)
            {
                int matchStrength = numMatchedKeywords + numExactMatches;
                if (numMatchedKeywords >= config.Keywords.Count)
                {   // If we match all keywords in a multi-keyword search, double the score
                    matchStrength *= 2;
                }

                // Weight the score based on match type
                matchStrength += numFieldMatches * 3;
                matchStrength += numTooltipMatches * 2;
                matchStrength += numContentMatches * 1;

                result.ProfileMatchStrength += matchStrength;
                result.Fields.Add(new FieldSearchResult()
                {
                    Property = property.Copy(),
                    MatchStrength = numMatchedKeywords,
                });
            }
        }
    }
}