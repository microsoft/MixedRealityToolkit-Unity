// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    [AddComponentMenu("MRTK/Data Binding/Consumers/Data Consumer Material")]
    public class DataConsumerMaterial : DataConsumerThemableBase<Material>
    {
        [Serializable]
        private struct ValueToMaterial
        {
            [Tooltip("Value from the data source to be mapped to an object.")]
            [SerializeField]
            private string value;

            /// <summary>
            /// Value from the data source to be mapped to an object.
            /// </summary>
            public string Value => value;

            [Tooltip("Object that this value maps to.")]
            [SerializeField]
            private Material material;

            /// <summary>
            /// Object that this value maps to.
            /// </summary>
            public Material Material => material;
        }

        [Tooltip("(Optional) List of <key,Material> mappings where a list index or a string key can be used to identify the desired Material to use. Note: The key can be left blank or used as a description if only a list index will be used.")]
        [SerializeField]
        private ValueToMaterial[] materialLookup;

        [Tooltip("(Optional) Explicit list of Renderer Components that should be modified. If none specified, then all renderers found are considered.")]
        [SerializeField]
        private Renderer[] renderersToModify;


        /// </inheritdoc/>
        protected override Type[] GetComponentTypes()
        {
            Type[] types = { typeof(Renderer) };
            return types;
        }

        /// <inheritdoc/>
        protected override Material GetObjectByIndex(int n)
        {
            if (n < materialLookup.Length)
            {
                return materialLookup[n].Material;
            }
            else
            {
                return null;
            }
        }

        /// </inheritdoc/>
        protected override bool DoesManageSpecificComponents()
        {
            return renderersToModify != null && renderersToModify.Length > 0;
        }

        /// </inheritdoc/>
        protected override void AttachDataConsumer()
        {
            if (renderersToModify != null)
            {
                foreach (Component component in renderersToModify)
                {
                    _componentsToManage.Add(component);
                }
            }
            base.AttachDataConsumer();
        }

        /// </inheritdoc/>
        protected override Material GetObjectByKey(string keyValue)
        {
            foreach (ValueToMaterial valueToMaterial in materialLookup)
            {
                if (keyValue == valueToMaterial.Value)
                {
                    return valueToMaterial.Material;
                }
            }

            return null;
        }

        /// </inheritdoc/>
        protected override void SetObject(Component component, object inValue, Material materialToSet)
        {
            Renderer renderer = component as Renderer;
            renderer.sharedMaterial = materialToSet;
        }
    }
}
