// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Data
{
    [CreateAssetMenu(fileName = "MRTK_Theme", menuName = "MRTK/Theme")]
    public class ThemeProfile : ScriptableObject
    {
        [Serializable]
        public class SpriteSet
        {
            [SerializeField, FormerlySerializedAs("Add")]
            private Sprite add;

            public Sprite Add
            {
                get => add;
                set => add = value;
            }

            [SerializeField, FormerlySerializedAs("Check")]
            private Sprite check;

            public Sprite Check
            {
                get => check;
                set => check = value;
            }

            [SerializeField, FormerlySerializedAs("Circle")]
            private Sprite circle;

            public Sprite Circle
            {
                get => circle;
                set => circle = value;
            }

            [SerializeField, FormerlySerializedAs("Close")]
            private Sprite close;

            public Sprite Close
            {
                get => close;
                set => close = value;
            }

            [SerializeField, FormerlySerializedAs("Contacts")]
            private Sprite contacts;

            public Sprite Contacts
            {
                get => contacts;
                set => contacts = value;
            }

            [SerializeField, FormerlySerializedAs("Favorite")]
            private Sprite favorite;

            public Sprite Favorite
            {
                get => favorite;
                set => favorite = value;
            }

            [SerializeField, FormerlySerializedAs("Music")]
            private Sprite music;

            public Sprite Music
            {
                get => music;
                set => music = value;
            }

            [SerializeField, FormerlySerializedAs("Play")]
            private Sprite play;

            public Sprite Play
            {
                get => play;
                set => play = value;
            }

            [SerializeField, FormerlySerializedAs("Search")]
            private Sprite search;

            public Sprite Search
            {
                get => search;
                set => search = value;
            }

            [SerializeField, FormerlySerializedAs("Trash")]
            private Sprite trash;

            public Sprite Trash
            {
                get => trash;
                set => trash = value;
            }

            [SerializeField, FormerlySerializedAs("Volume")]
            private Sprite volume;

            public Sprite Volume
            {
                get => volume;
                set => volume = value;
            }
        }

        [Serializable]
        public class ButtonParameters
        {
            [SerializeField, FormerlySerializedAs("BackPlateMaterial")]
            private Material backPlateMaterial;

            public Material BackPlateMaterial
            {
                get => backPlateMaterial;
                set => backPlateMaterial = value;
            }

            [SerializeField, FormerlySerializedAs("FrontPlateHighlightMaterial")]
            private Material frontPlateHighlightMaterial;

            public Material FrontPlateHighlightMaterial
            {
                get => frontPlateHighlightMaterial;
                set => frontPlateHighlightMaterial = value;
            }

            [SerializeField, FormerlySerializedAs("BackPlateHighlightMaterial")]
            private Material backPlateHighlightMaterial;

            public Material BackPlateHighlightMaterial
            {
                get => backPlateHighlightMaterial;
                set => backPlateHighlightMaterial = value;
            }

            [SerializeField, FormerlySerializedAs("FontStyleSheetName")]
            private string fontStyleSheetName;

            public string FontStyleSheetName
            {
                get => fontStyleSheetName;
                set => fontStyleSheetName = value;
            }

            [SerializeField, FormerlySerializedAs("Sprites")]
            private SpriteSet sprites;

            public SpriteSet Sprites
            {
                get => sprites;
                set => sprites = value;
            }
        }

        [Serializable]
        public class SlateParameters
        {
            [SerializeField, FormerlySerializedAs("BackPlateMaterial")]
            private Material backPlateMaterial;

            public Material BackPlateMaterial
            {
                get => backPlateMaterial;
                set => backPlateMaterial = value;
            }

            [SerializeField, FormerlySerializedAs("FontStyleSheetName")]
            private string fontStyleSheetName;

            public string FontStyleSheetName
            {
                get => fontStyleSheetName;
                set => fontStyleSheetName = value;
            }
        }

        [SerializeField, FormerlySerializedAs("ButtonStyle")]
        private ButtonParameters buttonStyle;

        public ButtonParameters ButtonStyle
        {
            get => buttonStyle;
            set => buttonStyle = value;
        }

        [SerializeField, FormerlySerializedAs("SlateStyle")]
        private SlateParameters slateStyle;

        public SlateParameters SlateStyle
        {
            get => slateStyle;
            set => slateStyle = value;
        }
    }
}
