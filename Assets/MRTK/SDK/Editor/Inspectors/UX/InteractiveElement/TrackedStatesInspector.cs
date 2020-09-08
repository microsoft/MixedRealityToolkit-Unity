// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI.Interaction;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Custom inspector for the Tracked States scriptable object that is contained in an object that inherits from 
    /// BaseInteractiveElement.
    /// </summary>
    [CustomEditor(typeof(TrackedStates))]
    public class TrackedStatesInspector : UnityEditor.Editor
    {
        private TrackedStates instance;
        private SerializedProperty stateList;

        private static GUIContent RemoveStateButtonLabel;
        private static GUIContent AddStateButtonLabel;

        private static string newStateName = "New State Name"; 

        protected virtual void OnEnable()
        {
            instance = target as TrackedStates;

            RemoveStateButtonLabel = new GUIContent(InspectorUIUtility.Minus, "Remove State");
            AddStateButtonLabel = new GUIContent(InspectorUIUtility.Plus, "Add State");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            InspectorUIUtility.DrawTitle("Tracked States");

            stateList = serializedObject.FindProperty("states");

            RenderStates();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            RenderAddCoreStateButton();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            RenderCreateNewStateButton();

            serializedObject.ApplyModifiedProperties();
        }

        private void RenderStates()
        {
            for (int i = 0; i < stateList.arraySize; i++)
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    SerializedProperty state = stateList.GetArrayElementAtIndex(i);
                    SerializedProperty stateName = state.FindPropertyRelative("stateName");
                    SerializedProperty stateValue = state.FindPropertyRelative("stateValue");
                    SerializedProperty stateEventConfiguration = state.FindPropertyRelative("eventConfiguration");

                    EditorGUILayout.Space();

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        InspectorUIUtility.DrawLabel(stateName.stringValue, 14, InspectorUIUtility.ColorTint10);

                        // Draw the state value in the state while the application is playing for debugging
                        if (Application.isPlaying)
                        {
                            EditorGUILayout.LabelField(stateValue.intValue.ToString());
                        }

                        // Do not draw a remove button for the default state
                        if (stateName.stringValue != "Default")
                        {
                            // Draw a button with a '-' for state removal
                            if (InspectorUIUtility.SmallButton(RemoveStateButtonLabel))
                            {
                                stateList.DeleteArrayElementAtIndex(i);
                                break;
                            }
                        }
                    }

                    using (new EditorGUILayout.VerticalScope())
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.Space();

                        using (new EditorGUI.IndentLevelScope())
                        {
                            // If an event configuration for a state exists then draw the event config scriptable 
                            RenderStateEventConfiguration(stateName, stateEventConfiguration);

                            // When a new core state is added via inspector, the name is initialized to "New Core State" and then changed
                            // to the name the user selects from the enum list of CoreInteractionStates
                            if (stateName.stringValue == "New Core State")
                            {
                                SetCoreStateType(state, stateName);
                            }

                            // When a new state is added via inspector, the name is initialized to "Create New State" and then changed
                            // to the name the user enters a name in the text field and then selects the "Set State Name" button
                            if (stateName.stringValue == "Create New State")
                            {
                                using (new EditorGUILayout.HorizontalScope())
                                {
                                    newStateName = EditorGUILayout.TextField("New State Name", newStateName);

                                    if (GUILayout.Button("Set State Name"))
                                    {
                                        stateName.stringValue = newStateName;
                                    }
                                }
                            }
                        }
                    }

                    EditorGUILayout.Space();                    
                }
            }
        }

        // Render the event configuration of a state
        private void RenderStateEventConfiguration(SerializedProperty stateName, SerializedProperty stateEventConfiguration)
        {
            // If the current state does have a matching event configuration but the event configuration is null, draw a button to 
            // give the user an option to 'Add State Events'.
            if (stateEventConfiguration.objectReferenceValue == null && StateHasExistingEventConfiguration(stateName.stringValue))
            {
                // Draw a button in the state container titled 'Add State Event'. On button press, an instance of the matching
                // event configuration scriptable is created and assigned to the state
                if (GUILayout.Button("Add " + stateName.stringValue + " State Events"))
                {
                    CreateEventScriptable(stateEventConfiguration, stateName.stringValue);
                }
            }
            else if (stateEventConfiguration.objectReferenceValue != null)
            {
                string stateFoldoutID = stateName.stringValue + "EventConfiguration";

                // Draw a foldout for the event configuration
                if (InspectorUIUtility.DrawSectionFoldoutWithKey(stateName.stringValue + " State Events", stateFoldoutID, MixedRealityStylesUtility.TitleFoldoutStyle, false))
                {
                    // Draw the events for the associated state if this state has an event configuration
                    DrawStateEventsScriptableSubEditor(stateEventConfiguration);
                }
            }
        }

        // Check if the given state has a valid event configuration to render
        private bool StateHasExistingEventConfiguration(string stateName)
        {
            // Get the list of the subclasses for BaseInteractionEventConfiguration and find the
            // event configuration that contains the given state name
            var eventConfigurationTypes = TypeCacheUtility.GetSubClasses<BaseInteractionEventConfiguration>();
            var eventConfigType = eventConfigurationTypes.Find(t => t.Name.StartsWith(stateName));

            // If there is associated event configuration for the state, then it is valid
            if (eventConfigType != null)
            {
                return true;
            }
            // If an associated class for the state could not be found then the event config is not valid
            else
            {
                return false;
            }
        }

        // Create an instance of an associated event scriptable object given the state.
        private void CreateEventScriptable(SerializedProperty eventConfiguration, string stateName)
        {
            string className = stateName + "InteractionEventConfiguration";

            // If the event configuration for the state is currently, then create a scrpitable instance
            if (eventConfiguration.objectReferenceValue == null)
            {
                // Initialize the associated scriptable object event configuration with the correct state 
                eventConfiguration.objectReferenceValue = ScriptableObject.CreateInstance(className);
                eventConfiguration.objectReferenceValue.name = stateName + "EventConfiguration";
            }
        }

        // Draw custom container for the event configuration scriptable 
        private void DrawStateEventsScriptableSubEditor(SerializedProperty scriptable)
        {
            if (scriptable.objectReferenceValue != null)
            {
                UnityEditor.Editor configEditor = UnityEditor.Editor.CreateEditor(scriptable.objectReferenceValue);
                
                using (new EditorGUILayout.VerticalScope(InspectorUIUtility.Box(10)))
                {
                    EditorGUILayout.Space();
                    configEditor.OnInspectorGUI();
                    EditorGUILayout.Space();
                }
            }
        }

        private void SetCoreStateType(SerializedProperty stateProp, SerializedProperty stateNameProp)
        {
            Rect position = EditorGUILayout.GetControlRect();
            using (new EditorGUI.PropertyScope(position, new GUIContent("State"), stateProp))
            {
                string[] coreInteractionStateNames = Enum.GetNames(typeof(CoreInteractionState)).ToArray();
                int id = Array.IndexOf(coreInteractionStateNames, -1);
                int newId = EditorGUI.Popup(position, id, coreInteractionStateNames);

                // NOTE FOR THE FUTURE: Sort the core states in a menu that indicates whether a core state is a near 
                // interaction state, far interaction state, or both to futher push the mental model of the MRTK interaction model

                if (newId != -1)
                {
                    string selectedState = coreInteractionStateNames[newId];

                    // If this state is not already being tracked then change the name
                    if (!instance.IsStateTracked(selectedState))
                    {
                        stateNameProp.stringValue = selectedState;
                    }
                    else
                    {
                        // If a state that is already being tracked is selected, reset the id until a 
                        // state that is not being tracked is selected
                        newId = -1;
                    }
                }
            }
        }

        // Add a new state to track.  
        private SerializedProperty AddNewState(string initialStateName)
        {
            stateList.InsertArrayElementAtIndex(stateList.arraySize);

            SerializedProperty newState = stateList.GetArrayElementAtIndex(stateList.arraySize - 1);
            SerializedProperty name = newState.FindPropertyRelative("stateName");
            SerializedProperty eventConfiguration = newState.FindPropertyRelative("eventConfiguration");

            eventConfiguration.objectReferenceValue = null;

            // Set the initial name of the state added.  This name will be changed once a core state is selected from the list or after a new 
            // name is set via text field.
            name.stringValue = initialStateName;

            return name;
        }

        private void RenderAddCoreStateButton()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                if (InspectorUIUtility.FlexButton(AddStateButtonLabel))
                {
                    AddNewState("New Core State");
                }
            }
        }

        private void RenderCreateNewStateButton()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                if (GUILayout.Button("Create New State"))
                {
                    AddNewState("Create New State");
                }
            }
        }
    }
}
