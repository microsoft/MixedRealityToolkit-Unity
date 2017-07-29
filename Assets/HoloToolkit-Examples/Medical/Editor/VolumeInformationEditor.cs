// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEditor;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Editor script for editor configuration of volume files
    /// Conversion of stacked images to volumes happens here
    /// </summary>
    [CustomEditor(typeof(VolumeInformation))]
    public class VolumeInformationEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var volInfo = this.target as VolumeInformation;

            base.OnInspectorGUI();
            
            if (GUILayout.Button("Bake"))
            {
                if (volInfo == null)
                {
                    throw new ArgumentNullException();
                }

                var texPath = EditorUtility.SaveFilePanel("Save volume", Application.dataPath, "volume", "asset");
                texPath = texPath.Replace(Application.dataPath, "Assets");

                AssetDatabase.DeleteAsset(texPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                var size = Int3.zero;
                var bakedSliceData = VolumeImportImages.ConvertFolderToVolume(Application.dataPath + volInfo.ImageSourceFolder,
                                                                              volInfo.InferAlpha, out size);

                if (volInfo.AutoSizeOnBake)
                {
                    volInfo.Size = size;
                }

                var volumeSizePow2 = MathExtensions.PowerOfTwoGreaterThanOrEqualTo(volInfo.Size);
                var tex3D = VolumeTextureUtils.BuildTexture(bakedSliceData, volInfo.Size, volumeSizePow2);

                AssetDatabase.CreateAsset(tex3D, texPath);
                AssetDatabase.SaveAssets();

                volInfo.BakedTexture = AssetDatabase.LoadAssetAtPath<Texture3D>(texPath);

                Debug.Log("Baked volume to: " + texPath);

                EditorUtility.SetDirty(volInfo);
            }
        }
    }
}