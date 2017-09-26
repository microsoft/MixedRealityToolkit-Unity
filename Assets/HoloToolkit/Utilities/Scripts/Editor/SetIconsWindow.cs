// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HoloToolkit.Unity
{
    public class SetIconsWindow : EditorWindow
    {
        private const string InitialOutputDirectoryName = "TileGenerator";
        private const float GUISectionOffset = 10.0f;
        private const string GUIHorizSpacer = "     ";
        private const string EditorPrefsKey_AppIcon = "_EditorPrefsKey_AppIcon";
        private const string EditorPrefsKey_SplashImage = "_EditorPrefsKey_SplashImage";
        private const string EditorPrefsKey_DirectoryName = "_EditorPrefsKey_DirectoryName";

        private static string _outputDirectoryName;
        private static string _originalAppIconPath;
        private static string _newAppIconPath;
        private static string _originalSplashImagePath;
        private static string _newSplashImagePath;
        private static Texture2D _originalAppIcon;
        private static Texture2D _originalSplashImage;
        private static float defaultLabelWidth;

        [MenuItem("HoloToolkit/Tile Generator", false, 0)]
        private static void OpenWindow()
        {
            // Dock it next to the inspector.
            Type inspectorType = Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll");
            var window = GetWindow<SetIconsWindow>(inspectorType);
            window.titleContent = new GUIContent("Tile Generator");
            window.minSize = new Vector2(320, 256);
            window.Show();
        }

        private void OnEnable()
        {
            // Load settings
            _originalAppIconPath = EditorPrefsUtility.GetEditorPref(EditorPrefsKey_AppIcon, _originalAppIconPath);
            _originalSplashImagePath = EditorPrefsUtility.GetEditorPref(EditorPrefsKey_SplashImage, _originalSplashImagePath);
            _outputDirectoryName = EditorPrefsUtility.GetEditorPref(EditorPrefsKey_DirectoryName, _outputDirectoryName);

            if (!string.IsNullOrEmpty(_originalAppIconPath))
            {
                _originalAppIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(_originalAppIconPath);
            }

            if (!string.IsNullOrEmpty(_originalSplashImagePath))
            {
                _originalSplashImage = AssetDatabase.LoadAssetAtPath<Texture2D>(_originalSplashImagePath);
            }

            if (string.IsNullOrEmpty(_outputDirectoryName))
            {
                _outputDirectoryName = Application.dataPath + "/" + InitialOutputDirectoryName;
            }

            defaultLabelWidth = EditorGUIUtility.labelWidth;
        }

        private void OnDisable()
        {
            SaveSettings();
        }

        private void OnGUI()
        {
            GUILayout.Space(GUISectionOffset);

            // Images section
            GUILayout.Label("Images");

            // Inputs for images
            _originalAppIcon = CreateImageInput("App Icon", 1240, 1240, _originalAppIcon, ref _originalAppIconPath);
            _originalSplashImage = CreateImageInput("Splash Image", 2480, 1200, _originalSplashImage, ref _originalSplashImagePath);

            if (GUILayout.Button("Choose Output Folder"))
            {
                _outputDirectoryName = EditorUtility.OpenFolderPanel("Output Folder", Application.dataPath, "");
            }

            // Input for directory name
            EditorGUIUtility.labelWidth = 85f;
            EditorGUILayout.TextField("Output folder:", _outputDirectoryName);
            EditorGUIUtility.labelWidth = defaultLabelWidth;

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            // Update Icons
            if (GUILayout.Button("Update\nIcons", GUILayout.Height(64f), GUILayout.Width(64f)))
            {
                if (_originalAppIcon == null)
                {
                    EditorUtility.DisplayDialog("App Icon not set", "Please select the App Icon first", "Ok");
                }
                else if (_originalSplashImage == null)
                {
                    EditorUtility.DisplayDialog("Splash Image not set", "Please select the Splash Image first", "Ok");
                }
                else
                {
                    EditorApplication.delayCall += ResizeImages;
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private static void SaveSettings()
        {
            EditorPrefsUtility.SetEditorPref(EditorPrefsKey_AppIcon, _originalAppIconPath);
            EditorPrefsUtility.SetEditorPref(EditorPrefsKey_SplashImage, _originalSplashImagePath);
            EditorPrefsUtility.SetEditorPref(EditorPrefsKey_DirectoryName, _outputDirectoryName);
        }

        private static Texture2D CreateImageInput(string imageTitle, int width, int height, Texture2D texture, ref string path)
        {
            EditorGUIUtility.labelWidth = 200f;
            var newIcon = (Texture2D)EditorGUILayout.ObjectField(GUIHorizSpacer + imageTitle + " (" + width + "x" + height + ")", texture, typeof(Texture2D), false);
            EditorGUIUtility.labelWidth = defaultLabelWidth;

            if (newIcon == null || newIcon == texture) { return newIcon; }

            if (newIcon.width != width && newIcon.height != height)
            {
                // reset
                EditorUtility.DisplayDialog("Invalid Image",
                    string.Format("{0} should be an image with preferred size of {1}x{2}. Provided image was: {3}x{4}.", imageTitle, width, height, newIcon.width, newIcon.height),
                    "Ok");
                newIcon = texture;
            }
            else
            {
                path = AssetDatabase.GetAssetPath(newIcon);
            }

            return newIcon;
        }

        private static void ResizeImages()
        {
            try
            {
                EditorUtility.DisplayProgressBar("Generating images", "Checking Texture Importers", 0);

                // Check if we need to reimport the original textures, for enabling reading.
                if (CheckTextureImporter(_originalAppIconPath) || CheckTextureImporter(_originalSplashImagePath))
                {
                    AssetDatabase.Refresh();
                }

                if (!Directory.Exists(_outputDirectoryName))
                {
                    Directory.CreateDirectory(_outputDirectoryName);
                }
                else
                {
                    foreach (string file in Directory.GetFiles(_outputDirectoryName))
                    {
                        File.Delete(file);
                    }
                }

                // Create a copy of the original images
                string outputDirectoryBasePath = _outputDirectoryName;
                outputDirectoryBasePath = outputDirectoryBasePath.Replace(Application.dataPath, "Assets");

                _newAppIconPath = outputDirectoryBasePath + "/BaseIcon_1240x1240.png";
                _newSplashImagePath = outputDirectoryBasePath + "/BaseSplashImage_2480x1200.png";

                AssetDatabase.CopyAsset(_originalAppIconPath, _newAppIconPath);
                AssetDatabase.CopyAsset(_originalSplashImagePath, _newSplashImagePath);

                // Set Default Icon in Player Settings (Multiple platforms, can be overridden per platform)
                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new[] { AssetDatabase.LoadAssetAtPath<Texture2D>(_newAppIconPath) });
                PlayerSettings.virtualRealitySplashScreen = AssetDatabase.LoadAssetAtPath<Texture2D>(_newSplashImagePath);

                // Loop through available types and scales for UWP
                var types = Enum.GetValues(typeof(PlayerSettings.WSAImageType)).Cast<PlayerSettings.WSAImageType>().ToList();
                var scales = Enum.GetValues(typeof(PlayerSettings.WSAImageScale)).Cast<PlayerSettings.WSAImageScale>().ToList();
                float progressTotal = types.Count * scales.Count;
                float progress = 0;
                bool canceled = false;

                foreach (var type in types)
                {
                    if (canceled)
                    {
                        break;
                    }

                    foreach (var scale in scales)
                    {
                        PlayerSettings.WSA.SetVisualAssetsImage(CloneAndResizeToFile(type, scale), type, scale);

                        progress++;
                        if (EditorUtility.DisplayCancelableProgressBar("Generating images", string.Format("Generating resized images {0} of {1}", progress, progressTotal), progress / progressTotal))
                        {
                            canceled = true;
                            break;
                        }
                    }
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

                if (canceled)
                {
                    EditorUtility.DisplayDialog("Generation canceled",
                        string.Format("{0} Images of {1} were resized and updated in the Player Settings.", progress, progressTotal),
                        "Ok");
                }
                else
                {
                    EditorUtility.DisplayDialog("Images resized!",
                        "All images were resized and updated in the Player Settings",
                        "Ok");
                }

                EditorUtility.ClearProgressBar();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                EditorUtility.ClearProgressBar();
            }
        }

        private static bool CheckTextureImporter(string assetPath)
        {
            var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (tImporter == null || tImporter.isReadable) { return false; }

            tImporter.isReadable = true;

            AssetDatabase.ImportAsset(assetPath);
            return true;
        }

        private static string CloneAndResizeToFile(PlayerSettings.WSAImageType type, PlayerSettings.WSAImageScale scale)
        {
            string texturePath = GetUWPImageTypeTexture(type, scale);

            if (string.IsNullOrEmpty(texturePath)) { return string.Empty; }

            var iconSize = GetUWPImageTypeSize(type, scale);
            if (iconSize == Vector2.zero) { return string.Empty; }

            if (iconSize.x == 1240 && iconSize.y == 1240 || iconSize.x == 2480 && iconSize.y == 1200) { return texturePath; }

            string filePath = string.Format("{0}/{1}_{2}x{3}.png", _outputDirectoryName, type.ToString(), iconSize.x, iconSize.y);
            filePath = filePath.Replace(Application.dataPath, "Assets");

            // Create copy of original image
            try
            {
                AssetDatabase.CopyAsset(texturePath, filePath);
                var clone = AssetDatabase.LoadAssetAtPath<Texture2D>(filePath);

                if (clone == null)
                {
                    Debug.LogError("Unable to load texture at " + filePath);
                    return string.Empty;
                }

                // Resize clone to desired size
                TextureScale.Bilinear(clone, (int)iconSize.x, (int)iconSize.y);
                clone.Compress(true);
                AssetDatabase.SaveAssets();

                if (clone.GetRawTextureData().Length > 204800)
                {
                    Debug.LogWarningFormat("{0} exceeds the minimum file size of 204,800 bytes, please use a smaller image for generating your icons.", clone.name);
                    return string.Empty;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return string.Empty;
            }

            return filePath;
        }

        private static string GetUWPImageTypeTexture(PlayerSettings.WSAImageType type, PlayerSettings.WSAImageScale scale)
        {
            switch (type)
            {
                case PlayerSettings.WSAImageType.PackageLogo:
                case PlayerSettings.WSAImageType.StoreTileLogo:
                case PlayerSettings.WSAImageType.StoreTileSmallLogo:
                case PlayerSettings.WSAImageType.StoreSmallTile:
                case PlayerSettings.WSAImageType.StoreLargeTile:
                case PlayerSettings.WSAImageType.UWPSquare44x44Logo:
                case PlayerSettings.WSAImageType.UWPSquare71x71Logo:
                case PlayerSettings.WSAImageType.UWPSquare150x150Logo:
                case PlayerSettings.WSAImageType.UWPSquare310x310Logo:
                    return _newAppIconPath;
                case PlayerSettings.WSAImageType.SplashScreenImage:
                case PlayerSettings.WSAImageType.StoreTileWideLogo:
                case PlayerSettings.WSAImageType.UWPWide310x150Logo:
                    if (scale != PlayerSettings.WSAImageScale.Target16 &&
                        scale != PlayerSettings.WSAImageScale.Target24 &&
                        scale != PlayerSettings.WSAImageScale.Target32 &&
                        scale != PlayerSettings.WSAImageScale.Target48 &&
                        scale != PlayerSettings.WSAImageScale.Target256)
                    {
                        return _newSplashImagePath;
                    }
                    else
                    {
                        return _newAppIconPath;
                    }
                case PlayerSettings.WSAImageType.PhoneAppIcon:
                case PlayerSettings.WSAImageType.PhoneSmallTile:
                case PlayerSettings.WSAImageType.PhoneMediumTile:
                case PlayerSettings.WSAImageType.PhoneWideTile:
                case PlayerSettings.WSAImageType.PhoneSplashScreen:
                    return string.Empty;
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

        private static Vector2 GetUWPImageTypeSize(PlayerSettings.WSAImageType type, PlayerSettings.WSAImageScale scale)
        {
            switch (scale)
            {
                case PlayerSettings.WSAImageScale.Target16:
                    return CreateSize(16);
                case PlayerSettings.WSAImageScale.Target24:
                    return CreateSize(24);
                case PlayerSettings.WSAImageScale.Target32:
                    return CreateSize(32);
                case PlayerSettings.WSAImageScale.Target48:
                    return CreateSize(48);
                case PlayerSettings.WSAImageScale.Target256:
                    return CreateSize(256);
                default:
                    return GetWSAImageTypeSize(type, scale);
            }
        }

        private static Vector2 GetWSAImageTypeSize(PlayerSettings.WSAImageType type, PlayerSettings.WSAImageScale scale)
        {
            switch (type)
            {
                case PlayerSettings.WSAImageType.PackageLogo:
                    return CreateSize(50);
                case PlayerSettings.WSAImageType.StoreTileLogo:
                    return CreateSize(150);
                case PlayerSettings.WSAImageType.StoreTileSmallLogo:
                    return CreateSize(30);
                case PlayerSettings.WSAImageType.StoreSmallTile:
                    return CreateSize(70);
                case PlayerSettings.WSAImageType.StoreLargeTile:
                    return CreateSize(310);
                case PlayerSettings.WSAImageType.PhoneAppIcon:
                    return CreateSize(44);
                case PlayerSettings.WSAImageType.PhoneSmallTile:
                    return CreateSize(71);
                case PlayerSettings.WSAImageType.PhoneMediumTile:
                    return CreateSize(150);
                case PlayerSettings.WSAImageType.UWPSquare44x44Logo:
                    return CreateSize(44);
                case PlayerSettings.WSAImageType.UWPSquare71x71Logo:
                    return CreateSize(71);
                case PlayerSettings.WSAImageType.UWPSquare150x150Logo:
                    return CreateSize(150);
                case PlayerSettings.WSAImageType.UWPSquare310x310Logo:
                    return CreateSize(310);

                // WIDE 31:15
                case PlayerSettings.WSAImageType.PhoneWideTile:
                case PlayerSettings.WSAImageType.StoreTileWideLogo:
                case PlayerSettings.WSAImageType.UWPWide310x150Logo:
                    return new Vector2(310, 150);
                case PlayerSettings.WSAImageType.SplashScreenImage:
                    return new Vector2(620, 300);
                case PlayerSettings.WSAImageType.PhoneSplashScreen:
                default:
                    var size = Vector2.zero;
                    float scaleFactor = float.Parse(scale.ToString().Replace("_", "")) * 0.01f;
                    size = size * scaleFactor;
                    size.x = (float)Math.Ceiling(size.x);
                    size.y = (float)Math.Ceiling(size.y);

                    if (size == Vector2.zero)
                    {
                        Debug.LogWarningFormat("Invalid image size for {0} with scale {1}", type, scale);
                    }

                    return size;
            }
        }

        private static Vector2 CreateSize(int size)
        {
            return new Vector2(size, size);
        }
    }
}