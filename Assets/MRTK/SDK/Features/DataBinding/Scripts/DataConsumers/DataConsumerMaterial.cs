using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{


    public class DataConsumerMaterial : DataConsumerThemableBase<Material>
    {
        [Serializable]
        public struct ValueToMaterial
        {
            [Tooltip("Value from the data source to be mapped to an object.")]
            [SerializeField] public string value;

            [Tooltip("Object that this value maps to.")]
            [SerializeField] public Material material;
        }

        [Tooltip("(Optional) List of <key,Material> mappings where a list index or a string key can be used to identify the desired Material to use. Note: The key can be left blank or used as a description if only a list index will be used.")]
        [SerializeField]
        private ValueToMaterial[] materialLookup;

        [Tooltip("(Optional) Explicit list of Renderer Components that should be modified.")]
        [SerializeField]
        private Renderer[] renderersToModify;

        protected override Type[] GetComponentTypes()
        {

            Type[] types = { typeof(Renderer) };
            return types;
        }

        protected override Material GetObjectByIndex(int n)
        {
            if (n < materialLookup.Length)
            {
                return materialLookup[n].material;
            }
            else
            {
                return null;
            }
        }


        protected override bool DoesManageSpecificComponents()
        {
            return renderersToModify.Length > 0;
        }


        protected override void AttachDataConsumer()
        {
            foreach ( Component component in renderersToModify )
            {
                AddComponentToManage(component);
            }

            base.AttachDataConsumer();
        }


        protected override Material GetObjectByKey(string keyValue)
        {

            foreach (ValueToMaterial valueToMaterial in materialLookup)
            {
                if (keyValue == valueToMaterial.value)
                {
                    return valueToMaterial.material;
                }
            }

            return null;
        }


        protected override void SetObject(Component component, object inValue, Material materialToSet)
        {
            Renderer renderer = component as Renderer;

            renderer.material = materialToSet as UnityEngine.Material;
        }
 
    }
}
