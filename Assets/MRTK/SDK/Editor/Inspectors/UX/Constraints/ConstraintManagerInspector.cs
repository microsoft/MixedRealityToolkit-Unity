//
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
//

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Experimental.Editor;
using Vuforia.EditorClasses;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(ConstraintManager), true)]
    [CanEditMultipleObjects]
    public class ConstraintManagerInspector : UnityEditor.Editor
    {
        private SerializedProperty autoConstraintSelection;
        private SerializedProperty selectedConstraints;
       
        private ConstraintManager constraintManager;

        //private static bool constraintsFoldout = true;

        private const string autoMsg = "Constraint manager is currently set to auto mode. In auto mode all" +
            " constraints attached to this gameobject will automatically be processed by this manager.";
        private const string manualMsg = "Constraint manager is currently set to manual mode. In manual mode" +
            " only constraints that are linked in the below component list will be processed by this manager.";

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
            //DetachAndRemove,
            Highlight
        }

        private static EntryAction RenderManualConstraintItem(SerializedProperty constraintEntry, bool canRemove = true)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                var constraint = constraintEntry.objectReferenceValue;
                if (constraint == null)
                {
                    // clean up deleted constraints
                    return EntryAction.Detach;
                }

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

                    //if (InspectorUIUtility.FlexButton(new GUIContent("Remove Constraint", "Remove constraint from constraint manager and delete from game object.")))
                    //{
                    //    return EntryAction.DetachAndRemove;
                    //}
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
        private void AttachEmpty()
        {
            AttachConstraint(null);
        }

        private void RenderAutoConstraintMenu()
        {
            // component list
            var constraints = constraintManager.gameObject.GetComponents<TransformConstraint>();
            foreach (var constraint in constraints)
            {
                EditorGUILayout.BeginHorizontal();
                string constraintName = constraint.GetType().Name;
                EditorGUILayout.LabelField(constraintName);
                if (GUILayout.Button("Go to component"))
                {
                    Highlighter.Highlight("Inspector", $"{ObjectNames.NicifyVariableName(constraintName)} (Script)");
                    EditorGUIUtility.ExitGUI();
                }
                EditorGUILayout.EndHorizontal();
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
                     AddNewConstraint((Type)t), derivedType);
                }

                menu.ShowAsContext();
            }
        }

        private void RenderManualConstraintMenu()
        {
            for (int i = 0; i < selectedConstraints.arraySize; i++)
            {
                SerializedProperty constraintProperty = selectedConstraints.GetArrayElementAtIndex(i);
                var removeAction = RenderManualConstraintItem(constraintProperty, true);
                if (removeAction == EntryAction.Detach)
                {
                    selectedConstraints.DeleteArrayElementAtIndex(i);
                    serializedObject.ApplyModifiedProperties();
                }
                //else if (removeAction == EntryAction.DetachAndRemove)
                //{
                //    GameObject.Destroy(constraintProperty.objectReferenceValue);
                //    selectedConstraints.DeleteArrayElementAtIndex(i);
                //    serializedObject.ApplyModifiedProperties();
                //}
                else if (removeAction == EntryAction.Highlight)
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

                        if (hasEntries == false)
                        {
                            var guiEnabledRestore = GUI.enabled;
                            GUI.enabled = false;
                            menu.AddItem(new GUIContent("No constraint available", 
                                "Either there's no constraint attached to this game object or all available constraints " +
                                "are already part of the list."), false, null);
                            GUI.enabled = guiEnabledRestore;
                        }
                        // add an empty slot to manually drag in components
                        // REMOVE THIS? menu.AddItem(new GUIContent("Empty", "Add an empty slot to the list."), false, AttachEmpty);

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
                //TODO InspectorUIUtility.RenderHelpURL(target.GetType());

                // Data section
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.Space();
                    
                    EditorGUILayout.HelpBox(autoConstraintSelection.boolValue == true ? autoMsg : manualMsg
                        , UnityEditor.MessageType.Info);
                    EditorGUILayout.Space();

                    int tab = autoConstraintSelection.boolValue == true ? 0 : 1;
                    tab = GUILayout.Toolbar(tab, new string[] { "Auto Constraint Selection", "Manual Constraint Selection" });
                    EditorGUILayout.Space();
                    //EditorGUILayout.PropertyField(autoConstraintSelection);
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

                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                    }
                }
            }
        }
    }
}
