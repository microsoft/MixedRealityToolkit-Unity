// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor.Search
{
    /// <summary>
    /// Utility for searching profile 
    /// </summary>
    public static partial class MixedRealitySearchUtility
    {
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

            // The result that we will return, if not empty
            ProfileSearchResult result = new ProfileSearchResult();

            // Ignore these requirements if criteria is empty
            config.RequireAllKeywords &= config.Keywords.Count > 0;
            config.RequireAllSubjects &= config.SelectedSubjects != 0;
            // Subject requirements are met by the profile's subject tag
            // If not all subjects are required, a profile with the wrong tags or no tags can still match
            int profileMatchStrength = 0;
            bool subjectReqsMet = CheckProfileSubjects(config, profile, out profileMatchStrength);            
            if (subjectReqsMet)
            { 
                result.Profile = profile;
                result.ProfileMatchStrength = profileMatchStrength;
                result.Fields = new List<FieldSearchResult>();
            }

            // Otherwise go through the profile's serialized fields
            SerializedProperty iterator = new SerializedObject(profile).GetIterator();
            bool hasNextProperty = iterator.Next(true);

            while (hasNextProperty)
            {
                if (serializedPropertiesToIgnore.Contains(iterator.name))
                {   // We don't care about this property
                    hasNextProperty = iterator.Next(false);
                    continue;
                }

                // Check to see if this is a profile - if it is, enter the profile and check its children
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
                int fieldMatchStrength = 0;

                if (subjectReqsMet)
                {   // Only check keywords if our subject requirements are met
                    CheckFieldKeywords(config, iterator, ref fieldMatchStrength, ref keywordReqsMet);
                }

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
                                // Check to see if this is a profile - if it is, enter the profile and check its children
                                if (childIterator.propertyType == SerializedPropertyType.ObjectReference && childIterator.objectReferenceValue != null)
                                {   // If the property is itself a profile, perform a recursive search
                                    Type referenceType = childIterator.objectReferenceValue.GetType();
                                    if (typeof(BaseMixedRealityProfile).IsAssignableFrom(referenceType))
                                    {
                                        isProfileField = true;
                                        foreach (ProfileSearchResult subResult in SearchProfileFields(childIterator.objectReferenceValue, config))
                                        {
                                            yield return subResult;
                                        }
                                    }
                                }

                                if (CheckFieldKeywords(config, childIterator, ref fieldMatchStrength, ref keywordReqsMet))
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

                if (subjectReqsMet && keywordReqsMet)
                {
                    int matchStrength = fieldMatchStrength + profileMatchStrength;
                    if (ProfileSearchResult.IsEmpty(result))
                    {
                        result.Profile = profile;
                        result.ProfileMatchStrength = profileMatchStrength;
                        result.Fields = new List<FieldSearchResult>();
                    }

                    result.MaxFieldMatchStrength = Mathf.Max(result.MaxFieldMatchStrength, matchStrength);
                    result.Fields.Add(
                        new FieldSearchResult()
                        {
                            Property = iterator.Copy(),
                            MatchStrength = matchStrength,
                        });
                }

                hasNextProperty = iterator.Next(false);
            }

            if (!ProfileSearchResult.IsEmpty(result))
            {
                // Sort results by match, then by name
                result.Fields.Sort(delegate (FieldSearchResult r1, FieldSearchResult r2)
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

        /// <summary>
        /// Converts a string flags value to a subject tag. Useful for storing in a session state string value.
        /// </summary>
        public static SubjectTag SubjectFlagsFromString(string subjectFlagValueString)
        {
            ulong ulongValue;
            ulong.TryParse(subjectFlagValueString, out ulongValue);
            return (SubjectTag)ulongValue;
        }

        /// <summary>
        /// Converts a subject tag to string flags value. Useful for storing in a session state string value.
        /// </summary>
        public static string SubjectFlagsToString(SubjectTag subjectFlags)
        {
            ulong ulongValue = (ulong)subjectFlags;
            return ulongValue.ToString();
        }

        private static bool CheckProfileSubjects(SearchConfig config, UnityEngine.Object profile, out int subjectMatchStrength)
        {
            subjectMatchStrength = 0;

            if (config.SelectedSubjects == 0)
            {
                // If we haven't selected any subjects then we're done
                return true;
            }

            bool subjectReqsMet = !config.RequireAllSubjects;

            List<SubjectTag> subjectValues = new List<SubjectTag>();
            foreach (SubjectTag value in Enum.GetValues(typeof(SubjectTag)))
            {
                if (value == SubjectTag.All)
                {
                    continue;
                }

                if ((value & config.SelectedSubjects) != 0)
                {
                    subjectValues.Add(value);
                }
            }

            Type profileType = profile.GetType();
            SubjectAttribute subject = profileType.GetCustomAttribute<SubjectAttribute>();
            if (subject != null)
            {
                SubjectTag profileTags = subject.Tags;
                foreach (SubjectTag value in subjectValues)
                {
                    subjectMatchStrength += (value & profileTags) != 0 ? 1 : 0;
                }

                if (subjectValues.Count > 1 && subjectMatchStrength >= subjectValues.Count)
                {   // If we match eevery single subject in a multi-subject search, double the score
                    subjectMatchStrength *= 2;
                }

                subjectReqsMet = config.RequireAllSubjects ? subjectMatchStrength >= subjectValues.Count : subjectMatchStrength > 0;
            }

            return subjectReqsMet;
        }


        private static bool CheckFieldKeywords(SearchConfig config, SerializedProperty property, ref int fieldMatchStrength, ref bool keywordReqsMet)
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