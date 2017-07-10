// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HoloToolkit.Unity
{
    [CustomEditor(typeof(HeadsetAdjustment))]
    public class HeadsetAdjustmentEditor : Editor
    {
        private static SceneAsset sceneAsset;
        private static Object sceneObj;
        private static HeadsetAdjustment myTarget;

        private void OnEnable()
        {
            myTarget = (HeadsetAdjustment)target;
            sceneAsset = GetSceneObject(myTarget.NextSceneName);
        }

        public override void OnInspectorGUI()
        {
            sceneObj = EditorGUILayout.ObjectField(
                new GUIContent("Next Scene", "The name of the scene to load when the user is ready. If left empty, " +
                                             "the next scene is loaded as specified in the 'Scenes in Build')"),
                sceneAsset,
                typeof(SceneAsset),
                true);

            if (GUI.changed)
            {
                if (sceneObj == null)
                {
                    sceneAsset = null;
                    myTarget.NextSceneName = string.Empty;
                }
                else
                {
                    sceneAsset = GetSceneObject(sceneObj.name);
                    myTarget.NextSceneName = sceneObj.name;
                }
            }
        }

        private static SceneAsset GetSceneObject(string sceneObjectName)
        {
            if (string.IsNullOrEmpty(sceneObjectName))
            {
                return null;
            }

            foreach (EditorBuildSettingsScene editorScene in EditorBuildSettings.scenes)
            {
                if (editorScene.path.IndexOf(sceneObjectName, StringComparison.Ordinal) != -1)
                {
                    return AssetDatabase.LoadAssetAtPath(editorScene.path, typeof(SceneAsset)) as SceneAsset;
                }
            }

            Debug.LogWarning("Scene [" + sceneObjectName + "] cannot be used.  To use this scene add it to the build settings for the project.");
            return null;
        }
    }
}
