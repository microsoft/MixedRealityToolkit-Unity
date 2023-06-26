// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// This profile combines binding profiles for all Data Consumers used in the MRTK UXComponents components and
    /// is used by the BindingConfigurator that can be dropped on any prefab (or game object hierarchy)
    /// to automatically configure all Data Consumers that will be needed to make those game objects data bindable
    /// and capable of theming.
    /// </summary>
    /// <remarks>
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven’t fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </remarks>
    [CreateAssetMenu(fileName = "MRTK_UX_BindingProfile", menuName = "MRTK/Data Binding/UXBindingProfile")]
    public class UXBindingProfileTemplate : ScriptableObject
    {
        [Tooltip("Optional type specifiers for data sources to be injected into data consumers. This will typically be one data and one theme data source. If left empty, the default 'data' and 'theme' data sources will be used.")]
        [SerializeField, Experimental]
        private string[] dataSourceTypes;

        /// <summary>
        /// Optional type specifiers for data sources to be injected into data consumers. This will typically be one data and one theme data source. 
        /// If left empty, the default 'data' and 'theme' data sources will be used.
        /// </summary>
        public string[] DataSourceTypes => dataSourceTypes;

        [Tooltip("An array of binding profiles for specific classes, typically data consumers.")]
        [SerializeField]
        private ClassDataBindingProfile[] classBindings;
        
        /// <summary>
        /// An array of binding profiles for specific classes, typically data consumers.
        /// </summary>
        public ClassDataBindingProfile[] ClassBindings => classBindings;
    }
}

