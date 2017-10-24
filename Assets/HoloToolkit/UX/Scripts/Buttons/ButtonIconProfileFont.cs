//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using MRDL;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace HoloToolkit.Unity.Buttons
{
    /// <summary>
    /// Icon profile that returns font characters
    /// For simplicity's sake this profile is hard-coded to use the HoloSymMDL2 font
    /// However it could easily be rewritten to use any font asset, or a dynamically generated OS font
    /// /// </summary>
    public class ButtonIconProfileFont : ButtonIconProfile
    {
        [Header("Font Settings")]
        /// <summary>
        /// Name of the font this profile uses
        /// </summary>
        //public const string DefaultUnicodeFont = "Segoe MDL2 Assets";
        public const string DefaultUnicodeFont = "HoloLens MDL2 Symbols";

        // Used to make renderer scales more manageable
        const float RendererScaleMultiplier = 0.01f;

        // Scale factor is the more intuitive way to adjust font size
        // So we use a default font size value
        const int FontSize = 75;

        /// <summary>
        /// Default is set to the default range of usable characters for the HoloLens font
        /// </summary>
        public static string[] UnicodeKeys = new string[] {
            "E700","E70D","E70E","E710","E711","E71E","E720",
            "E722","E72A","E72B","E72C","E72E","E738","E760",
            "E761","E765","E767","E76B","E76C","E77B","E783",
            "E7BA","E894","E8A7","E8FB","E930","E9EA","EA39",
            "EBD2","EC90","EC97","EC98","ECA2","ED17"
        };

        /// <summary>
        /// The name of the font being used
        /// </summary>
        [Header("Font Asset Name")]
        public string OSFontName = DefaultUnicodeFont;

        [Tooltip("Scale of the text generator's mesh")]
        public float RendererScale = 0.001f;

        [Tooltip("Size of the rendered font")]
        public int FontScaleFactor = 16;

        public Font IconFont {
            get {
                return iconFont;
            }
            set {
                if (iconFont != value) {
                    // We'll need to re-initialize
                    initialized = false;
                    iconFont = value;
                }
            }
        }

        [SerializeField]
        private Font iconFont;
        private TextGenerationSettings settings = new TextGenerationSettings();
        private TextGenerator generator;
        private bool initialized = false;
        private string charactersInFont;

        public override List<string> GetIconKeys() {
            Initialize();

            return new List<string>(UnicodeKeys);
        }

        public override bool GetIcon(string iconName, MeshRenderer targetRenderer, MeshFilter targetMesh, bool useDefaultIfNotFound) {
            Initialize();

            if (targetRenderer == null || targetMesh == null) {
                return false;
            }

            Texture2D icon = null;
            if (useDefaultIfNotFound) {
                icon = _IconNotFound;
            }

            bool useDefaultMesh = true;

            if (IconFont != null) {
                if (!string.IsNullOrEmpty(iconName)) {
                    if (GetIntValueFromHex(iconName) == 0) {
                        if (useDefaultIfNotFound) {
                            icon = _IconNotFound;
                        }
                        return icon != null;
                    }
                    string textValue = GetCharStringFromHex(iconName);

                    if (!string.IsNullOrEmpty(textValue)) {
                        Mesh sharedMesh = targetMesh.sharedMesh;
                        // Use the character info to generate text
                        if (generator == null) {
                            generator = new TextGenerator(1);
                        }

                        // Prep our text settings
                        settings.font = iconFont;
                        settings.fontStyle = FontStyle.Normal;
                        settings.fontSize = FontSize;
                        settings.verticalOverflow = VerticalWrapMode.Overflow;
                        settings.generateOutOfBounds = true;
                        settings.textAnchor = TextAnchor.MiddleCenter;
                        settings.color = Color.white;
                        settings.richText = true;
                        settings.pivot = Vector2.zero;
                        settings.resizeTextForBestFit = false;
                        settings.scaleFactor = FontScaleFactor;

                        // Create the text mesh
                        generator.Populate(textValue, settings);
                        CreateMeshFromGenerator(generator, ref sharedMesh);

                        // Assign our mesh and texture
                        targetMesh.transform.localScale = Vector3.one * RendererScale * RendererScaleMultiplier;
                        icon = (Texture2D)IconFont.material.mainTexture;
                        targetRenderer.sharedMaterial.mainTexture = icon;
                        targetMesh.sharedMesh = sharedMesh;
                        useDefaultMesh = false;
                    }
                }
            }
            targetRenderer.sharedMaterial.mainTexture = icon;
            if (useDefaultMesh) {
                targetMesh.sharedMesh = IconMesh;
                targetMesh.transform.localScale = Vector3.one;
            }
            return icon != null;
        }

        private void Initialize() {
            if (initialized) {
                return;
            }

            StringBuilder charsInFontString = new StringBuilder();
            foreach (string hexString in UnicodeKeys) {
                charsInFontString.Append(GetCharStringFromHex(hexString));
            }

            charactersInFont = charsInFontString.ToString();

            if (iconFont == null) {
                return;
            }

            // Make sure we have all the characters we need
            iconFont.RequestCharactersInTexture(charactersInFont, FontSize, FontStyle.Normal);

            initialized = true;
        }

        private void CreateMeshFromGenerator(TextGenerator gen, ref Mesh targetMesh) {
            if (targetMesh == null) {
                targetMesh = new Mesh();
            }

            int vertSize = gen.vertexCount;
            Vector3[] tempVerts = new Vector3[vertSize];
            Color32[] tempColours = new Color32[vertSize];
            Vector2[] tempUvs = new Vector2[vertSize];
            IList<UIVertex> generatorVerts = gen.verts;
            for (int i = 0; i < vertSize; ++i) {
                tempVerts[i] = generatorVerts[i].position;
                tempColours[i] = generatorVerts[i].color;
                tempUvs[i] = generatorVerts[i].uv0;
            }

            // If the temp verts is zero, something's gone wrong
            // Possibly an empty character
            // Don't assign anything
            if (tempVerts.Length == 0) {
                return;
            }

            targetMesh.vertices = tempVerts;
            targetMesh.colors32 = tempColours;
            targetMesh.uv = tempUvs;

            int characterCount = vertSize / 4;
            int[] tempIndices = new int[characterCount * 6];
            for (int i = 0; i < characterCount; ++i) {
                int vertIndexStart = i * 4;
                int trianglesIndexStart = i * 6;
                tempIndices[trianglesIndexStart++] = vertIndexStart;
                tempIndices[trianglesIndexStart++] = vertIndexStart + 1;
                tempIndices[trianglesIndexStart++] = vertIndexStart + 2;
                tempIndices[trianglesIndexStart++] = vertIndexStart;
                tempIndices[trianglesIndexStart++] = vertIndexStart + 2;
                tempIndices[trianglesIndexStart] = vertIndexStart + 3;
            }
            targetMesh.triangles = tempIndices;
            targetMesh.RecalculateBounds();
        }

        private static int GetIntValueFromHex(string hexString) {
            int code;
            int.TryParse(hexString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out code);
            return code;
        }

        private static string GetCharStringFromHex(string hexString) {
            int code;
            if (int.TryParse(hexString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out code)) {
                return char.ConvertFromUtf32(code);
            }
            return string.Empty;
        }

#if UNITY_EDITOR
        public override string DrawIconSelectField(string iconName) {
            if (iconFont == null) {
                return iconName;
            }

            List<string> iconKeys = GetIconKeys();
            int selectedIconIndex = -1;
            for (int i = 0; i < iconKeys.Count; i++) {
                if (iconName == iconKeys[i]) {
                    selectedIconIndex = i;
                    break;
                }
            }

            int code = 0;
            float buttonSize = 50f;
            string textValue = string.Empty;

            Color tColor = GUI.color;
            GUIStyle fontStyle = new GUIStyle(UnityEditor.EditorStyles.miniButton);
            fontStyle.font = IconFont;
            fontStyle.fontSize = 24;

            UnityEditor.EditorGUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);

            if (showChooser) {
                // Show a list of all the icons
                int maxPerRow = 6;
                int rowCount = -1;
                editorScrollView = UnityEditor.EditorGUILayout.BeginScrollView(editorScrollView, false, false, GUILayout.MinHeight(buttonSize * 1.5f), GUILayout.Height(Mathf.Max(buttonSize * 1.5f, (iconKeys.Count / maxPerRow * buttonSize))));
                UnityEditor.EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < iconKeys.Count; i++) {
                    if (int.TryParse(iconKeys[i], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out code)) {
                        textValue = char.ConvertFromUtf32(code);
                    }

                    // Don't display 'empty' characters
                    if (!iconFont.HasCharacter(textValue[0]))
                        continue;

                    GUI.color = selectedIconIndex == i ? Color.grey : Color.white;
                    if (GUILayout.Button(textValue,
                        fontStyle,
                        GUILayout.MaxWidth(buttonSize),
                        GUILayout.MaxHeight(buttonSize),
                        GUILayout.MinWidth(buttonSize),
                        GUILayout.MinHeight(buttonSize))) {
                        selectedIconIndex = i;
                        showChooser = false;
                    }

                    rowCount++;
                    if (rowCount >= maxPerRow) {
                        rowCount = -1;
                        UnityEditor.EditorGUILayout.EndHorizontal();
                        UnityEditor.EditorGUILayout.BeginHorizontal();
                    }
                }
                UnityEditor.EditorGUILayout.EndHorizontal();
                UnityEditor.EditorGUILayout.EndScrollView();
                iconName = (selectedIconIndex < 0 ? string.Empty : iconKeys[selectedIconIndex]);
                if (GUILayout.Button("Hide button icons")) {
                    showChooser = false;
                }
            } else {
                // Just show the main icon
                if (int.TryParse(iconName, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out code)) {
                    textValue = char.ConvertFromUtf32(code);
                }
                UnityEditor.EditorGUILayout.LabelField("(Click button to choose icon)", UnityEditor.EditorStyles.miniLabel);
                if (GUILayout.Button(textValue,
                    fontStyle,
                    GUILayout.MaxWidth(buttonSize),
                    GUILayout.MaxHeight(buttonSize),
                    GUILayout.MinWidth(buttonSize),
                    GUILayout.MinHeight(buttonSize))) {
                    showChooser = true;
                }
            }
            UnityEditor.EditorGUILayout.EndVertical();
            GUI.color = tColor;
            return iconName;
        }

        private bool showChooser = false;
        private Vector2 editorScrollView;

        [UnityEditor.CustomEditor(typeof(ButtonIconProfileFont))]
        public class CustomEditor : ProfileInspector { }
#endif
    }
}