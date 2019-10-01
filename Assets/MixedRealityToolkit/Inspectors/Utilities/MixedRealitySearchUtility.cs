// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor.Search
{
    public static class MixedRealitySearchUtility
    {
        /// <summary>
        /// Struct for configuring a search.
        /// </summary>
        public struct SearchConfig
        {
            public SubjectTag SelectedSubjects;
            public string SearchFieldString;
            public bool RequireAllKeywords;
            public bool RequireAllSubjects;
            public bool SearchTooltips;
            public bool SearchFieldObjectNames;
            public bool SearchChildProperties;
            public HashSet<string> Keywords;
        }

        /// <summary>
        /// Struct for storing search results
        /// </summary>
        public struct FieldResult
        {
            public FieldInfo Field;
            public SerializedProperty Property;
            public SubjectAttribute Subject;
            public int MatchStrength;
        }

        /// <summary>
        /// Struct for pairing profiles with a set of search results
        /// </summary>
        public struct ProfileSearchResult
        {
            public static bool IsEmpty(ProfileSearchResult result)
            {
                return result.Profile == null || result.Fields == null;
            }

            public int ProfileMatchStrength;
            public int MaxFieldMatchStrength;
            public UnityEngine.Object Profile;
            public List<FieldResult> Fields;
        }

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
        /// Field types that don't need their child properties displayed
        /// </summary>
        private static readonly HashSet<string> serializedPropertyTypesToFlatten = new HashSet<string>()
        {
            typeof(SystemType).Name,
        };

        private const int minSearchStringLength = 3;

        /// <summary>
        /// Searches all profiles and sub profiles for matching fields. Returns results in unranked order.
        /// </summary>
        /// <param name="profile">The configuration profile you're searching.</param>
        /// <param name="config">Configuration for search.</param>
        public static IEnumerable<ProfileSearchResult> SearchProfileFields(UnityEngine.Object profile, SearchConfig config)
        {
            if (string.IsNullOrEmpty(config.SearchFieldString) && config.SelectedSubjects == 0)
            {   // If the config is empty, bail early
                yield break;
            }

            // Generate keywords if we haven't yet
            if (config.Keywords == null)
            {
                config.Keywords = new HashSet<string>(config.SearchFieldString.Split(new string[] { " ", "," }, StringSplitOptions.RemoveEmptyEntries));
                config.Keywords.RemoveWhere(s => s.Length < minSearchStringLength);
            }

            // Ignore these requirements if criteria is empty
            config.RequireAllKeywords &= config.Keywords.Count > 0;
            config.RequireAllSubjects &= config.SelectedSubjects != 0;

            // We will use these to evaluate match strength
            List<SubjectTag> subjectValues = new List<SubjectTag>();
            foreach (SubjectTag value in Enum.GetValues(typeof(SubjectTag)))
            {
                if ((value & config.SelectedSubjects) != 0)
                    subjectValues.Add(value);
            }

            // The result that we will return, if not empty
            ProfileSearchResult result = new ProfileSearchResult();

            // First check the profile for subject matches
            // Profile tags will be combined with field tags
            Type profileType = profile.GetType();
            SubjectTag profileTags = 0;

            if (config.SelectedSubjects != 0)
            {
                SubjectAttribute subject = profileType.GetCustomAttribute<SubjectAttribute>();
                if (subject != null)
                {
                    profileTags = subject.Tags;
                }
            }

            // Now go through the profile's serialized fields
            SerializedProperty iterator = new SerializedObject(profile).GetIterator();
            bool hasNextProperty = iterator.Next(true);

            while (hasNextProperty)
            {
                if (serializedPropertiesToIgnore.Contains(iterator.name))
                {   // We don't care about this property
                    hasNextProperty = iterator.Next(false);
                    continue;
                }

                bool isProfileField = false;
                if (iterator.propertyType == SerializedPropertyType.ObjectReference && iterator.objectReferenceValue != null)
                {   // If the property is itself a profile, perform a recursive search
                    Type referenceType = iterator.objectReferenceValue.GetType();
                    if (typeof(BaseMixedRealityProfile).IsAssignableFrom(referenceType))
                    {
                        isProfileField = true;
                        foreach (ProfileSearchResult subResult in SearchProfileFields(iterator.objectReferenceValue, config))
                        {
                            yield return subResult;
                        }
                    }
                }

                // Gather our top-level info
                bool keywordReqsMet = !config.RequireAllKeywords;
                bool subjectReqsMet = !config.RequireAllSubjects;
                int fieldMatchStrength = 0;
                int subjectMatchStrength = 0;

                FieldInfo fieldInfo = profileType.GetField(iterator.name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                SubjectAttribute subject = fieldInfo.GetCustomAttribute<SubjectAttribute>();

                CheckKeywords(config, iterator, ref fieldMatchStrength, ref keywordReqsMet);
                CheckSubjects(config, subjectValues, profileTags, subject, iterator, ref subjectMatchStrength, ref subjectReqsMet);

                if (config.SearchChildProperties)
                {
                    // If this property has children, we want to search those children for relevant results
                    // But we don't want to exit our current level
                    if (!isProfileField && iterator.hasChildren && (iterator.isArray || iterator.propertyType == SerializedPropertyType.Generic))
                    {
                        if (!serializedPropertyTypesToFlatten.Contains(iterator.type))
                        {
                            SerializedProperty childIterator = iterator.Copy();
                            bool hasChildProperties = childIterator.NextVisible(true);
                            int depthOnEnter = childIterator.depth;
                            while (hasChildProperties)
                            {
                                if (CheckKeywords(config, childIterator, ref fieldMatchStrength, ref keywordReqsMet))
                                {
                                    childIterator.isExpanded = true;
                                }
                                if (childIterator.depth < depthOnEnter)
                                {
                                    break;
                                }
                                hasChildProperties = childIterator.Next(true);
                            }
                        }
                    }
                }

                if (keywordReqsMet & subjectReqsMet)
                {
                    int matchStrength = fieldMatchStrength + subjectMatchStrength;
                    if (ProfileSearchResult.IsEmpty(result))
                    {
                        result.Profile = profile;
                        result.Fields = new List<FieldResult>();
                    }

                    result.MaxFieldMatchStrength = Mathf.Max(result.MaxFieldMatchStrength, matchStrength);
                    result.Fields.Add(
                        new FieldResult()
                        {
                            Property = iterator.Copy(),
                            Field = fieldInfo,
                            Subject = subject,
                            MatchStrength = matchStrength,
                        });
                }

                hasNextProperty = iterator.Next(false);
            }

            if (!ProfileSearchResult.IsEmpty(result))
            {
                // Sort results by match, then by name
                result.Fields.Sort(delegate (FieldResult r1, FieldResult r2)
                {
                    if (r1.MatchStrength != r2.MatchStrength)
                    {
                        return r2.MatchStrength.CompareTo(r1.MatchStrength);
                    }
                    return r2.Property.name.CompareTo(r1.Property.name);
                });
                yield return result;
            }

            yield break;
        }

        private static void CheckSubjects(SearchConfig config, List<SubjectTag> subjectValues, SubjectTag profileTags, SubjectAttribute subject, SerializedProperty iterator, ref int subjectMatchStrength, ref bool subjectReqsMet)
        {
            if (subject != null && config.SelectedSubjects != 0)
            {
                // Combine the subject tags with the profile's tags
                SubjectTag fieldTags = profileTags | subject.Tags;
                foreach (SubjectTag value in subjectValues)
                {
                    subjectMatchStrength += (value & fieldTags) != 0 ? 1 : 0;
                }

                if (subjectValues.Count > 1 && subjectMatchStrength >= subjectValues.Count)
                {   // If we match eevery single subject in a multi-subject search, double the score
                    subjectMatchStrength *= 2;
                }

                subjectReqsMet = config.RequireAllSubjects ? subjectMatchStrength >= subjectValues.Count : subjectMatchStrength > 0;
            }
        }

        private static bool CheckKeywords(SearchConfig config, SerializedProperty property, ref int fieldMatchStrength, ref bool keywordReqsMet)
        {
            bool increasedMatchStrength = false;

            if (config.Keywords.Count > 0)
            {
                // Search by string, then limit if tags are being used
                foreach (string keyword in config.Keywords)
                {
                    bool matches = property.name.ToLower().Contains(keyword);

                    if (config.SearchTooltips)
                    {
                        matches |= property.tooltip.ToLower().Contains(keyword);
                    }

                    if (config.SearchFieldObjectNames)
                    {
                        matches |= (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue != null && property.objectReferenceValue.name.ToLower().Contains(keyword));
                    }

                    if (matches)
                    {
                        increasedMatchStrength = true;
                        fieldMatchStrength++;
                    }
                }

                if (config.Keywords.Count > 1 && fieldMatchStrength == config.Keywords.Count)
                {   // If we match multiple keywords in a multi-keyword search, increase the score
                    fieldMatchStrength *= 2;
                }

                keywordReqsMet = config.RequireAllKeywords ? fieldMatchStrength >= config.Keywords.Count : fieldMatchStrength > 0;
            }

            return increasedMatchStrength;
        }
    }
}