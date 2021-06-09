//
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
//

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Custom inspector for constraint manager.
    /// Offers two modes depending on if auto constraint selection is active or not.
    /// In auto constraint selection mode, all constraints attached to the current game object
    /// will be displayed with respective goto buttons as well as an add button that allows adding 
    /// new constraint components to the game object.
    /// In manual constraint selection mode, a list of user configured constraints is shown with options
    /// to modify the list, goto buttons as well as adding new constraints to the game object.
    /// </summary>
    [CustomEditor(typeof(ConstraintManager), true)]
    [CanEditMultipleObjects]
    public class ConstraintManagerInspector : UnityEditor.Editor
    {
        private SerializedProperty autoConstraintSelection;
        private SerializedProperty selectedConstraints;

        private ConstraintManager constraintManager;

        private const string autoMsg = "Constraint manager is currently set to auto mode. In auto mode all" +
            " constraints attached to this gameobject will automatically be processed by this manager.";
        private const string manualMsg = "Constraint manager is currently set to manual mode. In manual mode" +
            " only constraints that are linked in the below component list will be processed by this manager.";

        private List<int> indicesToRemove = new List<int>(); // list for deferred deletion in our selected constraint list to not break unity GUI layout

        private void OnEnable()
        {
            constraintManager = (ConstraintManager)target;

            autoConstraintSelection = serializedObject.FindProperty("autoConstraintSelection");
            selectedConstraints = serializedObject.FindProperty("selectedConstraints");
        }

        private enum EntryAction
        {
            None,
            Attach,
            Detach,
            AddAndAttach,
            Highlight
        }

        private static EntryAction RenderManualConstraintItem(SerializedProperty constraintEntry, bool canRemove = true)
        {
            var constraint = constraintEntry.objectReferenceValue;
            if (constraint == null)
            {
                // clean up deleted constraints
                return EntryAction.Detach;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(constraint.GetType().Name, GUILayout.ExpandWidth(true));

                if (canRemove)
                {
                    if (InspectorUIUtility.FlexButton(new GUIContent("Go to Constraint", "Scroll inspector to and visually highlight constraint.")))
                    {
                        return EntryAction.Highlight;
                    }

                    if (InspectorUIUtility.FlexButton(new GUIContent("Remove Entry", "Remove constraint from constraint manager but keep constraint component attached to the gameobject.")))
                    {
                        return EntryAction.Detach;
                    }
                }
                return EntryAction.None;
            }
        }

        private void AddNewConstraint(Type t)
        {
            var constraint = constraintManager.gameObject.AddComponent((Type)t);
            AttachConstraint((TransformConstraint)constraint);
        }

        private void AttachConstraint(TransformConstraint constraint)
        {
            int newElementIndex = selectedConstraints.arraySize;
            selectedConstraints.InsertArrayElementAtIndex(newElementIndex);
            selectedConstraints.GetArrayElementAtIndex(newElementIndex).objectReferenceValue = constraint;
            serializedObject.ApplyModifiedProperties();
        }

        private void RenderAutoConstraintMenu()
        {
            // component list
            var constraints = constraintManager.gameObject.GetComponents<TransformConstraint>();
            foreach (var constraint in constraints)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    string constraintName = constraint.GetType().Name;
                    EditorGUILayout.LabelField(constraintName);
                    if (GUILayout.Button("Go to component"))
                    {
                        Highlighter.Highlight("Inspector", $"{ObjectNames.NicifyVariableName(constraintName)} (Script)");
                        EditorGUIUtility.ExitGUI();
                    }
                }
            }

            // add button
            if (EditorGUILayout.DropdownButton(new GUIContent("Add Constraint to GameObject", "Add a constraint to the gameobject that will be picked up by the constraint manager auto mode."), FocusType.Keyboard))
            {
                // create the menu and add items to it
                GenericMenu menu = new GenericMenu();

                var type = typeof(TransformConstraint);
                var types = AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(s => s.GetLoadableTypes())
                            .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract);

                foreach (var derivedType in types)
                {
                    menu.AddItem(new GUIContent(derivedType.Name), false, t =>
                     constraintManager.gameObject.AddComponent((Type)t), derivedType);
                }

                menu.ShowAsContext();
            }
        }

        private void RenderManualConstraintMenu()
        {
            for (int i = 0; i < selectedConstraints.arraySize; i++)
            {
                SerializedProperty constraintProperty = selectedConstraints.GetArrayElementAtIndex(i);
                var buttonAction = RenderManualConstraintItem(constraintProperty, true);
                if (buttonAction == EntryAction.Detach)
                {
                    indicesToRemove.Add(i);
                }
                else if (buttonAction == EntryAction.Highlight)
                {
                    string constraintName = constraintProperty.objectReferenceValue.GetType().Name;
                    Highlighter.Highlight("Inspector", $"{ObjectNames.NicifyVariableName(constraintName)} (Script)");
                    EditorGUIUtility.ExitGUI();
                }
            }

            // add buttons
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (EditorGUILayout.DropdownButton(new GUIContent("Add Entry", "Attach an already existing component from this gameobject to the constraint manager selection."), FocusType.Keyboard))
                    {
                        // create the menu and add items to it
                        GenericMenu menu = new GenericMenu();

                        var constraints = constraintManager.gameObject.GetComponents<TransformConstraint>();

                        bool hasEntries = false;
                        foreach (var constraint in constraints)
                        {
                            // only show available constraints that haven't been added yet
                            var existingConstraint = constraintManager.SelectedConstraints.Find(t => t == constraint);
                            if (existingConstraint == null)
                            {
                                hasEntries = true;
                                string constraintName = constraint.GetType().Name;
                                menu.AddItem(new GUIContent(constraintName), false, t =>
                                 AttachConstraint((TransformConstraint)t), constraint);
                            }
                        }

                        // if all constraint components are already part of the list display disabled "no constraint available" entry
                        if (hasEntries == false)
                        {
                            var guiEnabledRestore = GUI.enabled;
                            GUI.enabled = false;
                            menu.AddItem(new GUIContent("No constraint available",
                                "Either there's no constraint attached to this game object or all available constraints " +
                                "are already part of the list."), false, null);
                            GUI.enabled = guiEnabledRestore;
                        }

                        menu.ShowAsContext();
                    }

                    if (EditorGUILayout.DropdownButton(new GUIContent("Add New Constraint", "Add a constraint to the gameobject and attach to this constraint manager selection."), FocusType.Keyboard))
                    {
                        // create the menu and add items to it
                        GenericMenu menu = new GenericMenu();

                        var type = typeof(TransformConstraint);
                        var types = AppDomain.CurrentDomain.GetAssemblies()
                                    .SelectMany(s => s.GetLoadableTypes())
                                    .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract);

                        foreach (var derivedType in types)
                        {
                            menu.AddItem(new GUIContent(derivedType.Name), false, t =>
                             AddNewConstraint((Type)t), derivedType);
                        }

                        menu.ShowAsContext();
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            if (target != null)
            {
                serializedObject.Update();

                // Help url
                InspectorUIUtility.RenderHelpURL(target.GetType());

                // Data section
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUILayout.Space();

                    EditorGUILayout.HelpBox(autoConstraintSelection.boolValue == true ? autoMsg : manualMsg
                        , UnityEditor.MessageType.Info);
                    EditorGUILayout.Space();

                    int tab = autoConstraintSelection.boolValue == true ? 0 : 1;
                    tab = GUILayout.Toolbar(tab, new string[] { "Auto Constraint Selection", "Manual Constraint Selection" });
                    EditorGUILayout.Space();
                    switch (tab)
                    {
                        case 0:
                            autoConstraintSelection.boolValue = true;
                            RenderAutoConstraintMenu();
                            break;

                        case 1:
                            bool oldAutoConstraintSelection = autoConstraintSelection.boolValue;
                            autoConstraintSelection.boolValue = false;
                            bool newAutoConstraintSelection = autoConstraintSelection.boolValue;

                            // manual constraint selection was enabled
                            if (newAutoConstraintSelection == false && oldAutoConstraintSelection != newAutoConstraintSelection)
                            {
                                // manual selection is active and manual list is empty -> auto populate with 
                                // existing constraints so user has a base to work on
                                if (selectedConstraints.arraySize == 0)
                                {
                                    var constraints = constraintManager.gameObject.GetComponents<TransformConstraint>();
                                    foreach (var constraint in constraints)
                                    {
                                        int currentId = selectedConstraints.arraySize;
                                        selectedConstraints.InsertArrayElementAtIndex(currentId);
                                        selectedConstraints.GetArrayElementAtIndex(currentId).objectReferenceValue = constraint;
                                    }
                                }
                            }

                            RenderManualConstraintMenu();
                            break;
                    }

                    // we render the instance id of this component so our highlighting function can distinguish between 
                    // the different instances of constraint manager - highlighting in the inspector is currently 
                    // only available for string search which causes problems with multiple components of the same type 
                    // attached to the same gameobject.
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("ComponentId: " + constraintManager.GetInstanceID(), EditorStyles.miniLabel);

                    // deferred delete elements from array to not break unity layout
                    for (int i = indicesToRemove.Count - 1; i > -1; i--)
                    {
                        var currentArraySize = selectedConstraints.arraySize;
                        selectedConstraints.DeleteArrayElementAtIndex(indicesToRemove[i]);
                        if (currentArraySize == selectedConstraints.arraySize)
                        {
                            selectedConstraints.DeleteArrayElementAtIndex(indicesToRemove[i]);
                        }
                    }

                    indicesToRemove.Clear();

                    if (check.changed)
                    {
                        serializedObject.ApplyModifiedProperties();
                    }
                }
            }
        }

        /// <summary>
        /// Util method for drawing a consistent constraints section.
        /// Use this method in a component inspector for linking to a constraint manager. 
        /// </summary>
        /// <param name="gameObject">Game object the constraint manager is attached to.</param>
        /// <param name="managerEnabled">Serialized property for enabling the manager - needs to be of type bool.</param>
        /// <param name="managerRef">Serialized property of the constraint manager component link - needs to be type of ConstraintManager.</param>
        /// <param name="isExpanded">Flag for indicating if the constraint foldout was previously collapsed or expanded.</param>
        /// <returns>Current state of expanded or collapsed constraint foldout. Returns true if expanded / contents visible.</returns>
        static public bool DrawConstraintManagerFoldout(GameObject gameObject, SerializedProperty managerEnabled, SerializedProperty managerRef, bool isExpanded)
        {
            isExpanded = EditorGUILayout.Foldout(isExpanded, "Constraints", true);

            if (isExpanded)
            {
                EditorGUILayout.PropertyField(managerEnabled);
                GUI.enabled = managerEnabled.boolValue;
                // Make sure we're having at least one constraint manager available.
                // Usually this should be ensured by the component requirement. However 
                // for components that had this requirement added after they were serialized
                // this won't work out of the box.
                gameObject.EnsureComponent<ConstraintManager>();
                var constraintManagers = gameObject.GetComponents<ConstraintManager>();

                int selected = 0;

                string[] options = new string[constraintManagers.Length];

                int manualSelectionCount = 0;
                for (int i = 0; i < constraintManagers.Length; ++i)
                {
                    var manager = constraintManagers[i];
                    if (managerRef.objectReferenceValue == manager)
                    {
                        selected = i;
                    }

                    // popups will only show unique elements
                    // in case of auto selection we don't care which one we're selecting as the behavior will be the same.
                    // in case of manual selection users might want to differentiate which constraintmanager they are referring to.
                    if (manager.AutoConstraintSelection == true)
                    {
                        options[i] = manager.GetType().Name + " (auto)";
                    }
                    else
                    {
                        manualSelectionCount++;
                        options[i] = manager.GetType().Name + " (manual " + manualSelectionCount + ")";
                    }
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    selected = EditorGUILayout.Popup("Constraint Manager", selected, options, GUILayout.ExpandWidth(true));
                    ConstraintManager selectedConstraintManager = constraintManagers[selected];
                    managerRef.objectReferenceValue = selectedConstraintManager;
                    if (GUILayout.Button("Go to component"))
                    {
                        EditorGUIUtility.PingObject(selectedConstraintManager);
                        Highlighter.Highlight("Inspector", $"ComponentId: {selectedConstraintManager.GetInstanceID()}");
                        EditorGUIUtility.ExitGUI();
                    }
                }

                GUI.enabled = true;
            }

            return isExpanded;
        }
    }
}
