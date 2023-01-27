// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(StatefulInteractable), true)]
    [CanEditMultipleObjects]
    public class StatefulInteractableEditor : BaseInteractableEditor
    {
        private SerializedProperty IsToggled;
        private SerializedProperty IsToggledStateActive;
        private SerializedProperty SelectThreshold;
        private SerializedProperty DeselectThreshold;
        private SerializedProperty ToggleMode;
        private SerializedProperty TriggerOnRelease;
        private SerializedProperty allowSelectByVoice;
        private SerializedProperty SelectRequiresHover;
        private SerializedProperty speechRecognitionKeyword;
        private SerializedProperty VoiceRequiresFocus;
        private SerializedProperty UseGazeDwell;
        private SerializedProperty GazeDwellTime;
        private SerializedProperty UseFarDwell;
        private SerializedProperty FarDwellTime;
        private SerializedProperty OnClicked;
        private SerializedProperty OnToggled;
        private SerializedProperty OnDetoggled;
        private SerializedProperty OnEnabled;
        private SerializedProperty OnDisabled;
        private static bool advancedFoldout = false;
        private static bool enabledEventsFoldout = false;

        protected override void OnEnable()
        {
            base.OnEnable();

            IsToggled = SetUpAutoProp(nameof(IsToggled));
            IsToggledStateActive = IsToggled.FindPropertyRelative("active");

            SelectThreshold = SetUpAutoProp(nameof(SelectThreshold));
            DeselectThreshold = SetUpAutoProp(nameof(DeselectThreshold));

            ToggleMode = SetUpAutoProp(nameof(ToggleMode));
            TriggerOnRelease = SetUpAutoProp(nameof(TriggerOnRelease));

            allowSelectByVoice = SetUpProperty(nameof(allowSelectByVoice));
            speechRecognitionKeyword = SetUpProperty(nameof(speechRecognitionKeyword));
            VoiceRequiresFocus = SetUpAutoProp(nameof(VoiceRequiresFocus));

            SelectRequiresHover = SetUpAutoProp(nameof(SelectRequiresHover));

            UseGazeDwell = SetUpAutoProp(nameof(UseGazeDwell));
            GazeDwellTime = SetUpAutoProp(nameof(GazeDwellTime));
            UseFarDwell = SetUpAutoProp(nameof(UseFarDwell));
            FarDwellTime = SetUpAutoProp(nameof(FarDwellTime));

            OnClicked = SetUpAutoProp(nameof(OnClicked));

            OnEnabled = SetUpAutoProp(nameof(OnEnabled));
            OnDisabled = SetUpAutoProp(nameof(OnDisabled));

            // OnToggle/Detoggle aliases to IsToggled.OnEntered/IsToggled.OnExited
            OnToggled = IsToggled.FindPropertyRelative("onEntered");
            OnDetoggled = IsToggled.FindPropertyRelative("onExited");
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
                EditorGUILayout.PropertyField(ToggleMode, new GUIContent("Selection Mode", "Does this interactable fire toggle events, or does it only act like a button?"));

                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();

                if ((StatefulInteractable.ToggleType)ToggleMode.intValue != StatefulInteractable.ToggleType.Button)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(IsToggledStateActive, new GUIContent("Is Toggled", "Directly set the internal IsToggled state at edit-time"));
                        if (EditorGUI.EndChangeCheck())
                        {
                            // Actually set the toggle state with the public setter so that events fire.
                            interactable.ForceSetToggled(IsToggledStateActive.boolValue);
                        }
                    }
                }

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(OnClicked);

                if ((StatefulInteractable.ToggleType)ToggleMode.intValue != StatefulInteractable.ToggleType.Button)
                {
                    EditorGUILayout.PropertyField(OnToggled, new GUIContent("On Toggled", "Fired when the toggle state has changed from false to true."));
                    EditorGUILayout.PropertyField(OnDetoggled, new GUIContent("On Detoggled", "Fired when the toggle state has changed from true to false."));
                }
            }

            base.DrawProperties();

            advancedFoldout = EditorGUILayout.Foldout(advancedFoldout, EditorGUIUtility.TrTempContent("Advanced StatefulInteractable Settings"), true, EditorStyles.foldoutHeader);
            if (advancedFoldout)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(SelectThreshold);
                    EditorGUILayout.PropertyField(DeselectThreshold);

                    EditorGUILayout.PropertyField(UseGazeDwell);
                    if (UseGazeDwell.boolValue)
                    {
                        using (new EditorGUI.IndentLevelScope())
                        {
                            EditorGUILayout.PropertyField(GazeDwellTime);
                        }
                    }

                    EditorGUILayout.PropertyField(UseFarDwell);
                    if (UseFarDwell.boolValue)
                    {
                        {
                            EditorGUILayout.PropertyField(FarDwellTime);
                        }
                    }

                    EditorGUILayout.PropertyField(allowSelectByVoice);
                    if (allowSelectByVoice.boolValue)
                    {
                        using (new EditorGUI.IndentLevelScope())
                        {
                            EditorGUILayout.PropertyField(speechRecognitionKeyword);
                            EditorGUILayout.PropertyField(VoiceRequiresFocus);
                        }
                    }

                    EditorGUILayout.PropertyField(TriggerOnRelease);

                    EditorGUILayout.PropertyField(SelectRequiresHover);
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

                DrawTimedFlag(IsToggled, interactable.IsToggled, previousGUIColor, Color.cyan);
                
                enabledEventsFoldout = EditorGUILayout.Foldout(enabledEventsFoldout, "OnEnable/Disable", true);
                
                if (enabledEventsFoldout)
                {
                    EditorGUILayout.PropertyField(OnEnabled);
                    EditorGUILayout.PropertyField(OnDisabled);
                }
            }

            EditorGUILayout.Space();
            base.DrawMRTKInteractableFlags();
        }
    }
}
