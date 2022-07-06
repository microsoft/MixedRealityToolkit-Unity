// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(StatefulInteractable), true)]
    [CanEditMultipleObjects]
    public class StatefulInteractableInspector : BaseInteractableEditor
    {
        private SerializedProperty isToggled;
        private SerializedProperty isToggledStateActive;
        private SerializedProperty selectThreshold;
        private SerializedProperty deselectThreshold;
        private SerializedProperty toggleMode;
        private SerializedProperty triggerOnRelease;
        private SerializedProperty allowSelectByVoice;
        private SerializedProperty speechRecognitionKeyword;
        private SerializedProperty voiceRequiresFocus;
        private SerializedProperty useGazeDwell;
        private SerializedProperty gazeDwellTime;
        private SerializedProperty useFarDwell;
        private SerializedProperty farDwellTime;
        private SerializedProperty onClicked;
        private SerializedProperty onToggled;
        private SerializedProperty onDetoggled;
        private SerializedProperty onEnabled;
        private SerializedProperty onDisabled;
        private static bool advancedFoldout = false;
        private static bool enabledEventsFoldout = false;

        protected override void OnEnable()
        {
            base.OnEnable();

            isToggled = SetUpProperty(nameof(isToggled));
            isToggledStateActive = isToggled.FindPropertyRelative("active");

            selectThreshold = SetUpProperty(nameof(selectThreshold));
            deselectThreshold = SetUpProperty(nameof(deselectThreshold));

            toggleMode = SetUpProperty(nameof(toggleMode));
            triggerOnRelease = SetUpProperty(nameof(triggerOnRelease));

            allowSelectByVoice = SetUpProperty(nameof(allowSelectByVoice));
            speechRecognitionKeyword = SetUpProperty(nameof(speechRecognitionKeyword));
            voiceRequiresFocus = SetUpProperty(nameof(voiceRequiresFocus));

            useGazeDwell = SetUpProperty(nameof(useGazeDwell));
            gazeDwellTime = SetUpProperty(nameof(gazeDwellTime));
            useFarDwell = SetUpProperty(nameof(useFarDwell));
            farDwellTime = SetUpProperty(nameof(farDwellTime));

            onClicked = SetUpProperty(nameof(onClicked));

            onEnabled = SetUpProperty(nameof(onEnabled));
            onDisabled = SetUpProperty(nameof(onDisabled));

            // OnToggle/Detoggle aliases to IsToggled.OnEntered/IsToggled.OnExited
            onToggled = isToggled.FindPropertyRelative("onEntered");
            onDetoggled = isToggled.FindPropertyRelative("onExited");
        }

        protected override void DrawProperties()
        {
            DrawProperties(true);
        }

        /// <summary>
        /// Overload to <see cref="DrawProperties"/> to allow subclasses to specify whether they'd like
        /// to show toggle-related properties. Some subclasses hide this,
        /// as showing toggle settings wouldn't make much sense for their use case.
        /// </summary>
        protected void DrawProperties(bool showToggleMode)
        {
            EditorGUILayout.Space();

            StatefulInteractable interactable = target as StatefulInteractable;

            bool interactableActive = EditorGUILayout.Toggle(new GUIContent("Is Interactable", "Convenience alias for StatefulInteractable.enabled"), interactable.enabled);

            if (interactableActive != (target as StatefulInteractable).enabled)
            {
                Undo.RecordObject(target, string.Concat("Set Interactable ", target.name));
                interactable.enabled = interactableActive;
            }

            // Only show toggle settings if the subclass hasn't told us not to.
            // Some subclasses can choose to hide this section, as it won't be relevant.
            if (showToggleMode)
            {
                EditorGUILayout.PropertyField(toggleMode, new GUIContent("Selection Mode", "Does this interactable fire toggle events, or does it only act like a button?"));

                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();

                if ((StatefulInteractable.ToggleType)toggleMode.intValue != StatefulInteractable.ToggleType.Button)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(isToggledStateActive, new GUIContent("Is Toggled", "Directly set the internal IsToggled state at edit-time"));
                        if (EditorGUI.EndChangeCheck())
                        {
                            // Actually set the toggle state with the public setter so that events fire.
                            interactable.ForceSetToggled(isToggledStateActive.boolValue);
                        }
                    }
                }

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(onClicked);

                if ((StatefulInteractable.ToggleType)toggleMode.intValue != StatefulInteractable.ToggleType.Button)
                {
                    EditorGUILayout.PropertyField(onToggled, new GUIContent("On Toggled", "Fired when the toggle state has changed from false to true."));
                    EditorGUILayout.PropertyField(onDetoggled, new GUIContent("On Detoggled", "Fired when the toggle state has changed from true to false."));
                }
            }

            base.DrawProperties();

            advancedFoldout = EditorGUILayout.Foldout(advancedFoldout, EditorGUIUtility.TrTempContent("Advanced StatefulInteractable Settings"), true, EditorStyles.foldoutHeader);
            if (advancedFoldout)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(selectThreshold);
                    EditorGUILayout.PropertyField(deselectThreshold);

                    EditorGUILayout.PropertyField(useGazeDwell);
                    if (useGazeDwell.boolValue)
                    {
                        using (new EditorGUI.IndentLevelScope())
                        {
                            EditorGUILayout.PropertyField(gazeDwellTime);
                        }
                    }

                    EditorGUILayout.PropertyField(useFarDwell);
                    if (useFarDwell.boolValue)
                    {
                        {
                            EditorGUILayout.PropertyField(farDwellTime);
                        }
                    }

                    EditorGUILayout.PropertyField(allowSelectByVoice);
                    if (allowSelectByVoice.boolValue)
                    {
                        using (new EditorGUI.IndentLevelScope())
                        {
                            EditorGUILayout.PropertyField(speechRecognitionKeyword);
                            EditorGUILayout.PropertyField(voiceRequiresFocus);
                        }
                    }

                    EditorGUILayout.PropertyField(triggerOnRelease);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        protected override void DrawMRTKInteractableFlags()
        {
            Color previousGUIColor = GUI.color;
            StatefulInteractable interactable = target as StatefulInteractable;

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("StatefulInteractable Events", EditorStyles.boldLabel);
                EditorGUILayout.Space();

                DrawTimedFlag(isToggled, interactable.IsToggled, previousGUIColor, Color.cyan);
                
                enabledEventsFoldout = EditorGUILayout.Foldout(enabledEventsFoldout, "OnEnable/Disable", true);
                
                if (enabledEventsFoldout)
                {
                    EditorGUILayout.PropertyField(onEnabled);
                    EditorGUILayout.PropertyField(onDisabled);
                }
            }

            EditorGUILayout.Space();
            base.DrawMRTKInteractableFlags();
        }
    }
}
