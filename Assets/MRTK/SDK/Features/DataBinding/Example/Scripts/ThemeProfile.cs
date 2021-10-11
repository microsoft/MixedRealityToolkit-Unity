using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{

    [CreateAssetMenu(fileName = "MRTK_Theme", menuName = "Mixed Reality Toolkit/Theme")]
    public class ThemeProfile : ScriptableObject
    {
        [Serializable]
        public class SpriteSet
        {
            [SerializeField] public Sprite Add;
            [SerializeField] public Sprite Check;
            [SerializeField] public Sprite Circle;
            [SerializeField] public Sprite Close;
            [SerializeField] public Sprite Contacts;
            [SerializeField] public Sprite Favorite;
            [SerializeField] public Sprite Music;
            [SerializeField] public Sprite Play;
            [SerializeField] public Sprite Search;
            [SerializeField] public Sprite Trash;
            [SerializeField] public Sprite Volume;
        }

        [Serializable]
        public class ButtonParameters
        {
            [SerializeField]
            public Material BackplateMaterial;

            [SerializeField]
            public Material HighlightMaterial;

            [SerializeField]
            public string FontStyleSheetName;

            [SerializeField]
            public SpriteSet Sprites;
        }



        [SerializeField]
        public ButtonParameters ButtonStyle;
    }
}
