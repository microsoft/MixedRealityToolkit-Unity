// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Custom editor for MRTKProfile, enabling the selection
    /// of which subsystem should be loaded at runtime, as well
    /// as the configurations assigned to each subsystem.
    /// </summary>
    [CustomEditor(typeof(MRTKProfile))]
    public class MRTKProfileEditor : UnityEditor.Editor
    {
        /// <summary>
        /// A container class, intended to wrap all information about a subsystem
        /// relevant to this custom editor.
        /// </summary>
        private class SubsystemItem
        {
            /// <summary>
            /// Constructs a <see cref="SubsystemItem"> from a <see cref="Type">.
            /// Will retrieve relevant metadata about the subsystem and populate fields
            /// for use in the inspector.
            /// </summary>
            public SubsystemItem(Type type)
            {
                IMRTKSubsystemDescriptor metadata = XRSubsystemHelpers.RetrieveMetadata(type);

                DisplayName = new GUIContent(metadata.DisplayName);
                Name = new GUIContent(metadata.Name);
                Author = new GUIContent(metadata.Author);
                Reference = SystemType.GetReference(type);
                ConfigType = metadata.ConfigType;
            }

            /// <summary>
            /// The display name of the subsystem.
            /// </summary>
            public readonly GUIContent DisplayName;

            /// <summary>
            /// The id of the subsystem, typically in reverse-dot notation.
            /// </summary>
            public readonly GUIContent Name;

            /// <summary>
            /// The author of the subsystem.
            /// </summary>
            public readonly GUIContent Author;

            /// <summary>
            /// Is this subsystem currently enabled? (i.e., box checked?)
            /// </summary>
            public bool Enabled = false;

            /// <summary>
            /// The serialized type reference to the concrete subsystem type.
            /// Generated with <see cref="SystemType.GetReference(Type type)">
            /// </summary>
            public readonly string Reference;

            /// <summary>
            /// The <see cref="SystemType"> of the least-derived configuration
            /// class that is compatible with this subsystem.
            /// </summary>
            /// <remarks> 
            /// Any configuration that is assignable to this type will be allowed
            /// to be assigned to the subsystem.
            /// </remarks>
            public readonly SystemType ConfigType;
        }

        private static class Styles
        {
            /// <summary>
            /// Default style for MRTK subsystem selection panel
            /// </summary>
            public static readonly GUIStyle SubsystemSelectionPanel = new GUIStyle(EditorStyles.label)
            {
                border = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(0, 0, 20, 0),
                margin = new RectOffset(3, 3, 3, 3)
            };

            /// <summary>
            /// Default style for MRTK subsystem details panel
            /// </summary>
            public static readonly GUIStyle SubsystemDetailsPanel = new GUIStyle(EditorStyles.label)
            {
                border = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(20, 0, 20, 0),
                margin = new RectOffset(3, 3, 3, 3)
            };

            /// <summary>
            /// Default style for MRTK settings subsystem listings.
            /// </summary>
            public static readonly GUIStyle SubsystemListItem = new GUIStyle(EditorStyles.label)
            {
                border = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(12, 12, 12, 12),
                margin = new RectOffset(3, 3, 3, 3)
            };

            /// <summary>
            /// Selected style for MRTK settings subsystem listings.
            /// </summary>
            public static readonly GUIStyle SubsystemListItemSelected = new GUIStyle("TV Selection")
            {
                border = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(12, 12, 12, 12),
                margin = new RectOffset(3, 3, 3, 3)
            };

            /// <summary>
            /// Default width for subsystem listing.
            /// </summary>
            public static readonly float SubsystemListWidth = 200f;

            /// <summary>
            /// Style for the list of subsystems.
            /// </summary>
            public static GUIStyle SubsystemListStyle = new GUIStyle("ScrollViewAlt")
            {
                margin = new RectOffset(0, 0, 12, 0)
            };

            /// <summary>
            /// Style for the details box drawn for each subsystem.
            /// </summary>
            public static GUIStyle SubsystemDetailsBoxStyle = new GUIStyle(SubsystemListStyle)
            {
                border = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(12, 12, 12, 12),
                margin = new RectOffset(3, 3, 12, 3)
            };

            /// <summary>
            /// Style for the name/title of the subsystem in the info box.
            /// </summary>
            public static GUIStyle SubsystemNameStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                border = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(0, 0, 0, 0)
            };

            /// <summary>
            /// Style for the rest of the subsystem info in the info box.
            /// </summary>
            public static GUIStyle SubsystemInfoStyle = new GUIStyle(SubsystemNameStyle)
            {
                fontSize = 10,
                fontStyle = FontStyle.Normal
            };

            /// <summary>
            /// Style for the enabled toggle.
            /// </summary>
            public static GUIStyle ToggleStyle = new GUIStyle(EditorStyles.toggle)
            {
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(12, 12, 0, 12)
            };

            /// <summary>
            /// Empty GUIStyle to prevent button from rendering.
            /// </summary>
            public static GUIStyle ButtonStyle = new GUIStyle();
        }

        // The list of subsystems that are listed as enabled
        // in the underlying profile. Warning; may contain
        // subsystems that your project does not have!
        private SerializedProperty serializedSubsystems;

        // The serialized dictionary of configs, mapping
        // subsystem types to config ScriptableObjects.
        private SerializedProperty serializedConfigs;

        // The serialized audio mixer group that is to be
        // used when playing spatiailized sounds.
        private SerializedProperty serializedMixerGroup;

        // All concrete subsystems in currently loaded assemblies,
        // which are tagged with the MRTKSubsystemAttribute.
        private SortedDictionary<string, List<SubsystemItem>> allSubsystemTypes = new SortedDictionary<string, List<SubsystemItem>>(StringComparer.InvariantCultureIgnoreCase);

        // The currently selected subsystem, which will appear
        // in the right-hand-side panel for editing.
        private SubsystemItem selectedItem = null;

        // The timeSinceStartup when we last refreshed the list of subsystems.
        private double timeLastRefreshed = 0;

        // Interval between automatic refreshes.
        private const double refreshInterval = 5.0;

        // Cached config editor reference
        private UnityEditor.Editor configEditor;

        // A formatted string of missing subsystem names.
        // Populated with the subsystems that are present in the
        // profile, but not loaded in the project.
        private string missingNames = "";

        private void OnEnable()
        {
            serializedSubsystems = serializedObject.FindProperty("loadedSubsystems");
            serializedConfigs = serializedObject.FindProperty("subsystemConfigs");
            serializedMixerGroup = serializedObject.FindProperty(InspectorUIUtility.GetBackingField("SpatializationMixer"));
        }

        public override void OnInspectorGUI()
        {
            if (serializedObject == null || serializedObject.targetObject == null)
            {
                return;
            }

            serializedObject.UpdateIfRequiredOrScript();

            if (EditorApplication.timeSinceStartup - timeLastRefreshed > refreshInterval)
            {
                timeLastRefreshed = EditorApplication.timeSinceStartup;
                GetSubsystems();
                PopulateEnabledSubsystems();
            }

            if (!string.IsNullOrEmpty(missingNames))
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("The currently loaded MRTK Profile has subsystems(s) enabled that are not included in this project:\n"
                                 + missingNames
                                 + ".\nCheck the MR Feature Tool to import missing subsystems.", MessageType.Warning);
            }

            DrawSelectedMixer();

            using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                DrawSubsystemList();
                DrawSubsystemInspector();
            }

            serializedObject.ApplyModifiedProperties();
        }

        #region Drawing methods

        private void DrawSelectedMixer()
        {
            EditorGUILayout.ObjectField(
                serializedMixerGroup,
                typeof(AudioMixerGroup));
        }

        /// <summary>
        /// Draws the display name, author, and full name of the specified subsystem.
        /// </summary>
        private Rect DrawSubsystemDetails(SubsystemItem subsystem)
        {
            Rect rect = EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            {
                GUILayout.Label(subsystem.DisplayName, Styles.SubsystemNameStyle);
                GUILayout.Label(subsystem.Author, Styles.SubsystemInfoStyle);
                GUILayout.Label(subsystem.Name, Styles.SubsystemInfoStyle);
            }
            EditorGUILayout.EndVertical();

            return rect;
        }

        /// <summary>
        /// Draws the list of available subsystems, allowing the user to enable or
        /// disable each subsystem, and to click on them to reveal their individual
        /// configuration setup.
        /// </summary>
        private void DrawSubsystemList()
        {
            using (new EditorGUILayout.VerticalScope(Styles.SubsystemSelectionPanel, GUILayout.Width(Styles.SubsystemListWidth), GUILayout.ExpandWidth(true)))
            {
                EditorGUILayout.LabelField("Available MRTK Subsystems", MRTKEditorStyles.BoldLargeTitleStyle);

                foreach (KeyValuePair<string, List<SubsystemItem>> pair in allSubsystemTypes)
                {
                    using (new EditorGUILayout.VerticalScope(Styles.SubsystemListStyle, GUILayout.ExpandHeight(true)))
                    {
                        EditorGUILayout.LabelField(pair.Key, MRTKEditorStyles.BoldLargeTitleStyle);

                        foreach (SubsystemItem subsystemItem in pair.Value)
                        {
                            GUIStyle selectionStyle = (selectedItem == subsystemItem) ? Styles.SubsystemListItemSelected : Styles.SubsystemListItem;
                            using (new EditorGUILayout.HorizontalScope(selectionStyle, GUILayout.ExpandWidth(true)))
                            {
                                bool newEnabled = GUILayout.Toggle(subsystemItem.Enabled, "", Styles.ToggleStyle);
                                if (newEnabled != subsystemItem.Enabled)
                                {
                                    subsystemItem.Enabled = newEnabled;
                                    ApplyLoadedSubsystemsToProperty();
                                }
                                Rect subsystemDetails = DrawSubsystemDetails(subsystemItem);
                                if (GUI.Button(subsystemDetails, "", Styles.ButtonStyle))
                                {
                                    selectedItem = subsystemItem;
                                }
                                GUILayout.FlexibleSpace();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draws the pane that exposes the configuration slot + configuration data
        /// for the currently selected subsystem <see cref="selectedItem"/>
        /// </summary>
        private void DrawSubsystemInspector()
        {
            using (new EditorGUILayout.VerticalScope(Styles.SubsystemDetailsPanel))
            {
                EditorGUILayout.LabelField("Subsystem Details", MRTKEditorStyles.BoldLargeTitleStyle);

                if (selectedItem == null)
                {
                    return;
                }

                using (new EditorGUILayout.VerticalScope(Styles.SubsystemDetailsBoxStyle))
                {
                    DrawSubsystemDetails(selectedItem);

                    if (selectedItem.ConfigType == null || selectedItem.ConfigType == typeof(BaseSubsystemConfig))
                    {
                        return;
                    }

                    EditorGUILayout.Space();

                    SerializedProperty configProp = GetConfigPropertyForSubsystem(selectedItem);
                    if (configProp != null)
                    {
                        // We need to create an ObjectField that restricts the config type to the "least specific" type
                        // as specified by the configType on the subsystem item. (This is read in from the attribute on
                        // the actual subsystem definition.)
                        configProp.objectReferenceValue = EditorGUILayout.ObjectField("Configuration Asset", configProp.objectReferenceValue, selectedItem.ConfigType.Type, false);

                        CreateCachedEditor(configProp.objectReferenceValue, null, ref configEditor);

                        if (configEditor != null)
                        {
                            configEditor.OnInspectorGUI();
                        }

                        serializedObject.ApplyModifiedProperties();
                    }
                }
            }
        }

        #endregion Drawing methods

        #region Serialized data management + modification

        /// <summary>
        /// Update our local copy of the subsystems from the serialized object.
        /// </summary>
        private void PopulateEnabledSubsystems()
        {
            HashSet<string> enabledClassNames = new HashSet<string>();
            foreach (SerializedProperty serializedSubsystemType in serializedSubsystems)
            {
                string className = serializedSubsystemType.FindPropertyRelative("reference").stringValue;
                enabledClassNames.Add(className);
            }

            foreach (List<SubsystemItem> allSubsystems in allSubsystemTypes.Values)
            {
                foreach (SubsystemItem item in allSubsystems)
                {
                    item.Enabled = enabledClassNames.Contains(item.Reference);
                    enabledClassNames.Remove(item.Reference);
                }
            }

            // All remaining class names are missing from our current project.
            // Format these names and prep them for display in a warning message.
            if (enabledClassNames.Count != 0)
            {
                StringBuilder missingNamesBuilder = new StringBuilder();
                foreach (string className in enabledClassNames)
                {
                    missingNamesBuilder.Append(className.Split(',')[0] + ", ");
                }
                missingNamesBuilder.Remove(missingNamesBuilder.Length - 2, 2);
                missingNames = missingNamesBuilder.ToString();
            }
        }

        /// <summary>
        /// Apply our local copy of the loaded subsystems to the serialized object.
        /// </summary>
        private void ApplyLoadedSubsystemsToProperty()
        {
            // Keep track of which class names are currently serialized, and make sure
            // we only add more or remove ones that have been disabled on our side.
            // If the profile has references to a subsystem we don't have in this project,
            // we want to preserve it in the profile.
            List<string> currentClassNames = new List<string>();
            foreach (SerializedProperty serializedSubsystemType in serializedSubsystems)
            {
                currentClassNames.Add(serializedSubsystemType.FindPropertyRelative("reference").stringValue);
            }

            foreach (List<SubsystemItem> allSubsystems in allSubsystemTypes.Values)
            {
                foreach (SubsystemItem item in allSubsystems)
                {
                    int index = currentClassNames.IndexOf(item.Reference);

                    // We have an enabled subsystem that the serialized profile does not.
                    if (index == -1 && item.Enabled)
                    {
                        // Insert the new subsystem reference.
                        serializedSubsystems.InsertArrayElementAtIndex(0);
                        SerializedProperty itemProperty = serializedSubsystems.GetArrayElementAtIndex(0).FindPropertyRelative("reference");
                        itemProperty.stringValue = item.Reference;
                    }
                    else if (index >= 0 && item.Enabled == false)
                    {
                        // Both we and the serialized profile both have a subsystem
                        // that needs to be disabled.
                        serializedSubsystems.DeleteArrayElementAtIndex(index);
                    }
                }
            }
        }

        /// <summary>
        /// Get the <see cref="SerializedProperty"/> for the configuration object
        /// associated with <paramref name="item"/>
        /// </summary>
        /// <param name="item">
        /// The subsystem for which the configuration property will be retrieved
        /// </param>
        private SerializedProperty GetConfigPropertyForSubsystem(SubsystemItem item)
        {
            Debug.Assert(item != null);

            SerializedProperty keyValArray = serializedConfigs.FindPropertyRelative("entries");

            foreach (SerializedProperty keyVal in keyValArray)
            {
                if (item.Reference == keyVal.FindPropertyRelative("key").FindPropertyRelative("reference").stringValue)
                {
                    return keyVal.FindPropertyRelative("value");
                }
            }

            // No config registered for the specified subsystem
            // Add new blank key/value pair
            keyValArray.InsertArrayElementAtIndex(0);

            // Populate new key with subsystem type.
            keyValArray.GetArrayElementAtIndex(0).FindPropertyRelative("key").FindPropertyRelative("reference").stringValue = item.Reference;

            // Null-out the new dictionary value. (Prevents issues with uninitialized serialized properties)
            SerializedProperty newValue = keyValArray.GetArrayElementAtIndex(0).FindPropertyRelative("value");
            newValue.objectReferenceValue = null;

            serializedObject.ApplyModifiedProperties();

            return keyValArray.GetArrayElementAtIndex(0).FindPropertyRelative("value");
        }

        private void GetSubsystems()
        {
            allSubsystemTypes.Clear();
            TypeCache.TypeCollection subsystemTypes = TypeCache.GetTypesWithAttribute<MRTKSubsystemAttribute>();

            foreach (Type subsystemType in subsystemTypes)
            {
                Type baseSubsystemType = subsystemType.BaseType;
                while (baseSubsystemType != null && baseSubsystemType.BaseType != null
                    && !(baseSubsystemType.BaseType.IsGenericType && baseSubsystemType.BaseType.GetGenericTypeDefinition() == typeof(MRTKSubsystem<,,>)))
                {
                    baseSubsystemType = baseSubsystemType.BaseType;
                }

                if (baseSubsystemType == null)
                {
                    continue;
                }

                string baseSubsystemName = baseSubsystemType.Name;
                if (allSubsystemTypes.ContainsKey(baseSubsystemName))
                {
                    allSubsystemTypes[baseSubsystemName].Add(new SubsystemItem(subsystemType));
                }
                else
                {
                    allSubsystemTypes[baseSubsystemName] = new List<SubsystemItem>() { new SubsystemItem(subsystemType) };
                }
            }
        }

        #endregion Serialized data management + modification
    }
}
