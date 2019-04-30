// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEditor.Experimental.AssetImporters;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit.Input
{
    internal static class InputAnimationImportUtils
    {
        public const string Extension = "inputanim";
    }

    [ScriptedImporter(1, InputAnimationImportUtils.Extension)]
    public class InputAnimationImporter : ScriptedImporter
    {
        // public float m_Scale = 1;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            using (FileStream fs = new FileStream(ctx.assetPath, FileMode.Open))
            {
                var asset = ScriptableObject.CreateInstance<InputAnimationAsset>();
                asset.InputAnimation.FromStream(fs);

                ctx.AddObjectToAsset("animation", asset);
                ctx.SetMainObject(asset);

                fs.Close();
            }
        }
    }

    public class InputAnimationExporter
    {
        public static void ExportInputAnimation(string filePath, InputAnimation animation)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                animation.ToStream(fs);
                fs.Close();
            }
        }

#if UNITY_EDITOR
        [MenuItem("Mixed Reality Toolkit/Utilities/Export Input Animation")]
        static void ExportInputAnimationMenuItem()
        {
            InputAnimationAsset asset = Selection.activeObject as InputAnimationAsset;
            if (asset != null)
            {
                var path = EditorUtility.SaveFilePanel("Export input animation", "", asset.name + "." + InputAnimationImportUtils.Extension, InputAnimationImportUtils.Extension);

                if (path.Length != 0)
                {
                    InputAnimationExporter.ExportInputAnimation(path, asset.InputAnimation);
                }
            }
        }

        [MenuItem("Mixed Reality Toolkit/Utilities/Export Input Animation", true)]
        static bool ValidateExportInputAnimationMenuItem()
        {
            InputAnimationAsset asset = Selection.activeObject as InputAnimationAsset;
            return asset != null;
        }
#endif
    }
}