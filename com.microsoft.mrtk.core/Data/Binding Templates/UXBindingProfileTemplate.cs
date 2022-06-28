// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// This profile combines binding profiles for all Data Consumers used in the MRTK UXComponents components and
    /// is used by the BindingConfigurator that can be dropped on any prefab (or gameobject hierarchy)
    /// to automatically configure all Data Consumers that will be needed to make those game objects data bindable
    /// and themable.
    /// </summary>
    [CreateAssetMenu(fileName = "MRTK_UX_BindingProfile", menuName = "MRTK/Data Binding/UXBindingProfile")]
    public class UXBindingProfileTemplate : ScriptableObject
    {
        [Tooltip("Optional type specifiers for data sources to be injected into data consumers. This will typically be one data and one theme data source. If left empty, the default 'data' and 'theme' data sources will be used.")]
        [SerializeField]
        private string[] dataSourceTypes;
        public string[] DataSourceTypes => dataSourceTypes;

        [Tooltip("An array of binding profiles for specific classes, typically data consumers.")]
        [SerializeField]
        private ClassDataBindingProfile[] classBindings;
        public ClassDataBindingProfile[] ClassBindings => classBindings;
    }
}

