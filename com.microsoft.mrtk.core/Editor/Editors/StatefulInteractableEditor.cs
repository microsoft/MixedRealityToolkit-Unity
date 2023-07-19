// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// A custom editor for the <see cref="StatefulInteractable"/> class. 
    /// </summary>
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
        private SerializedProperty OnUntoggled;
        private SerializedProperty OnEnabled;
        private SerializedProperty OnDisabled;
        private static bool advancedFoldout = false;
        private static bool enabledEventsFoldout = false;

        /// <summary>
        /// A Unity event function that is called when the script component has been enabled.
        /// </summary> 
        protected override void OnEnable()
        {
            base.OnEnable();

            IsToggled = SetUpAutoProperty(nameof(IsToggled));
            IsToggledStateActive = IsToggled.FindPropertyRelative("active");

            SelectThreshold = SetUpAutoProperty(nameof(SelectThreshold));
            DeselectThreshold = SetUpAutoProperty(nameof(DeselectThreshold));

            ToggleMode = SetUpAutoProperty(nameof(ToggleMode));
            TriggerOnRelease = SetUpAutoProperty(nameof(TriggerOnRelease));

            allowSelectByVoice = SetUpProperty(nameof(allowSelectByVoice));
            speechRecognitionKeyword = SetUpProperty(nameof(speechRecognitionKeyword));
            VoiceRequiresFocus = SetUpAutoProperty(nameof(VoiceRequiresFocus));

            SelectRequiresHover = SetUpAutoProperty(nameof(SelectRequiresHover));

            UseGazeDwell = SetUpAutoProperty(nameof(UseGazeDwell));
            GazeDwellTime = SetUpAutoProperty(nameof(GazeDwellTime));
            UseFarDwell = SetUpAutoProperty(nameof(UseFarDwell));
            FarDwellTime = SetUpAutoProperty(nameof(FarDwellTime));

            OnClicked = SetUpAutoProperty(nameof(OnClicked));

            OnEnabled = SetUpAutoProperty(nameof(OnEnabled));
            OnDisabled = SetUpAutoProperty(nameof(OnDisabled));

            // OnToggled and OnUntoggled aliases to IsToggled.OnEntered and IsToggled.OnExited
            OnToggled = IsToggled.FindPropertyRelative("onEntered");
            OnUntoggled = IsToggled.FindPropertyRelative("onExited");
        }

        /// <inheritdoc/>
        protected override void DrawProperties()
        {
            DrawProperties(true);
        }

        /// <summary>
        /// Overload to <see cref="DrawProperties()"/> to allow subclasses to specify whether they'd like
        /// to show toggle-related properties. 
        /// </summary>
        /// <remarks>
        /// Some subclasses hide this, as showing toggle settings wouldn't make much sense for their use case.
        /// </remarks>
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
                    EditorGUILayout.PropertyField(OnUntoggled, new GUIContent("On Untoggled", "Fired when the toggle state has changed from true to false."));
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

        /// <summary>
        /// Draw the serialized flags fields from the <see cref="StatefulInteractable"/> object.
        /// </summary>
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
