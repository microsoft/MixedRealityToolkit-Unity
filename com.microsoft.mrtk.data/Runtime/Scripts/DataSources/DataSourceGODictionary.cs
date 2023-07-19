// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for the experimental package.
// While nice to have, documentation is not required for this experimental package.
#pragma warning disable CS1591

using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Data
{
    [AddComponentMenu("MRTK/Data Binding/Sources/Data Source Dictionary")]
    public class DataSourceGODictionary : DataSourceGOBase
    {
        public delegate void NotifyKeypathValueChangedDelegate(string keyPath, string value);

        [Serializable]
        protected class KeyPathValue
        {
            [Tooltip("A keypath used to access this value.")]
            [SerializeField, FormerlySerializedAs("KeyPath")]
            private string keyPath;

            /// <summary>
            /// A keypath used to access this value.
            /// </summary>
            public string KeyPath => keyPath;

            [Tooltip("A value accessible via its key path.")]
            [SerializeField, FormerlySerializedAs("Value")]
            private string value;

            /// <summary>
            /// A value accessible via its key path.
            /// </summary>
            public string Value
            {
                get => value;
                set => this.value = value;
            }
        }

        [Tooltip("A list of key value pairs that comprise a simple data source.")]
        [SerializeField]
        protected KeyPathValue[] keyPathValues;

        private bool pendingUpdate = false;

        /// <inheritdoc/>
        public override void SetValue(string resolvedKeyPath, object newValue, bool isAtomicChange = false)
        {
            base.SetValue(resolvedKeyPath, newValue, isAtomicChange);
            foreach (KeyPathValue kpv in keyPathValues)
            {
                if (kpv.KeyPath == resolvedKeyPath)
                {
                    kpv.Value = newValue.ToString();
                    return;
                }
            }
        }

        /// <inheritdoc/>
        public override IDataSource AllocateDataSource()
        {
            return new DataSourceDictionary();
        }

        /// <inheritdoc/>
        protected override void InitializeDataSource()
        {
            DataSource.DataChangeSetBegin();

            foreach (KeyPathValue keyPathValue in keyPathValues)
            {
                DataSource.SetValue(keyPathValue.KeyPath, keyPathValue.Value);
            }

            DataSource.DataChangeSetEnd();
        }

        #region Unity methods

        /// <summary>
        /// A Unity event function that is called every frame, if this object is enabled.
        /// </summary>
        private void Update()
        {
            if (pendingUpdate)
            {
                UpdateChangedInspectorValues();
                pendingUpdate = false;
            }
        }

        /// <summary>
        /// A Unity Editor only event function that is called when the script is loaded or a value changes in the Unity Inspector.
        /// </summary>
        private void OnValidate()
        {
            pendingUpdate = true;
        }

        #endregion

        private void UpdateChangedInspectorValues()
        {
            DataSource.DataChangeSetBegin();
            foreach (KeyPathValue keyPathValue in keyPathValues)
            {
                string oldValue = DataSource.GetValue(keyPathValue.KeyPath) as string;
                if (!oldValue.Equals(keyPathValue.Value))
                {
                    DataSource.SetValue(keyPathValue.KeyPath, keyPathValue.Value);
                }
            }
            DataSource.DataChangeSetEnd();
        }
    }
}
#pragma warning restore CS1591