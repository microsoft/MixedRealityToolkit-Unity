﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// A simple data key path mapper that can be assigned to a data source in the Unity inspector.
    ///
    /// TODO: consider refactoring using SerializableDictionary class once core utilities are separable.
    /// </summary>
    /// <remarks>
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven’t fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </remarks>
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


        void OnEnable()
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
