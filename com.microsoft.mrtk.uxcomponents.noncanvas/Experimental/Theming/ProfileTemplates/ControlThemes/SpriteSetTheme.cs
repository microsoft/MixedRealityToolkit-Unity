// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX.Experimental
{
    /// <summary>
    /// A set of sprites that can be selected via the name of the property
    /// in a data consumer theme.  It allows the styling of the icons to be
    /// changed to match a particular design goal.
    ///
    /// This class is intended to be included in other theme ScriptableObject
    /// profiles that may want to allow for changing icon styles.
    /// </summary>
    /// <remarks>
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven’t fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </remarks>
    public class SpriteSetTheme
    {
        [Tooltip("An 'Add' icon.")]
        [SerializeField]
        private Sprite add;
        public Sprite Add => add;

        [Tooltip("A 'Check' icon.")]
        [SerializeField]
        private Sprite check;
        public Sprite Check => check;

        [Tooltip("A 'Circle' icon.")]
        [SerializeField]
        private Sprite circle;
        public Sprite Circle => circle;

        [Tooltip("A 'Close' icon.")]
        [SerializeField]
        private Sprite close;
        public Sprite Close => close;

        [Tooltip("A 'Contacts' icon.")]
        [SerializeField]
        private Sprite contacts;
        public Sprite Contacts => contacts;

        [Tooltip("A 'Favorites' icon.")]
        [SerializeField]
        private Sprite favorite;
        public Sprite Favorite => favorite;

        [Tooltip("A 'Music' icon.")]
        [SerializeField]
        private Sprite music;
        public Sprite Music => music;

        [Tooltip("A 'Play' icon.")]
        [SerializeField]
        private Sprite play;
        public Sprite Play => play;

        [Tooltip("A 'Search' icon.")]
        [SerializeField]
        private Sprite search;
        public Sprite Search => search;

        [Tooltip("A 'Trash' icon.")]
        [SerializeField]
        private Sprite trash;
        public Sprite Trash => trash;

        [Tooltip("A 'Volume' icon.")]
        [SerializeField]
        private Sprite volume;
        public Sprite Volume => volume;
    }
}
