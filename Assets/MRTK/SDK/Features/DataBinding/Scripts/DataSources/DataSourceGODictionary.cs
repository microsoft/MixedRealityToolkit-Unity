using System;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Data
{


    public class DataSourceGODictionary : DataSourceGOBase
    {

        [Serializable]
        public class KeyPathValue
        {

            [Tooltip("A keypath used to access this value.")]
            [SerializeField]
            public string KeyPath;


            [Tooltip("A value accessible via its key path.")]
            [SerializeField] public string Value;

        }

        [Tooltip("A list of key value pairs that comprise a simple data source.")]
        [SerializeField]
        private KeyPathValue[] keyPathValues;



        public override void SetValue(string resolvedKeyPath, object newValue)
        {
            base.SetValue(resolvedKeyPath, newValue);
            foreach( KeyPathValue kpv in keyPathValues)
            {
                if ( kpv.KeyPath == resolvedKeyPath )
                {
                    kpv.Value = newValue.ToString();
                    return;
                }
            }
        }


        internal override IDataSource AllocateDataSource()
        {
            return new DataSourceDictionary();
        }

        private void Start()
        {
            _dataSource.DataChangeSetBegin();

            foreach (KeyPathValue keyPathValue in keyPathValues)
            {
                _dataSource.SetValue(keyPathValue.KeyPath, keyPathValue.Value);
            }

            _dataSource.DataChangeSetEnd();
        }
    }
}