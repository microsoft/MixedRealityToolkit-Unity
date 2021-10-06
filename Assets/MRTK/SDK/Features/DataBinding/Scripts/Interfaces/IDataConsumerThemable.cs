using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.Data
{
    public interface IDataConsumerThemable
    {

        /// <summary>
        /// Receive changed theme data, determine its, type and process appropriately
        /// </summary>
        /// <remarks>
        /// The object can be any of a number of types and loaded accordingly:
        /// 
        /// int                     Use as index to select Nth entry in ValueToObjectInfo
        /// T                       Directly use the value to replace the managed variable of that type
        /// "resource://<<path>>"   Use path to load a Unity Resource
        /// "file://<<path>>"       Use path to load a streaming asset
        /// other string            Use string value to find entry by value in ValueToObjectInfo
        /// 
        /// </remarks>
        /// <param name="themeHelper">Theme helper instance</param>
        /// <param name="resolvedKeyPath"></param>
        /// <param name="localKeyPath"></param>
        /// <param name="inValue"></param>
        /// <param name="dataChangeType"></param>
        void ProcessThemeDataChanged(IDataConsumer themeHelper, string resolvedKeyPath, string localKeyPath, object inValue, DataChangeType dataChangeType);
    }
}

