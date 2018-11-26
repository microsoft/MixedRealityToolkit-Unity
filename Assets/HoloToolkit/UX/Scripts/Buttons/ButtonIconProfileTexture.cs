// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

#if ENABLE_WINMD_SUPPORT && !UNITY_EDITOR
using System.Reflection;
#endif

namespace HoloToolkit.Unity.Buttons
{
    /// <summary>
    /// Icon profile that returns textures
    /// </summary>
    public class ButtonIconProfileTexture : ButtonIconProfile
    {
#if UNITY_EDITOR
        private static float textureSize = 50f;
#endif

        [Header("Navigation icons")]
        public Texture2D GlobalNavButton;
        public Texture2D ChevronUp;
        public Texture2D ChevronDown;
        public Texture2D ChevronLeft;
        public Texture2D ChevronRight;
        public Texture2D Forward;
        public Texture2D Back;
        public Texture2D PageLeft;
        public Texture2D PageRight;

        [Header("Common action icons")]
        public Texture2D Add;
        public Texture2D Remove;
        public Texture2D Clear;
        public Texture2D Cancel;
        public Texture2D Zoom;
        public Texture2D Refresh;
        public Texture2D Lock;
        public Texture2D Accept;
        public Texture2D OpenInNewWindow;

        [Header("Common notification icons")]
        public Texture2D Completed;
        public Texture2D Error;

        [Header("Common object icons")]
        public Texture2D Contact;
        public Texture2D Volume;
        public Texture2D KeyboardClassic;
        public Texture2D Camera;
        public Texture2D Video;
        public Texture2D Microphone;

        [Header("Common gesture icons")]
        public Texture2D Ready;
        public Texture2D AirTap;
        public Texture2D PressHold;
        public Texture2D Drag;
        public Texture2D TapToPlaceArt;
        public Texture2D AdjustWithHand;
        public Texture2D AdjustHologram;
        public Texture2D RemoveHologram;

        [HideInMRTKInspector]
        public Texture2D[] CustomIcons;

        private bool initialized;
        private List<string> iconKeys;
        private Dictionary<string, Texture2D> iconLookup;

        public override bool GetIcon(string iconName, MeshRenderer targetRenderer, MeshFilter targetMesh, bool useDefaultIfNotFound)
        {
            Initialize();

            Texture2D icon = null;
            if (useDefaultIfNotFound)
            {
                icon = _IconNotFound;
            }

            if (!string.IsNullOrEmpty(iconName))
            {
                // See if the icon exists
                if (!iconLookup.TryGetValue(iconName, out icon))
                    if (useDefaultIfNotFound)
                        icon = _IconNotFound;

                if (icon == null)
                    if (useDefaultIfNotFound)
                        icon = _IconNotFound;
            }

            // Set the texture on the material
            targetMesh.sharedMesh = IconMesh;
            targetMesh.transform.localScale = Vector3.one;
            targetRenderer.sharedMaterial.mainTexture = icon;
            return icon != null;
        }

        /// <summary>
        /// (Icons starting with '_' will not be included in icon list)
        /// </summary>
        public override List<string> GetIconKeys()
        {
            Initialize();

            return new List<string>(iconKeys);
        }

        private void Initialize()
        {
            if (iconLookup != null)
                return;

            iconLookup = new Dictionary<string, Texture2D>();
            iconKeys = new List<string>();

            // Store all icons in iconLookup via reflection
#if ENABLE_WINMD_SUPPORT && !UNITY_EDITOR
            var fields = GetType().GetTypeInfo().DeclaredFields;
#else
            var fields = this.GetType().GetFields();
#endif
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(Texture2D) && !field.Name.StartsWith("_"))
                {
                    iconLookup.Add(field.Name, (Texture2D)field.GetValue(this));
                    iconKeys.Add(field.Name);
                }
            }

            // These icons will override the common icons if they exist, so do them last
            foreach (Texture2D icon in CustomIcons)
            {
                if (iconLookup.ContainsKey(icon.name))
                {
                    iconLookup[icon.name] = icon;
                }
                else
                {
                    iconLookup.Add(icon.name, icon);
                    iconKeys.Add(icon.name);
                }
            }
        }

#if UNITY_EDITOR
        public override string DrawIconSelectField(string iconName)
        {
            int selectedIconIndex = -1;
            List<string> iconKeys = GetIconKeys();
            for (int i = 0; i < iconKeys.Count; i++)
            {
                if (iconName == iconKeys[i])
                {
                    selectedIconIndex = i;
                    break;
                }
            }
            int newIconIndex = UnityEditor.EditorGUILayout.Popup("Icon", selectedIconIndex, iconKeys.ToArray());
            // This will automatically set the icon in the editor view
            iconName = (newIconIndex < 0 ? string.Empty : iconKeys[newIconIndex]);
            return iconName;
        }

        [UnityEditor.CustomEditor(typeof(ButtonIconProfileTexture))]
        public class CustomEditor : ProfileInspector
        {
            protected override void DrawCustomFooter()
            {
                ButtonIconProfileTexture iconProfile = (ButtonIconProfileTexture)target;
                UnityEditor.EditorGUILayout.LabelField("Custom Icons", UnityEditor.EditorStyles.boldLabel);

                for (int i = 0; i < iconProfile.CustomIcons.Length; i++)
                {
                    Texture2D icon = iconProfile.CustomIcons[i];
                    icon = (Texture2D)UnityEditor.EditorGUILayout.ObjectField(icon != null ? icon.name : "(Empty)", icon, typeof(Texture2D), false, GUILayout.MaxHeight(textureSize));
                    iconProfile.CustomIcons[i] = icon;
                }

                if (GUILayout.Button("Add custom icon"))
                {
                    System.Array.Resize<Texture2D>(ref iconProfile.CustomIcons, iconProfile.CustomIcons.Length + 1);
                }
            }
        }
#endif
    }

}