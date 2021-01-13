// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.UI.Interaction;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Custom inspector for the StateVisualizer component
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(StateVisualizer))]
    public class StateVisualizerInspector : UnityEditor.Editor
    {
        private StateVisualizer instance;
        private SerializedProperty interactiveElement;
        private SerializedProperty animator;
        private SerializedProperty stateContainers;

        private AnimatorController animatorController;

        private bool inPlayMode;

        private static GUIContent RemoveButtonLabel;
        

        private List<StylePropertyMenu> stylePropertyMenus = new List<StylePropertyMenu>();

        protected virtual void OnEnable()
        {
            instance = (StateVisualizer)target;
            interactiveElement = serializedObject.FindProperty("interactiveElement");
            animator = serializedObject.FindProperty("animator");

            stateContainers = serializedObject.FindProperty("stateContainers");

            RemoveButtonLabel = new GUIContent(InspectorUIUtility.Minus, "Remove");

            for (int i =0; i < stateContainers.arraySize; i++)
            {
                if (stateContainers.arraySize != stylePropertyMenus.Count)
                {
                    stylePropertyMenus.Add(ScriptableObject.CreateInstance<StylePropertyMenu>());
                }   
            }
        }

        public override void OnInspectorGUI()
        {
            inPlayMode = EditorApplication.isPlaying || EditorApplication.isPaused;

            serializedObject.Update();

            RenderInitialProperties();

            RenderStateContainers();

            RenderEndingButtons();

            serializedObject.ApplyModifiedProperties();
        }

        private void RenderInitialProperties()
        {
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(interactiveElement);

            EditorGUILayout.PropertyField(animator);

            EditorGUILayout.Space();
        }

        private void RenderStateContainers()
        {
            InspectorUIUtility.DrawTitle("State Animations");

            for (int i = 0; i < stateContainers.arraySize; i++)
            {
                SerializedProperty stateContainer = stateContainers.GetArrayElementAtIndex(i);
                SerializedProperty stateContainerName = stateContainer.FindPropertyRelative("stateName");
                SerializedProperty animationTargetsList = stateContainer.FindPropertyRelative("animationTargets");
                SerializedProperty stateContainerAnimationClip = stateContainer.FindPropertyRelative("animationClip");
                SerializedProperty animationTransitionDuration = stateContainer.FindPropertyRelative("animationTransitionDuration");

                Color previousGUIColor = GUI.color;

                using (new EditorGUILayout.HorizontalScope())
                {
                    string stateFoldoutID = stateContainerName.stringValue + "StateContainer" + "_" + target.name;

                    if (inPlayMode)
                    {
                        InteractiveElement intereactiveElem = interactiveElement.objectReferenceValue as InteractiveElement;

                        if (intereactiveElem.isActiveAndEnabled)
                        {
                            if (intereactiveElem.IsStateActive(stateContainerName.stringValue))
                            {
                                GUI.color = Color.cyan;
                            }
                        }
                    }

                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        EditorGUILayout.Space();

                        if (InspectorUIUtility.DrawSectionFoldoutWithKey(stateContainerName.stringValue, stateFoldoutID, MixedRealityStylesUtility.TitleFoldoutStyle, false))
                        {
                            using (new EditorGUI.IndentLevelScope())
                            {
                                EditorGUILayout.PropertyField(stateContainerAnimationClip);

                                using (var check = new EditorGUI.ChangeCheckScope())
                                {
                                    EditorGUILayout.PropertyField(animationTransitionDuration);
                                    
                                    if (check.changed)
                                    {
                                        instance.SetAnimationTransition(stateContainerName.stringValue, animationTransitionDuration.floatValue);
                                    }
                                }

                                RenderAnimationTargetList(animationTargetsList, stateContainerName);
                            }
                        }

                        EditorGUILayout.Space();
                    }

                    GUI.color = previousGUIColor;

                    if (!inPlayMode)
                    {
                        if (InspectorUIUtility.SmallButton(RemoveButtonLabel))
                        {
                            instance.RemoveAnimatorState(instance.RootStateMachine, stateContainerName.stringValue);
                            stateContainers.DeleteArrayElementAtIndex(i);
                            break;
                        }
                    }
                }
            }
        }

        private void RenderStylePropertyList(SerializedProperty stylePropertyList, SerializedProperty stateContainerName, int animationTargetIndex)
        {
            using (new EditorGUI.IndentLevelScope())
            {
                for (int i = 0; i < stylePropertyList.arraySize; i++)
                {
                    SerializedProperty styleProperty = stylePropertyList.GetArrayElementAtIndex(i);
                    SerializedProperty stylePropertyName = styleProperty.FindPropertyRelative("stylePropertyName");

                    using (new EditorGUILayout.VerticalScope())
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            StateVisualizerStylePropertyInspector.RenderStyleProperty(styleProperty, instance, animationTargetIndex);
                            serializedObject.ApplyModifiedProperties();
                            
                            if (InspectorUIUtility.SmallButton(RemoveButtonLabel))
                            {
                                StateVisualizerStylePropertyInspector.RemoveKeyFrames(instance, stateContainerName.stringValue, stylePropertyName.stringValue, animationTargetIndex);
                                stylePropertyList.DeleteArrayElementAtIndex(i);
                                break;
                            }
                        }
                    }
                }
            }

            InspectorUIUtility.DrawDivider();

            // Show Add Style Property Button
            stylePropertyMenus[animationTargetIndex].DisplayMenu();

            EditorGUILayout.Space();

            if (stylePropertyMenus[animationTargetIndex].stylePropertySelected)
            {
                StateVisualizerStylePropertyInspector.CreateStylePropertyInstance(instance, stateContainerName.stringValue, stylePropertyMenus[animationTargetIndex].stylePropertyNameSelected.ToString(), animationTargetIndex);

                serializedObject.ApplyModifiedProperties();

                stylePropertyMenus[animationTargetIndex].stylePropertySelected = false;
            }

        }

        private void RenderAnimationTargetList(SerializedProperty animationTargetList, SerializedProperty stateContainerName)
        {
            using (new EditorGUI.IndentLevelScope())
            {
                for (int i = 0; i < animationTargetList.arraySize; i++)
                {
                    SerializedProperty animationTarget = animationTargetList.GetArrayElementAtIndex(i);
                    SerializedProperty targetObj = animationTarget.FindPropertyRelative("target");
                    SerializedProperty stylePropertyList = animationTarget.FindPropertyRelative("stateStyleProperties");

                    EditorGUILayout.Space();

                    using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                    {
                        EditorGUILayout.Space();

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.PropertyField(targetObj);

                            if (InspectorUIUtility.SmallButton(RemoveButtonLabel))
                            {
                                // Clear keyframes of a deleted target
                                for (int j = 0; j < stylePropertyList.arraySize; j++)
                                {
                                    SerializedProperty styleProperty = stylePropertyList.GetArrayElementAtIndex(j);
                                    SerializedProperty stylePropertyName = styleProperty.FindPropertyRelative("stylePropertyName");
                                    StateVisualizerStylePropertyInspector.RemoveKeyFrames(instance, stateContainerName.stringValue, stylePropertyName.stringValue, i);
                                }

                                animationTargetList.DeleteArrayElementAtIndex(i);
                                break;
                            }
                        }

                        InspectorUIUtility.DrawDivider();

                        using (new EditorGUILayout.VerticalScope())
                        {
                            if (targetObj.objectReferenceValue != null)
                            {
                                GameObject targetGameObject = targetObj.objectReferenceValue as GameObject;

                                string stylePropertiesFoldoutID = stateContainerName.stringValue + "StyleProperties" + "_" + targetGameObject.name + target.name;

                                if (InspectorUIUtility.DrawSectionFoldoutWithKey(targetGameObject.name + " Style Properties", stylePropertiesFoldoutID, MixedRealityStylesUtility.BoldFoldoutStyle, false))
                                {
                                    using (new EditorGUI.IndentLevelScope())
                                    {
                                        RenderStylePropertyList(stylePropertyList, stateContainerName, i);
                                    }
                                }

                                EditorGUILayout.Space();
                            }
                        }
                    }
                }

                EditorGUILayout.Space();

                if (GUILayout.Button("Add Target"))
                {
                    animationTargetList.InsertArrayElementAtIndex(animationTargetList.arraySize);

                    serializedObject.ApplyModifiedProperties();

                    SerializedProperty newAnimationTarget = animationTargetList.GetArrayElementAtIndex(animationTargetList.arraySize - 1);
                    newAnimationTarget.FindPropertyRelative("target").objectReferenceValue = null;

                    SerializedProperty stateStylePropertiesList = newAnimationTarget.FindPropertyRelative("stateStyleProperties");

                    // Clear the new list
                    for (int i = 0; i < stateStylePropertiesList.arraySize; i++)
                    {
                        stateStylePropertiesList.DeleteArrayElementAtIndex(i);
                    }
                }
            }
        }

        private void RenderSetStateMachineButton()
        {
            if (GUILayout.Button("Generate Animation Clips"))
            {
                // Create MRTK_Animation Directory if it does not exist
                string animationAssetDirectory = instance.GetAnimationDirectoryPath();
                string animationControllerName = instance.gameObject.name + ".controller";
                string animationControllerPath = Path.Combine(animationAssetDirectory, animationControllerName);

                // Create Animation Controller 
                animatorController = AnimatorController.CreateAnimatorControllerAtPath(animationControllerPath);

                // Set the runtime animation controller 
                instance.gameObject.GetComponent<Animator>().runtimeAnimatorController = animatorController;

                instance.SetUpStateMachine(animatorController);
            }
        }

        private void RenderSyncStatesButton()
        {
         
            if (GUILayout.Button("Sync States with Interactive Element"))
            {
                instance.UpdateStateContainerStates();
            }
        }

        private void RenderEndingButtons()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                RenderSetStateMachineButton();
                RenderSyncStatesButton();
            }
        }
    }
}
