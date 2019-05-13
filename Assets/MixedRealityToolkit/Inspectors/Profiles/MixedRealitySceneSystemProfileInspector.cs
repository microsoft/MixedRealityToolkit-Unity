// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information. 


using Microsoft.MixedReality.Toolkit.SceneSystem;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(MixedRealitySceneSystemProfile))]
    public class MixedRealitySceneSystemProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        const float DragAreaWidth = 0f;
        const float DragAreaHeight = 30f;
        const float DragAreaOffset = 10;

        private static bool showManagerProperties = true;
        private SerializedProperty useManagerScene;
        private SerializedProperty managerScene;

        private static bool showContentProperties = true;
        private SerializedProperty contentScenes;

        private static bool showLightingProperties = true;
        private SerializedProperty useLightingScene;
        private SerializedProperty defaultLightingSceneIndex;
        private SerializedProperty lightingScenes;

        private static bool showEditorProperties = true;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured(false))
            {
                return;
            }

            useManagerScene = serializedObject.FindProperty("useManagerScene");
            managerScene = serializedObject.FindProperty("managerScene");

            useLightingScene = serializedObject.FindProperty("useLightingScene");
            defaultLightingSceneIndex = serializedObject.FindProperty("defaultLightingSceneIndex");
            lightingScenes = serializedObject.FindProperty("lightingScenes");

            contentScenes = serializedObject.FindProperty("contentScenes");
        }

        private void OnSelecteContentScene(ReorderableList list)
        {
            Debug.Log("Selecting");
        }

        private void DrawContentSceneElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SceneInfoDrawer.DrawProperty(rect, contentScenes.GetArrayElementAtIndex(index), GUIContent.none, isActive, isFocused);
        }

        public override void OnInspectorGUI()
        {
            RenderTitleDescriptionAndLogo(
                "Scene System",
                "The Scene System Profile helps configure your scene settings.");

            if (MixedRealityInspectorUtility.CheckMixedRealityConfigured(true, true))
            {
                if (DrawBacktrackProfileButton("Back to Configuration Profile", MixedRealityToolkit.Instance.ActiveProfile))
                {
                    return;
                }
            }

            CheckProfileLock(target);

            serializedObject.Update();

            EditorGUILayout.Space();

            EditorGUILayout.Space();
            showEditorProperties = EditorGUILayout.Foldout(showEditorProperties, "Editor Settings", true);
            if (showEditorProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    // TBD
                }
            }

            showManagerProperties = EditorGUILayout.Foldout(showManagerProperties, "Manager Scene Settings", true);
            if (showManagerProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    // Disable the tag field since we're drawing manager scenes
                    SceneInfoDrawer.DrawTagProperty = false;
                    EditorGUILayout.PropertyField(useManagerScene);
                    EditorGUILayout.PropertyField(managerScene, includeChildren: true);
                }
            }

            EditorGUILayout.Space();
            showContentProperties = EditorGUILayout.Foldout(showContentProperties, "Content Scene Settings", true);
            if (showContentProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    // Enable the tag field since we're drawing content scenes
                    SceneInfoDrawer.DrawTagProperty = true;
                    EditorGUILayout.PropertyField(contentScenes, includeChildren: true);
                    //DrawSceneInfoDragAndDrop(contentScenes);
                }
            }

            EditorGUILayout.Space();
            showLightingProperties = EditorGUILayout.Foldout(showLightingProperties, "Lighting Scene Settings", true);
            if (showLightingProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(useLightingScene);
                    if (useLightingScene.boolValue)
                    {
                        // Disable the tag field since we're drawing lighting scenes
                        SceneInfoDrawer.DrawTagProperty = false;
                        defaultLightingSceneIndex.intValue = EditorGUILayout.IntSlider(defaultLightingSceneIndex.displayName, defaultLightingSceneIndex.intValue, 0, lightingScenes.arraySize - 1);
                        EditorGUILayout.PropertyField(lightingScenes, includeChildren: true);
                        //DrawSceneInfoDragAndDrop(lightingScenes);
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSceneInfoDragAndDrop(SerializedProperty arrayProperty)
        {
            if (!Application.isPlaying)
            {
                Rect dropArea = GUILayoutUtility.GetRect(DragAreaWidth, DragAreaHeight, GUILayout.ExpandWidth(true));
                dropArea.width -= DragAreaOffset * 2;
                dropArea.x += DragAreaOffset;
                GUI.Box(dropArea, "Drag-and-drop new " + arrayProperty.displayName, EditorStyles.helpBox);

                switch (Event.current.type)
                {
                    case EventType.DragUpdated:
                    case EventType.DragPerform:
                        if (!dropArea.Contains(Event.current.mousePosition))
                        {
                            break;
                        }

                        DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                        if (Event.current.type == EventType.DragPerform)
                        {
                            SerializedProperty arrayElement = null;
                            SerializedProperty assetProperty = null;

                            DragAndDrop.AcceptDrag();
                            foreach (Object draggedObject in DragAndDrop.objectReferences)
                            {
                                if (draggedObject.GetType() != typeof(SceneAsset))
                                {   // Skip anything that isn't a scene asset
                                    continue;
                                }

                                bool isDuplicate = false;
                                for (int i = 0; i < arrayProperty.arraySize; i++)
                                {
                                    arrayElement = arrayProperty.GetArrayElementAtIndex(i);
                                    assetProperty = arrayElement.FindPropertyRelative("Asset");
                                    if (assetProperty.objectReferenceValue != null && assetProperty.objectReferenceValue == draggedObject)
                                    {   
                                        isDuplicate = true;
                                        break;
                                    }
                                }

                                if (isDuplicate)
                                {   // Skip any duplicates
                                    Debug.LogWarning("Skipping " + draggedObject.name + " - it's already in the " + arrayProperty.displayName + " list.");
                                    continue;
                                }
                                
                                // Create the new element at 0
                                arrayProperty.InsertArrayElementAtIndex(0);
                                arrayProperty.serializedObject.ApplyModifiedProperties();

                                // Get the new element and assign the dragged object
                                arrayElement = arrayProperty.GetArrayElementAtIndex(0);
                                assetProperty = arrayElement.FindPropertyRelative("Asset");
                                assetProperty.objectReferenceValue = draggedObject;
                                arrayProperty.serializedObject.ApplyModifiedProperties();

                                // Move the new element to end of list
                                arrayProperty.MoveArrayElement(0, arrayProperty.arraySize - 1);
                                arrayProperty.serializedObject.ApplyModifiedProperties();
                            }
                        }
                        break;
                }
            }
        }
    }
}
