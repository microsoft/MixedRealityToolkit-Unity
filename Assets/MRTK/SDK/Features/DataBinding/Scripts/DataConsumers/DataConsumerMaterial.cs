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

        [Tooltip("(Optional) List of <key,value> mappings where a string key is provided by the data source.")]
        [SerializeField]
        private ValueToMaterial[] materialLookup;


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
