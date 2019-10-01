// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    public static class MixedRealityProfileUtility
    {
        /// <summary>
        /// Struct for storing search results
        /// </summary>
        public struct FieldResult
        {
            public FieldInfo Field;
            public SerializedProperty Property;
            public SubjectAttribute Subject;
            public TooltipAttribute ToolTip;
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

        public static IEnumerable<ProfileSearchResult> SearchProfileFields(UnityEngine.Object profile, HashSet<string> keywords, SubjectTag selectedSubjects, bool requireAllSubjects, bool requireAllKeywords)
        {
            // Ignore these requirements if criteria is empty
            requireAllKeywords &= keywords.Count > 0;
            requireAllSubjects &= selectedSubjects != 0;

            // We will use these to evaluate match strength
            List<SubjectTag> subjectValues = new List<SubjectTag>();
            foreach (SubjectTag value in Enum.GetValues(typeof(SubjectTag)))
            {
                if ((value & selectedSubjects) != 0)
                    subjectValues.Add(value);
            }

            // The result that we will return, if not empty
            ProfileSearchResult result = new ProfileSearchResult();

            // First check the profile for subject matches
            // Profile tags will be combined with field tags
            SubjectTag profileTags;
            int profileMatchStrength;
            if (DoesProfileMatchSubject(profile, selectedSubjects, requireAllSubjects, out profileTags, out profileMatchStrength))
            {
                result.Profile = profile;
                result.ProfileMatchStrength = profileMatchStrength;
                result.Fields = new List<FieldResult>();
            }

            // Now go through the profile's serialized fields
            Type profileType = profile.GetType();
            SerializedProperty iterator = new SerializedObject(profile).GetIterator();
            bool hasNextProperty = iterator.Next(true);

            while (hasNextProperty)
            {
                if (iterator.propertyType == SerializedPropertyType.ObjectReference && iterator.objectReferenceValue != null)
                {   // If the property is itself a profile, perform a recursive search
                    Type referenceType = iterator.objectReferenceValue.GetType();
                    if (typeof(BaseMixedRealityProfile).IsAssignableFrom(referenceType))
                    {
                        foreach (ProfileSearchResult subResult in SearchProfileFields(iterator.objectReferenceValue, keywords, selectedSubjects, requireAllSubjects, requireAllKeywords))
                        {
                            yield return subResult;
                        }
                    }
                }

                if (!serializedPropertiesToIgnore.Contains(iterator.name))
                {
                    bool keywordReqsMet = !requireAllKeywords;
                    bool subjectReqsMet = !requireAllSubjects;
                    string fieldName = iterator.name.ToLower();
                    int fieldMatchStrength = 0;
                    int subjectMatchStrength = 0;

                    FieldInfo fieldInfo = profileType.GetField(iterator.name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    SubjectAttribute subject = fieldInfo.GetCustomAttribute<SubjectAttribute>();
                    TooltipAttribute toolTip = fieldInfo.GetCustomAttribute<TooltipAttribute>();

                    if (keywords.Count > 0)
                    {
                        // Search by string, then limit if tags are being used
                        foreach (string keyword in keywords)
                        {
                            if (fieldName.Contains(keyword) || toolTip != null && toolTip.tooltip.Contains(keyword))
                            {
                                fieldMatchStrength++;
                            }
                        }

                        if (keywords.Count > 1 && fieldMatchStrength == keywords.Count)
                        {   // If we match multiple keywords in a multi-keyword search, double the score
                            fieldMatchStrength *= 2;
                        }

                        keywordReqsMet = requireAllKeywords ? fieldMatchStrength >= keywords.Count : fieldMatchStrength > 0;
                    }

                    if (subject != null && selectedSubjects != 0)
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

                        subjectReqsMet = requireAllSubjects ? subjectMatchStrength >= subjectValues.Count : subjectMatchStrength > 0;
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
                                ToolTip = toolTip,
                                MatchStrength = matchStrength,
                            });
                    }
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

        private static bool DoesProfileMatchSubject(UnityEngine.Object profile, SubjectTag selectedSubjects, bool requireAllSubjects, out SubjectTag profileTags, out int subjectMatchStrength)
        {
            // We will use these to evaluate match strength
            List<SubjectTag> subjectValues = new List<SubjectTag>();
            foreach (SubjectTag value in Enum.GetValues(typeof(SubjectTag)))
            {
                if ((value & selectedSubjects) != 0)
                    subjectValues.Add(value);
            }

            profileTags = 0;
            subjectMatchStrength = 0;
            bool include = false;

            if (selectedSubjects != 0)
            {
                Type profileType = profile.GetType();
                SubjectAttribute subject = profileType.GetCustomAttribute<SubjectAttribute>();
                if (subject != null)
                {
                    profileTags = subject.Tags;

                    foreach (SubjectTag value in subjectValues)
                    {
                        subjectMatchStrength += (value & profileTags) != 0 ? 1 : 0;
                    }

                    if (subjectValues.Count > 1 && subjectMatchStrength >= subjectValues.Count)
                    {   // If we match eevery single subject in a multi-subject search, double the score
                        subjectMatchStrength *= 2;
                    }

                    include = requireAllSubjects ? subjectMatchStrength >= subjectValues.Count : subjectMatchStrength > 0;
                }
            }

            return include;
        }
    }
}