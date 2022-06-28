// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// Given a value from a data source, use that value to look up the correct Sprite
    /// specified in the Unity inspector list. That Sprite is then associated
    /// with any SpriteRenderer being managed by this object.
    /// </summary>
    ///
    /// <remarks>
    /// TODO: Allow for a default sprite if no look up can be found.
    /// </remarks>
    [Serializable]
    [AddComponentMenu("MRTK/Data Binding/Consumers/Data Consumer Sprite")]
    public class DataConsumerSprite : DataConsumerThemableBase<Sprite>
    {
        /// </inheritdoc/>
        protected override Type[] GetComponentTypes()
        {
            Type[] types = { typeof(SpriteRenderer) };
            return types;
        }

        /// </inheritdoc/>
        protected override void SetObject(Component component, object inValue, Sprite sprite)
        {
            SpriteRenderer renderer = component as SpriteRenderer;

            renderer.sprite = sprite;
        }
    }
}
