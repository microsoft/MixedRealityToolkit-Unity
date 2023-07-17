// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for the experimental package.
// While nice to have, documentation is not required for this experimental package.
#pragma warning disable CS1591

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// A simple data key path mapper that can be assigned to a data source in the Unity inspector.
    ///
    /// TODO: consider refactoring using SerializableDictionary class once core utilities are separable.
    /// </summary>
    [AddComponentMenu("MRTK/Data Binding/Data Key Path Mapper Dictionary")]
    public class DataKeyPathMapperGODictionary : MonoBehaviour, IDataKeyPathMapper
    {
        [Serializable]
        public class ViewToDataKeypathMap
        {
            [Tooltip("Local view key path associated with a data consumer, typically used in a prefab.")]
            [SerializeField] public string viewKeyPath = null;

            [Tooltip("Key path in data source to be mapped to. This will be a relative path if referencing data within an array")]
            [SerializeField] public string dataKeyPath = null;
        }

        [Tooltip("A collection of mappings between a view (consumer) key path and a data source key path.")]
        [SerializeField, Experimental]
        private ViewToDataKeypathMap[] viewKeypathToDataKeypathMapper = null;

        public IDataKeyPathMapper DataKeyPathMapper { get { return _dataKeyPathMapperDictionary; } }

        protected DataKeyPathMapperDictionary _dataKeyPathMapperDictionary = new DataKeyPathMapperDictionary();


        /// <summary>
        /// A Unity event function that is called when the script component has been enabled.
        /// </summary>
        private void OnEnable()
        {
            foreach (ViewToDataKeypathMap v2dKeyPath in viewKeypathToDataKeypathMapper)
            {
                _dataKeyPathMapperDictionary.AddKeyMapping(v2dKeyPath.viewKeyPath, v2dKeyPath.dataKeyPath);
            }
        }

        /// <inheritdoc/>
        public string GetDataKeyPathFromViewKeyPath(string viewKeyPath)
        {
            if (_dataKeyPathMapperDictionary != null)
            {
                return _dataKeyPathMapperDictionary.GetDataKeyPathFromViewKeyPath(viewKeyPath);
            }
            else
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public string GetViewKeyPathFromDataKeyPath(string dataKeyPath)
        {
            if (_dataKeyPathMapperDictionary != null)
            {
                return _dataKeyPathMapperDictionary.GetViewKeyPathFromDataKeyPath(dataKeyPath);
            }
            else
            {
                return null;
            }
        }
    }
}
#pragma warning restore CS1591