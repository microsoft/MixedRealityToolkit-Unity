// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
        /// <param name="themeHelper">The theme helper data consumer that called this method.</param>
        /// <param name="resolvedKeyPath"Fully resolved keypath for datum that changed.</param>
        /// <param name="localKeyPath">>Local keypath for the datum that changed.</param>
        /// <param name="themeValue">The current value of the theme data.</param>
        /// <param name="dataChangeType">The type of change that has occurred.</param>
        void ProcessThemeDataChanged(IDataConsumer themeHelper, string resolvedKeyPath, string localKeyPath, object themeValue, DataChangeType dataChangeType);
    }
}

