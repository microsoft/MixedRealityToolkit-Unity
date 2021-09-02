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
    /// 
    /// TODO: Allow for a default sprite if no look up can be found.
    /// 
    /// </remarks>
    /// 
    [Serializable]
    public class DataConsumerSprite : DataConsumerGOBase
    {
        [Tooltip("Manage a sprite in either this or a child game object.")]
        [SerializeField] private bool manageChildren = true;

        [Tooltip("Key path within the data source for the Sprite object.")]
        [SerializeField] private string keyPath;

        protected SpriteRenderer _spriteRenderer;

        protected override Type[] GetComponentTypes()
        {

            Type[] types = { typeof(SpriteRenderer) };
            return types;
        }


        protected override bool ManageChildren()
        {
            return manageChildren;
        }


        protected override void AddVariableKeyPathsForComponent(Type componentType, Component component)
        {
            _spriteRenderer = component as SpriteRenderer;
            AddKeyPathListener(keyPath);
        }


        protected override void ProcessDataChanged(IDataSource dataSource, string resolvedKeyPath, string localKeyPath, object newValue, DataChangeType dataChangeType)
        {
            if (localKeyPath == keyPath)
            {
                 _spriteRenderer.sprite = newValue as Sprite;
            }
        }

    }
}
