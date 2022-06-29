// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.using System.Collections

using System;
using UnityEngine;

#if MRTK_DATA_PRESENT && MRTK_UX_DATABINDING_THEMING_ENABLED
using Microsoft.MixedReality.Toolkit.Data;
#endif // MRTK_DATA_PRESENT && MRTK_UX_DATABINDING_THEMING_ENABLED

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// Automatically configures and adds DataConsumer components needed to manage all data bound and/or themable
    /// elements in a prefab
    /// </summary>
    [Serializable]
    [AddComponentMenu("MRTK/UX/UX Binding Configurator")]
    public class UXBindingConfigurator : MonoBehaviour
    {
        [Tooltip("The binding profile scriptable object that defines a standard set of keypath mappings for data sources and theme sources across all themable and bindable elements.")]
        [SerializeField]
        private UXBindingProfileTemplate bindingProfile;
        public UXBindingProfileTemplate BindingProfile => bindingProfile;

#if MRTK_DATA_PRESENT && MRTK_UX_DATABINDING_THEMING_ENABLED
        private void Awake()
        {
            TryDataBindingConfiguration();
        }

        /// <summary>
        /// Configures data binding for this prefab whenever the Data Binding and Theming package (com.microsoft.mrtk.data) is
        /// included.
        ///
        /// Note that MRTK_DATA_PRESENT define ensures that there is no perf hit whenever data binding package is not included.
        /// This is done in such a way that no dependency is created between the two packages even
        /// if the MRTK_DATA_PRESENT define is
        /// </summary>
        protected void TryDataBindingConfiguration()
        {
            if (bindingProfile != null)
            {
                try
                {
                    IDataBindingConfigurator bindingConfigurator = new DataBindingConfigurator();
                    if (bindingConfigurator != null)
                    {
                        bindingConfigurator.ConfigureBinding(gameObject, bindingProfile);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning("To enable theming and data binding, add the com.microsoft.mrtk.data Data Binding and Theming package.");
                    Debug.LogWarning(e.Message);
                }
            }
            else
            {
                Debug.LogWarning("Binding profile is missing. Not able to automatically configure binding.");
            }
        }
#endif // MRTK_DATA_PRESENT && MRTK_UX_DATABINDING_THEMING_ENABLED
    }
}
