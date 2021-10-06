﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Data
{


    public class DataSourceGODictionary : DataSourceGOBase
    {
        public delegate void NotifyKeypathValueChangedDelegate(string keyPath, string value);

        [Serializable]
        public class KeyPathValue
        {

            [Tooltip("A keypath used to access this value.")]
            [SerializeField]
            public string KeyPath;


            [Tooltip("A value accessible via its key path.")]
            [SerializeField]
            public string Value;

        }

        [Tooltip("A list of key value pairs that comprise a simple data source.")]
        [SerializeField]
        protected KeyPathValue[] keyPathValues;

        private bool pendingUpdate = false;

          public override void SetValue(string resolvedKeyPath, object newValue, bool isAtomicChange = false)
        {
            base.SetValue(resolvedKeyPath, newValue, isAtomicChange);
            foreach( KeyPathValue kpv in keyPathValues)
            {
                if ( kpv.KeyPath == resolvedKeyPath )
                {
                    kpv.Value = newValue.ToString();
                    return;
                }
            }
        }


        public override IDataSource AllocateDataSource()
        {
            return new DataSourceDictionary();
        }

        protected override void InitializeDataSource()
        {
            DataSource.DataChangeSetBegin();

            foreach (KeyPathValue keyPathValue in keyPathValues)
            {
                DataSource.SetValue(keyPathValue.KeyPath, keyPathValue.Value);
            }

            DataSource.DataChangeSetEnd();
        }

        private void Update()
        {
            if ( pendingUpdate )
            {
                UpdateChangedInspectorValues();
                pendingUpdate = false;
            }
        }


        private void UpdateChangedInspectorValues()
        {
            DataSource.DataChangeSetBegin();
            foreach (KeyPathValue keyPathValue in keyPathValues)
            {
                string oldValue = DataSource.GetValue(keyPathValue.KeyPath) as string;
                if ( !oldValue.Equals( keyPathValue.Value ) ) {
                    DataSource.SetValue(keyPathValue.KeyPath, keyPathValue.Value);
                }
            }
            DataSource.DataChangeSetEnd();
        }


        void OnValidate()
        {
            pendingUpdate = true;
        }
    }
}
