// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.UX;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

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
        private SerializedProperty interactable;
        private SerializedProperty animator;
        private SerializedProperty stateContainers;

        private List<Type> effectTypeList = new List<Type>();

        private string[] effectTypeNames;

        private GUIStyle valueLabelStyle;

        protected virtual void OnEnable()
        {
            instance = target as StateVisualizer;
            interactable = serializedObject.FindProperty("interactable");
            animator = serializedObject.FindProperty("animator");
            stateContainers = serializedObject.FindProperty("stateContainers").FindPropertyRelative("entries");

            List<Type> types = GetEffectTypes();

            effectTypeNames = new string[types.Count];

            for (int i = 0; i < types.Count; i++)
            {
                effectTypeNames[i] = types[i].Name;
            }
        }

        public override void OnInspectorGUI()
        {
            valueLabelStyle = EditorStyles.miniLabel;
            valueLabelStyle.alignment = TextAnchor.MiddleRight;
            valueLabelStyle.normal.textColor = Color.gray;

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(interactable);
            EditorGUILayout.PropertyField(animator);

            RenderStates();

            serializedObject.ApplyModifiedProperties();
        }

        private Dictionary<string, ReorderableList> effectLists = new Dictionary<string, ReorderableList>();

        private void RenderStates()
        {
            InspectorUIUtility.DrawTitle("Visual States");

            foreach (var kvp in stateContainers)
            {
                SerializedProperty kvpProperty = kvp as SerializedProperty;
                string stateName = kvpProperty.FindPropertyRelative("key").stringValue;

                // Don't show Toggle state for things that don't toggle.
                if (interactable.objectReferenceValue != null && stateName == "Toggle" &&
                    (interactable.objectReferenceValue as StatefulInteractable).ToggleMode == StatefulInteractable.ToggleType.Button)
                {
                    continue;
                }

                var previousColor = GUI.color;
                GUI.color = Color.Lerp(previousColor, Color.cyan, instance.stateContainers[stateName].Value);

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();

                    int numEffects = instance.stateContainers[stateName].Effects.Count;
                    string effectsLabel = numEffects > 0 ? (numEffects + " effect" + (numEffects == 1 ? "" : "s") + ", ") : "";

                    if (instance.stateContainers[stateName].IsVariable)
                    {
                        EditorGUILayout.LabelField(effectsLabel + "Value: " + instance.stateContainers[stateName].Value.ToString("F2"), EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
                    }
                    else
                    {
                        EditorGUILayout.LabelField(effectsLabel + "Value: " + (Mathf.Approximately(instance.stateContainers[stateName].Value, 0.0f) ? "Off" : "On"), EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
                    }
                }

                GUILayout.Space(-EditorGUIUtility.singleLineHeight);

                string stateFoldoutID = stateName + "State" + "_" + target.name;

                if (InspectorUIUtility.DrawSectionFoldoutWithKey(stateName, stateFoldoutID, MRTKEditorStyles.TitleFoldoutStyle, false))
                {
                    GUI.color = previousColor;
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.Space();

                        using (var check = new EditorGUI.ChangeCheckScope())
                        {

                            if (effectLists.ContainsKey(stateName))
                            {
                                effectLists[stateName].DoLayoutList();
                            }
                            else
                            {
                                var newList = new ReorderableList(
                                                serializedObject,
                                                kvpProperty.FindPropertyRelative("value").FindPropertyRelative("effects"),
                                                true,
                                                true,
                                                true,
                                                true);

                                newList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                                {
                                    rect.x += 10;
                                    rect.width -= 10;
                                    EditorGUI.PropertyField(rect, newList.serializedProperty.GetArrayElementAtIndex(index), true);
                                };

                                newList.elementHeightCallback = (int index) => EditorGUI.GetPropertyHeight(newList.serializedProperty.GetArrayElementAtIndex(index), true);
                                newList.drawHeaderCallback = (Rect rect) => EditorGUI.LabelField(rect, "Effects");

                                newList.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) =>
                                {
                                    GenericMenu menu = new GenericMenu();
                                    foreach (var type in effectTypeList)
                                    {
                                        menu.AddItem(
                                            new GUIContent(type.Name.Replace("Effect", string.Empty)),
                                            false,
                                            (object t) =>
                                            {
                                                Undo.RecordObject(instance, "Add effect");
                                                instance.AddEffect(stateName, Activator.CreateInstance((Type)t) as IEffect);
                                                PrefabUtility.RecordPrefabInstancePropertyModifications(instance);
                                                serializedObject.ApplyModifiedProperties();
                                            },
                                            type);
                                    }
                                    menu.ShowAsContext();
                                };

                                effectLists.Add(stateName, newList);
                            }
                        }
                    }
                }
                GUI.color = previousColor;
                EditorGUILayout.Space(1.0f);
            }
        }

        private List<Type> GetEffectTypes()
        {
            effectTypeList.Clear();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                // Exclude test assemblies
                if (assembly.FullName?.Contains("Tests") ?? true) { continue; }
                FilterTypes(assembly, effectTypeList);
            }

            effectTypeList.Sort((a, b) => string.Compare(a.FullName, b.FullName, StringComparison.Ordinal));
            return effectTypeList;
        }

        private void FilterTypes(Assembly assembly, List<Type> output)
        {
            foreach (var type in assembly.GetLoadableTypes())
            {
                if (type.IsAbstract || type.IsInterface || !DoesImplement(type, typeof(IEffect)))
                {
                    continue;
                }

                output.Add(type);
            }
        }

        private bool DoesImplement(Type type, Type interfaceType)
        {
            var interfaces = type.GetInterfaces();
            for (var i = 0; i < interfaces.Length; i++)
            {
                if (interfaces[i] == interfaceType)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
