using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    public static class MixedRealityProfileUtility
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

        public struct ProfileSearchResult
        {
            public static bool IsEmpty(ProfileSearchResult result)
            {
                return result.profile == null || result.properties == null || result.properties.Count == 0;
            }

            public UnityEngine.Object profile;
            public List<SerializedProperty> properties;
        }

        public static IEnumerable<ProfileSearchResult> SearchProfileFields(UnityEngine.Object profile, HashSet<string> searchFieldStrings, SubjectTag selectedTags)
        {
            if (searchFieldStrings.Count == 0 && selectedTags == 0)
                yield break;

            SerializedProperty iterator = new SerializedObject(profile).GetIterator();
            bool hasNextProperty = iterator.Next(true);
            ProfileSearchResult result = new ProfileSearchResult();

            while (hasNextProperty)
            {
                // If the property is itself a profile, don't include it - just perform a recursive search
                if (iterator.propertyType == SerializedPropertyType.ObjectReference && iterator.objectReferenceValue != null)
                {
                    Type referenceType = iterator.objectReferenceValue.GetType();
                    if (typeof(BaseMixedRealityProfile).IsAssignableFrom(referenceType))
                    {
                        foreach (ProfileSearchResult subResult in SearchProfileFields(iterator.objectReferenceValue, searchFieldStrings, selectedTags))
                        {
                            yield return subResult;
                        }
                    }
                }

                if (!serializedPropertiesToIgnore.Contains(iterator.name))
                {
                    bool included = false;
                    string fieldName = iterator.name.ToLower();
                    foreach (string searchFieldString in searchFieldStrings)
                    {
                        if (fieldName.Contains(searchFieldString))
                        {
                            if (ProfileSearchResult.IsEmpty(result))
                            {
                                result.profile = profile;
                                result.properties = new List<SerializedProperty>();
                            }
                            result.properties.Add(iterator.Copy());
                            included = true;
                        }
                    }

                    if (!included && selectedTags != 0)
                    {   // See if this field has a subject tag
                        Type type = profile.GetType();
                        FieldInfo fieldInfo = type.GetField(iterator.name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        if (fieldInfo != null)
                        {
                            SubjectAttribute subjectAttribute = fieldInfo.GetCustomAttribute<SubjectAttribute>();
                            if (subjectAttribute != null && ((subjectAttribute.Tags & selectedTags) != 0))
                            {
                                if (ProfileSearchResult.IsEmpty(result))
                                {
                                    result.profile = profile;
                                    result.properties = new List<SerializedProperty>();
                                }
                                result.properties.Add(iterator.Copy());
                            }
                        }
                    }
                }

                hasNextProperty = iterator.Next(false);
            }

            if (!ProfileSearchResult.IsEmpty(result))
                yield return result;

            yield break;
        }
    }
}