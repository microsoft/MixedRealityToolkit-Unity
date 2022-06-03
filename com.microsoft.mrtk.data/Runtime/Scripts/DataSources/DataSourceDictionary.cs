// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// Data Source that stores arbitrary information in a Dictionary where
    /// the dictionary key is the keypath and the value is any object that
    /// can be used by some data consumer.
    ///
    /// Any change to a value in this dictionary will cause any data consumers that have
    /// registered to listen to its key path will be notified of a data change.
    ///
    /// This simple data source is great for providing a simple list of data to data consumers dynamically at run-time.
    /// </summary>
    public class DataSourceDictionary : DataSourceBase
    {
        protected Dictionary<string, object> _dataDictionary = new Dictionary<string, object>();

        /// <summary>
        /// Set the dictionary being managed by this data source.
        /// </summary>
        /// <remarks>
        /// This is useful if the dictionary to use is already populated elsewhere and
        /// not via SetValue().
        ///
        /// NOTE: Make sure to call NotifyAllChanged() method if you want data consumers to update
        /// to the new values.
        /// </remarks>
        /// <param name="dataDictionary">The dictionary to set.</param>
        public void SetDataSourceDictionary(Dictionary<string, object> dataDictionary)
        {
            _dataDictionary = dataDictionary;
        }


        /// </inheritdoc/>
        public override object GetValueInternal(string resolvedKeyPath)
        {
            if (_dataDictionary.ContainsKey(resolvedKeyPath))
            {
                return _dataDictionary[resolvedKeyPath];
            }
            else
            {
                return null;
            }
        }


        /// </inheritdoc/>
        public override void SetValueInternal(string resolvedKeyPath, object newValue)
        {
            _dataDictionary[resolvedKeyPath] = newValue;
        }
    } // End of class DataSourceDictionary
} // End of namespace Microsoft.MixedReality.Toolkit.Data
