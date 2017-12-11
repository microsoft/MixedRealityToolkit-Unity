// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using HoloToolkit.Unity.InputModule;
using Cursor = HoloToolkit.Unity.InputModule.Cursor;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Renders the UI and handles update logic for MixedRealityToolkit/Configure/Apply Mixed Reality Scene Settings.
    /// </summary>
    public class SceneSettingsWindow : AutoConfigureWindow<SceneSettingsWindow.SceneSetting>
    {
        /// <summary>
        /// Can be found in the meta file of the camera prefab.  We use the GUID in case people move the toolkit folders &amp; assets around in their own projects.
        /// <remarks>Currently points to the MixedRealityCameraParent.prefab</remarks>
        /// </summary>
        private const string CameraPrefabGUID = "d29bc40b7f3df26479d6a0aac211c355";

        /// <summary>
        /// Can be found in the meta file of the camera prefab.  We use the GUID in case people move the toolkit folders &amp; assets around in their own projects.
        /// <remarks>Currently points to the InputManager.prefab</remarks>
        /// </summary>
        private const string InputSystemPrefabGUID = "3eddd1c29199313478dd3f912bfab2ab";

        /// <summary>
        /// Can be found in the meta file of the camera prefab.  We use the GUID in case people move the toolkit folders &amp; assets around in their own projects.
        /// <remarks>Currently points to the DefaultCursor.prefab</remarks>
        /// </summary>
        private const string DefaultCursorPrefabGUID = "a611e772ef8ddf64d8106a9cbb70f31c";

        #region Nested Types

        public enum SceneSetting
        {
            AddMixedRealityCamera,
            CameraToOrigin,
            AddInputSystem,
            AddDefaultCursor,
        }

        #endregion // Nested Types

        #region Overrides / Event Handlers

        protected override void ApplySettings()
        {
            if (Values[SceneSetting.AddMixedRealityCamera])
            {
                if (CameraCache.Main != null)
                {
                    DestroyImmediate(CameraCache.Main.gameObject.GetParentRoot());
                }

                PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(CameraPrefabGUID)));
            }

            var mainCamera = CameraCache.Refresh(Camera.main);

            if (mainCamera == null)
            {
                Debug.LogWarning("Could not find a valid \"MainCamera\"!  Unable to update camera position.");
            }
            else
            {
                if (Values[SceneSetting.CameraToOrigin])
                {
                    mainCamera.transform.position = Vector3.zero;
                }
            }

            if (Values[SceneSetting.AddInputSystem])
            {
                var inputManager = FindObjectOfType<InputManager>();
                if (inputManager != null)
                {
                    DestroyImmediate(inputManager.gameObject);
                }

                var eventSystems = FindObjectsOfType<EventSystem>();
                foreach (var eventSystem in eventSystems)
                {
                    DestroyImmediate(eventSystem.gameObject);
                }

                var inputModules = FindObjectsOfType<StandaloneInputModule>();
                foreach (var inputModule in inputModules)
                {
                    DestroyImmediate(inputModule.gameObject);
                }

                PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(InputSystemPrefabGUID)));
                FocusManager.Instance.UpdateCanvasEventSystems();
            }

            if (Values[SceneSetting.AddDefaultCursor])
            {
                var cursors = FindObjectsOfType<Cursor>();
                foreach (var cursor in cursors)
                {
                    DestroyImmediate(cursor.gameObject.GetParentRoot());
                }

                PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(DefaultCursorPrefabGUID)));

                FindObjectOfType<InputManager>().GetComponent<SimpleSinglePointerSelector>().Cursor = FindObjectOfType<Cursor>();
            }

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

            Close();
        }

        protected override void LoadSettings()
        {
            for (int i = 0; i <= (int)SceneSetting.AddDefaultCursor; i++)
            {
                Values[(SceneSetting)i] = true;
            }
        }

        protected override void OnGuiChanged()
        {
        }

        protected override void LoadStrings()
        {
            Names[SceneSetting.AddMixedRealityCamera] = "Add the Mixed Reality Camera Prefab";
            Descriptions[SceneSetting.AddMixedRealityCamera] =
                "Recommended\n\n" +
                "Adds the Mixed Reality Camera Prefab to the scene.\n\n" +
                "The prefab comes preset with all the components and options for automatically handling Occluded and Transparent Mixed Reality Applications.";

            Names[SceneSetting.CameraToOrigin] = "Move Camera to Origin";
            Descriptions[SceneSetting.CameraToOrigin] =
                "Recommended\n\n" +
                "Moves the main camera to the world origin of the scene (0, 0, 0).\n\n" +
                "<color=#ffff00ff><b>Note:</b></color> When a Mixed Reality application starts, the users head is the center of the world. By not having your Main Camera centered at " +
                "the world origin (0, 0, 0) will result in GameObjects not appearing where they are expected. This option should remain checked unless you have alternative methods " +
                "that explicitly deal with any apparent offset.";

            Names[SceneSetting.AddInputSystem] = "Add the Input Manager Prefab";
            Descriptions[SceneSetting.AddInputSystem] =
                "Recommended\n\n" +
                "Adds the Input Manager Prefab to the scene.\n\n" +
                "The prefab comes preset with all the components and options for automatically handling input for Mixed Reality Applications.\n\n" +
                "<color=#ff0000ff><b>Warning!</b></color> This will remove and replace any currently existing Input Managers or Event Systems in your scene.";

            Names[SceneSetting.AddDefaultCursor] = "Add the Default Cursor Prefab";
            Descriptions[SceneSetting.AddDefaultCursor] =
                "Recommended\n\n" +
                "Adds the  Default Cursor Prefab to the scene.\n\n" +
                "The prefab comes preset with all the components and options for automatically handling cursor animations for Mixed Reality Applications.\n\n" +
                "<color=#ff0000ff><b>Warning!</b></color> This will remove and replace any currently existing Cursors in your scene.";
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            minSize = new Vector2(350, 250);
            maxSize = minSize;
        }
        #endregion // Overrides / Event Handlers
    }
}