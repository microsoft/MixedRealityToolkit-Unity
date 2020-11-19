// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI.Interaction;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Custom inspector for the BaseInteractiveElement class. 
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BaseInteractiveElement))]
    public class BaseInteractiveElementInspector : UnityEditor.Editor
    {
        private BaseInteractiveElement instance;
        private SerializedProperty states;
        private SerializedProperty active;

        private static GUIContent RemoveStateButtonLabel;

        // Used to set the default name for the text input field when a user creates a new state
        private static string newStateName = "New State Name";
        private const string newCoreStateName = "New Core State";
        private const string createNewStateName = "Create New State";
        private const string removeStateLabel = "Remove State";
        private const string statesLabel = "States";
        private const string settingsLabel = "Settings";
        private const string setStateNameLabel = "Set State Name";
        private const string addCoreStateLabel = "Add Core State";

        private const int stateValueDisplayWidth = 20;
        private const int stateValueDisplayContainerWidth = 10;

        private bool previousActiveStatus;

        private string defaultStateName = CoreInteractionState.Default.ToString();
        private string touchStateName = CoreInteractionState.Touch.ToString();
        private string focusNearStateName = CoreInteractionState.FocusNear.ToString();

        // The state selection menu is displayed when a user selects the "Add Core State" button
        private StateSelectionMenu stateSelectionMenu; 
        
        protected virtual void OnEnable()
        {
            instance = (BaseInteractiveElement)target;

            active = serializedObject.FindProperty("active");
            states = serializedObject.FindProperty("states");

            RemoveStateButtonLabel = new GUIContent(InspectorUIUtility.Minus, removeStateLabel);

            previousActiveStatus = active.boolValue;

            stateSelectionMenu = ScriptableObject.CreateInstance<StateSelectionMenu>();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            RenderSettings();

            RenderStates();

            RenderAddStateButtons();

            serializedObject.ApplyModifiedProperties();
        }

        private void RenderSettings()
        {
            // Draw a title for the Settings section 
            InspectorUIUtility.DrawTitle(settingsLabel);

            EditorGUILayout.PropertyField(active);

            if (Application.isPlaying)
            {
                if (!active.boolValue)
                {
                    // Add a notice in the inspector to let the user know that they have disabled internal updates, i.e. state values will not update
                    InspectorUIUtility.DrawNotice("Internal updates to this Interactive Element have been disabled.");
                }
                
                if (previousActiveStatus != active.boolValue)
                {
                    // Only reset all states when Active is set to false
                    if (!active.boolValue)
                    {
                        ResetAllStates();
                    }
                }

                previousActiveStatus = active.boolValue;
            }
        }

        private void RenderStates()
        {
            // Draw a States title for the State list section 
            InspectorUIUtility.DrawTitle(statesLabel);

            for (int i = 0; i < states.arraySize; i++)
            {
                SerializedProperty state = states.GetArrayElementAtIndex(i);
                SerializedProperty stateName = state.FindPropertyRelative("stateName");
                SerializedProperty stateValue = state.FindPropertyRelative("stateValue");
                SerializedProperty stateEventConfiguration = state.FindPropertyRelative("eventConfiguration");

                using (new EditorGUILayout.HorizontalScope())
                {
                    string stateFoldoutID = stateName.stringValue + "_" + target.name;

                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.Space();

                        // Draw a foldout for the state configuration
                        if (InspectorUIUtility.DrawSectionFoldoutWithKey(stateName.stringValue, stateFoldoutID, MixedRealityStylesUtility.TitleFoldoutStyle, true))
                        {
                            EditorGUILayout.Space();

                            using (new EditorGUILayout.VerticalScope())
                            {
                                using (new EditorGUI.IndentLevelScope())
                                {
                                    // Show the event configuration for the state
                                    RenderStateEventConfiguration(stateName, stateEventConfiguration);

                                    // When a new core state is added via inspector, the name is initialized to "New Core State" and then changed
                                    // to the name the user selects from the list of CoreInteractionStates
                                    if (stateName.stringValue == newCoreStateName)
                                    {
                                        SetCoreStateType(state, stateName);
                                    }

                                    // When a new state is added via inspector, the name is initialized to "Create New State" and then changed
                                    // to the name the user enters a name in the text field and then selects the "Set State Name" button
                                    if (stateName.stringValue == createNewStateName)
                                    {
                                        SetUserDefinedState(stateName);
                                    }
                                }
                            }
                        }

                        EditorGUILayout.Space();                       
                        EditorGUILayout.Space();
                    }

                    // Do not draw a remove button for the default state
                    if (stateName.stringValue != defaultStateName && !Application.isPlaying)
                    {
                        // Draw a button with a '-' for state removal
                        if (InspectorUIUtility.SmallButton(RemoveStateButtonLabel))
                        {
                            states.DeleteArrayElementAtIndex(i);
                            break;
                        }
                    }

                    // Draw the state value in the inspector next to the state during play mode for debugging
                    if (Application.isPlaying)
                    {
                        Color prevColor = GUI.color;

                        // Highlight the container displaying the state value during play mode if the state value is 1
                        if (stateValue.intValue == 1)
                        {
                            GUI.color = Color.cyan;
                        }

                        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox,GUILayout.Width(stateValueDisplayWidth)))
                        {
                            EditorGUILayout.Space();
                            EditorGUILayout.LabelField(stateValue.intValue.ToString(), GUILayout.Width(stateValueDisplayContainerWidth));
                            EditorGUILayout.Space();
                        }

                        GUI.color = prevColor;
                    }
                }
            }
        }

        // Render the event configuration of a state
        private void RenderStateEventConfiguration(SerializedProperty stateName, SerializedProperty stateEventConfiguration)
        {
            // Do not draw the state events if the state is in the selection process, i.e. the state name is "New Core State" or "Create New State"
            if (stateEventConfiguration.managedReferenceFullTypename != string.Empty && stateName.stringValue != newCoreStateName && stateName.stringValue != createNewStateName)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    // Draw this state's event configuration 
                    EditorGUILayout.PropertyField(stateEventConfiguration, true);
                }
            }
        }

        // Display a state selection menu 
        private void SetCoreStateType(SerializedProperty state, SerializedProperty stateName)
        {
            stateSelectionMenu.DisplayMenu(instance);

            if (stateSelectionMenu.stateSelected)
            {
                // Set the name of the state selected from the menu
                string selectedStateName = stateSelectionMenu.state;

                stateName.stringValue = selectedStateName;

                serializedObject.ApplyModifiedProperties();

                // Set the event configuration
                instance.SetEventConfigurationInstance(stateName.stringValue);

                // Add a near interaction touchable if the state selected was Touch or FocusNear
                if (stateName.stringValue == touchStateName || stateName.stringValue == focusNearStateName)
                {
                    instance.AddNearInteractionTouchable();
                }

                stateSelectionMenu.stateSelected = false;
            }
        }

        // Prompt the user with a text field in the inspector to allow naming of their new state
        private void SetUserDefinedState(SerializedProperty stateName)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                newStateName = EditorGUILayout.TextField("New State Name", newStateName);

                if (GUILayout.Button(setStateNameLabel))
                {
                    if (newStateName != string.Empty && !instance.IsStatePresentEditMode(newStateName) && newStateName != "New State Name")
                    {
                        stateName.stringValue = newStateName;

                        serializedObject.ApplyModifiedProperties();

                        instance.SetEventConfigurationInstance(stateName.stringValue);
                    }
                    else
                    {
                        // Display dialog telling the user to change the state name entered
                        EditorUtility.DisplayDialog("Change State Name", "The state name entered is empty or this state name is already taken, please add characters to the state name or choose another name.","OK");
                    }
                }
            }
        }

        // Add a new state to the States list
        private SerializedProperty AddNewState(string initialStateName)
        {
            states.InsertArrayElementAtIndex(states.arraySize);

            SerializedProperty newState = states.GetArrayElementAtIndex(states.arraySize - 1);
            SerializedProperty name = newState.FindPropertyRelative("stateName");
            SerializedProperty eventConfiguration = newState.FindPropertyRelative("eventConfiguration");

            eventConfiguration.managedReferenceValue = null;

            // Set the initial name of the state added.  This name will be changed once a core state is selected from the list or after a new 
            // name is set via text field.
            name.stringValue = initialStateName;

            return name;
        }

        // Draw the buttons for adding or creating a new state at the bottom of the inspector window side by side
        private void RenderAddStateButtons()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(addCoreStateLabel))
                {
                    AddNewState(newCoreStateName);
                }

                if (GUILayout.Button(createNewStateName))
                {
                    AddNewState(createNewStateName);
                }
            }
        }

        // Reset all the state values if the Active property is set to false via inspector
        private void ResetAllStates()
        {
            for (int i = 0; i < states.arraySize; i++)
            {
                SerializedProperty state = states.GetArrayElementAtIndex(i);
                SerializedProperty stateName = state.FindPropertyRelative("stateName");
                SerializedProperty stateValue = state.FindPropertyRelative("stateValue");
                SerializedProperty stateActive = state.FindPropertyRelative("active");

                if (stateName.stringValue != defaultStateName)
                {
                    stateValue.intValue = 0;
                    stateActive.boolValue = false;
                }
            }
        }
    }
}
