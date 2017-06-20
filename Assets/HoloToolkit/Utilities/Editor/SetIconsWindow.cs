using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HoloToolkit.Unity
{
    public class SetIconsWindow : EditorWindow
    {
        private const string WindowTitle = "Tile Generator";
        private const string InitialOutputDirectoryName = "TileGenerator";
        private const float GUISectionOffset = 10.0f;
        private const string GUIHorizSpacer = "     ";
        private const string EditorPrefsKey_AppIcon = "EditorPrefsKey_AppIcon";
        private const string EditorPrefsKey_SplashImage = "EditorPrefsKey_SplashImage";
        private const string EditorPrefsKey_DirectoryName = "EditorPrefsKey_DirectoryName";

        private string _outputDirectoryName;
        private string _originalAppIconPath;
        private string _originalSplashImagePath;
        private Texture2D _originalAppIcon;
        private Texture2D _originalSplashImage;

        [MenuItem("HoloToolkit/Tile Generator", false, 0)]
        private static void OpenWindow()
        {
            SetIconsWindow window = GetWindow<SetIconsWindow>(WindowTitle) as SetIconsWindow;
            if (window != null)
            {
                window.Show();
            }
        }

        private void OnEnable()
        {
            Setup();
        }

        private void OnDisable()
        {
            // Save settings
            EditorPrefs.SetString(EditorPrefsKey_AppIcon, _originalAppIconPath);
            EditorPrefs.SetString(EditorPrefsKey_SplashImage, _originalSplashImagePath);
            EditorPrefs.SetString(EditorPrefsKey_DirectoryName, _outputDirectoryName);
        }

        private void OnGUI()
        {
            GUILayout.Space(GUISectionOffset);

            // Setup
            int buttonWidth_Full = Screen.width - 25;

            // Images section
            GUILayout.BeginVertical();
            GUILayout.Label("Images");

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUIUtility.labelWidth = 200f;

            // Inputs for images
            _originalAppIcon = CreateImageInput("App Icon", 1240, 1240, _originalAppIcon, ref _originalAppIconPath);
            _originalSplashImage = CreateImageInput("Splash Image", 2480, 1200, _originalSplashImage, ref _originalSplashImagePath);

            // Input for directory name
            _outputDirectoryName = EditorGUILayout.TextField(GUIHorizSpacer + "Output folder", _outputDirectoryName);

            // Update Icons
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Update Icons", GUILayout.Width(buttonWidth_Full)))
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
                        // Resize Images
                        EditorApplication.delayCall += () => { ResizeImages(); };
                    }
                }
                GUI.enabled = true;
            }

            GUILayout.EndVertical();
        }

        private void Setup()
        {
            this.titleContent = new GUIContent(WindowTitle);
            this.minSize = new Vector2(600, 200);

            // Load settings
            _originalAppIconPath = EditorPrefs.GetString(EditorPrefsKey_AppIcon);
            _originalSplashImagePath = EditorPrefs.GetString(EditorPrefsKey_SplashImage);
            _outputDirectoryName = EditorPrefs.GetString(EditorPrefsKey_DirectoryName);

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
                _outputDirectoryName = InitialOutputDirectoryName;
            }
        }

        private Texture2D CreateImageInput(string title, int width, int height, Texture2D texture, ref string path)
        {
            var newIcon = (Texture2D)EditorGUILayout.ObjectField(GUIHorizSpacer + title + " (" + width + "x" + height + ")", texture, typeof(Texture2D), false);

            if (newIcon != null && newIcon != texture)
            {
                int newIconWidth, newIconHeight = 0;
                GetImageSize(newIcon, out newIconWidth, out newIconHeight);

                if (newIconWidth != width && newIconHeight != height)
                {
                    // reset
                    newIcon = texture;
                    EditorUtility.DisplayDialog("Invalid Image", title + " should be an image with prefered size of " + width + " x " + height + ". Provided image was: " + newIconWidth + " x " + newIconHeight + ".", "Ok");
                }
                else
                {
                    path = AssetDatabase.GetAssetPath(newIcon);
                }
            }

            return newIcon;
        }

        private void ResizeImages()
        {
            try
            {
                EditorUtility.DisplayProgressBar("Generating images", "Checking Texture Importers", 0);

                // Check if we need to reimport the original textures, for enabling reading.
                var reimportedAppIcon = CheckTextureImporter(_originalAppIconPath);
                var reimportedSplashImage = CheckTextureImporter(_originalSplashImagePath);

                if (reimportedAppIcon || reimportedSplashImage)
                {
                    AssetDatabase.Refresh();
                }

                // Create a copy of the original images
                string directoryPath = Application.dataPath + "/" + _outputDirectoryName;

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                File.Copy(_originalAppIconPath, directoryPath + "/1240x1240.png");
                File.Copy(_originalSplashImagePath, directoryPath + "/2480x1200.png");

                // Loop through available types and scales for UWP
                var types = Enum.GetValues(typeof(PlayerSettings.WSAImageType)).Cast<PlayerSettings.WSAImageType>().ToList();
                var scales = Enum.GetValues(typeof(PlayerSettings.WSAImageScale)).Cast<PlayerSettings.WSAImageScale>().ToList();
                float progressTotal = types.Count * scales.Count;
                float progress = 0;
                bool cancelled = false;

                foreach (var type in types)
                {
                    if (cancelled)
                    {
                        break;
                    }

                    foreach (var scale in scales)
                    {
                        var texture = GetUWPImageTypeTexture(type, scale);
                        if (texture != null)
                        {
                            string filename = null; //string.Format("{0}{1}.png", type, scale);
                            CloneAndResizeToFile(texture, type, scale, ref filename);
                            PlayerSettings.WSA.SetVisualAssetsImage(string.Format("Assets/{0}/{1}", _outputDirectoryName, filename), type, scale);

                            progress++;
                            cancelled = EditorUtility.DisplayCancelableProgressBar("Generating images", string.Format("Generating resized images {0} of {1}", progress, progressTotal), progress / progressTotal);
                            if (cancelled)
                            {
                                break;
                            }
                        }
                    }
                }

                if (!cancelled)
                {
                    // Set Default Icon in Player Settings (Multiple platforms, can be overridden per platform)
                    PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new[] { _originalAppIcon });

                    EditorUtility.DisplayDialog("Images resized!", "All images were resized and updated in the Player Settings", "Ok");
                }
                else
                {
                    EditorUtility.DisplayDialog("Generation cancelled", string.Format("{0} Images of {1} were resized and updated in the Player Settings.", progress, progressTotal), "Ok");
                }

                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();
            }
            catch (Exception)
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private bool CheckTextureImporter(string assetPath)
        {
            var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (tImporter != null && !tImporter.isReadable)
            {
                tImporter.isReadable = true;

                AssetDatabase.ImportAsset(assetPath);
                return true;
            }
            return false;
        }

        private string CloneAndResizeToFile(Texture2D texture, PlayerSettings.WSAImageType type, PlayerSettings.WSAImageScale scale, ref string fileName)
        {
            var iconSize = GetUWPImageTypeSize(type, scale);
            if (iconSize != Vector2.zero)
            {
                fileName = string.Format("{0}x{1}.png", iconSize.x, iconSize.y);
                return CloneAndResizeToFile(texture, iconSize, ref fileName);
            }

            return null;
        }

        private string CloneAndResizeToFile(Texture2D texture, Vector2 iconSize, ref string fileName)
        {
            string directoryPath = Application.dataPath + "/" + _outputDirectoryName;
            string filepath = directoryPath + "/" + fileName;

            if (!((iconSize.x == 1240 && iconSize.y == 1240) || (iconSize.x == 2480 && iconSize.y == 1200)))
            {

                // Create copy of original image
                var clone = CloneAndResizeToTexture(texture, iconSize);

                // Write clone to assets folder
                var bytes = clone.EncodeToPNG();
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                File.WriteAllBytes(filepath, bytes);
            }

            return filepath;
        }

        private Texture2D CloneAndResizeToTexture(Texture2D texture, Vector2 iconSize)
        {
            // Create copy of original image
            var clone = Instantiate(texture);

            // Resize clone to desired size
            TextureScale.Bilinear(clone, (int)iconSize.x, (int)iconSize.y);

            return clone;
        }

        private Texture2D GetUWPImageTypeTexture(PlayerSettings.WSAImageType type, PlayerSettings.WSAImageScale scale)
        {
            switch (type)
            {
                case PlayerSettings.WSAImageType.PackageLogo:
                case PlayerSettings.WSAImageType.StoreTileLogo:
                case PlayerSettings.WSAImageType.StoreTileSmallLogo:
                case PlayerSettings.WSAImageType.StoreSmallTile:
                case PlayerSettings.WSAImageType.StoreLargeTile:
                case PlayerSettings.WSAImageType.PhoneAppIcon:
                case PlayerSettings.WSAImageType.PhoneSmallTile:
                case PlayerSettings.WSAImageType.PhoneMediumTile:
                case PlayerSettings.WSAImageType.PhoneWideTile:
                case PlayerSettings.WSAImageType.PhoneSplashScreen:
                case PlayerSettings.WSAImageType.UWPSquare44x44Logo:
                case PlayerSettings.WSAImageType.UWPSquare71x71Logo:
                case PlayerSettings.WSAImageType.UWPSquare150x150Logo:
                case PlayerSettings.WSAImageType.UWPSquare310x310Logo:
                    return _originalAppIcon;
                case PlayerSettings.WSAImageType.SplashScreenImage:
                case PlayerSettings.WSAImageType.StoreTileWideLogo:
                case PlayerSettings.WSAImageType.UWPWide310x150Logo:
                    if (scale != PlayerSettings.WSAImageScale.Target16 &&
                        scale != PlayerSettings.WSAImageScale.Target24 &&
                        scale != PlayerSettings.WSAImageScale.Target32 &&
                        scale != PlayerSettings.WSAImageScale.Target48 &&
                        scale != PlayerSettings.WSAImageScale.Target256)
                    {
                        return _originalSplashImage;
                    }
                    else
                    {
                        break;
                    }
                default:
                    break;
            }

            return null;
        }

        private Vector2 GetUWPImageTypeSize(PlayerSettings.WSAImageType type, PlayerSettings.WSAImageScale scale)
        {
            Vector2 size = Vector2.zero;

            switch (scale)
            {
                case PlayerSettings.WSAImageScale.Target16:
                    size = CreateSize(16);
                    break;
                case PlayerSettings.WSAImageScale.Target24:
                    size = CreateSize(24);
                    break;
                case PlayerSettings.WSAImageScale.Target32:
                    size = CreateSize(32);
                    break;
                case PlayerSettings.WSAImageScale.Target48:
                    size = CreateSize(48);
                    break;
                case PlayerSettings.WSAImageScale.Target256:
                    size = CreateSize(256);
                    break;
                default:
                    size = GetWSAImageTypeSize(type, scale);
                    break;
            }

            return size;
        }

        private Vector2 GetWSAImageTypeSize(PlayerSettings.WSAImageType type, PlayerSettings.WSAImageScale scale)
        {
            Vector2 size = Vector2.zero;
            float scaleFactor = (float)scale / 100;

            switch (type)
            {
                case PlayerSettings.WSAImageType.PackageLogo:
                    size = CreateSize(50);
                    break;
                case PlayerSettings.WSAImageType.StoreTileLogo:
                    size = CreateSize(150);
                    break;
                case PlayerSettings.WSAImageType.StoreTileSmallLogo:
                    size = CreateSize(30);
                    break;
                case PlayerSettings.WSAImageType.StoreSmallTile:
                    size = CreateSize(70);
                    break;
                case PlayerSettings.WSAImageType.StoreLargeTile:
                    size = CreateSize(310);
                    break;
                case PlayerSettings.WSAImageType.PhoneAppIcon:
                    size = CreateSize(44);
                    break;
                case PlayerSettings.WSAImageType.PhoneSmallTile:
                    size = CreateSize(71);
                    break;
                case PlayerSettings.WSAImageType.PhoneMediumTile:
                    size = CreateSize(150);
                    break;
                case PlayerSettings.WSAImageType.PhoneSplashScreen:
                    break;
                case PlayerSettings.WSAImageType.UWPSquare44x44Logo:
                    size = CreateSize(44);
                    break;
                case PlayerSettings.WSAImageType.UWPSquare71x71Logo:
                    size = CreateSize(71);
                    break;
                case PlayerSettings.WSAImageType.UWPSquare150x150Logo:
                    size = CreateSize(150);
                    break;
                case PlayerSettings.WSAImageType.UWPSquare310x310Logo:
                    size = CreateSize(310);
                    break;

                // WIDE 31:15
                case PlayerSettings.WSAImageType.PhoneWideTile:
                case PlayerSettings.WSAImageType.StoreTileWideLogo:
                case PlayerSettings.WSAImageType.UWPWide310x150Logo:
                    size = new Vector2(310, 150);
                    break;
                case PlayerSettings.WSAImageType.SplashScreenImage:
                    size = new Vector2(620, 300);
                    break;
                default:
                    break;
            }

            if (size != Vector2.zero)
            {
                size = (size * scaleFactor);
                size.x = (float)Math.Ceiling(size.x);
                size.y = (float)Math.Ceiling(size.y);
            }
            return size;
        }

        private Vector2 CreateSize(int size)
        {
            return new Vector2(size, size);
        }

        private bool GetImageSize(Texture2D asset, out int width, out int height)
        {
            if (asset != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(asset);
                TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

                if (importer != null)
                {
                    object[] args = new object[2] { 0, 0 };
                    MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
                    mi.Invoke(importer, args);

                    width = (int)args[0];
                    height = (int)args[1];

                    return true;
                }
            }

            height = width = 0;
            return false;
        }
    }
}