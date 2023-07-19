// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX.Experimental
{
    /// <summary>
    /// A set of sprites that can be selected via the name of the property
    /// in a data consumer theme.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class enables the styling of the icons to be
    /// changed to match a particular design goal.
    /// </para>
    /// <para>
    /// This class is intended to be included in other theme <see cref="ScriptableObject"/>
    /// profiles that may want to allow for changing icon styles.
    /// </para>
    /// <para>
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven't fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </para>
    /// </remarks>
    public class SpriteSetTheme
    {
        [Tooltip("An 'add' icon.")]
        [SerializeField]
        private Sprite add;

        /// <summary>
        /// An 'Add' icon.
        /// </summary>
        public Sprite Add => add;

        [Tooltip("A 'check' icon.")]
        [SerializeField]
        private Sprite check;

        /// <summary>
        /// A 'check' icon.
        /// </summary>
        public Sprite Check => check;

        [Tooltip("A 'circle' icon.")]
        [SerializeField]
        private Sprite circle;

        /// <summary>
        /// A 'circle' icon.
        /// </summary>
        public Sprite Circle => circle;

        [Tooltip("A 'close' icon.")]
        [SerializeField]
        private Sprite close;

        /// <summary>
        /// A 'close' icon.
        /// </summary>
        public Sprite Close => close;

        [Tooltip("A 'contacts' icon.")]
        [SerializeField]
        private Sprite contacts;

        /// <summary>
        /// A 'contacts' icon.
        /// </summary>
        public Sprite Contacts => contacts;

        [Tooltip("A 'favorites' icon.")]
        [SerializeField]
        private Sprite favorite;

        /// <summary>
        /// A 'favorites' icon.
        /// </summary>
        public Sprite Favorite => favorite;

        [Tooltip("A 'music' icon.")]
        [SerializeField]
        private Sprite music;

        /// <summary>
        /// A 'music' icon.
        /// </summary>
        public Sprite Music => music;

        [Tooltip("A 'play' icon.")]
        [SerializeField]
        private Sprite play;

        /// <summary>
        /// A 'play' icon.
        /// </summary>
        public Sprite Play => play;

        [Tooltip("A 'search' icon.")]
        [SerializeField]
        private Sprite search;

        /// <summary>
        /// A 'search' icon.
        /// </summary>
        public Sprite Search => search;

        [Tooltip("A 'trash' icon.")]
        [SerializeField]
        private Sprite trash;

        /// <summary>
        /// A 'trash' icon.
        /// </summary>
        public Sprite Trash => trash;

        [Tooltip("A 'volume' icon.")]
        [SerializeField]
        private Sprite volume;

        /// <summary>
        /// A 'volume' icon.
        /// </summary>
        public Sprite Volume => volume;
    }
}
