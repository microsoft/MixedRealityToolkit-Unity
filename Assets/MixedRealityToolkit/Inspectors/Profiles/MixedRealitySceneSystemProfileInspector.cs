// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information. 

using Microsoft.MixedReality.Toolkit.SceneSystem;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(MixedRealitySceneSystemProfile))]
    public class MixedRealitySceneSystemProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        const float DragAreaWidth = 0f;
        const float DragAreaHeight = 50f;
        const float DragAreaOffset = 10;

        private static bool showManagerProperties = true;
        private SerializedProperty useManagerScene;
        private SerializedProperty managerScene;

        private static bool showLightingProperties = true;
        private SerializedProperty useLightingScene;
        private SerializedProperty defaultLightingSceneIndex;
        private SerializedProperty lightingScenes;

        private static bool showContentProperties = true;
        private SerializedProperty contentScenes;

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

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured())
            {
                return;
            }

            if (DrawBacktrackProfileButton("Back to Configuration Profile", MixedRealityToolkit.Instance.ActiveProfile))
            {
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Scene System Profile", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("The Scene System Profile helps configure your scene settings.", MessageType.Info);

            CheckProfileLock(target);

            serializedObject.Update();

            EditorGUILayout.Space();

            showManagerProperties = EditorGUILayout.Foldout(showManagerProperties, "Manager Scene Settings", true);
            if (showManagerProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(useManagerScene);
                    EditorGUILayout.PropertyField(managerScene, includeChildren: true);
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
                        defaultLightingSceneIndex.intValue = EditorGUILayout.IntSlider(defaultLightingSceneIndex.displayName, defaultLightingSceneIndex.intValue, 0, lightingScenes.arraySize - 1);
                        EditorGUILayout.PropertyField(lightingScenes, includeChildren: true);
                        DrawSceneInfoDragAndDrop(lightingScenes);
                    }
                }
            }

            EditorGUILayout.Space();
            showContentProperties = EditorGUILayout.Foldout(showContentProperties, "Content Scene Settings", true);
            if (showContentProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(contentScenes, includeChildren: true);
                    DrawSceneInfoDragAndDrop(contentScenes);
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
                            return;
                        }

                        DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                        if (Event.current.type == EventType.DragPerform)
                        {
                            SerializedProperty arrayElement = null;
                            SerializedProperty assetProperty = null;
                            bool changed = false;

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

                                int newIndex = arrayProperty.arraySize;
                                arrayProperty.InsertArrayElementAtIndex(newIndex);
                                arrayProperty.serializedObject.ApplyModifiedProperties();

                                arrayElement = arrayProperty.GetArrayElementAtIndex(newIndex);
                                assetProperty = arrayElement.FindPropertyRelative("Asset");
                                assetProperty.objectReferenceValue = draggedObject;
                                changed = true;
                            }

                            if (changed)
                            {
                                arrayProperty.serializedObject.ApplyModifiedProperties();
                            }
                        }
                        break;
                }
            }
        }
    }
}
