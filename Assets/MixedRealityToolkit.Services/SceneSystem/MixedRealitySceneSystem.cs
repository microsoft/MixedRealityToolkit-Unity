// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Microsoft.MixedReality.Toolkit.SceneSystem
{
    /// <summary>
    /// The default implementation of the <see cref="Microsoft.MixedReality.Toolkit.SceneSystem.IMixedRealitySceneSystem"/>
    /// </summary>
    public class MixedRealitySceneSystem : BaseCoreSystem, IMixedRealitySceneSystem
    {
        public MixedRealitySceneSystem(
            IMixedRealityServiceRegistrar registrar,
            MixedRealitySceneSystemProfile profile) : base(registrar, profile)
        {
            this.profile = profile;
        }

        private MixedRealitySceneSystemProfile profile;

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object x, object y)
        {
            // There shouldn't be other Boundary Managers to compare to.
            return false;
        }

        /// <inheritdoc />
        int IEqualityComparer.GetHashCode(object obj)
        {
            return Mathf.Abs(SourceName.GetHashCode());
        }

        /// <inheritdoc />
        public uint SourceId { get; } = 0;

        /// <inheritdoc />
        public string SourceName { get; } = "Mixed Reality Scene System";

        public override void Initialize()
        {
#if UNITY_EDITOR
            // Subscribe to editor events
            EditorApplication.playModeStateChanged += playModeStateChanged;
            EditorApplication.projectChanged += projectChanged;
            EditorSceneManager.newSceneCreated += newSceneCreated;
            EditorSceneManager.sceneOpened += sceneOpened;
            EditorSceneManager.sceneClosed += sceneClosed;
            EditorSceneManager.activeSceneChangedInEditMode += activeSceneChangedInEditMode;

            UpdateBuildSettings();
#endif
        }

        public override void Update()
        {
            Debug.Log("Updating MixedRealitySceneSystem");
#if UNITY_EDITOR
            UpdateBuildSettings();
#endif
        }


#if UNITY_EDITOR
        private bool updatingBuildSettings = false;

        private void playModeStateChanged(PlayModeStateChange change)
        {
            switch (change)
            {
                case PlayModeStateChange.EnteredEditMode:
                case PlayModeStateChange.ExitingPlayMode:
                    UpdateBuildSettings();
                    break;
            }
        }

        private void activeSceneChangedInEditMode(Scene arg0, Scene arg1)
        {
            UpdateBuildSettings();
        }

        private void projectChanged()
        {
            UpdateBuildSettings();
        }

        private void sceneClosed(Scene scene)
        {
            UpdateBuildSettings();
        }

        private void sceneOpened(Scene scene, OpenSceneMode mode)
        {
            UpdateBuildSettings();
        }

        private void newSceneCreated(Scene scene, NewSceneSetup setup, NewSceneMode mode)
        {
            UpdateBuildSettings();
        }

        private void UpdateBuildSettings()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
                return;
            
            if (updatingBuildSettings)
                return;

            updatingBuildSettings = true;

            if (profile.UseManagerScene && profile.ManagerSceneObject != null)
            {
                long localID;
                string managerGuidString;
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(profile.ManagerSceneObject, out managerGuidString, out localID);
                GUID managerGuid = new GUID(managerGuidString);

                if (EditorBuildSettings.scenes.Length == 0 ||
                   EditorBuildSettings.scenes[0].guid != managerGuid)
                {
                    Debug.LogWarning("Manager scene '" + profile.ManagerSceneObject.name + "' was not first in build order. Changing build settings now.");

                    EditorBuildSettingsScene buildScene = new EditorBuildSettingsScene(managerGuid, true);
                    var scenes = EditorBuildSettings.scenes
                        .Where(scene => scene.guid != managerGuid)
                        .Prepend(buildScene)
                        .ToArray();
                    EditorBuildSettings.scenes = scenes;
                    Debug.Assert(EditorBuildSettings.scenes[0].guid == managerGuid);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                Scene editorManagerScene = EditorSceneManager.GetSceneByName(profile.ManagerSceneObject.name);
                if (!editorManagerScene.isLoaded)
                {
                    string managerScenePath = AssetDatabase.GetAssetOrScenePath(profile.ManagerSceneObject);
                    EditorSceneManager.OpenScene(managerScenePath, OpenSceneMode.Additive);
                }
                EditorSceneManager.SetActiveScene(editorManagerScene);
                // Move the manager scene to first in order
                if (EditorSceneManager.sceneCount >= 1)
                {
                    Scene nextScene = EditorSceneManager.GetSceneAt(0);
                    EditorSceneManager.MoveSceneBefore(editorManagerScene, nextScene);
                }
            }

            updatingBuildSettings = false;
        }
#endif
    }
}