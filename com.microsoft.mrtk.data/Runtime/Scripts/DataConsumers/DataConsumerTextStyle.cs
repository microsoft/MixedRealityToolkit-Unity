// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// A data consumer that can alter the style of a text
    /// component based on a style lookup.
    ///
    /// Currently supported are:
    ///     TextMeshPro and TextMeshProUGUI
    ///
    /// One of these data consumer components can manage any number
    /// of text components so long as they are being populated by
    /// the same data source.
    /// </summary>
    [AddComponentMenu("MRTK/Data Binding/Consumers/Data Consumer Text Style")]
    public class DataConsumerTextStyle : DataConsumerThemableBase<TMP_StyleSheet>
    {
        /// </inheritdoc/>
        protected override Type[] GetComponentTypes()
        {
            Type[] types = { typeof(TextMeshPro), typeof(TextMeshProUGUI) };
            return types;
        }

        /// </inheritdoc/>
        protected override void SetObject(Component component, object inValue, TMP_StyleSheet styleSheet)
        {
            TMP_Text textMeshPro = component as TMP_Text;

            textMeshPro.styleSheet = styleSheet;
        }
    }
}
