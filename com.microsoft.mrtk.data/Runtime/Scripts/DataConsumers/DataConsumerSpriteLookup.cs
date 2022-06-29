// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;
using UnityEngine.Serialization;

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
    [AddComponentMenu("MRTK/Data Binding/Consumers/Data Consumer Sprite Lookup")]
    public class DataConsumerSpriteLookup : DataConsumerGOBase
    {
        [Serializable]
        internal struct ValueToSpriteInfo
        {
            [Tooltip("Value from the data source to be mapped to a sprite.")]
            [SerializeField, FormerlySerializedAs("Value")]
            private string value;

            /// <summary>
            /// Value from the data source to be mapped to a sprite.
            /// </summary>
            public string Value => value;

            [Tooltip("Sprite to map to for this value.")]
            [SerializeField, FormerlySerializedAs("Sprite")]
            private Sprite sprite;

            /// <summary>
            /// Sprite to map to for this value.
            /// </summary>
            public Sprite Sprite => sprite;
        }

        [Tooltip("Manage sprites in child game objects as well as this one.")]
        [SerializeField]
        private bool manageChildren = true;

        [Tooltip("Key path within the data source for the value used for sprite lookup.")]
        [SerializeField]
        private string keyPath = null;

        [Tooltip("Array of value-to-sprite mappings.")]
        [SerializeField]
        private ValueToSpriteInfo[] valueToSpriteLookup = null;


        protected SpriteRenderer _spriteRenderer;

        /// </inheritdoc/>
        protected override Type[] GetComponentTypes()
        {
            Type[] types = { typeof(SpriteRenderer) };
            return types;
        }

        /// </inheritdoc/>
        protected override bool ManageChildren()
        {
            return manageChildren;
        }

        /// </inheritdoc/>
        protected override void AddVariableKeyPathsForComponent(Component component)
        {
            _spriteRenderer = component as SpriteRenderer;
            AddKeyPathListener(keyPath);
        }

        /// </inheritdoc/>
        protected override void ProcessDataChanged(IDataSource dataSource, string resolvedKeyPath, string localKeyPath, object value, DataChangeType dataChangeType)
        {
            if (localKeyPath == keyPath)
            {
                string strValue = value.ToString();

                foreach (ValueToSpriteInfo v2si in valueToSpriteLookup)
                {
                    if (strValue == v2si.Value)
                    {
                        _spriteRenderer.sprite = v2si.Sprite;
                        break;
                    }
                }
            }
        }
    }
}
