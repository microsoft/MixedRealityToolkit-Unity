// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// The method of retrieving the final value of the correct type for a data/theme binding to a UX element.
    ///
    /// There may be both variable data and theme data:
    ///
    /// 1. Retrieve data value at dataKeyPath from data-centric data source.
    /// 2. Optionally transform this value into a form that can be used to retrieve the final themed value.
    /// 3. Use this new value to then retrieve the final themed value.
    ///
    /// An example:
    /// 1. A numeric value called "SystemInfo.Status" exists in the database and available from a data source.
    /// 2. The numeric value can be used to lookup a theme keypath:
    ///        0 => Icons.Queued
    ///        1 => Icons.Started
    ///        2 => Icons.Cancelled
    ///        3 => Icons.Completed
    /// 3. This new value "Icons.Queued" is then used to retrieve the correct Sprite from the theme data source.
    /// </summary>
    /// <remarks>
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven’t fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </remarks>
    public enum DataRetrievalMethod
    {
        /// <summary>
        /// Automatically determine the type from analyzing the nature of the provided data
        /// </summary>
        AutoDetect,

        /// <summary>
        /// Direct value is expected of the correct type from data source with no theming
        /// </summary>
        DirectValue,

        /// <summary>
        /// An integral index or string key is expected and used to look up the desired value from a local lookup table
        /// </summary>
        DirectLookup,

        /// <summary>
        /// A themed object of the correct type is expected from the theme data source, where the object to be themed is itself not variable via a separate data binding value.
        /// </summary>
        StaticThemedValue,

        /// <summary>
        /// An integral index or string key used to look up the desired theme keypath locally. This theme keypath is then used to retrieve the actual object of the expected type from the theme data source.
        /// </summary>
        ThemeKeypathLookup,

        /// <summary>
        /// A string property value that will be appended to the theme base keypath provided in the Theme Helper.
        /// </summary>
        ThemeKeypathProperty,

        /// <summary>
        /// A resource path for retrieving the value from a Unity resource. This is a string of the form "resource://path_to_resource"
        /// </summary>
        ResourcePath,

        /// <summary>
        /// A file path for retrieving a Unity streaming asset. This is a string of the form "file://path_to_streamingasset
        /// </summary>
        FilePath
    }

    /// <summary>
    /// This profile can be used to configure data and theme binding settings for a themable element of the UX. It makes the connection between a data source, a theme source and the UX elements that should be affected.
    /// </summary>
    /// <remarks>
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven’t fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </remarks>
    [Serializable]
    public class DataBindingProfile
    {
        /// <summary>
        /// A mapping from a datum to a keypath.  The datum can be a numeric value that is provided as a string, and it will properly map to a numeric value from the actual data source.
        /// </summary>
        [Serializable]
        public struct ValueToKeypath
        {
            [Tooltip("Value from the data source to be mapped to an object.")]
            [SerializeField] private string value;
            public string Value => value;

            [Tooltip("Relative or absolute theme keypath that is used to retrieve the desired object.")]
            [SerializeField] private string keypath;
            public string Keypath => keypath;
        }

        [Tooltip("Key path for the datum in the data source that can be used to retrieve the data bound value. This can be null or empty string if only theme bound.")]
        [SerializeField, Experimental]
        private string dataKeyPath;
        public string DataKeyPath => dataKeyPath;

        [Tooltip("Key path for the theme datum in a theme data source that can be used to retrieve the theme value. This can be null or empty string if only data bound.")]
        [SerializeField]
        private string themeKeyPath;
        public string ThemeKeyPath => themeKeyPath;

        [Tooltip("Whether to do an exact match, or to use regex when comparing GameObject names.")]
        [SerializeField]
        private bool useRegex = false;
        public bool UseRegex => useRegex;

        [Tooltip("Optional pattern that can be used to find all GameObjects that contain an element (Component or otherwise) that should be bound to this keypath. If 'use regex' option is not checked, an exact match is used. If no value provided, it is assumed that all elements of the correct type should be included.")]
        [SerializeField]
        private string gameObjectNameRegex;
        public string GameObjectNameRegex => gameObjectNameRegex;

        [Tooltip("The type of data expected from the primary data source that will be used either directly, or to fetch the final object from the specified source. The value could be provided locally, or via a theme.")]
        [SerializeField]
        private DataRetrievalMethod retrievalMethod = DataRetrievalMethod.AutoDetect;
        public DataRetrievalMethod RetrievalMethod => retrievalMethod;

        [Header("Optional Theme Value to Keypath Lookup")]

        [Tooltip("List of <value, themeKeypath> pairs, where either an index or a specific string key is used to lookup a theme keypath. This is used by the ThemeKeypathLookup and AutoDetect retrieval methods.")]
        [SerializeField]
        private ValueToKeypath[] valueToThemeKeypathLookup;
        public ValueToKeypath[] ValueToThemeKeypathLookup => valueToThemeKeypathLookup;
    }
}

