// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEditor;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public static class InputAnimationMenu
    {
        [MenuItem("Mixed Reality Toolkit/Utilities/Import Input Animation")]
        private static void ImportInputAnimation()
        {
            string filepath = EditorUtility.OpenFilePanel(
                "Select input animation file",
                "",
                InputAnimationSerializationUtils.Extension);

            if (filepath.Length > 0)
            {
                try
                {
                    AnimationClip clip = null;
                    using (FileStream fs = new FileStream(filepath, FileMode.Open))
                    {
                        clip = new AnimationClip();
                        InputAnimationSerializationUtils.AnimationClipFromStream(clip, fs);
                    }

                    if (clip)
                    {
                        string newFilename = Path.GetFileNameWithoutExtension(filepath) + ".asset";
                        string newFilepath = Path.Combine(GetOutputAssetDirectory(), newFilename);
                        AssetDatabase.CreateAsset(clip, newFilepath);
                    }
                }
                catch (IOException ex)
                {
                    Debug.LogError(ex.Message);
                }
            }
        }

        [MenuItem("Mixed Reality Toolkit/Utilities/Export Input Animation")]
        private static void ExportInputAnimation()
        {
            AnimationClip clip = Selection.activeObject as AnimationClip;
            if (!clip)
            {
                return;
            }

            string filepath = AssetDatabase.GetAssetPath(clip);
            string filename = Path.GetFileName(filepath);
            string newFilename = Path.ChangeExtension(filename, InputAnimationSerializationUtils.Extension);

            string outputPath = EditorUtility.SaveFilePanel(
                "Select output path",
                GetOutputAssetDirectory(),
                newFilename,
                InputAnimationSerializationUtils.Extension);

            if (outputPath.Length > 0)
            {
                try
                {
                    using (FileStream fs = new FileStream(outputPath, FileMode.Create))
                    {
                        InputAnimationSerializationUtils.AnimationClipToStream(clip, fs);
                    }
                }
                catch (IOException ex)
                {
                    Debug.LogError(ex.Message);
                }
            }
        }

        [MenuItem("Mixed Reality Toolkit/Utilities/Export Input Animation", true)]
        private static bool ExportInputAnimationValidation()
        {
            return Selection.activeObject is AnimationClip;
        }

        private static string GetOutputAssetDirectory()
        {
            string directory = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (directory.Length == 0)
            {
                directory = "Assets";
            }
            else if (File.Exists(directory))
            {
                directory = Path.GetDirectoryName(directory);
            }
            return directory;
        }
    }
}
