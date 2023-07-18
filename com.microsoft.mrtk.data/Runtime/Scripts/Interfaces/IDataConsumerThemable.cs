// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for the experimental package.
// While nice to have, documentation is not required for this experimental package.
#pragma warning disable CS1591

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
        /// <list type="bullet">
        ///     <item>
        ///         <term>int</term>
        ///         <description>Use as index to select Nth entry in ValueToObjectInfo.</description>         
        ///     </item>
        ///     <item>
        ///         <term>T</term>
        ///         <description>Directly use the value to replace the managed variable of that type.</description>         
        ///     </item>
        ///     <item>
        ///         <term>"resource://[path]"</term>
        ///         <description>Use path to load a Unity Resource.</description>         
        ///     </item>
        ///     <item>
        ///         <term>"file://[path]"</term>
        ///         <description>Use path to load a streaming asset.</description>         
        ///     </item>
        ///     <item>
        ///         <term>other string</term>
        ///         <description>Use string value to find entry by value in ValueToObjectInfo.</description>         
        ///     </item>
        /// </list>
        /// </remarks>
        /// <param name="themeHelper">The theme helper data consumer that called this method.</param>
        /// <param name="resolvedKeyPath">Fully resolved keypath for datum that changed.</param>
        /// <param name="localKeyPath">Local keypath for the datum that changed.</param>
        /// <param name="themeValue">The current value of the theme data.</param>
        /// <param name="dataChangeType">The type of change that has occurred.</param>
        void ProcessThemeDataChanged(IDataConsumer themeHelper, string resolvedKeyPath, string localKeyPath, object themeValue, DataChangeType dataChangeType);
    }
}
#pragma warning restore CS1591