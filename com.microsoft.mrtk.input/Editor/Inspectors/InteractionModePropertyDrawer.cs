// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input.Editor
{
    /// <summary>
    /// A custom drawer used when rendering information about a <see cref="Microsoft.MixedReality.Toolkit.Input.InteractionMode">InteractionMode</see>
    /// property within a Unity inspector window.
    /// </summary>
    [CustomPropertyDrawer(typeof(InteractionMode))]
    public class InteractionModePropertyDrawer : PropertyDrawer
    {
        private static List<InteractionModeDefinition> profile = null;
        private static GUIContent[] actionLabels = new[] { new GUIContent("Missing Interaction Modes") };
        private static int[] actionPriorities = { 0 };

        private const string NoManagerMessage = "No InteractionModeManager found.\nThis is expected in the prefab editor.\nOtherwise, add one to the scene.";
        private const float MessageRectHeight = 42f;

        private static float MessageRectOffset => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent content)
        {
            InteractionModeManager instance = InteractionModeManager.Instance;
            profile = instance != null ? instance.PrioritizedInteractionModes : null;

            if (profile == null || profile.Count == 0)
            {
                actionLabels = new[] { new GUIContent("Missing Interaction Modes") };
                actionPriorities = new[] { 0 };

                GUIContent label = EditorGUI.BeginProperty(rect, content, property);
                // Reset this rect to only be a single line. A second rect is created below for the message box in the remaining space
                rect.height = EditorGUIUtility.singleLineHeight;
                // We don't want to overwrite the id property here, since we're temporarily displaying the "missing" message
                _ = EditorGUI.IntPopup(rect, label, 0, actionLabels, actionPriorities);
                EditorGUI.EndProperty();

                EditorGUI.indentLevel += 1;
                Rect messageRect = EditorGUI.IndentedRect(rect);
                messageRect.y += MessageRectOffset;
                messageRect.height = MessageRectHeight;
                EditorGUI.HelpBox(messageRect, NoManagerMessage, MessageType.Info);
            }
            // We are guaranteed to have a valid profile and list of interaction modes after this point;
            else
            {
                var label = EditorGUI.BeginProperty(rect, content, property);
                var modeName = property.FindPropertyRelative("name");
                var modePriority = property.FindPropertyRelative("priority");

                if (actionLabels.Length != profile.Count)
                {
                    actionLabels = new GUIContent[profile.Count];
                    actionPriorities = new int[profile.Count];
                }

                int initialModeIndex = 0;
                for (int i = 0; i < profile.Count; i++)
                {
                    InteractionModeDefinition definition = profile[i];
                    actionLabels[i] = new GUIContent(definition.ModeName);
                    actionPriorities[i] = i;

                    if (definition.ModeName == modeName.stringValue)
                    {
                        initialModeIndex = i;
                    }
                }

                modePriority.intValue = EditorGUI.IntPopup(rect, label, initialModeIndex, actionLabels, actionPriorities);
                modeName.stringValue = actionLabels[modePriority.intValue].text;

                EditorGUI.EndProperty();
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return InteractionModeManager.Instance != null ?
                base.GetPropertyHeight(property, label)
                : MessageRectOffset + MessageRectHeight;
        }
    }
}
