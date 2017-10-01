// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Renders the UI and handles update logic for MixedRealityToolkit/Configure/Apply Mixed Reality Scene Settings.
    /// </summary>
    public class SceneSettingsWindow : AutoConfigureWindow<SceneSettingsWindow.SceneSetting>
    {
        /// <summary>
        /// Can be found in the meta file of the camera prefab.  We use the GUID in case people move the toolkit folders & assets around in their own projects.
        /// TODO: Update prefab GUID to point to MixedRealityCamera.
        /// </summary>
        private const string CameraPrefabGUID = "d379ed0a5618c9f479f58bd83a2d0ad3";

        #region Nested Types

        public enum SceneSetting
        {
            AddMixedRealityCamera,
            CameraToOrigin,
        }

        #endregion // Nested Types

        #region Overrides / Event Handlers

        protected override void ApplySettings()
        {
            if (Values[SceneSetting.AddMixedRealityCamera])
            {
                Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(CameraPrefabGUID)));
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

            Close();
        }

        protected override void LoadSettings()
        {
            for (int i = (int)SceneSetting.CameraToOrigin; i <= (int)SceneSetting.CameraToOrigin; i++)
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